using System.Collections;

namespace Everkin.Core.Battle;

/// <summary>
/// An append-only list that copies cheaply: <see cref="Share"/> hands out a second list over the same items and whichever of the two is
/// added to first copies them. The battle state's hook lists are filled at battle start and almost never change afterwards, while the AI
/// search clones the state hundreds of thousands of times, so they are shared instead of copied.
/// </summary>
public sealed class CowList<T> : IReadOnlyList<T>
{
    private List<T> _items = new();
    private bool _shared;

    public int Count => _items.Count;

    public T this[int index] => _items[index];

    public bool Contains(T item) => _items.Contains(item);

    public void Add(T item)
    {
        if (_shared)
        {
            _items = new List<T>(_items);
            _shared = false;
        }
        _items.Add(item);
    }

    /// <summary>A list with the same items that does not change when this one is added to, and the other way round.</summary>
    public CowList<T> Share()
    {
        _shared = true;
        return new CowList<T> { _items = _items, _shared = true };
    }

    public List<T>.Enumerator GetEnumerator() => _items.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}
