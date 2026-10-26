# Santa's Christmas Run

**TEAM CK · Game Design & Development · Winter Semester 2017/18**

Santa's Christmas Run is a 2D endless-runner prototype. After delivering the presents on Christmas Day, Santa discovers that they have been stolen and sets off to find them. The project explores a replayable run through a snowy world built from procedurally arranged platforms.

![Santa's Christmas Run prototype](docs/img/prototype2/main.png)

## Play

The repository includes a prebuilt Windows version in `Game/`. To start it, run `Game/RunnerGame.exe`. The executable is an earlier build; rebuild the Unity project with its documented Unity 2017.2.0f3 editor version to include the new characters.

Choose Santa or one of four additional Christmas characters from the selector in the main menu: Peppermint Elf, Snowflake Scout, Gingerbread Runner, and Reindeer Dash. Click a character number or press `1`–`5`; your choice is saved for the next run. The new characters have separate idle and running animations.

| Action | Controls |
| --- | --- |
| Jump | `Space` or `Up Arrow` |
| Move left | `A` or `Left Arrow` |
| Move right | `D` or `Right Arrow` |
| Pause / resume | `Escape` |
| Restart after a run | `R` |
| Return to the main menu after a run | `M` |

Holding the jump button is designed to produce a higher jump than a brief press. In the prototype design, movement also starts the camera scrolling, and its speed increases as the run progresses.

## Gameplay and design

The central design goal is procedural platform generation: the route is assembled as the player runs, with different platform arrangements and gaps to navigate. The design documents describe increasing difficulty over time and a snowy setting. Collectibles, enemies, and additional settings appear in the broader concept, but some are identified as future or lower-priority features; their availability in the included build is not guaranteed.

The prototype presentation describes a platform-selection approach that considers the previous platform and its height, then chooses a neighboring placement or a gap. Treat this as design documentation rather than a guarantee that every planned mechanic is present in the shipped executable.

## Repository contents

- `Game/` — packaged Windows game build and Unity runtime files; editable Unity project/source files are not included.
- `docs/1_[TEAM_CK]_GDD.pdf` — game design document.
- `docs/1_[TEAM_CK]_GDD_PRESENTATION.pdf` — design presentation.
- `docs/2_[TEAM_CK]_Prototype1.pdf` — first prototype presentation, including early platform-generation notes.
- `docs/3_AND_4_TEAM_CK/` — second prototype presentation, post-mortem, and screenshots.
- `docs/img/` — supporting concept art, diagrams, and prototype screenshots.

## Project history

This is a student game-design project from the 2017/18 winter semester. The included presentations document how the focus shifted toward procedural map generation and a playable prototype. They provide useful context for intended mechanics and development decisions, but may describe plans that differ from the packaged build.
