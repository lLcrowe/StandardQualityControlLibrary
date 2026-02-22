# Standard Quality Control Library (SQCL)

품질관리를 위한 Unity C# 유틸리티 라이브러리
A Unity C# utility library for quality control and game development.

## Overview

SQCL provides a collection of reusable systems and utilities for Unity game development. It is organized as a set of Assembly Definition-based modules that can be selectively included in your project.

## Modules

| Module | Description |
|--------|-------------|
| **lLcroweUtil** | Core utilities — math helpers, vector operations, tilemap utilities, priority queues, event queues, and more |
| **ASMCode/AchievementFunc** | Achievement system with editor tools for managing game achievements |
| **ASMCode/DBBase** | Database utilities with Google Sheets CSV integration and editor tooling |
| **ASMCode/InPutKeySystem** | Input key binding and remapping system |
| **ASMCode/LocalizingSystem** | Localization system for multi-language support |
| **ASMCode/Physics** | Physics helper utilities |
| **ASMCode/ThirdpartyAssetScript** | Wrapper scripts for third-party asset integration |
| **ASMCode/UILogic** | UI logic framework and base classes |
| **ContentCode** | Game content code — score system, visual objects, terrain/farm systems, signal system, and more |
| **CustomAtrribute** | Custom C# attributes for Unity inspector enhancement |
| **CustomReportSystem** | Custom reporting and logging system |
| **Editor** | Editor-only tools — custom inspectors, scene view utilities, build tools, and common editor helpers |
| **EditorComponent** | Reusable editor GUI components |
| **Interface** | Shared interface definitions |
| **ObjectPoolSystem** | Generic object pooling system for performance optimization |
| **Pathfinding** | Pathfinding algorithms and utilities |
| **SoundSystem** | Audio management and sound playback system |
| **UI** | UI subsystems — Dialogue, Quest, and Notice display |
| **UnityDOTS** | Unity DOTS (Data-Oriented Technology Stack) integration utilities |

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

## License

This project is licensed under the **GNU Lesser General Public License v2.1** (LGPL-2.1).
See [LICENSE](LICENSE) for the full text.
