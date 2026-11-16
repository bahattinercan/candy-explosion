# Candy Explosion

Candy Explosion is an archived 2D match-3 puzzle game prototype developed in the Unity engine targeting Unity 2020.3.20f1. The core gameplay loop implements a classic match-3 board mechanic where players select and swipe adjacent candies on a grid using mouse or touch input to form horizontal or vertical alignments of three or more matching pieces. Forming matches clears candies from the board, triggers special bomb transformations (row, column, adjacent 3x3, and color bombs) for larger alignments, damages obstacle tiles (such as breakable tiles, locks, concrete, and slime), and advances collection counters toward designated level goals. A scoring system accumulates points toward three star milestones, while an end-game controller monitors remaining move or time counters to trigger victory or defeat sequences.

## Gameplay

The game mechanics are implemented across dedicated C# scripts located in `Assets/Scripts/Base Game/`:

- **Board model (`Board.cs`)**: Manages the 2D grid dimensions (`width` and `height`), initializes obstacle tile layers (blank spaces, breakable tiles, lock tiles, concrete, and slime), spawns candy pieces, executes gravity fall and board refill delays, checks for slime spreading, handles deadlock reshuffling, and coordinates bomb clearing logic.
- **Matching (`FindMatches.cs`)**: Iterates through grid columns and rows to detect horizontal and vertical runs of three or more matching pieces, tags matched dots for destruction, and triggers special piece abilities including row-clearing bombs, column-clearing bombs, 3x3 adjacent area bombs, and color-matching bombs.
- **Goal tracking (`GoalManager.cs`)**: Loads required candy types, colors, and target quantities defined in level data, instantiates UI goal display panels, tracks collected counts as matched candies are destroyed, and signals the end-game manager when all level objectives are met.
- **Scoring (`ScoreManager.cs`)**: Calculates player score increments during matches, updates the on-screen score display and progress bar fill amount relative to the 3-star threshold, and records high scores and earned stars through the persistent data manager.
- **Hints (`HintManager.cs`)**: Runs an idle countdown timer when no player move is detected, scans the board via simulation (`SwitchAndCheck`) to locate potential valid matches, selects a match candidate at random, and instantiates an animated particle effect on the target piece until player input destroys the hint.
- **End-of-game flow (`EndGameManager.cs`)**: Enforces level win/loss criteria according to move limits or countdown timers, displays victory (`youWinPanel`) or defeat (`tryAgainPanel`) panels, saves progress, and transitions board state to prevent further moves.
- **Audio (`SoundManager.cs`, `MusicPlayer.cs`)**: `SoundManager.cs` manages background music playback from randomly selected clips defined in `MusicsSO`, handles volume and mute states stored in `PlayerPrefs`, and triggers randomized sound effects upon candy destruction; `MusicPlayer.cs` plays a random music clip from `MusicsSO` on start.
- **Camera behaviour (`CameraScalar.cs`)**: Centers the orthographic camera on the board grid based on width and height parameters with configurable vertical offset, and dynamically scales `Camera.main.orthographicSize` to fit the screen aspect ratio.
- **UI fade (`FadePanelController.cs`)**: Interfaces with Unity Animator components (`panelAnim` and `gameInfoPanel`) to transition the UI out when starting gameplay and trigger game-over animation states.
- **Background tiles (`BackgroundTile.cs`)**: Represents obstacle tiles with customizable hit points, reduces tile opacity upon taking damage, notifies `GoalManager` upon depletion, and destroys the tile GameObject.

## Project structure

| Path | Contents | Origin |
| --- | --- | --- |
| `Assets/Scripts/Base Game/` | Core gameplay scripts: board management, match detection, piece behavior, scoring, goals, hints, camera scaling, sound, and transitions | Own code |
| `Assets/Scripts/DataPersistence/` | Save and load system using JSON file serialization and `IDataPersistence` interfaces | Own code |
| `Assets/Scripts/Enums/` | Enumeration definitions for candy colors (`EDotColor`) and types (`EDotType`) | Own code |
| `Assets/Scripts/Game Data/` | Game data wrapper scripts (`MyGameData`) | Own code |
| `Assets/Scripts/Scriptable Objects/` | ScriptableObject classes for candies (`CandiesSO`, `CandySO`), levels (`Level`), world definitions (`World`), and music lists (`MusicsSO`) | Own code |
| `Assets/Scripts/UI/` | UI controllers for level select menus, start screens, confirmation panels, and pause dialogs | Own code |
| `Assets/Scenes/` | Project Unity scenes (`Splash.unity`, `Main.unity`, `Picture.unity`, `ShapesWorkspace.unity`) | Own scenes |
| `Assets/Prefab/` | Prefabs for candy pieces, background tiles, particles, and UI elements | Own assets |
| `Assets/Scriptable Objects/` | Serialized ScriptableObject assets including level configurations (Level 0 through 26), candy definitions, music collections, and world settings | Own assets |
| `Assets/Animation/` | UI and game element animation clips and Animator controllers | Own assets |
| `Assets/Music Tracks/` | Audio tracks in WAV format (`Chunky_Monkey.wav`, `InfiniteDoors.wav`, `JumpyGame.wav`, `Potato.wav`, `SunnyDay.wav`, `Tiny_Blocks.wav`) | Project audio |
| `Assets/Sounds/` | Sound effects in WAV and OGG format for pops and game audio | Project audio |
| `Assets/Sprites/` | 2D textures and sprites for candies, UI panels, tiles, and logos | Project graphics |
| `Assets/Fonts/` | Font assets (`Wednesday.ttf`, TextMesh Pro font asset) | Project fonts |
| `Assets/Materials/` | Material assets (`Dot Material.mat`, `Ring Material.mat`) | Project materials |
| `Assets/Resources/` | Folder containing `Shapes2D Sprites.meta` | Project configuration |
| `Assets/Packages/Sprite/Candy UI with Animation Free - Cyko/` | Vendored candy-themed UI assets, animations, prefabs, demo scene, and PDF guide | Vendored asset pack |
| `Assets/Packages/Sprite/Shapes2D/` | Vendored 2D procedural vector shape rendering tool with shaders, editor scripts, demo scenes, and documentation | Vendored asset pack |
| `Assets/Packages/Sprite/TextMesh Pro/` | Vendored TextMesh Pro asset files, shaders, fonts, and documentation | Vendored asset pack |
| `Packages/` | Unity Package Manager manifest (`manifest.json`) specifying package dependencies | Project configuration |
| `ProjectSettings/` | Unity project settings including editor version (`ProjectVersion.txt`), build scenes (`EditorBuildSettings.asset`), and graphics configuration | Project configuration |
| `GameManager.cs` & `GameManager1.cs` (root) | Two identical orphan C# source files at the project root outside `Assets/`; they declare an earlier prototype variant with hardcoded `Resources.Load` calls and an empty method stub (`GetRowCandy()`), unused by the Unity editor | Orphan scratch files |

## Scenes

| Scene | Description | Status |
| --- | --- | --- |
| `Assets/Scenes/Splash.unity` | Main entry scene configured at index 0 in `EditorBuildSettings.asset`. Implements the start menu, paged level selection panels, high score displays, and confirmation dialogs before starting a level. | Build scene (Index 0) |
| `Assets/Scenes/Main.unity` | Primary match-3 puzzle gameplay scene configured at index 1 in `EditorBuildSettings.asset`. Contains the game board, camera scaler, score and goal HUD, pause menu, and game over panels. | Build scene (Index 1) |
| `Assets/Scenes/Picture.unity` | Auxiliary level select scene with the same GameObject structure as `Splash.unity`, likely used as an alternate layout or visual test scene. | Standalone scene (not in build) |
| `Assets/Scenes/ShapesWorkspace.unity` | Sandbox scene containing numerous vector shape instances and material configurations, used for experimenting with the Shapes2D package. | Standalone scene (not in build) |

*Note*: Additional scene files located under `Assets/Packages/Sprite/` (`Assets/Packages/Sprite/Candy UI with Animation Free - Cyko/Scenes/Demo.unity` and `Assets/Packages/Sprite/Shapes2D/Demos/**/*.unity`) are vendor demonstration scenes packaged with third-party assets and are not part of the game.

## Running the project

1. Install **Unity 2020.3.20f1** (as defined in `ProjectSettings/ProjectVersion.txt`) using Unity Hub or the Unity download archive.
2. Open Unity Hub, select **Add**, and choose the repository root folder (`candy-explosion`).
3. In the Unity Editor Project browser, open `Assets/Scenes/Splash.unity` (for the start menu and level selection) or `Assets/Scenes/Main.unity` (for direct puzzle gameplay).
4. Press the **Play** button in the Unity Editor toolbar to enter Play Mode.

*Note*: The steps above are derived from the repository configuration files and asset structure; the project was not launched or verified inside a live Unity editor environment during the compilation of this documentation.

## Third-party assets

The repository vendors third-party packages inside `Assets/Packages/Sprite/`:

- `Assets/Packages/Sprite/Candy UI with Animation Free - Cyko`: Candy-themed UI sprites, animations, UI prefabs, scripts, and demo scene. Contains documentation file `Animated Candy UI Docs v1.0.pdf`; no license file is stated in the repository.
- `Assets/Packages/Sprite/Shapes2D`: Vector shape generation toolkit for Unity 2D with custom shaders, editor tools, and demo scenes. Contains `readme.txt` (pointing to `https://sub-c.org/Shapes2D/documentation/`) and `changelog.txt`; no license file is stated in the repository.
- `Assets/Packages/Sprite/TextMesh Pro`: TextMesh Pro resources, shaders, documentation (`TextMesh Pro User Guide 2016.pdf`), and font assets. Includes font license `Fonts/LiberationSans - OFL.txt` (SIL Open Font License) and attribution `Sprites/EmojiOne Attribution.txt`; no overall package license is stated in the repository.

*(Note: An unused metadata file `Assets/Packages/Music.meta` exists without a corresponding directory).*

## Repository notes

- **Commit history**: The Git history consists of only two commits: `ebfd4c6` ("Initial commit") and `91cf0b2` ("1").
- **Automation and tests**: The repository contains no continuous integration (CI) workflows, build automation scripts, or automated unit tests.
- **Configuration**: Root configuration is limited to `.gitignore`, `.gitattributes`, and `.vsconfig`.
- **Status**: The repository represents an archived development prototype rather than a production-ready or actively maintained project.
- **Root scripts**: The root directory contains two identical unreferenced script files (`GameManager.cs` and `GameManager1.cs`). Because they reside outside the `Assets/` directory, they are not compiled by Unity and appear to be discarded early experiments.
