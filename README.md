# Standard Quality Control Library (SQCL)

품질관리를 위한 Unity C# 유틸리티 라이브러리
A Unity C# utility library for quality control and game development.

## Overview

SQCL provides a collection of reusable systems and utilities for Unity game development. It is organized as a set of Assembly Definition-based modules that can be selectively included in your project.

## Modules

| Module | Description |
|--------|-------------|
| **lLcroweUtil** | Core utilities — math helpers, vector operations, tilemap utilities, priority queues, event queues, and more |
| **ASMCode/AchievementFunc** | Achievement and unlockable tracking with automatic/manual unlock and notification UI |
| **ASMCode/DBBase** | Database management with Google Sheets CSV integration for immutable game data |
| **ASMCode/InPutKeySystem** | Customizable input key binding and remapping with legacy and new Input System support |
| **ASMCode/LocalizingSystem** | Multi-language localization with runtime language switching for TextMeshPro and UI |
| **ASMCode/Physics** | Spring physics utilities for elastic motion, damping, and physics-based animation |
| **ASMCode/ThirdpartyAssetScript** | Integration wrappers for third-party assets (exploration interactions, sound path detection) |
| **ASMCode/UILogic** | Reusable UI logic — option menus, confirmation windows, and portrait card components |
| **ContentCode** | Game content systems — score management, visual objects, terrain/farm, signal broadcasting |
| **CustomAtrribute** | Custom C# attributes (`ButtonMethod`, `SceneName`, `Tag`) for enhanced inspector functionality |
| **CustomReportSystem** | Bug reporting system with Google Forms integration for collecting user feedback |
| **Editor** | Editor tools — custom inspectors, compilation analyzers, build helpers, scene view utilities |
| **EditorComponent** | Reusable editor GUI components |
| **Interface** | Shared interface definitions for interaction systems and game rules |
| **ObjectPoolSystem** | Generic object pooling with dynamic component recycling for performance optimization |
| **Pathfinding** | A* Pathfinding wrapper with formation handling (Rect, Circle, SemiCircle, Charge) and RVO |
| **SoundSystem** | Audio management with sound pooling, tag-based organization, and distance-based playback |
| **UI/Dialogue** | Node-based dialogue system with animated text, dialogue database, and NPC interaction |
| **UI/Quest** | Quest management with quest diary UI, mission checkers, reward givers, and visual node editing |
| **UI/Notice** | Notification display with configurable animations, directional movement, and infinite scroll |
| **UnityDOTS** | DOTS integration — job system helpers, entity utilities, CPU core detection, thread-safe ops |

## Installation

1. Clone or download this repository into your Unity project's `Assets` folder (or as a subfolder):
   ```
   git clone https://github.com/lLcrowe/StandardQualityControlLibrary.git
   ```
2. Unity will automatically detect the Assembly Definition (`.asmdef`) files and compile each module.
3. Reference the desired assembly definitions from your project's own assembly definitions.

## Requirements

- **Unity** 2020.3 or later (LTS recommended)
- **Optional:** Doozy UI (for BuildingSystem archive code)

## Namespace

All core code lives under the `lLCroweTool` namespace:

```csharp
using lLCroweTool;
```

Sub-namespaces include:
- `lLCroweTool.QC.EditorOnly` — Editor-only utilities
- `lLCroweTool.Achievement` — Achievement system
- `lLCroweTool.ClassObjectPool` — Object pool system
- `lLCroweTool.DataBase` — Database utilities
- `lLCroweTool.InputKey` — Input key system
- `lLCroweTool.Sound` — Sound system
- `lLCroweTool.DialogueSystem` — Dialogue system
- `lLCroweTool.QuestSystem` — Quest system
- `lLCroweTool.NoticeDisplay` — Notice display
- `lLCroweTool.DOTS` — Unity DOTS utilities

## License

This project is licensed under the **MIT License**.
See [LICENSE](LICENSE) for the full text.
