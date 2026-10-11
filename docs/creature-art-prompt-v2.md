# Creature art prompt v2 (more classes, more variety)

Written 2026-10-11. It replaces nothing: `creature-art-prompt.md` stays as the v1 template. v2 exists because the first 143 creatures lean heavily on Feral / Guardian / Warrior. This file has

1. a **style block** to paste after each creature brief (two variants: with reference images, or text only),
2. **design rules** for variety and class readability,
3. **48 ready-to-run creature briefs**, 3 per profession as the main class, spread over all 11 attack types.

The 11 reference favorites are copied to `art/creatures-new/`. Put new results there too, named `{unitName}_{attackType}_{profession1}_{profession2}.png` (see the unit name schema in `creature-classes.md`). Add a row per creature to `creature-classes.md` before running `tools/art/clean_creatures.py`.

## How to use

1. Pick a brief from the table below and write it as one short paragraph (creature, body type, gear, glow, mood).
2. Paste the style block after it. Use **Variant A** if you attach the reference images from `art/creatures-new/` (attach 3–4 per request, mixing sizes, not all 11), **Variant B** if you cannot attach any.
3. One image per request, or several briefs in one request with "ONE separate image per creature".

## Style block, Variant A (references attached)

```
The attached images are style references for the Everkin card game. Match their look exactly: chunky low-poly faceting, simple matte color blocks, soft top-left light, big expressive eyes, friendly readable faces, and one small glowing or contrasting accent. Do NOT copy their species, poses or props; use them only for style, proportions and level of detail.

Render the creature described above as card art. Make ONE separate image per creature.

FORMAT
- PNG, 1024 × 1536 px portrait (2:3), fully transparent background (real alpha, not a checkerboard or white).
- Exactly one creature per image. No ground, shadow, scenery, frame, text, logo, watermark or signature.
- The whole creature is inside the image (ears, horns, antennae, tails, wings, feet, props) with at least 48 px of empty space on every side.
- Scale the creature as large as it fits in those margins, centered. Its lowest point sits about 80 px above the bottom edge. Very wide or very tall creatures are scaled down to fit, never cropped.

STYLE
- Low-poly 3D game model: broad flat polygon facets, matte colors, gentle facet shading. No fur strands, individual scales or fine texture.
- Soft directional light from the upper left, moderate contrast. No rim glow, bloom, lens effects or outlines.
- Crisp clean silhouette, no halo, fringe or stray pixels.
- Natural, slightly muted colors that read on a dark plum background, plus ONE brighter accent (a gem, ember, spore, glow). Glowing parts may be brighter.

POSE AND VIEW
- Three-quarter front view, slightly turned, from about chest height. Alert, characterful stance with believable anatomy; limbs correctly attached and joints bending the right way.
- Face clearly visible and mostly toward the viewer. Nothing covers the face.
- Wears or carries only the gear the description lists (at most 2–3 pieces), built in the same low-poly style.

TONE
- Friendly but capable, suitable for ages 7 and up. No blood, wounds, gore or horror, even for dark or sneaky classes (use mood, color and props, not menace).
```

## Style block, Variant B (text only)

Use the Variant A block, but replace its first paragraph with this one:

```
Style target: a stylized low-poly 3D game character, like a polished mobile-game collectible. Chunky, slightly oversized head and hands, big glossy expressive eyes with a small highlight, a friendly readable face, simplified shapes made of broad visible triangular facets, matte pastel-muted base colors with one saturated accent, soft toy-like proportions. Think "premium low-poly figurine", not realistic and not pixel art.
```

## Design rules (for every brief)

- **Vary the body type.** Each brief names one: tiny, round, tall and lanky, wide and low, serpentine, many-limbed, floating, or hulking. Do not make every creature an upright, robed, staff-holding figure.
- **Show the class in the body, not only the prop.** Healers are soft, round and warm-colored. Assassins are lean, low and angular with dark cool colors. Bards have a showy crest, tail or throat. Engineers have goggles, brass and bolts. Summoners have small helpers beside them. Black Mages use purple and bone tones with glowing eyes. Leaders are upright with a banner or horn. Hunters have long limbs or sharp eyes. Fliers have large readable wings. Guardians and Bodyguards are broad and plated.
- **Mix the palette.** No two neighbours in a batch share a main color. Keep one accent color per creature.
- **Humanoids are the exception (about 1 in 7).** Give them non-generic features (a race trait, an unusual hat, an animal-like feature).
- **Props stay inside the canvas.** If a staff, banner or tail would break the 48 px margin, shorten or angle it.
- **Small creatures stay big on the canvas** (scale up), and large creatures stay inside it (scale down).

## Briefs

Format: `unit name` – attack type – professions – brief. Body type is in brackets.

### Healer
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `moth-cleric-lantern` | Magic | Healer, Flying | [tiny, floating] A plump fuzzy cream moth with huge feathered antennae and soft wings, holding a hanging lantern that glows warm gold. Sleepy kind eyes. |
| `hippo-healer-potion` | Breath | Healer, Guardian | [hulking, wide] A calm round hippo medic with a leather satchel of herb bundles and a steaming ceramic pot on its back, exhaling a soft green mist. Pink cheeks. |
| `elf-healer-leaf` | Magic | Healer, Primalist | [humanoid, slim] A young elf with long pointed ears, a hooded leaf cloak and moss-green sash, cupping a glowing seedling in both hands. |

### Sorcerer
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `axolotl-mage-water` | Magic | Sorcerer, Trickster | [small, round] A pink axolotl with big frilly gills and a goofy grin, juggling three floating blue water orbs. A tiny pointed hat sits between the gills. |
| `jelly-mage-storm` | Aura | Sorcerer, Flying | [floating] A translucent violet jellyfish with a bell-shaped body and long ribbon tentacles, crackling with small yellow lightning bolts. Two eyes inside the bell. |
| `catfolk-spellblade-moon` | Bladed | Sorcerer, Warrior | [humanoid, agile] A slender silver-gray catfolk in a short dark-blue coat, holding a curved sword with a glowing crescent-moon blade. Long ringed tail. |

### Black Mage
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `crow-necromancer-skull` | Magic | Black Mage, Flying | [medium, winged] A sleek black crow with a ragged violet cloak and a small bone-white skull charm on a chain, glowing purple eyes, wings half spread. |
| `spider-witch-web` | Lashing | Black Mage, Trickster | [tall, many-limbed] A lanky dark-plum spider with eight long thin legs, wearing a crooked witch hat, trailing glowing green silk threads from its fingertips. Mischievous grin. |
| `wisp-hexer-shadow` | Magic | Black Mage, Assassin | [floating] A hooded ghost-like wisp of gray smoke with a pale lantern face, big glowing teal eyes, wispy tail instead of legs, holding a small hex-marked orb. |

### Alchemist
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `raccoon-alchemist-potion` | Thrown | Alchemist, Trickster | [medium, chubby] A raccoon with brass goggles pushed up on its head, a bandolier of colorful flasks and bubbling vials, one orange potion ready to throw. |
| `wasp-alchemist-potion` | Thrown | Alchemist, Flying | [small, winged] A slim yellow-and-black wasp with clear wings and a tiny apron, carrying a corked flask of glowing lime poison. |
| `gnome-alchemist-flask` | Thrown | Alchemist, Engineer | [humanoid, tiny] A very short gnome with a huge pointed red hat, round spectacles and a long white beard, hugging a bubbling glass flask with copper pipes. |

### Bard
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `cricket-bard-fiddle` | Magic | Bard, Trickster | [lanky, slim] A green cricket standing on its hind legs, bowing its own serrated foreleg like a fiddle, with a jaunty feathered cap. Visible music-note sparkles. |
| `peacock-bard-feather` | Magic | Bard, Leader | [tall, showy] A proud blue peacock with a full fan of tail feathers shaped like glowing eye-gems, a tiny golden lyre held in one wing. |
| `walrus-bard-horn` | Breath | Bard, Guardian | [hulking, wide] A big friendly walrus with long ivory tusks and a striped scarf, blowing into a huge curved brass horn. Eyes closed, joyful. |

### Assassin
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `weasel-rogue-dagger` | Bladed | Assassin, Trickster | [lanky, serpentine] A long sleek brown weasel with a dark scarf over the nose and neck, a short dagger in each paw, crouched and ready. Sharp amber eyes. |
| `octopus-assassin-ink` | Lashing | Assassin, Trickster | [many-limbed, low] A deep-indigo octopus with a tight black hood on its head, tentacles coiled around small curved blades, a trail of ink at its base. |
| `lizardfolk-ninja-kunai` | Thrown | Assassin, Hunter | [humanoid, lean] A slim green-brown lizardfolk in dark-gray wraps with a red knot, holding three throwing knives. Long tail balanced behind. |

### Trickster
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `monkey-trickster-sling` | Ranged | Trickster, Hunter | [medium, agile] A cheeky brown monkey with a long curled tail, a slingshot in one hand and a banana-yellow vest. Big ears, one eyebrow raised. |
| `chameleon-trickster-tongue` | Lashing | Trickster, Assassin | [small, curled] A multicolor chameleon with a spiral tail and rotating-eye look, tongue unrolled in a long elegant curl. Patches of purple, orange and teal. |
| `tanuki-trickster-leaf` | Magic | Trickster, Alchemist | [round, chubby] A round tanuki with a big belly, a green leaf balanced on its head and a gourd bottle. Smug smile, striped ringed tail. |

### Engineer
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `mole-engineer-gear` | Blunt | Engineer, Hunter | [stocky, wide-handed] A velvety gray mole with huge digging claws, brass goggles, a miner's helmet lamp and a wrench in one hand. |
| `ant-engineer-gear` | Ranged | Engineer, Leader | [slim, six-limbed] A red-brown ant in a tiny hard hat holding a brass rivet-gun, with a toolbelt and a small cog backpack. |
| `golem-engineer-gear` | Aura | Engineer, Guardian | [hulking, boxy] A squat clockwork stone golem with visible brass gears, a glowing furnace window in its chest and little steam vents on its shoulders. Round friendly eyes. |

### Summoner
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `shrew-summoner-totem` | Magic | Summoner, Primalist | [tiny] A tiny long-nosed shrew holding a carved totem, with three little translucent spirit helpers (fox, bird, frog) floating around it. |
| `crane-summoner-feather` | Magic | Summoner, Healer | [tall, lanky] A tall white crane with a red crown and long legs, wing-tips trailing glowing paper-like feathers, two small spirit koi circling it. |
| `orc-summoner-skull` | Blunt | Summoner, Black Mage | [humanoid, hulking] A broad green orc with a skull-topped staff, a bone-bead necklace and a fur mantle, two tiny grinning imps clinging to its shoulders. |

### Primalist
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `treant-druid-leaf` | Lashing | Primalist, Guardian | [tall, tree-like] A sapling-sized treant with a bark face, branch arms and a canopy of leaves, with vines curling from its hands. Mossy feet. |
| `bee-shaman-sun` | Aura | Primalist, Flying | [small, round, winged] A fuzzy bumblebee wearing a woven sun-disc pendant and a flower crown, with a faint golden pollen halo drifting around it. |
| `goat-shaman-totem` | Feral | Primalist, Warrior | [medium, horned] A shaggy mountain goat with long spiral horns hung with feathers and small bells, painted face markings, holding a bone rattle. |

### Leader
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `rooster-captain-banner` | Polearm | Leader, Warrior | [upright, showy] A bold red rooster with a tall comb, a short cape and a pennant spear. Chest puffed out. |
| `meerkat-captain-spyglass` | Ranged | Leader, Hunter | [small, upright] A meerkat standing tall on hind legs in a tiny officer coat, holding a brass spyglass in one paw and a signal flag in the other. |
| `dwarf-captain-banner` | Blunt | Leader, Engineer | [humanoid, stocky] A burly dwarf captain with a braided red beard, a riveted steel helmet, a war hammer and a banner pole with a gear emblem. |

### Hunter
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `heron-hunter-harpoon` | Polearm | Hunter, Assassin | [tall, lanky] A gray heron with a long neck and legs, a slim harpoon held low, a bandolier of fishing-line coils. Narrowed patient eyes. |
| `otter-hunter-sling` | Ranged | Hunter, Trickster | [long, supple] A river otter in a small green hood with a sling and a pouch of smooth stones, playful grin, whiskers prominent. |
| `hawk-scout-bow` | Ranged | Hunter, Flying | [medium, winged] A russet hawk with a leather hood-band, wings half raised, a small shortbow clutched in one talon. |

### Bodyguard
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `mastiff-guard-shield` | Feral | Bodyguard, Hunter | [hulking, low] A heavy tan mastiff with a studded collar and a round wooden shield strapped to one shoulder, stern but loyal face. |
| `gorilla-bodyguard-fists` | Unarmed | Bodyguard, Warrior | [hulking, wide] A silverback gorilla in a small dark vest with wrist wraps, broad chest and fists lowered in a protective stance. Gentle eyes. |
| `capybara-guard-calm` | Aura | Bodyguard, Healer | [wide, low] A huge, relaxed capybara with a small flower crown, with two tiny birds and a baby duckling perched on its back. Soft green glow beneath. |

### Flying
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `dragonfly-scout-bow` | Ranged | Flying, Hunter | [slim, winged] An iridescent blue dragonfly with four clear wings and a slender body, a tiny bow in its forelegs. |
| `griffin-knight-lance` | Polearm | Flying, Warrior | [large, winged] A young griffin with an eagle's head and a lion's body, a small steel breastplate and a pennant lance. Wings spread wide. |
| `hummingbird-duelist-rapier` | Bladed | Flying, Trickster | [tiny, winged] A shimmering green hummingbird with a long beak like a rapier, a tiny feathered hat and a red cape, wings blurred in a pose. |

### Warrior
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `panda-monk-fists` | Unarmed | Warrior, Healer | [round, hulking] A round panda in a loose orange monk robe with wrapped fists, in a calm martial-arts stance with one palm raised. |
| `mandrill-brawler-gloves` | Unarmed | Warrior, Trickster | [stocky, expressive] A mandrill with a bright blue-and-red face, oversized boxing gloves, a tiny gold tooth. Bouncing fighter stance. |
| `lizardfolk-barbarian-axe` | Bladed | Warrior, Leader | [humanoid, hulking] A big orange-brown lizardfolk with a fur kilt, a bone-handled axe and a spiked crest, roaring cheerfully with its mouth open. |

### Guardian
| Unit | Attack | Professions | Brief |
|---|---|---|---|
| `mushroom-sentinel-shield` | Aura | Guardian, Primalist | [wide, low] A broad toadstool with a thick cap like a shield dome, a stout stem body and short root legs, a faint spore haze around it. Sleepy determined face. |
| `ankylosaur-guardian-club` | Blunt | Guardian, Warrior | [wide, low] A small armored ankylosaur with a heavy club tail, stubby legs and bumpy plates on its back, calm brave expression. |
| `gargoyle-sentinel-stone` | Polearm | Guardian, Flying | [hunched, winged] A gray stone gargoyle with bat wings, small horns and a stone halberd, perched alert. Mossy patches and glowing amber eyes. |

## Coverage check

- Main professions: 3 briefs for each of the 16 professions (48 total).
- Attack types: Unarmed 3, Bladed 4, Polearm 4, Ranged 6, Feral 2, Magic 10, Blunt 4, Breath 2, Thrown 4, Lashing 4, Aura 5 (Magic stays the most common, Feral is low on purpose because the existing art already has plenty).
- Humanoids: 7 of 48 (`elf-healer-leaf`, `catfolk-spellblade-moon`, `gnome-alchemist-flask`, `lizardfolk-ninja-kunai`, `orc-summoner-skull`, `dwarf-captain-banner`, `lizardfolk-barbarian-axe`).
- Names avoid collisions with existing files in `art/creatures/` (checked 2026-10-11).
