# TileOut - Game Project

Dự án game puzzle tile-based được xây dựng trên Unity với kiến trúc module hóa, sử dụng VContainer cho Dependency Injection và MessagePipe cho Event System.

## 📋 Mục lục

- [Tổng quan](#tổng-quan)
- [Kiến trúc dự án](#kiến-trúc-dự-án)
- [Các Module chính](#các-module-chính)
- [Cách chạy game](#cách-chạy-game)
- [Game Flow](#game-flow)
- [Cấu trúc thư mục](#cấu-trúc-thư-mục)

## 🎮 Tổng quan

**TileOut** là một game puzzle nơi người chơi cần di chuyển các tile theo hướng được chỉ định để loại bỏ chúng khỏi bảng. Mục tiêu là loại bỏ tất cả các tile để hoàn thành level.

### Tính năng chính:
- Hệ thống level với cơ chế tile movement
- UI system với Screen/Sheet/Popup architecture
- Camera system tự động điều chỉnh theo grid
- Resource management system
- Event-driven architecture

## 🏗️ Kiến trúc dự án

Dự án sử dụng **Modular Architecture** với các đặc điểm:

- **Dependency Injection**: VContainer với auto-install system
- **Event System**: MessagePipe với wrapper EventBus
- **Module System**: Tự động scan và install modules theo scope (Project/Scene/GameObject)
- **Lifetime Management**: Scoped services với IDisposable pattern

### Scope System:
- **Project Scope**: Services tồn tại xuyên suốt project (ResourceManager, EventBus)
- **Scene Scope**: Services chỉ tồn tại trong scene hiện tại (GameplayManager, SpawnerService)

## 📦 Các Module chính

### 1. **ModuleManager** (`Assets/Maris-Module/ModuleManager/`)
Module core quản lý việc tự động scan và install các module khác.

**Tính năng:**
- Auto-install modules dựa trên `[AutoInstall]` attribute
- Module scope management (Project/Scene/GameObject)
- Auto-inject MonoBehaviour components
- Lifetime scope management (ProjectLifeTimeScope, SceneLifetimeScope)

**Các class chính:**
- `ModuleRegistry`: Quản lý danh sách installers
- `AutoInstaller`: Tự động install modules
- `AutoInjectProcessor`: Tự động inject dependencies vào MonoBehaviour

### 2. **UI Module** (`Assets/Maris-Module/UI/`)
Hệ thống UI dựa trên ZBase.UnityScreenNavigator với 3 tầng: Screen, Sheet, Popup.

**Tính năng:**
- Navigation service cho Screen/Sheet/Popup
- Base classes: `BaseScreen`, `BaseSheet`, `BasePopup`
- Container management
- Transition animations

**Các class chính:**
- `UINavigationService`: Service điều hướng UI
- `BaseScreen`: Base class cho screens
- `BaseSheet`: Base class cho sheets (InGameSheet, OutGameSheet)
- `BasePopup`: Base class cho popups (VictoryPopup, LoadingPopup, ConfirmPopup)

### 3. **Gameplay Module** (`Assets/0 Game/Scripts/Gameplay/`)
Module chứa logic game chính.

**Các component:**
- **GameplayManager**: Quản lý gameplay state, xử lý events, kiểm tra win condition
- **TileGrid**: Quản lý grid và tiles
- **TileMovementSystem**: Logic di chuyển tiles
- **TileMapSpawner**: Spawn và quản lý tile views
- **GameplayService**: Interface service cho gameplay

**Lifetime**: Scoped (Scene scope)

### 4. **Spawner Module** (`Assets/Maris-Module/Spawner/`)
Object pooling system sử dụng uPools.

**Tính năng:**
- Rent/Return pattern cho GameObject pooling
- Tự động quản lý parent transforms
- Prewarm support

**Các class chính:**
- `SpawnerService`: Service chính cho object pooling
- `ISpawner`: Interface cho spawner

**Lifetime**: Scoped (Scene scope)

### 5. **ResourceManager Module** (`Assets/Maris-Module/ResourceManager/`)
Quản lý game resources (coins, gems, energy, etc.).

**Tính năng:**
- Resource definitions và registry
- Storage system (PlayerPrefs)
- Resource regeneration
- Offline regeneration calculation

**Lifetime**: Singleton (Project scope)

### 6. **CameraSystem Module** (`Assets/Maris-Module/CameraSystem/`)
Hệ thống quản lý camera với Cinemachine.

**Tính năng:**
- Camera rig authoring
- Camera director service
- Auto-framing theo grid bounds
- Camera behaviors (zoom, movement)

**Các class chính:**
- `CameraRigAuthoring`: Authoring component cho camera rigs
- `ICameraDirectorService`: Service điều khiển camera
- `GameCameraSetup`: Setup camera cho gameplay scene

### 7. **PubSubEvent Module** (`Assets/Maris-Module/PubSubEvent/`)
Event system wrapper trên MessagePipe.

**Tính năng:**
- Subscribe/Unsubscribe/Publish events
- Global event bus
- Handler pooling
- Auto-registration

**Các class chính:**
- `IEventBus` / `IGlobalEventBus`: Interface cho event bus
- `EventBus`: Implementation của event bus

**Lifetime**: Singleton (Project scope)

### 8. **DataManager Module** (`Assets/Maris-Module/DataManager/`)
Quản lý data persistence.

**Tính năng:**
- Data providers
- Save/Load system
- Data registry

### 9. **IAP Module** (`Assets/Maris-Module/IAP/`)
In-app purchase system.

**Tính năng:**
- Product management
- Purchase handling
- Reward system

## 🚀 Cách chạy game

### Yêu cầu:
- Unity 2021.3 LTS trở lên
- Các packages: VContainer, MessagePipe, DOTween, uPools, Cinemachine

### Các bước:

1. **Mở project trong Unity Editor**

2. **Chọn LoadingScene làm scene khởi động:**
   - Vào `File > Build Settings`
   - Kéo `Assets/0 Game/Scenes/LoadingScene.unity` vào Build Settings
   - Đặt làm scene đầu tiên (index 0)

3. **Đảm bảo có ProjectLifeTimeScope trong scene:**
   - LoadingScene cần có GameObject với component `ProjectLifeTimeScope`
   - Component này sẽ tự động install các Project-scope modules

4. **Chạy game:**
   - Nhấn Play trong Unity Editor
   - Hoặc build và chạy executable

### Scene Flow:
```
LoadingScene → PlayScene
```

**LoadingScene:**
- Hiển thị loading progress
- Tự động chuyển sang PlayScene sau khi load xong
- Duration: 2.5 giây (có thể config)

**PlayScene:**
- Chứa SceneLifetimeScope để install Scene-scope modules
- Tự động hiển thị OutGameSheet (HomePage) khi start
- Chứa UI containers (ScreenContainer, SheetContainer, PopupContainer)

## 🎯 Game Flow

### 1. **Khởi động (LoadingScene)**
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
- Hiển thị level chain với các level nodes
- Nút Play để bắt đầu level
- Nút Settings (coming soon)

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
- **Tile Movement**: Mỗi tile có hướng (Up/Down/Left/Right)
- **Movement Logic**: Tile di chuyển theo hướng cho đến khi gặp obstacle hoặc ra khỏi grid
- **Win Condition**: Tất cả tiles được loại bỏ khỏi grid
- **Blocked Feedback**: Hiển thị animation khi tile không thể di chuyển

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

## 📁 Cấu trúc thư mục

```
Assets/
├── 0 Game/                          # Game code chính
│   ├── Scripts/
│   │   ├── Camera/                  # Camera behaviors
│   │   ├── Gameplay/                 # Gameplay logic
│   │   │   ├── Data/                # Gameplay data (LevelData, GameplayConfig)
│   │   │   ├── Entity/              # Game entities (Tile, TileGrid)
│   │   │   ├── Event/               # Gameplay events
│   │   │   ├── Service/             # Services và Installers
│   │   │   ├── System/              # Game systems (GameplayManager, TileMovementSystem)
│   │   │   └── View/                # Views (TileView, TileMapSpawner)
│   │   ├── Shop/                    # Shop system
│   │   └── UI/                      # UI controllers và pages
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
│   ├── ModuleManager/               # Module management core
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

## 🔧 Kiến trúc kỹ thuật

### Dependency Injection Pattern:
- **VContainer**: DI framework
- **Auto-Install**: Modules tự động được install dựa trên `[AutoInstall]` attribute
- **Lifetime Scopes**: 
  - Project: Singleton services (ResourceManager, EventBus)
  - Scene: Scoped services (GameplayManager, SpawnerService)

### Event-Driven Architecture:
- **MessagePipe**: Event system backend
- **EventBus**: Wrapper với Subscribe/Unsubscribe/Publish API
- **Event Types**: 
  - `TileTappedEvent`: Khi user tap vào tile
  - `TileMovedEvent`: Khi tile di chuyển thành công
  - `TileBlockedEvent`: Khi tile không thể di chuyển
  - `LevelInitializedEvent`: Khi level được load
  - `LevelCompletedEvent`: Khi level hoàn thành

### Object Pooling:
- **uPools**: Pooling library
- **SpawnerService**: Wrapper service với Rent/Return pattern
- **TileView**: Sử dụng pooling để tối ưu performance

## 📝 Lưu ý

1. **Scene Setup**: Mỗi scene cần có `SceneLifetimeScope` để install Scene-scope modules
2. **Project Scope**: Cần có `ProjectLifeTimeScope` trong scene đầu tiên (LoadingScene)
3. **Level Data**: Level data được lưu trong `Resources/Levels/` dưới dạng JSON
4. **Config Files**: GameplayConfig và các config khác trong `Resources/`

## 🎨 Gameplay Details

### Tile Movement:
- Mỗi tile có một hướng (Up, Down, Left, Right)
- Khi tap vào tile, nó sẽ di chuyển theo hướng đó
- Tile di chuyển cho đến khi:
  - Gặp tile khác chặn đường
  - Ra khỏi grid boundaries
  - Đạt đến vị trí target

### Win Condition:
- Tất cả tiles phải được loại bỏ khỏi grid
- Khi không còn tile nào, level hoàn thành

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
