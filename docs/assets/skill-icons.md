# Skill icons for the action bar

The action bar (ui_spec, Battle screen) shows each skill as a square icon slot with the name below, Delay in a corner and the Momentum cost in a gold gem. The game draws the slot frame, the name, the key number and the gems; the icon is only the picture inside.

Every slot has its own icon (ui_spec): **84 class actives**, **319 unique attacks** and Move and Skip Turn, **405 icons**. Keystones never sit on the bar and have no icon. Each file is named after the skill ID: the skill or attack name in snake case, apostrophes and punctuation dropped (`hunters_shot.png`, `spotted.png`). Save to `assets/icons/skills/<file>`.

**Status (2026-11-13).** Only `action-move.png` and `action-skip.png` are final. The 47 old `attack-<type>.png` and `<profession>-<branch>.png` icons stand in until the new set exists and are deleted then (milestone M5.6). The prompts of the old run are kept in [skill-icons-prompts-used.json](skill-icons-prompts-used.json).

## Style block (paste after the subject)

```
Make ONE separate square game icon for the subject described above, for the action bar of the Everkin card game (a night fairytale creature card battler).

FORMAT
- PNG, 512 × 512 px, square, full bleed (no transparent border, no rounded corners, no frame: the game draws its own bronze slot frame).
- No text, letters, numbers or logos.

STYLE
- One bold central symbol that reads clearly at 64 × 64 px. Simple silhouette first, detail second.
- Chunky low-poly faceted style matching the game's creature art: broad flat facets, matte colors, soft light from the upper left.
- Background: a dark plum-to-midnight-blue gradient with a faint vignette, the same on every icon so the set reads as one.
- The symbol uses the given main color plus one brighter accent glow. High contrast against the dark background.
- Storybook tone for ages 7 and up: no blood, gore or skulls dripping anything.
```

## Move and Skip Turn (final)

| File | For | Subject |
|---|---|---|
| `action-move.png` | Move | Two curved arrows, one up and one down, around a small card shape. Cream with gold accent. |
| `action-skip.png` | Skip Turn | A small hourglass with a curved "pass" arrow over it. Cream with blue accent. |

## Class actives (84)

One main color per class keeps a class's three icons readable as a group. Classes and skills: [class_system.md](../class_system.md), [skills.md](../skills.md).

| File | Skill (class) | Subject and color |
|---|---|---|
| `vanish.png` | Vanish (Assassin) | A hooded silhouette dissolving into teal smoke wisps. Charcoal with teal accent. |
| `feint.png` | Feint (Assassin) | A dagger tip making a fake curl, a dizzy spiral beside it. Charcoal with teal accent. |
| `backstab.png` | Backstab (Assassin) | A dagger striking from behind a dark crescent of shadow. Charcoal with teal accent. |
| `target.png` | Target (Tracker) | A paw print inside a glowing target ring. Forest green with amber accent. |
| `read_tracks.png` | Read Tracks (Tracker) | A row of paw prints with a magnifying eye glowing above them. Forest green with amber accent. |
| `hunters_shot.png` | Hunter's Shot (Tracker) | An arrow flying straight into the centre of a target ring. Forest green with amber accent. |
| `leap_to_defend.png` | Leap to Defend (Bulwark) | A tower shield leaping forward in an arc with motion lines. Steel blue with white accent. |
| `rescue.png` | Rescue (Bulwark) | Two shields swapping places along a curved double arrow. Steel blue with white accent. |
| `shield_bash.png` | Shield Bash (Bulwark) | A shield rim slamming forward with a backward-pushing shock wave. Steel blue with white accent. |
| `ward.png` | Ward (Keeper) | A single glowing hexagonal ward bubble. Pale cyan with white accent. |
| `shield_dome.png` | Shield Dome (Keeper) | A wide dome of small hexagon panels. Pale cyan with white accent. |
| `ward_burst.png` | Ward Burst (Keeper) | A hexagon ward shattering outward into bright shards. Pale cyan with white accent. |
| `mend.png` | Mend (Mender) | Cupped hands holding a warm glowing droplet. Warm gold with soft green accent. |
| `rain_of_mending.png` | Rain of Mending (Mender) | Golden drops falling from a small cloud. Warm gold with soft green accent. |
| `searing_light.png` | Searing Light (Mender) | A beam of hard white-gold light striking down. Warm gold with soft green accent. |
| `absolve.png` | Absolve (Cleric) | A dark chain breaking apart in a gentle golden glow. White and gold with sky blue accent. |
| `dispel.png` | Dispel (Cleric) | A sparkle being wiped away by an open palm. White and gold with sky blue accent. |
| `turn_back.png` | Turn Back (Cleric) | A dark sigil bouncing back along a curved return arrow. White and gold with sky blue accent. |
| `burnout_heal.png` | Burnout Heal (Lifebinder) | A heart-shaped flame burning out into warm sparks. Rose with gold accent. |
| `life_for_life.png` | Life for Life (Lifebinder) | A heart split in two, one half flowing into a faded heart. Rose with gold accent. |
| `bound_resurrect.png` | Bound Resurrect (Lifebinder) | Two hearts tied by a glowing thread, one rising. Rose with gold accent. |
| `echo_enchant.png` | Echo Enchant (Enchanter) | A sword with a glowing double echo outline behind it. Violet with pink accent. |
| `phase_weapon.png` | Phase Weapon (Enchanter) | A ghostly translucent blade passing through a small shield. Violet with pink accent. |
| `piercing_enchant.png` | Piercing Enchant (Enchanter) | A glowing arrow piercing through two stacked rings. Violet with pink accent. |
| `transmute.png` | Transmute (Transmuter) | Two swirling orbs trading places in a figure-eight. Amethyst with stone grey accent. |
| `glass_body.png` | Glass Body (Transmuter) | A cracked glass figure silhouette with a glint. Amethyst with stone grey accent. |
| `petrify.png` | Petrify (Transmuter) | A paw turning to grey stone from the tip. Amethyst with stone grey accent. |
| `curse_of_betrayal.png` | Curse of Betrayal (Hexer) | A small heart pulled away by a hooked hex thread. Dark purple with green accent. |
| `curse_of_weight.png` | Curse of Weight (Hexer) | A heavy iron weight with a violet hex sigil on it. Dark purple with green accent. |
| `withering_curse.png` | Withering Curse (Hexer) | A wilting leaf wrapped in a curling hex sigil. Dark purple with green accent. |
| `fireball.png` | Fireball (Pyromancer) | A roaring fireball with a short flame tail. Orange and red with yellow core. |
| `kindle.png` | Kindle (Pyromancer) | A small flame catching on a spot of cracked ground. Orange and red with yellow core. |
| `row_of_fire.png` | Row of Fire (Pyromancer) | A low wall of flames in one straight line. Orange and red with yellow core. |
| `lightning_bolt.png` | Lightning Bolt (Stormcaller) | A single forked lightning bolt striking down. Yellow and slate with white accent. |
| `chain_lightning.png` | Chain Lightning (Stormcaller) | A lightning arc jumping between three small orbs. Yellow and slate with white accent. |
| `tempest.png` | Tempest (Stormcaller) | A swirling storm gust with a backward arrow. Yellow and slate with white accent. |
| `ice_lance.png` | Ice Lance (Cryomancer) | A long sharp shard of ice flying diagonally. Ice blue with white glow. |
| `freeze.png` | Freeze (Cryomancer) | A solid ice block with a snowflake inside. Ice blue with white glow. |
| `frost_nova.png` | Frost Nova (Cryomancer) | A ring of ice spikes bursting outward. Ice blue with white glow. |
| `rampage.png` | Rampage (Berserker) | A huge axe smashing down with a red fury aura. Crimson with orange accent. |
| `sweeping_kick.png` | Sweeping Kick (Berserker) | A clawed foot sweeping in a wide arc, a push-back arrow. Crimson with orange accent. |
| `battle_roar.png` | Battle Roar (Berserker) | An open roaring maw with shock rings. Crimson with orange accent. |
| `challenge.png` | Challenge (Champion) | A gauntlet thrown down before a target mark. Silver with red accent. |
| `single_combat.png` | Single Combat (Champion) | Two crossed swords inside a ring. Silver with red accent. |
| `execution.png` | Execution (Champion) | A great sword coming down on a target mark. Silver with red accent. |
| `swap_turns.png` | Swap Turns (Tactician) | An hourglass with two crossing arrows. Royal blue with gold accent. |
| `press_the_advantage.png` | Press the Advantage (Tactician) | A chess knight leaping forward with a golden spark. Royal blue with gold accent. |
| `coordinated_strike.png` | Coordinated Strike (Tactician) | Two arrows converging on one target point. Royal blue with gold accent. |
| `advance.png` | Advance (Commander) | A pennant banner with a forward arrow. Royal blue with bronze accent. |
| `switch_places.png` | Switch Places (Commander) | Two small banners swapping along a double arrow. Royal blue with bronze accent. |
| `rotation.png` | Rotation (Commander) | Three small banners on a circular arrow. Royal blue with bronze accent. |
| `song_of_haste.png` | Song of Haste (Bard) | A lute with quick swirling notes and speed lines. Magenta with gold accent. |
| `war_drums.png` | War Drums (Bard) | A war drum with two sticks and bold sound rings. Magenta with gold accent. |
| `lullaby.png` | Lullaby (Bard) | A crescent moon with soft floating notes and a "z" curl shape (no letters). Magenta with gold accent. |
| `melt_away.png` | Melt Away (Thief) | A hooded figure melting into a dark puddle of shadow. Charcoal with gold accent. |
| `lift_a_skill.png` | Lift a Skill (Thief) | A gloved hand plucking a glowing spark from the air. Charcoal with gold accent. |
| `snatch.png` | Snatch (Thief) | A quick grabbing claw holding a small gold pouch. Charcoal with gold accent. |
| `flash_powder.png` | Flash Powder (Saboteur) | A bright burst of powder with a closed eye. Soot grey with yellow accent. |
| `cut_the_strings.png` | Cut the Strings (Saboteur) | Scissors snipping a taut glowing string. Soot grey with yellow accent. |
| `jam.png` | Jam (Saboteur) | A gear jammed by a wedge, sparks flying. Soot grey with yellow accent. |
| `bolt_turret.png` | Bolt Turret (Engineer) | A small crossbow turret on a tripod. Brass with green lens accent. |
| `barricade.png` | Barricade (Engineer) | Crossed timber planks with an iron-banded board. Brass with green lens accent. |
| `net_trap.png` | Net Trap (Engineer) | A coiled rope net with a small bell. Brass with green lens accent. |
| `poison_gas.png` | Poison Gas (Chemist) | A cracked flask releasing a green gas cloud. Lime green and glass with orange accent. |
| `firebomb.png` | Firebomb (Chemist) | A round flask with a lit fuse bursting into flame. Lime green and glass with orange accent. |
| `healing_draught.png` | Healing Draught (Chemist) | A corked bottle of glowing red-pink potion with a heart bubble. Lime green and glass with orange accent. |
| `raise_undead.png` | Raise Undead (Necromancer) | A pale green spirit flame rising from a small grave mound. Bone white with sickly green accent. |
| `corpse_explosion.png` | Corpse Explosion (Necromancer) | A green gas cloud bursting from a cracked bone. Bone white with sickly green accent. |
| `desecrate.png` | Desecrate (Necromancer) | A small grave stone with a dark sealing sigil. Bone white with sickly green accent. |
| `clay_golem.png` | Clay Golem (Summoner) | A stout clay golem fist rising from a summoning circle. Stone grey with purple runes. |
| `rune_golem.png` | Rune Golem (Summoner) | A stone golem head with glowing purple runes. Stone grey with purple runes. |
| `animate_blades.png` | Animate Blades (Summoner) | Two floating blades circling a glowing ring. Stone grey with purple runes. |
| `healing_totem.png` | Healing Totem (Totemist) | A smiling totem top with a green glowing crystal. Wood brown with green accent. |
| `spirit_link_totem.png` | Spirit Link Totem (Totemist) | A stone totem with two faces joined by a carved chain. Wood brown with green accent. |
| `totem_of_return.png` | Totem of Return (Totemist) | An owl-shaped totem with an hourglass carved in its chest. Wood brown with green accent. |
| `rain_dance.png` | Rain Dance (Ritualist) | Rain falling from a cloud over a small ritual circle of stones. Sea blue with white accent. |
| `hoarfrost.png` | Hoarfrost (Ritualist) | Frost crystals spreading over a stone. Sea blue with white accent. |
| `rite_of_mists.png` | Rite of Mists (Ritualist) | Swirling mist over a still pool. Sea blue with white accent. |
| `puppet_soul.png` | Puppet Soul (Spiritwalker) | A ghostly hand with spirit strings pulling a small figure. Turquoise with pale accent. |
| `life_swap.png` | Life Swap (Spiritwalker) | Two hearts trading places along a looping arrow. Turquoise with pale accent. |
| `forced_step.png` | Forced Step (Spiritwalker) | A footprint pushed by a ghostly hand. Turquoise with pale accent. |
| `take_off.png` | Take Off (Skyborne) | A spread pair of wings lifting off with an up arrow. Sky blue and white with gold accent. |
| `gust.png` | Gust (Skyborne) | A gust of wind swirls blowing sideways. Sky blue and white with gold accent. |
| `dive.png` | Dive (Skyborne) | A diving bird silhouette plunging down with speed lines. Sky blue and white with gold accent. |

## Unique attacks (319)

Each creature's attack ([creature-classes.md](../creature-classes.md)). **Attach the creature's card art** (`assets/creatures/<art file>`) as a reference for the weapon, prop or body part and its colors, and add this line before the subject: "Use the attached creature only as a reference for the weapon, prop or body part and its colors. Do not draw the whole creature." The icon shows the attack's motion (from its animation style) and one small accent for its extra. Colors come from the reference; the accent keeps the color of the matching status (ui-icons.md).

When an attack changes in creature-classes.md, change its row here: the file name follows the attack name, the motion follows the style column, the accent follows the effect.

| File | Attack (creature) | Reference art | Subject |
|---|---|---|---|
| `bead_count.png` | Bead Count (Tallymarch) | `abacus-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a small hourglass. |
| `sour_chord.png` | Sour Chord (Wheezewarden) | `accordion-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `chain_haul.png` | Chain Haul (Barnacle Bulwark) | `anchor-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a hooked arrow pulling back toward the attacker. |
| `tail_club.png` | Tail Club (Clubtail Grumbleback) | `ankylosaur-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `rivet_shot.png` | Rivet Shot (Cogmarch Ant) | `ant-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: three little stars circling. |
| `scale_swipe.png` | Scale Swipe (Armadillo Recruit) | `armadillo-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `shield_rush.png` | Shield Rush (Armadillo Knight) | `armadillo-02.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: three little stars circling. |
| `ring_spin.png` | Ring Spin (Starwright Judge) | `astrolabe-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small hourglass. |
| `trample.png` | Trample (Ironhide Aurochs) | `aurochs-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `bubble_orb.png` | Bubble Orb (Ripplequill) | `axolotl-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a splash of water below. |
| `wick_flicker.png` | Wick Flicker (Candlefinger) | `ayeaye-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: small flames licking up from below. |
| `drumroll.png` | Drumroll (Drumbadger) | `badger-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `sand_spill.png` | Sand Spill (Sandbag Sentinel) | `ballast-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a puff of grey-violet powder. |
| `stone_gaze.png` | Stone Gaze (Stonegaze Basilisk) | `basilisk-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: three little stars circling. |
| `reed_lash.png` | Reed Lash (Dirgewind) | `bassoon-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a small pink spiral. |
| `screech.png` | Screech (Moonwing) | `bat-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `shadow_carve.png` | Shadow Carve (Bat Rogue) | `bat-02.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `ribbon_lash.png` | Ribbon Lash (Marshal Tempo) | `baton-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a hooked arrow pulling back toward the attacker. |
| `lamp_beam.png` | Lamp Beam (Lighthouse Wakelamp) | `beacon-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `pump_spray.png` | Pump Spray (Pumpwhisker) | `beaver-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a splash of water below. |
| `sun_sting.png` | Sun Sting (Sunhum) | `bee-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a toxic green drop with bubbles. |
| `beetle_butt.png` | Beetle Butt (Beetle Recruit) | `beetle-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `carapace_bash.png` | Carapace Bash (Beetle Guardian) | `beetle-02.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `sprout_shot.png` | Sprout Shot (Seedshot Scarab) | `beetle-04.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small green gas cloud. |
| `iron_stomp.png` | Iron Stomp (Ironbound Behemoth) | `behemoth-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `toll.png` | Toll (Clapperward) | `bell-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `bellows_spit.png` | Bellows Spit (Puffing Forgedrake) | `bellows-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: small flames licking up from below. |
| `vine_lash.png` | Vine Lash (Vinelilt) | `binturong-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a hooked arrow pulling back toward the attacker. |
| `petal_pierce.png` | Petal Pierce (Bloomspear) | `bloomspear-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `magma_tusk.png` | Magma Tusk (Magma Hog) | `boar-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: small flames licking up from below. |
| `cursed_gore.png` | Cursed Gore (Hexbristle) | `boar-02.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `acid_throw.png` | Acid Throw (Acidpop) | `bombardierbeetle-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small green gas cloud. |
| `paper_cut.png` | Paper Cut (Foldtome) | `book-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a single stylised red drop (not gore). |
| `ember_toss.png` | Ember Toss (Coalwraith Brazier) | `brazier-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: small flames licking up from below. |
| `root_snap.png` | Root Snap (Rootyoke Buffalo) | `buffalo-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: roots curling up to bind. |
| `fanfare.png` | Fanfare (Skybrass) | `bugle-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `hot_wax.png` | Hot Wax (Waxbrawler) | `candelabrum-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: roots curling up to bind. |
| `cannon_shot.png` | Cannon Shot (Powderbelch) | `cannon-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: three little stars circling. |
| `water_splash.png` | Water Splash (Calmwater Capybara) | `capybara-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a splash of water below. |
| `spear_throw.png` | Spear Throw (Tuftspear) | `caracal-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a single stylised red drop (not gore). |
| `carillon_peal.png` | Carillon Peal (Chimespire) | `carillon-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `horn_call.png` | Horn Call (Crestcaller) | `cassowary-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small pink spiral. |
| `silk_spit.png` | Silk Spit (Caterpillar Bard) | `caterpillar-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: roots curling up to bind. |
| `moonlit_feint.png` | Moonlit Feint (Moonclaw Spellblade) | `catfolk-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a small pink spiral. |
| `venom_fang.png` | Venom Fang (Sicklecoil) | `centipede-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a toxic green drop with bubbles. |
| `frost_spill.png` | Frost Spill (Stormgoblet) | `chalice-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a frosty ice patch below. |
| `snag_and_vanish.png` | Snag and Vanish (Shiftscale) | `chameleon-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: the strike fading into teal smoke. |
| `wax_drip.png` | Wax Drip (Candlecrown) | `chandelier-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: small flames licking up from below. |
| `lid_snap.png` | Lid Snap (Lockjaw Chest) | `chest-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: roots curling up to bind. |
| `lullnote.png` | Lullnote (Ocarina Puff) | `chinchilla-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `wing_buzz.png` | Wing Buzz (Chimewing Cicada) | `cicada-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `cinder_horn.png` | Cinder Horn (Cinderhorn) | `cinderhorn-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: small flames licking up from below. |
| `snake_eyes.png` | Snake Eyes (Diceroll Coati) | `coati-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small pink spiral. |
| `venom_spray.png` | Venom Spray (Hoodspitter) | `cobra-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a puff of grey-violet powder. |
| `needle_point.png` | Needle Point (Northneedle) | `compass-02.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `gramophone_blare.png` | Gramophone Blare (Conchhorn) | `conchphone-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small pink spiral. |
| `twist_in.png` | Twist In (Twistcork) | `corkscrew-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `flask_toss.png` | Flask Toss (Crab Alchemist) | `crab-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small green gas cloud. |
| `hammer_claw.png` | Hammer Claw (Crab Smith) | `crab-02.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `pincer_grip.png` | Pincer Grip (Decoy Crab) | `crab-03.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: roots curling up to bind. |
| `hook_swing.png` | Hook Swing (Hoist Titan) | `crane-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a hooked arrow pulling back toward the attacker. |
| `storm_plume.png` | Storm Plume (Featherveil Crane) | `crane-02.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: three little stars circling. |
| `fiddle_shriek.png` | Fiddle Shriek (Fiddlewhit) | `cricket-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small pink spiral. |
| `death_roll.png` | Death Roll (Crocodile Shaman) | `crocodile-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: roots curling up to bind. |
| `grave_peck.png` | Grave Peck (Skullcaw) | `crow-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `molten_pour.png` | Molten Pour (Moltencup) | `crucible-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: small flames licking up from below. |
| `ink_cloud.png` | Ink Cloud (Needleglow) | `cuttlefish-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a puff of grey-violet powder. |
| `antler_toss.png` | Antler Toss (Glade Stag) | `deer-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `yawn.png` | Yawn (Bellnap Dormouse) | `dormouse-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `ember_breath.png` | Ember Breath (Emberwyrm Colossus) | `dragon-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: small flames licking up from below. |
| `glass_shard_breath.png` | Glass Shard Breath (Obsidian Elder) | `dragon-02.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a single stylised red drop (not gore). |
| `tempest_breath.png` | Tempest Breath (Tempest Sovereign) | `dragon-03.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a short arrow pointing away. |
| `glacier_breath.png` | Glacier Breath (Glacier Titan) | `dragon-04.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a frosty ice patch below. |
| `reed_arrow.png` | Reed Arrow (Reedarrow) | `dragonfly-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small hourglass. |
| `prism_breath.png` | Prism Breath (Crystalback Drake) | `drake-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a puff of grey-violet powder. |
| `drawbridge_drop.png` | Drawbridge Drop (Chaingate) | `drawbridge-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `war_throb.png` | War Throb (Wardrum) | `drum-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `tidal_slap.png` | Tidal Slap (Crystalfin Dugong) | `dugong-02.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a splash of water below. |
| `idol_stomp.png` | Idol Stomp (Idolbearer) | `elephant-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `mask_swap.png` | Mask Swap (Maskwhisk) | `ferret-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a stolen sparkle flying back to the attacker. |
| `big_claw_pinch.png` | Big Claw Pinch (Coinclaw) | `fiddlercrab-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: roots curling up to bind. |
| `lantern_flash.png` | Lantern Flash (Lanternwing) | `firefly-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a puff of grey-violet powder. |
| `glow_dart.png` | Glow Dart (Firefly) | `firefly-02.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `dart_shot.png` | Dart Shot (Dartfin) | `flyingfish-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: three little stars circling. |
| `harpoon_throw.png` | Harpoon Throw (Harpoon Fossa) | `fossa-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a hooked arrow pulling back toward the attacker. |
| `sly_nip.png` | Sly Nip (Fox Bard) | `fox-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a stolen sparkle flying back to the attacker. |
| `throat_boom.png` | Throat Boom (Throatdrum) | `frigatebird-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `leaping_stab.png` | Leaping Stab (Frog Bard Dagger) | `frog-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `furnace_blast.png` | Furnace Blast (Furnace Titan) | `furnace-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: small flames licking up from below. |
| `petrifying_glare.png` | Petrifying Glare (Stonewatch Gargoyle) | `gargoyle-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: roots curling up to bind. |
| `armor_stomp.png` | Armor Stomp (Bolted Gaur) | `gaur-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `resin_flask.png` | Resin Flask (Resinpaw) | `gecko-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small green gas cloud. |
| `split_beam.png` | Split Beam (Prismtoe) | `gecko-02.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `horn_butt.png` | Horn Butt (Totemhorn) | `goat-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `holy_pour.png` | Holy Pour (Grailward) | `goblet-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a splash of water below. |
| `furnace_claw.png` | Furnace Claw (Furnaceheart Colossus) | `golem-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: small flames licking up from below. |
| `steam_vent.png` | Steam Vent (Gearwright Golem) | `golem-02.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small pink spiral. |
| `dissonance.png` | Dissonance (Discord Gong) | `gong-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a stitched line across a small mouth shape. |
| `chest_pound.png` | Chest Pound (Ironfist Silverback) | `gorilla-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: three little stars circling. |
| `lance_charge.png` | Lance Charge (Lancewing Griffin) | `griffin-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `key_cascade.png` | Key Cascade (Keyclatter) | `harpsichord-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `raking_claws.png` | Raking Claws (Talonstrike) | `harpy-eagle-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `pinning_arrow.png` | Pinning Arrow (Skybow Hawk) | `hawk-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: roots curling up to bind. |
| `magma_crack.png` | Magma Crack (Basaltshell) | `hermitcrab-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: small flames licking up from below. |
| `wing_gust.png` | Wing Gust (Wingwright Heron) | `heron-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a short arrow pointing away. |
| `spear_hook.png` | Spear Hook (Stillspear) | `heron-02.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a hooked arrow pulling back toward the attacker. |
| `potion_spit.png` | Potion Spit (Tonic Hippo) | `hippo-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small green gas cloud. |
| `beak_drum.png` | Beak Drum (Drumbill) | `hornbill-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a small pink spiral. |
| `lucky_kick.png` | Lucky Kick (Lucky Ironshoe) | `horseshoe-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `bronze_spike.png` | Bronze Spike (Bronzeback Sentinel) | `horseshoecrab-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `sand_pulse.png` | Sand Pulse (Sandhex Glass) | `hourglass-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `howl.png` | Howl (Roarleaf) | `howlermonkey-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small pink spiral. |
| `flask_barrage.png` | Flask Barrage (Gerald Hollins) | `human-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small green gas cloud. |
| `quick_jab.png` | Quick Jab (Maggie Wren) | `human-02.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a stolen sparkle flying back to the attacker. |
| `crystal_arrow.png` | Crystal Arrow (Mira Castell) | `human-03.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small hourglass. |
| `frost_arrow.png` | Frost Arrow (Sigrid Halvorsen) | `human-04.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a frosty ice patch below. |
| `gear_arrow.png` | Gear Arrow (Otto Brenner) | `human-05.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: roots curling up to bind. |
| `sun_arrow.png` | Sun Arrow (Lucia Marin) | `human-06.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a puff of grey-violet powder. |
| `leg_shot.png` | Leg Shot (Tom Reed) | `human-07.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: roots curling up to bind. |
| `frost_axe.png` | Frost Axe (Ragnhild Eriksen) | `human-08.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a frosty ice patch below. |
| `axe_hook.png` | Axe Hook (Gunnar Holt) | `human-09.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a hooked arrow pulling back toward the attacker. |
| `shield_smash.png` | Shield Smash (Felix Lowell) | `human-10.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `lute_ballad.png` | Lute Ballad (Lorenzo Varga) | `human-11.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small pink spiral. |
| `rope_hook.png` | Rope Hook (Jack Morrow) | `human-12.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a hooked arrow pulling back toward the attacker. |
| `banner_thrust.png` | Banner Thrust (Helena Strand) | `human-13.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `warning_shot.png` | Warning Shot (Carlos Fonseca) | `human-14.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small hourglass. |
| `riposte_cut.png` | Riposte Cut (Beatrix Alvarez) | `human-15.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `lantern_light.png` | Lantern Light (Agnes Brook) | `human-16.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `lightning_thrust.png` | Lightning Thrust (Amara Okafor) | `human-17.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: three little stars circling. |
| `veil_dance.png` | Veil Dance (Nadia Petrova) | `human-18.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a small pink spiral. |
| `thorn_volley.png` | Thorn Volley (Fiona Ashby) | `human-19.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a single stylised red drop (not gore). |
| `root_grasp.png` | Root Grasp (Ivy Calloway) | `human-20.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: roots curling up to bind. |
| `rapier_carve.png` | Rapier Carve (Camille Moreau) | `human-21.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `arc_discharge.png` | Arc Discharge (Lukas Weiss) | `human-22.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: three little stars circling. |
| `grapple_yank.png` | Grapple Yank (Max Hooper) | `human-23.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a hooked arrow pulling back toward the attacker. |
| `ice_lance_thrust.png` | Ice Lance Thrust (Ingrid Lund) | `human-24.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a frosty ice patch below. |
| `stone_fist.png` | Stone Fist (Leo Stone) | `human-25.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `shield_push.png` | Shield Push (Peter Holm) | `human-26.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a short arrow pointing away. |
| `shield_bump.png` | Shield Bump (Johanna Weber) | `human-27.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a short arrow pointing away. |
| `rallying_strike.png` | Rallying Strike (Albrecht Lang) | `human-28.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a small hourglass. |
| `hammer_smash.png` | Hammer Smash (Johanka Weber) | `human-29.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `shield_wall_shove.png` | Shield Wall Shove (Edwin Marsh) | `human-30.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a short arrow pointing away. |
| `valiant_strike.png` | Valiant Strike (William Hart) | `human-31.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `crystal_ray.png` | Crystal Ray (Kai Lindqvist) | `human-32.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: three little stars circling. |
| `fire_spark.png` | Fire Spark (Carmen Costa) | `human-33.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: small flames licking up from below. |
| `frost_spark.png` | Frost Spark (Astor Nilsen) | `human-34.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a frosty ice patch below. |
| `stone_bolt.png` | Stone Bolt (Henrik Berg) | `human-35.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: three little stars circling. |
| `frost_missile.png` | Frost Missile (Mateo Santos) | `human-36.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a frosty ice patch below. |
| `spinning_hammer.png` | Spinning Hammer (Kenji Mori) | `human-37.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `soul_rend.png` | Soul Rend (Viktor Graves) | `human-38.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `bell_toll.png` | Bell Toll (Martin Becker) | `human-39.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `eclipse.png` | Eclipse (Luna Ortiz) | `human-40.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a puff of grey-violet powder. |
| `sun_flare.png` | Sun Flare (Aurelia Day) | `human-41.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `smite.png` | Smite (Thomas Abbott) | `human-42.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `vial_toss.png` | Vial Toss (Paul Fischer) | `human-43.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small green gas cloud. |
| `void_shard.png` | Void Shard (Nika Volkova) | `human-44.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `dirty_trick.png` | Dirty Trick (Daniel Hart) | `human-45.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a puff of grey-violet powder. |
| `lantern_glare.png` | Lantern Glare (Elsa Lindgren) | `human-46.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a puff of grey-violet powder. |
| `ancestor_call.png` | Ancestor Call (Kofi Mensah) | `human-47.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `smith_crush.png` | Smith Crush (Hugo Schmidt) | `human-48.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `arcane_edge.png` | Arcane Edge (Laura Cole) | `human-49.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a stitched line across a small mouth shape. |
| `glyph_strike.png` | Glyph Strike (Richard Novak) | `human-51.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `hat_trick.png` | Hat Trick (Arthur Pembroke) | `human-52.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small pink spiral. |
| `flurry.png` | Flurry (Rapierbeak) | `hummingbird-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a small hourglass. |
| `trident_gust.png` | Trident Gust (Tridentmaw Hydra) | `hydra-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: three little stars circling. |
| `cackle.png` | Cackle (Sawgrin) | `hyena-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `horn_charge.png` | Horn Charge (Standardhorn) | `ibex-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `incense_cloud.png` | Incense Cloud (Smokeswing) | `incense-censor-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small pink spiral. |
| `ink_splash.png` | Ink Splash (Quillspatter) | `inkwell-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a puff of grey-violet powder. |
| `iron_roll.png` | Iron Roll (Ironroll) | `isopod-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `curse_glow.png` | Curse Glow (Cursehorn) | `jackalope-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a stitched line across a small mouth shape. |
| `storm_hum.png` | Storm Hum (Stormbell Jelly) | `jelly-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: three little stars circling. |
| `sting_tendril.png` | Sting Tendril (Mendbloom Jelly) | `jellyfish-02.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a toxic green drop with bubbles. |
| `acorn_shot.png` | Acorn Shot (Acornpop) | `jerboa-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: three little stars circling. |
| `talon_dive.png` | Talon Dive (Lurewing) | `kestrel-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `boiling_spout.png` | Boiling Spout (Siegekettle) | `kettle-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: small flames licking up from below. |
| `kiln_blast.png` | Kiln Blast (Kiln Behemoth) | `kiln-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: small flames licking up from below. |
| `dagger_slice.png` | Dagger Slice (Captain Daggerbill) | `kingfisher-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `idol_glow.png` | Idol Glow (Idolpaw) | `kinkajou-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small pink spiral. |
| `updraft.png` | Updraft (Tailwind Kite) | `kite-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a short arrow pointing away. |
| `venom_bite.png` | Venom Bite (Cleaver Komodo) | `komodo-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a toxic green drop with bubbles. |
| `tentacle_grab.png` | Tentacle Grab (Basalt Kraken) | `kraken-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a hooked arrow pulling back toward the attacker. |
| `prism_wave.png` | Prism Wave (Prismlamp) | `lantern-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a puff of grey-violet powder. |
| `reef_thrust.png` | Reef Thrust (Reef Warden) | `lizardfolk-02.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `jaw_snap.png` | Jaw Snap (Axejaw Lizardfolk) | `lizardfolk-03.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `smoke_kunai.png` | Smoke Kunai (Kunai Lizardfolk) | `lizardfolk-04.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a puff of grey-violet powder. |
| `castanet_clack.png` | Castanet Clack (Castanet Lobster) | `lobster-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `flask_lob.png` | Flask Lob (Rivetclaw) | `lobster-02.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small green gas cloud. |
| `lid_chomp.png` | Lid Chomp (Lockbox Dreadnought) | `lockbox-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: roots curling up to bind. |
| `thread_bind.png` | Thread Bind (Threadbinder Loom) | `loom-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: roots curling up to bind. |
| `plume_fan.png` | Plume Fan (Plumesong) | `lyrebird-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a puff of grey-violet powder. |
| `tundra_stomp.png` | Tundra Stomp (Tundra Titan) | `mammoth-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a frosty ice patch below. |
| `herb_puff.png` | Herb Puff (Shellmend Manatee) | `manatee-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small pink spiral. |
| `mandrake_scream.png` | Mandrake Scream (Screamroot) | `mandrake-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: three little stars circling. |
| `haymaker.png` | Haymaker (Glovegrin) | `mandrill-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `mannequin_slam.png` | Mannequin Slam (Stitchfist Mannequin) | `mannequin-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `barb_lash.png` | Barb Lash (Barbtail Manticore) | `manticore-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a toxic green drop with bubbles. |
| `blade_cut.png` | Blade Cut (Bladewright Mantis) | `mantis-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `balm_flask.png` | Balm Flask (Balmbeak) | `marabou-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small green gas cloud. |
| `echo_scream.png` | Echo Scream (Echomask Rack) | `maskrack-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small hourglass. |
| `leg_bite.png` | Leg Bite (Shieldjaw Mastiff) | `mastiff-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: roots curling up to bind. |
| `spotted.png` | Spotted! (Spyglass Meerkat) | `meerkat-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small orange-red crosshair. |
| `millstone_crush.png` | Millstone Crush (Millstone Juggernaut) | `millstone-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `mirror_ray.png` | Mirror Ray (Mirror Revenant) | `mirror-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `headlamp_flash.png` | Headlamp Flash (Gearclaw Mole) | `mole-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `hexhammer.png` | Hexhammer (Hexhammer Mole) | `mole-02.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a stitched line across a small mouth shape. |
| `sickle_slash.png` | Sickle Slash (Sicklewhisk) | `mongoose-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `hex_dart.png` | Hex Dart (Hexdart Mongoose) | `mongoose-02.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a stitched line across a small mouth shape. |
| `sling_stone.png` | Sling Stone (Slingtail) | `monkey-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: three little stars circling. |
| `moray_bite.png` | Moray Bite (Hookgrin Moray) | `moray-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `grind.png` | Grind (Bastion Mortar) | `mortar-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `lantern_ray.png` | Lantern Ray (Lanternmoth) | `moth-02.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `spore_ray.png` | Spore Ray (Sporedruid) | `mushroom-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small green gas cloud. |
| `spore_pulse.png` | Spore Pulse (Capshield) | `mushroom-03.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small green gas cloud. |
| `windup_punch.png` | Windup Punch (Windup Drummer) | `musicbox-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `buckler_bash.png` | Buckler Bash (Bucklerhorn) | `muskox-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `coil_crack.png` | Coil Crack (Coilnewt) | `newt-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `shade_ray.png` | Shade Ray (Shadeolm) | `olm-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a puff of grey-violet powder. |
| `dice_flick.png` | Dice Flick (Dicetail) | `opossum-01.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small hourglass. |
| `skull_club.png` | Skull Club (Grukk Skullcaller) | `orc-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `petal_slash.png` | Petal Slash (Petalshade) | `orchidmantis-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `pipe_chord.png` | Pipe Chord (Pipe Organ Choir) | `organ-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `orbit_missile.png` | Orbit Missile (Orbitwright) | `orrery-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small hourglass. |
| `bone_rattle.png` | Bone Rattle (Bonecube) | `ossuary-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a small pink spiral. |
| `river_stone.png` | River Stone (Slingwhisker) | `otter-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: three little stars circling. |
| `owl_spark.png` | Owl Spark (Hootmage) | `owl-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small pink spiral. |
| `palm_strike.png` | Palm Strike (Bamboo Monk) | `panda-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `scale_bash.png` | Scale Bash (Decoy Pangolin) | `pangolin-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `thunder_hum.png` | Thunder Hum (Thunderscale) | `pangolin-02.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: three little stars circling. |
| `reed_melody.png` | Reed Melody (Reedcaller) | `panpipe-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small pink spiral. |
| `feather_fan.png` | Feather Fan (Plumecrown) | `peacock-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small pink spiral. |
| `solar_flare.png` | Solar Flare (Solar Phoenix) | `phoenix-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: small flames licking up from below. |
| `squeak.png` | Squeak (Hornpip) | `pika-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small pink spiral. |
| `rune_spray.png` | Rune Spray (Runebill) | `platypus-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small green gas cloud. |
| `plough.png` | Plough (Furrow Ravager) | `plough-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a short arrow pointing away. |
| `tick_tock.png` | Tick Tock (Tickwarden) | `pocketwatch-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `quill_volley.png` | Quill Volley (Quillflask) | `porcupine-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a single stylised red drop (not gore). |
| `portcullis_drop.png` | Portcullis Drop (Portcullis Colossus) | `portcullis-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: roots curling up to bind. |
| `press_down.png` | Press Down (Typesetter) | `printingpress-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: roots curling up to bind. |
| `refraction.png` | Refraction (Shardhex) | `prism-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small pink spiral. |
| `shield_thump.png` | Shield Thump (Shieldpuff) | `puffin-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a short arrow pointing away. |
| `millstone_spin.png` | Millstone Spin (Grindstone Oracle) | `quern-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a short arrow pointing away. |
| `happy_glow.png` | Happy Glow (Pebblegrin) | `quokka-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small pink spiral. |
| `snatch_and_run.png` | Snatch and Run (Potionpaw) | `raccoon-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a stolen sparkle flying back to the attacker. |
| `sun_slam.png` | Sun Slam (Sunhorn Paladin) | `ram-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `squeeze_song.png` | Squeeze Song (Bellows Ram) | `ram-02.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `relic_glow.png` | Relic Glow (Sealed Reliquary) | `reliquary-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `retort_spray.png` | Retort Spray (Bubblehex Retort) | `retort-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small green gas cloud. |
| `shadow_slash.png` | Shadow Slash (Shadowhorn) | `rhinoceros-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `shrill_whistle.png` | Shrill Whistle (Whistledash) | `roadrunner-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `roc_shriek.png` | Roc Shriek (Thunder Roc) | `roc-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a small hourglass. |
| `dawn_crow.png` | Dawn Crow (Banner Cock) | `rooster-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `rallying_charge.png` | Rallying Charge (Pennant Sable) | `sableantelope-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: three little stars circling. |
| `lockdown.png` | Lockdown (Vaulthex) | `safe-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: roots curling up to bind. |
| `moonwind_theft.png` | Moonwind Theft (Nightsail) | `sail-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a stolen sparkle flying back to the attacker. |
| `ember_swipe.png` | Ember Swipe (Slinkember) | `salamander-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: small flames licking up from below. |
| `fume_flask.png` | Fume Flask (Flaskember) | `salamander-02.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small green gas cloud. |
| `tomb_curse.png` | Tomb Curse (Tombwarden) | `sarcophagus-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a stitched line across a small mouth shape. |
| `rune_cannon.png` | Rune Cannon (Runeshell Scarab) | `scarab-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small orange-red crosshair. |
| `stinger.png` | Stinger (Stingtail) | `scorpion-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a toxic green drop with bubbles. |
| `ribbon_bind.png` | Ribbon Bind (Ribbonsting) | `scorpion-02.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: roots curling up to bind. |
| `splash_song.png` | Splash Song (Bottlenose Bard) | `seal-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a splash of water below. |
| `wax_press.png` | Wax Press (Waxseal Press) | `sealpress-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: roots curling up to bind. |
| `javelin_throw.png` | Javelin Throw (Javelin Stalker) | `secretarybird-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a single stylised red drop (not gore). |
| `royal_venom.png` | Royal Venom (Crowned Serpent) | `serpent-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a toxic green drop with bubbles. |
| `crossbow_bolt.png` | Crossbow Bolt (Boltear) | `serval-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small orange-red crosshair. |
| `stitch_up.png` | Stitch Up (Stitchkit) | `sewingbox-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: roots curling up to bind. |
| `starsight_ray.png` | Starsight Ray (Admiral Starsight) | `sextant-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small orange-red crosshair. |
| `aegis_bash.png` | Aegis Bash (Vanguard Aegis) | `shield-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `pike_thrust.png` | Pike Thrust (Pikebill) | `shoebill-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a short arrow pointing away. |
| `spirit_nip.png` | Spirit Nip (Totemshrew) | `shrew-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a stitched line across a small mouth shape. |
| `gas_vent.png` | Gas Vent (Siphonworm) | `siphon-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small green gas cloud. |
| `rattling_chant.png` | Rattling Chant (Rattlechant) | `sistrum-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `sledge_slam.png` | Sledge Slam (Sledge Colossus) | `sledgehammer-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `slow_claw.png` | Slow Claw (Barkhold Sloth) | `sloth-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a small hourglass. |
| `slime_trail.png` | Slime Trail (Prism Snail) | `snail-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: roots curling up to bind. |
| `brew_toss.png` | Brew Toss (Brewshell) | `snail-05.png` | The weapon, prop or body part from the reference thrown, tumbling through the air; accent: a small pink spiral. |
| `spike_roll.png` | Spike Roll (Spikeback) | `spikeback-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `spindle_lash.png` | Spindle Lash (Spindlewhirl) | `spindle-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a hooked arrow pulling back toward the attacker. |
| `hunters_arrow.png` | Hunter's Arrow (Shieldhorn Stag) | `stag-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small orange-red crosshair. |
| `needle_jab.png` | Needle Jab (Needlewhisk) | `stoat-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `gliding_swoop.png` | Gliding Swoop (Sailglide) | `sugarglider-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a small pink spiral. |
| `noon_blade.png` | Noon Blade (Noonward) | `sundial-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a puff of grey-violet powder. |
| `booming_beat.png` | Booming Beat (Sunhex Drum) | `sundrum-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `woven_snare.png` | Woven Snare (Woven Host) | `tapestry-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: roots curling up to bind. |
| `skull_hex.png` | Skull Hex (Maskeye) | `tarsier-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a stitched line across a small mouth shape. |
| `volcano_spout.png` | Volcano Spout (Volcano Teapot) | `teapot-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: small flames licking up from below. |
| `drill_shot.png` | Drill Shot (Drillmite) | `termite-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a single stylised red drop (not gore). |
| `clockwork_nip.png` | Clockwork Nip (Clockmite) | `termite-02.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a small hourglass. |
| `thorn_snare.png` | Thorn Snare (Bramblestalker) | `thorntreant-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: roots curling up to bind. |
| `boulder_belly.png` | Boulder Belly (Boulder Toad) | `toad-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: three little stars circling. |
| `spore_croak.png` | Spore Croak (Druid Toad) | `toad-02.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small green gas cloud. |
| `croaking_toll.png` | Croaking Toll (Belltoad) | `toad-03.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `avalanche.png` | Avalanche (Mountainshell) | `tortoise-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
| `obsidian_shard.png` | Obsidian Shard (Obsidian Tortoise) | `tortoise-02.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a single stylised red drop (not gore). |
| `leaf_storm.png` | Leaf Storm (Leafwarden) | `treant-01.png` | The weapon, prop or body part from the reference blasting out as a cone of breath that fans out; accent: a puff of grey-violet powder. |
| `ancient_root.png` | Ancient Root (Elderbough) | `treant-02.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: roots curling up to bind. |
| `trowel_strike.png` | Trowel Strike (Trowel Crusader) | `trowel-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `harmonic_hum.png` | Harmonic Hum (Resonant Champion) | `tuningfork-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `turbine_blade.png` | Turbine Blade (Skyturbine) | `turbine-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `prayer_palm.png` | Prayer Palm (Shellmonk) | `turtle-02.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `frost_chime.png` | Frost Chime (Frostshell Bard) | `turtle-03.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a frosty ice patch below. |
| `parasol_gust.png` | Parasol Gust (Fanbrella) | `umbrella-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a short arrow pointing away. |
| `glue_spit.png` | Glue Spit (Runevelvet) | `velvetworm-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a stitched line across a small mouth shape. |
| `acid_pinch.png` | Acid Pinch (Vinegar Flask) | `vinegaroon-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a single stylised red drop (not gore). |
| `vise_grip.png` | Vise Grip (Vise Enforcer) | `vise-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: roots curling up to bind. |
| `cinder_dive.png` | Cinder Dive (Cinderbeak) | `vulture-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: small flames licking up from below. |
| `horn_blast.png` | Horn Blast (Tuskhorn Walrus) | `walrus-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small pink spiral. |
| `hand_cannon.png` | Hand Cannon (Cannon Warthog) | `warthog-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a short arrow pointing away. |
| `venom_sting.png` | Venom Sting (Stingflask) | `wasp-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a toxic green drop with bubbles. |
| `quick_swipe.png` | Quick Swipe (Daggerwhisk) | `weasel-01.png` | The weapon, prop or body part from the reference lunging forward in a fast strike with motion lines; accent: a stolen sparkle flying back to the attacker. |
| `turning_gust.png` | Turning Gust (Gustarrow) | `weathervane-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a small pink spiral. |
| `banner_snap.png` | Banner Snap (Bannerweave) | `weaverbird-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a short arrow pointing away. |
| `thorn_claws.png` | Thorn Claws (Thornfang) | `werewolf-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: a single stylised red drop (not gore). |
| `shadow_touch.png` | Shadow Touch (Shadowwisp) | `wisp-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: a stitched line across a small mouth shape. |
| `ranger_arrow.png` | Ranger Arrow (Ranger Wolf) | `wolf-01.png` | The weapon, prop or body part from the reference shot as a projectile in flight with a short trail; accent: a small orange-red crosshair. |
| `mace_smash.png` | Mace Smash (Macefang Wolverine) | `wolverine-01.png` | The weapon, prop or body part from the reference sweeping in a strike with a bright arc trail; accent: three little stars circling. |
| `petrifying_gaze.png` | Petrifying Gaze (Obsidian Wyvern) | `wyvern-01.png` | The weapon, prop or body part from the reference released as a glowing magic bolt with a short comet tail; accent: three little stars circling. |
| `gong_crash.png` | Gong Crash (Gongyak) | `yak-01.png` | The weapon, prop or body part from the reference pulsing out as concentric rings from a bright centre; accent: a small hourglass. |
