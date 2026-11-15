# Production

Everything that is not a game mechanic: platforms, business model, saving, presentation, technology, online backend and the order of work. Game rules: [game_system.md](game_system.md). Screens and interactions: [ui_spec.md](ui_spec.md). Asset files: [assets/README.md](../assets/README.md).

## 1. Platforms, release and business model

### 1.1 Platforms

- **Release order.** Android and iOS first; Windows follows, sold on Steam.
- **Development order.** Windows and Android are built and tested first. iOS waits until a Mac with Xcode is available. Platform plugins for sign-in and purchases are picked when online play and purchases are built.
- **Minimum devices.** Godot's own minimum OS versions. The test device is a mid-range phone from about 2020; the game runs on phones and tablets.
- **Screen shapes.** 4:3 tablets to 21:9 phones. The board stays 16:9 and extra space goes to margins. Notches and other unsafe areas are kept clear.
- **Renderer.** Godot's Compatibility renderer (OpenGL) on every platform.
- **Controllers.** Supported on Windows; the Windows release aims for Steam Deck Verified (ui_spec.md, Controls).

### 1.2 Business model

- **Price.** One-time purchase: **4.99** on Steam, and on mobile a free download with the demo content plus a 4.99 unlock of the full game. Each store's suggested regional prices, no launch discount, no ads. No booster packs, paid random draws, trading or duplicate conversion.
- **Coins.** Earned in play (game_system.md §2.2) and also sold in three packs: **500, 1,200 and 2,500 coins for 1.99, 3.99 and 6.99**. They unlock specific creatures, never random ones. Packs are sold only after buying the full game, never in the demo.
- **Frames.** Every card wears its home biome's frame (`assets/frames/<biome_id>.png`). Frames never mark tier or class. Earned cosmetic frames are postponed; when they come, each chapter boss and a few harder achievements give one, they are never sold, and the player picks which creatures wear them.
- **Achievements.** About 30, listed in [campaign.md](campaign.md), in an in-game list mirrored to Steam, Google Play Games and Apple Game Center. None hidden, none need online play, no goal beyond about 100 wins.
- **Demo.** The tutorial and chapter 1, released together with the full game. On mobile the demo is the free download itself; on Steam a separate demo. Progress carries over on purchase. Online play needs the full game.
- **Store pages.** The Steam page goes up shortly before release, after the name check, led by a gameplay trailer of about 60 seconds from real battle footage.
  - *Genre line:* "A turn-based tactical creature card battler."
  - *Tagline:* "Gather your kin. Outthink the timeline."
  - *Main Steam tags:* Turn-Based Tactics, Card Battler, Creature Collector (not "Collectible Card Game").
  - *Description:* a short, warm storybook paragraph, then a feature list led by the tactics (timeline, formation, interception).
  - *Age rating target:* PEGI 7 / ESRB Everyone 10+: mild fantasy violence, no blood.
  - *Screenshots:* about five battles, then the team editor, the campaign map and the collection.
- **Playtesting.** A closed test with friends through TestFlight and Google Play internal testing before release.

### 1.3 Languages, accessibility and data

- **Languages.** English at release, more later. All text goes through translation keys.
- **Accessibility.** Statuses are readable without colour (icons with letters or short labels). Settings for reduced motion and UI text size.
- **Reporting.** Opt-in crash reports through Sentry's free plan. No gameplay tracking.

### 1.4 Saving

- **Profile.** One profile per device, stored locally, saved automatically after every battle, campaign step and battle action, so a closed battle resumes where it was. No manual save slots.
- **Format.** JSON with a version number; older saves are upgraded step by step when loaded.
- **Safety.** The game writes to a temporary file and swaps it in, and keeps the last three saves as backups. A damaged save loads the newest good backup with a short note.
- **Battles and updates.** A battle in progress resumes after an update only if the content it uses did not change; otherwise it is dropped with a short note.
- **Server profile.** Every profile also lives on the online server, tied to the platform account; the server is the authority for the collection, coins and purchases. The device copy keeps offline play working and syncs when online:
  - The game starts and plays offline, even on first launch; the server profile is created at the first connection.
  - Campaign progress and creatures won offline are accepted when the device syncs.
  - **Coins** exist only on the server: they are earned, spent and bought only while connected, so an offline random battle pays nothing and coin creatures cannot be bought offline.
  - Two devices on one account merge: every creature from both and the furthest campaign progress.
  - Coin purchases are checked by the server against the store receipt, and the server adds the coins.
  - "Delete my online data" removes the server profile, rating and battle records; the device copy stays and becomes a fresh server profile when the player goes online again.
  - If the online service shuts down, the game keeps working offline with everything the player owns.
- **No store cloud saves** (Steam Cloud, Google Play saved games, iCloud): the server profile is the backup. Profiles never move between platform families.

### 1.5 Story presentation

No story text and no player character: campaign locations show their illustrations, and chapters and encounters have short storybook names on the map as labels (game_system.md §15). No reason is given for why a beaten creature joins.

### 1.6 Art, audio and settings

- **Art.** One image per creature in `assets/creatures/`, 1024 × 1536 (2:3), transparent, standing on a shared ground line, named by the Art file column of creature-classes.md. Cards do not show creature size: every creature fills its frame. The card front shows the Front frame crop, the back the Rear frame crop (creature-classes.md, Card details).
- **Card backgrounds.** One faceted background per biome behind the creature.
- **Battle backgrounds.** One photo-real, dimmed night scene per biome, 16:9, calm in the centre. Campaign battles use their encounter's biome; free and random battles a random biome the player has reached.
- **UI look.** Night fairytale: menus on an illustrated fairytale background, with colours, panels and buttons styled to fit.
- **Logo and icon.** A hand-lettered gold storybook wordmark with a small emblem; the app icon is the emblem in gold on a plum night sky (`assets/brand/`).
- **Splash and title.** A short splash with the logo while loading, then the main menu as title screen, with the logo above the menu. No Godot logo, no animated intro.
- **Fonts.** **Alegreya** for headings and names, **Alegreya Sans** for body text and numbers (SIL Open Font License; the licence files ship with the game).
- **Credits.** None in the game.
- **Idle motion.** None. Portraits and cards react only to hits and effects.
- **Music.** Quiet ambient loops only, no melodic soundtrack.
- **Sounds.** One set per kind of effect, no creature cries:
  - *Interface:* confirm, cancel, invalid action, panel open and close, hover, tab switch, drag pickup and drop, card flip; a soft coin sound when coins are gained or spent.
  - *Battle:* three variants of each, picked at random with a small random pitch change: melee, projectile, magic, heal, statuses (helpful, harmful, ending), interception, Ward absorb, bump, stealth on and off, summon, each ground effect and the gas explosion. A soft chime when one of the player's units gets its turn (none for enemy turns). Momentum sounds only at a threshold and at the end of the bar. KO: a glassy crack, then a soft thud.
  - *Result:* a short non-melodic sting (about 2 seconds) for victory and one for defeat.
  - *Ambient:* one loop per biome; the campaign map plays the current chapter's loop, other menus a random biome's.
  - *Format:* OGG Vorbis 48 kHz, effects mono, ambient stereo, levelled to the same loudness.
- **Visual effects.**
  - *Style:* faceted shards and triangle sparks for physical hits; soft glow and light for magic and healing. Made in the engine with particles and shaders; drawn sprites only where they are clearly better.
  - *Length:* each skill takes the time it needs to read clearly, never more than 3 seconds.
  - *Melee:* no unit motion; a slash or impact on the target.
  - *Projectile:* a visible shot travels from attacker to target along a short arc.
  - *Direct:* by default a glow gathers on the caster, then the effect bursts on the target; a skill may show it differently where that suits it.
  - *Interception:* the defender flashes and slides briefly in front of the target as it takes the hit.
  - *Healing:* small motes rise and the portrait glows softly once.
  - *Statuses:* a short effect when a status is applied (poison bubbles, ice and so on).
  - *Summons:* the portrait assembles from shards on its spot.
  - *Momentum:* the marker slides with a short spark trail, with a bigger flare at the thresholds.
- **Vibration.** Light vibration on hits and KOs on phones, with a setting to turn it off.
- **Settings.** Music and sound volume, reduced motion, text size, vibration, language, crash reports, and key and controller bindings on Windows. No battle speed setting; only the development battle viewer has speed controls.
- **Performance.** The game must feel smooth: about 30 frames per second on the test phone and 60 on Windows, and the AI deciding within about 2 seconds.
- **Download size.** The mobile download stays under 200 MB. Creature and summon art is imported as lossy WebP, and mobile builds use 768 × 1152 copies; Windows keeps full size.

## 2. Technology

### 2.1 Foundation

- **Engine and language.** Godot 4.7 .NET, C# everywhere, including the UI.
- **Rules core.** A plain C# library with no Godot dependency, used by the game, the batch runner and the online battle service.
- **Batch runner.** A .NET command-line program on the rules core that runs AI-versus-AI batches (game_system.md §2.5).
- **Code location.** One repository: the core, the runner and the battle service in `src/`, the Spring server in `server/`, the Godot project at the root.
- **Content.** JSON files in `content/`, one per creature list, class and status set, using stable text IDs (unit names; skill names in snake case, Heavy Blow → `heavy_blow`; biome IDs from assets/README.md). They are generated from the document tables by `tools/content/build_content.py`, and a test checks that they match. Delivery, Delay and Momentum live in JSON for every skill. Unique attacks are fully data: the builder turns their effect text into JSON fields (damage, area, extra, chance, duration) and fails on any wording it does not know. Class skills and keystones keep their numbers in C# beside their mechanic.
- **Randomness.** One seeded random generator per battle, in the core (never the platform's). The AI uses its own seeded generator for weaker picks and never draws from the battle's.
- **Tests.** xUnit. Every worked check in game_system.md §20.5 is an automated test, plus unit tests per rule.
- **Where it runs.** Builds, tests and batches run on the owner's PC; implementing sessions work there through Remote Control. No GitHub CI, nothing built in cloud sessions.
- **.NET version.** The newest LTS .NET that Godot 4.7 supports, shared by all projects. Nullable reference types and warnings as errors in the core and the runner; relaxed in the Godot project.

### 2.2 Rules core

- **State.** A mutable battle state with a fast `Clone()`; the AI search clones before trying an action.
- **Events.** Every action returns an ordered list of events (hit, intercept, move, status applied, Momentum shift, KO), which the UI plays.
- **Authority.** Only the core changes battle state. Godot sends chosen actions in and reads state and events out.
- **Previews.** The core answers preview queries (damage ranges, interception chances, timeline position, forced movement) with the same code that resolves actions, without changing state.
- **Numbers.** Integers only: percentages as whole numbers, coefficients in hundredths, explicit rounding down. Damage that builds up in fractions (Poisoned, ground effects) is kept in hundredths of HP and taken as whole points.
- **Skill code.** One small C# class per skill, found by its skill ID. Shared behaviour (interception, area shapes, forced movement and so on) lives in reusable classes that skills delegate to, so no rule is written twice.
- **Broken content.** Content is validated at load; any error stops with a message naming the file and ID.
- **Parallel runs.** The runner plays battles in parallel on all CPU cores; each battle is single-threaded and deterministic.
- **Run output.** One folder per run: a snapshot (content JSON, git commit, AI settings), one CSV row per battle, and a Markdown report.

### 2.3 AI search and Godot app

- **Search.** Alpha-beta minimax with iterative deepening up to the difficulty's depth. All legal actions at the top; deeper down only the best few candidates (about 8) by a quick score. Fixed depth, no time cap; levels are tuned so the hardest stays quick on a mid-range phone.
- **Cards.** One reusable Card scene with display modes, used on every screen.
- **Input.** Godot input actions: mouse, touch, keyboard and controller map to the same commands (select, confirm, inspect, cancel).
- **Animation.** Battle events play one after another from a queue, each with its own duration, always at one speed (the battle viewer can speed it up).
- **Resolution.** Designed for 1920 × 1080 and scaled for phones.
- **Text.** All UI and content text through translation keys, English only at first.
- **Battle viewer.** Pause, step one action, speed (1x, 2x, instant), inspect any unit, and start from a recorded setup.

### 2.4 Online backend

- **Two services.** A **Spring Boot (Java)** server for sign-in, profiles, collections, coins, purchases, matchmaking and ratings, and an **ASP.NET Core (C#) battle service** that runs the rules core and plays every online battle. The server decides every result; clients send their chosen actions and play the returned events. Spring hands both players a signed match ticket; the game connects to the battle service, which checks the ticket with a shared key and reports the result back to Spring over internal HTTP.
- **Connections.** HTTPS to Spring; during a battle a **SignalR** connection (WebSockets) to the battle service, which handles reconnecting and the turn timer.
- **Sign-in.** Automatic with the platform account (Steam, Google Play Games, Apple Game Center); no Everkin accounts. The game sends its platform proof once; Spring checks it and hands out a short-lived access token plus a refresh token. Play is cross-platform in one pool.
- **Spring.** Newest Java LTS and Spring Boot, Maven, Spring MVC on virtual threads, Spring Data JPA (Hibernate), Flyway migrations. It reads the same JSON content as the game, bundled at build time.
- **Database.** PostgreSQL; battle records in the battle service's own schema.
- **Matchmaking.** An in-memory queue in Spring; after a restart, waiting players queue again. Hidden rating with Glicko-2. Only players on the same game version are matched; during a rollout the battle service runs the old and the new version side by side for about two weeks.
- **Restarts.** Each online battle's seed, teams and actions are stored as they happen; after a restart running battles are rebuilt by replay and continue.
- **Trust.** The server checks that a player owns every creature in their team and keeps the coin balance. A cheater is banned from online play; offline play keeps working.
- **Profile sync.** The device sends what changed since its last sync; the server answers with the whole merged profile.
- **Battle records.** Finished online battles are kept 30 days for bug hunting and cheat checks; players do not see them.
- **API versions.** The game sends its version with every request; Spring keeps old behaviour until that version is retired.
- **Request limits.** Per player in Spring, per address in Caddy.
- **Hosting.** One Hetzner server in Europe with both services in Docker Compose behind Caddy (automatic HTTPS), plus a staging setup with its own database for development builds. A script on the owner's PC builds and deploys. Budget up to about 30 euros a month.
- **Secrets.** In an env file on the server only, never in the repository; the owner's PC keeps an encrypted copy.
- **Operations.** Nightly database dump copied off the server, kept 30 days; server logs kept 14 days; health endpoints and an external uptime check that alerts the owner. Tests with JUnit 5 and Testcontainers.
- **Privacy.** A short privacy policy page on the game's website before release; "Delete my online data" in Settings.

## 3. Order of work

The specification is finished before implementation. The tasks are in [implementation-plan.md](implementation-plan.md), the live state in [PROGRESS.md](../PROGRESS.md), and how a session works in [CLAUDE.md](../CLAUDE.md). After the AI-versus-AI milestone: battle viewer, free battle against the AI, tutorial and chapter 1, local two-player, the rest of the campaign, post-story content, online play, release. Online play ships in the first release. Code and docs go straight to `main`, without pull requests.

## 4. Open

- The online server's domain (everkin.com is taken).
- The message format between the game and the servers, written when online play is built.
- The availability check for the name Everkin (trademarks, stores, domains) before a store page goes up (game_system.md §18).
