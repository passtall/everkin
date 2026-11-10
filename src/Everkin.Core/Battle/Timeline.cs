namespace Everkin.Core.Battle;

/// <summary>What a timeline entry is (game_system.md sections 6.1, 6.2).</summary>
public enum TimelineKind
{
    /// <summary>A unit's turn.</summary>
    UnitTurn,

    /// <summary>An auto event put on the timeline by a skill; it executes by itself when reached.</summary>
    AutoEvent,

    /// <summary>A delayed event such as a comet impact; its own visible timeline object.</summary>
    DelayedEvent,

    /// <summary>The end of a status or ground effect, shown as a marker on the timeline.</summary>
    EffectEnd,
}

/// <summary>One thing on the shared timeline.</summary>
public sealed class TimelineEntry
{
    internal TimelineEntry(int id, TimelineKind kind, int due, long order, int? ownerUnitId, string? tag, bool blocking, int lastWait)
    {
        Id = id;
        Kind = kind;
        Due = due;
        Order = order;
        OwnerUnitId = ownerUnitId;
        Tag = tag;
        Blocking = blocking;
        LastWait = lastWait;
    }

    public int Id { get; }
    public TimelineKind Kind { get; }

    /// <summary>The battle time, in time units, at which the entry arrives.</summary>
    public int Due { get; internal set; }

    /// <summary>Order of joining its time unit: of two entries with the same Due, the lower Order goes first.</summary>
    public long Order { get; internal set; }

    /// <summary>The unit whose turn it is, or the owner of the event or effect.</summary>
    public int? OwnerUnitId { get; }

    /// <summary>What the entry stands for (a skill ID, a status name): read by the rules that resolve it.</summary>
    public string? Tag { get; }

    /// <summary>Auto events only: the owner takes no turn until all its blocking auto events are done (section 6.2).</summary>
    public bool Blocking { get; }

    /// <summary>The full Wait the entry was scheduled with; Reset restores it (section 6.1).</summary>
    public int LastWait { get; }

    internal TimelineEntry Copy() => new(Id, Kind, Due, Order, OwnerUnitId, Tag, Blocking, LastWait);
}

/// <summary>
/// The single shared timeline of all units, events and effect ends (game_system.md section 6.1). Time is a whole number of
/// time units that only advances in jumps, to the next entry. Entries that fall into the same time unit keep the order in
/// which they joined it; entries created together are put in order by the battle's seeded generator.
/// </summary>
public sealed class Timeline
{
    public const int MinDelay = 1;
    public const int MaxDelay = 10;
    public const int MinSpeed = 1;
    public const int MaxSpeed = 10;

    /// <summary>A skipped turn costs as much as a Delay 5 skill (sections 6.1, 6.4).</summary>
    public const int SkippedTurnDelay = 5;

    private readonly List<TimelineEntry> _entries = new();
    private readonly Dictionary<int, int> _deferredTurns = new(); // owner unit -> Wait of its turn, started after its last blocking auto event
    private int _nextId;
    private long _nextOrder;

    public Timeline() { }

    private Timeline(Timeline other)
    {
        Now = other.Now;
        _nextId = other._nextId;
        _nextOrder = other._nextOrder;
        foreach (var e in other._entries) _entries.Add(e.Copy());
        foreach (var kv in other._deferredTurns) _deferredTurns[kv.Key] = kv.Value;
    }

    public Timeline Clone() => new(this);

    /// <summary>The battle clock, in time units.</summary>
    public int Now { get; private set; }

    public int Count => _entries.Count;

    // ---------------------------------------------------------------- the formula

    /// <summary>Wait = Delay + (11 − Speed). Delay is clamped to 1–10 and Speed to 1–10; the Wait is at least 1.</summary>
    public static int Wait(int delay, int speed) =>
        Math.Max(1, Math.Clamp(delay, MinDelay, MaxDelay) + 11 - Math.Clamp(speed, MinSpeed, MaxSpeed));

    /// <summary>The first Wait of a unit at battle start: 11 − Speed, as if its previous skill had Delay 0 (section 6.1).</summary>
    public static int InitialWait(int speed) => Math.Max(1, 11 - Math.Clamp(speed, MinSpeed, MaxSpeed));

    /// <summary>The Wait of a skipped turn: 5 + (11 − Speed).</summary>
    public static int SkippedTurnWait(int speed) => Wait(SkippedTurnDelay, speed);

    // ---------------------------------------------------------------- reading

    /// <summary>Time units until the entry arrives.</summary>
    public int Remaining(TimelineEntry entry) => entry.Due - Now;

    public TimelineEntry? Find(int entryId) => _entries.FirstOrDefault(e => e.Id == entryId);

    /// <summary>The unit's turn entry, or null (it is KO, or waits for blocking auto events).</summary>
    public TimelineEntry? TurnOf(int unitId) =>
        _entries.FirstOrDefault(e => e.Kind == TimelineKind.UnitTurn && e.OwnerUnitId == unitId);

    /// <summary>Every entry in the order it will arrive.</summary>
    public IReadOnlyList<TimelineEntry> Upcoming() => _entries.OrderBy(e => e.Due).ThenBy(e => e.Order).ToArray();

    public TimelineEntry? Peek() => _entries.Count == 0 ? null : Upcoming()[0];

    // ---------------------------------------------------------------- scheduling

    /// <summary>A request to put something on the timeline.</summary>
    public readonly record struct NewEntry(
        TimelineKind Kind, int Wait, int? OwnerUnitId = null, string? Tag = null, bool Blocking = false);

    /// <summary>
    /// Puts one entry on the timeline, <paramref name="wait"/> time units from now. It joins its time unit behind the
    /// entries already there. Wait must be at least 1.
    /// </summary>
    public TimelineEntry Add(TimelineKind kind, int wait, int? ownerUnitId = null, string? tag = null, bool blocking = false)
    {
        if (wait < 1) throw new ArgumentOutOfRangeException(nameof(wait), "A Wait is at least 1 time unit.");
        if (blocking && kind != TimelineKind.AutoEvent) throw new ArgumentException("Only auto events can be blocking.", nameof(blocking));
        if (blocking && ownerUnitId is null) throw new ArgumentException("A blocking auto event needs an owner.", nameof(ownerUnitId));
        if (kind == TimelineKind.UnitTurn)
        {
            if (ownerUnitId is null) throw new ArgumentException("A turn needs its unit.", nameof(ownerUnitId));
            if (TurnOf(ownerUnitId.Value) is not null) throw new InvalidOperationException($"Unit {ownerUnitId} already has a turn on the timeline.");
        }

        if (blocking)
        {
            // The owner's own turn waits until the last blocking auto event is done, then its cost starts (section 6.2).
            var owner = ownerUnitId!.Value;
            var turn = TurnOf(owner);
            if (turn is not null)
            {
                _deferredTurns[owner] = turn.LastWait;
                _entries.Remove(turn);
            }
        }

        var entry = new TimelineEntry(_nextId++, kind, Now + wait, _nextOrder++, ownerUnitId, tag, blocking, wait);
        _entries.Add(entry);
        return entry;
    }

    /// <summary>
    /// Puts an auto event on the timeline due in the current time unit, behind the entries already there (turret shots that resolve
    /// right after the Tinker's action). The only entry that may have no Wait.
    /// </summary>
    public TimelineEntry AddNow(int ownerUnitId, string tag)
    {
        var entry = new TimelineEntry(_nextId++, TimelineKind.AutoEvent, Now, _nextOrder++, ownerUnitId, tag, false, 0);
        _entries.Add(entry);
        return entry;
    }

    /// <summary>
    /// Puts several entries on the timeline at the same moment. Entries that fall into the same time unit are ordered by
    /// the seeded generator, every order equally likely (section 6.1: for example every unit's first turn at battle start).
    /// Returns the new entries in the order given.
    /// </summary>
    public IReadOnlyList<TimelineEntry> AddBatch(IReadOnlyList<NewEntry> requests, BattleRandom random)
    {
        var created = new TimelineEntry[requests.Count];
        // Create without order first, in a random order within each time unit, then number them.
        var indices = Enumerable.Range(0, requests.Count).ToList();
        var groups = indices.GroupBy(i => requests[i].Wait).OrderBy(g => g.Key);
        var sequence = new List<int>();
        foreach (var g in groups)
        {
            var list = g.ToList();
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = random.NextInt(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            sequence.AddRange(list);
        }
        foreach (var i in sequence)
        {
            var r = requests[i];
            created[i] = Add(r.Kind, r.Wait, r.OwnerUnitId, r.Tag, r.Blocking);
        }
        return created;
    }

    /// <summary>
    /// Battle start: every unit's first turn is 11 − Speed time units away. Turns in the same time unit are put in
    /// order by the seeded generator.
    /// </summary>
    public IReadOnlyList<TimelineEntry> ScheduleInitialTurns(IEnumerable<Unit> units, BattleRandom random) =>
        AddBatch(units.Select(u => new NewEntry(TimelineKind.UnitTurn, InitialWait(u.Speed), u.Id)).ToArray(), random);

    /// <summary>Schedules a unit's next turn after it used a skill of the given Delay (section 6.1). Null while blocking auto events hold the turn back.</summary>
    public TimelineEntry? ScheduleTurn(int unitId, int delay, int speed) =>
        ScheduleTurnWithWait(unitId, Wait(delay, speed));

    /// <summary>
    /// Schedules a unit's next turn with an explicit Wait. If the unit has blocking auto events pending, the turn is held
    /// back and starts only after the last one (returns null then).
    /// </summary>
    public TimelineEntry? ScheduleTurnWithWait(int unitId, int wait)
    {
        if (_entries.Any(e => e.Blocking && e.OwnerUnitId == unitId))
        {
            _deferredTurns[unitId] = wait;
            return null;
        }
        return Add(TimelineKind.UnitTurn, wait, unitId);
    }

    // ---------------------------------------------------------------- advancing

    /// <summary>
    /// Takes the next entry off the timeline and advances the clock to its time. Returns null when the timeline is empty.
    /// When it was the owner's last blocking auto event, the owner's held-back turn starts now.
    /// </summary>
    public TimelineEntry? Pop()
    {
        var next = Peek();
        if (next is null) return null;
        _entries.Remove(next);
        Now = Math.Max(Now, next.Due);
        ReleaseDeferredTurn(next);
        return next;
    }

    /// <summary>Removes an entry (its owner is KO, an effect was cleansed, Cut the Strings). Returns whether it was there.</summary>
    public bool Remove(int entryId)
    {
        var entry = Find(entryId);
        if (entry is null) return false;
        _entries.Remove(entry);
        ReleaseDeferredTurn(entry);
        return true;
    }

    /// <summary>
    /// Removes everything a unit owns: its pending auto events and delayed events and, with <paramref name="includeTurn"/>,
    /// its turn (section 10.3 pending events). A held-back turn is dropped with it. With <paramref name="keepEvents"/> only
    /// the turn goes: a skill stated that its launched events persist.
    /// </summary>
    public void RemoveOwnedBy(int unitId, bool includeTurn, bool keepEvents = false)
    {
        _entries.RemoveAll(e => e.OwnerUnitId == unitId
            && ((!keepEvents && e.Kind is TimelineKind.AutoEvent or TimelineKind.DelayedEvent)
                || (includeTurn && e.Kind == TimelineKind.UnitTurn)));
        if (includeTurn) _deferredTurns.Remove(unitId);
        else if (!_entries.Any(e => e.Blocking && e.OwnerUnitId == unitId) && _deferredTurns.TryGetValue(unitId, out var wait))
        {
            _deferredTurns.Remove(unitId);
            Add(TimelineKind.UnitTurn, wait, unitId);
        }
    }

    private void ReleaseDeferredTurn(TimelineEntry gone)
    {
        if (!gone.Blocking || gone.OwnerUnitId is not { } owner) return;
        if (_entries.Any(e => e.Blocking && e.OwnerUnitId == owner)) return;
        if (_deferredTurns.Remove(owner, out var wait) && TurnOf(owner) is null)
            Add(TimelineKind.UnitTurn, wait, owner);
    }

    // ---------------------------------------------------------------- moving entries

    /// <summary>
    /// Changes the remaining Wait by a number of time units (negative pulls forward). The turn never arrives before now:
    /// if more time is removed than remains, it arrives now and the extra is lost (overshoot, section 6.1). An entry that
    /// moves to a new time unit joins it behind the entries already there; one that stays keeps its place.
    /// </summary>
    public void AdjustRemaining(TimelineEntry entry, int delta)
    {
        var due = Math.Max(Now, entry.Due + delta);
        if (due == entry.Due) return;
        entry.Due = due;
        entry.Order = _nextOrder++;
    }

    /// <summary>Swap Turns: the two entries exchange their due times and their places in the tie order.</summary>
    public void SwapDue(TimelineEntry a, TimelineEntry b)
    {
        (a.Due, b.Due) = (b.Due, a.Due);
        (a.Order, b.Order) = (b.Order, a.Order);
    }

    /// <summary>Pull Forward: removes time units from the remaining Wait.</summary>
    public void PullForward(TimelineEntry entry, int timeUnits) => AdjustRemaining(entry, -Math.Abs(timeUnits));

    /// <summary>Push Back: adds time units to the remaining Wait.</summary>
    public void PushBack(TimelineEntry entry, int timeUnits) => AdjustRemaining(entry, Math.Abs(timeUnits));

    /// <summary>Reset: restores the full Wait of the last skill, starting from now.</summary>
    public void Reset(TimelineEntry entry) => AdjustRemaining(entry, Now + entry.LastWait - entry.Due);

    /// <summary>
    /// Haste or Slow on a unit: its Speed went from <paramref name="oldSpeed"/> to <paramref name="newSpeed"/> (both as the
    /// formula sees them, within 1–10). The remaining Wait changes by the same number of time units, opposite in sign;
    /// completed progress never changes (section 6.1). While the unit has blocking auto events pending, only those change.
    /// </summary>
    public void ChangeSpeed(int unitId, int oldSpeed, int newSpeed)
    {
        var delta = Math.Clamp(newSpeed, MinSpeed, MaxSpeed) - Math.Clamp(oldSpeed, MinSpeed, MaxSpeed);
        if (delta == 0) return;
        var blocking = _entries.Where(e => e.Blocking && e.OwnerUnitId == unitId).ToArray();
        if (blocking.Length > 0)
        {
            foreach (var e in blocking.OrderBy(e => e.Due).ThenBy(e => e.Order)) AdjustRemaining(e, -delta);
            return;
        }
        var turn = TurnOf(unitId);
        if (turn is not null) AdjustRemaining(turn, -delta);
    }
}
