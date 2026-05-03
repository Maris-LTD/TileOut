# TileOut - Game Project

A puzzle tile-based game project built on Unity with a modular architecture, utilizing VContainer for Dependency Injection and MessagePipe for the Event System.

## 📋 Table of Contents

- [Overview](#overview)
- [Project Architecture](#project-architecture)
- [Core Modules](#core-modules)
- [How to Run the Game](#how-to-run-the-game)
- [Game Flow](#game-flow)
- [Directory Structure](#directory-structure)

## 🎮 Overview

**TileOut** is a puzzle game where players need to move tiles in specified directions to remove them from the board. The goal is to clear all tiles to complete the level.

### Key Features:
- Level system with tile movement mechanics
- UI system with Screen/Sheet/Popup architecture
- Camera system that automatically frames the grid
- Resource management system
- Event-driven architecture

## 🏗️ Project Architecture

The project uses a **Modular Architecture** with the following characteristics:

- **Dependency Injection**: VContainer with auto-install system
- **Event System**: MessagePipe with an EventBus wrapper
- **Module System**: Auto-scans and installs modules by scope (Project/Scene/GameObject)
- **Lifetime Management**: Scoped services with the IDisposable pattern

### Scope System:
- **Project Scope**: Services exist throughout the project (ResourceManager, EventBus)
- **Scene Scope**: Services only exist in the current scene (GameplayManager, SpawnerService)

## 📦 Core Modules

### 1. **ModuleManager** (`Assets/Maris-Module/ModuleManager/`)
Core module managing the auto-scanning and installation of other modules.

**Features:**
- Auto-install modules based on the `[AutoInstall]` attribute
- Module scope management (Project/Scene/GameObject)
- Auto-inject MonoBehaviour components
- Lifetime scope management (ProjectLifeTimeScope, SceneLifetimeScope)

**Core Classes:**
- `ModuleRegistry`: Manages the list of installers
- `AutoInstaller`: Auto-installs modules
- `AutoInjectProcessor`: Auto-injects dependencies into MonoBehaviours

### 2. **UI Module** (`Assets/Maris-Module/UI/`)
UI system based on ZBase.UnityScreenNavigator with 3 layers: Screen, Sheet, Popup.

**Features:**
- Navigation service for Screen/Sheet/Popup
- Base classes: `BaseScreen`, `BaseSheet`, `BasePopup`
- Container management
- Transition animations

**Core Classes:**
- `UINavigationService`: UI navigation service
- `BaseScreen`: Base class for screens
- `BaseSheet`: Base class for sheets (InGameSheet, OutGameSheet)
- `BasePopup`: Base class for popups (VictoryPopup, LoadingPopup, ConfirmPopup)

### 3. **Gameplay Module** (`Assets/0 Game/Scripts/Gameplay/`)
Module containing the main game logic.

**Components:**
- **GameplayManager**: Manages gameplay state, handles events, checks win conditions
- **TileGrid**: Manages the grid and tiles
- **TileMovementSystem**: Tile movement logic
- **TileMapSpawner**: Spawns and manages tile views
- **GameplayService**: Interface service for gameplay

**Lifetime**: Scoped (Scene scope)

### 4. **Spawner Module** (`Assets/Maris-Module/Spawner/`)
Object pooling system using uPools.

**Features:**
- Rent/Return pattern for GameObject pooling
- Auto-manages parent transforms
- Prewarm support

**Core Classes:**
- `SpawnerService`: Main service for object pooling
- `ISpawner`: Interface for the spawner

**Lifetime**: Scoped (Scene scope)

### 5. **ResourceManager Module** (`Assets/Maris-Module/ResourceManager/`)
Manages game resources (coins, gems, energy, etc.).

**Features:**
- Resource definitions and registry
- Storage system (PlayerPrefs)
- Resource regeneration
- Offline regeneration calculation

**Lifetime**: Singleton (Project scope)

### 6. **CameraSystem Module** (`Assets/Maris-Module/CameraSystem/`)
Camera management system using Cinemachine.

**Features:**
- Camera rig authoring
- Camera director service
- Auto-framing based on grid bounds
- Camera behaviors (zoom, movement)

**Core Classes:**
- `CameraRigAuthoring`: Authoring component for camera rigs
- `ICameraDirectorService`: Camera control service
- `GameCameraSetup`: Camera setup for the gameplay scene

### 7. **PubSubEvent Module** (`Assets/Maris-Module/PubSubEvent/`)
Event system wrapper on MessagePipe.

**Features:**
- Subscribe/Unsubscribe/Publish events
- Global event bus
- Handler pooling
- Auto-registration

**Core Classes:**
- `IEventBus` / `IGlobalEventBus`: Interface for the event bus
- `EventBus`: Event bus implementation

**Lifetime**: Singleton (Project scope)

### 8. **DataManager Module** (`Assets/Maris-Module/DataManager/`)
Manages data persistence.

**Features:**
- Data providers
- Save/Load system
- Data registry

### 9. **IAP Module** (`Assets/Maris-Module/IAP/`)
In-app purchase system.

**Features:**
- Product management
- Purchase handling
- Reward system

## 🚀 How to Run the Game

### Requirements:
- Unity 2021.3 LTS or higher
- Packages: VContainer, MessagePipe, DOTween, uPools, Cinemachine

### Steps:

1. **Open the project in Unity Editor**

2. **Set LoadingScene as the startup scene:**
   - Go to `File > Build Settings`
   - Drag `Assets/0 Game/Scenes/LoadingScene.unity` into Build Settings
   - Set it as the first scene (index 0)

3. **Ensure ProjectLifeTimeScope is in the scene:**
   - LoadingScene needs a GameObject with the `ProjectLifeTimeScope` component
   - This component will automatically install Project-scope modules

4. **Run the game:**
   - Press Play in the Unity Editor
   - Or build and run the executable

### Scene Flow:
```
LoadingScene → PlayScene
```

**LoadingScene:**
- Displays loading progress
- Automatically transitions to PlayScene after loading is complete
- Duration: 2.5 seconds (configurable)

**PlayScene:**
- Contains SceneLifetimeScope to install Scene-scope modules
- Automatically displays OutGameSheet (HomePage) on start
- Contains UI containers (ScreenContainer, SheetContainer, PopupContainer)

## 🎯 Game Flow

### 1. **Startup (LoadingScene)**
```
LoadingScene Start
  ↓
Initialize UI (progress bar, text)
  ↓
Simulate Loading (2.5s)
  ↓
Fade Out
  ↓
Load PlayScene
```

### 2. **Main Menu (PlayScene - OutGameSheet)**
```
PlayScene Start
  ↓
SceneLifetimeScope installs Scene modules
  ↓
PlaySceneController.Start()
  ↓
Show OutGameSheet (HomePage)
  ↓
Display Level Chain
```

**HomePage Features:**
- Displays level chain with level nodes
- Play button to start a level
- Settings button (coming soon)

### 3. **Gameplay (InGameSheet)**
```
User clicks Play button
  ↓
Show LoadingPopup
  ↓
Load Level Data (from Resources/Levels/Level_X)
  ↓
Show InGameSheet
  ↓
GameplayManager.LoadLevel()
  ↓
  ├─ Create Tiles from LevelData
  ├─ Spawn Tile Views
  └─ Setup Camera (auto-frame grid)
  ↓
Gameplay Loop:
  ├─ User taps tile
  ├─ TileTappedEvent published
  ├─ GameplayManager handles event
  ├─ Check if tile can move
  ├─ If yes: Move tile, remove from grid
  ├─ If no: Show blocked feedback
  └─ Check win condition
```

**Gameplay Mechanics:**
- **Tile Movement**: Each tile has a direction (Up/Down/Left/Right)
- **Movement Logic**: Tile moves in its direction until it hits an obstacle or goes out of the grid
- **Win Condition**: All tiles are removed from the grid
- **Blocked Feedback**: Shows animation when a tile cannot move

### 4. **Level Completion**
```
All tiles removed from grid
  ↓
GameplayManager.CheckWinCondition()
  ↓
Publish LevelCompletedEvent
  ↓
InGameSheet handles event
  ↓
Show VictoryPopup
  ↓
User can:
  ├─ Play Next Level
  ├─ Retry Level
  └─ Return to Home
```

### 5. **Event Flow**
```
Tile Interaction:
  TileView.OnMouseDown()
    ↓
  Publish TileTappedEvent
    ↓
  GameplayManager.OnTileTapped()
    ↓
  TryMoveTile()
    ↓
  Publish TileMovedEvent / TileBlockedEvent
    ↓
  InGameSheet updates UI
```

## 📁 Directory Structure

```
Assets/
├── 0 Game/                          # Main game code
│   ├── Scripts/
│   │   ├── Camera/                  # Camera behaviors
│   │   ├── Gameplay/                 # Gameplay logic
│   │   │   ├── Data/                # Gameplay data (LevelData, GameplayConfig)
│   │   │   ├── Entity/              # Game entities (Tile, TileGrid)
│   │   │   ├── Event/               # Gameplay events
│   │   │   ├── Service/             # Services and Installers
│   │   │   ├── System/              # Game systems (GameplayManager, TileMovementSystem)
│   │   │   └── View/                # Views (TileView, TileMapSpawner)
│   │   ├── Shop/                    # Shop system
│   │   └── UI/                      # UI controllers and pages
│   │       ├── Page/                # Pages (HomePage)
│   │       ├── Popups/              # Popups (Victory, Loading, Confirm)
│   │       └── Sheets/              # Sheets (InGameSheet, OutGameSheet)
│   └── Scenes/                      # Game scenes
│       ├── LoadingScene.unity       # Entry scene
│       └── PlayScene.unity          # Main game scene
│
├── Maris-Module/                    # Core modules
│   ├── CameraSystem/                # Camera management
│   ├── DataManager/                 # Data persistence
│   ├── IAP/                         # In-app purchases
│   ├── ModuleManager/               # Core module management
│   ├── PubSubEvent/                 # Event system
│   ├── ResourceManager/            # Resource management
│   ├── Spawner/                     # Object pooling
│   └── UI/                          # UI system
│
├── Plugins/                         # Third-party plugins
│   └── MessagePipe/                 # MessagePipe library
│
└── Resources/                        # Resources folder
    └── Levels/                      # Level data files (JSON)
```

## 🔧 Technical Architecture

### Dependency Injection Pattern:
- **VContainer**: DI framework
- **Auto-Install**: Modules are auto-installed based on the `[AutoInstall]` attribute
- **Lifetime Scopes**: 
  - Project: Singleton services (ResourceManager, EventBus)
  - Scene: Scoped services (GameplayManager, SpawnerService)

### Event-Driven Architecture:
- **MessagePipe**: Event system backend
- **EventBus**: Wrapper with Subscribe/Unsubscribe/Publish API
- **Event Types**: 
  - `TileTappedEvent`: When a user taps a tile
  - `TileMovedEvent`: When a tile successfully moves
  - `TileBlockedEvent`: When a tile cannot move
  - `LevelInitializedEvent`: When a level is loaded
  - `LevelCompletedEvent`: When a level is completed

### Object Pooling:
- **uPools**: Pooling library
- **SpawnerService**: Wrapper service with Rent/Return pattern
- **TileView**: Uses pooling to optimize performance

## 📝 Notes

1. **Scene Setup**: Each scene needs a `SceneLifetimeScope` to install Scene-scope modules
2. **Project Scope**: A `ProjectLifeTimeScope` is required in the first scene (LoadingScene)
3. **Level Data**: Level data is saved in `Resources/Levels/` as JSON
4. **Config Files**: GameplayConfig and other configs are in `Resources/`

## 🎨 Gameplay Details

### Tile Movement:
- Each tile has a direction (Up, Down, Left, Right)
- When a tile is tapped, it moves in that direction
- The tile moves until:
  - It hits another tile blocking the path
  - It goes outside the grid boundaries
  - It reaches its target position

### Win Condition:
- All tiles must be removed from the grid
- When no tiles are left, the level is completed

### Level Data Format:
```json
{
  "nodes": [
    {
      "position": { "x": 0, "y": 0 },
      "direction": "Up"
    }
  ]
}
```

---
