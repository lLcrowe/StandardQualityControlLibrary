# SQCL ContentCode → SL 이관 계획서

## 현황 요약

| 구분 | SQCL 파일 | 결과 | 상태 |
|------|----------|------|------|
| 2DContentCode | 3 | SL 이동 | ✅ 완료 |
| GameWolrdTimeEvent | 3 | SL 이동 | ✅ 완료 |
| ScoreSystem | 5 | SL 이동 | ✅ 완료 |
| UI (SceneLoad/Session) | 4 | SL 이동 | ✅ 완료 |
| VisualObject | 36 | SL 이동 | ✅ 완료 |
| Root 파일 | 2 | SL 이동 | ✅ 완료 |
| TerrainSystem/FarmSystem | 6 | SL 이동 | ✅ 완료 |
| HexBasementTileMap | 1 | SL 이동 | ✅ 완료 |
| TestModule | 3 | SL 이동 | ✅ 완료 |
| SignalSystem | 2 | SL 이동 | ✅ 완료 |
| NodeMapSystem (그래프 코어) | 2 | **독립모듈** NodeGraphSystem | ✅ 완료 |
| NodeMapSystem (Voyage 등) | 5 | SL 이동 | ✅ 완료 |
| BasementLevelDesigner | 1 | 제거 (빈 클래스) | 🗑️ 완료 |
| RespawnSystem | 2 | 제거 (GameEcosystem 대체) | 🗑️ 완료 |
| TerrainSystem (FTL 전용) | 28 | **SQCL 보관** `_Archive_FTL_SpaceVehicle/` | 📦 보관 완료 |

**총 103파일 분류 완료 (100%)**

---

## 결과 분류

### SL로 이관 — 75파일
- 2DContentCode, GameWorldTimeEvent, ScoreSystem, UI, VisualObject, Root, FarmSystem, HexBasementTileMap, TestModule, SignalSystem, NodeMapSystem(Voyage)

### 독립 모듈 — NodeGraphSystem
- **경로**: `Modules/NodeGraphSystem/`
- **패키지**: `com.llcrowe.node-graph-system`
- Runtime: GraphNode, GraphMap, GraphSearcher, GraphEvents
- Editor: GraphNodeEditor (Scene 연결 모드), GraphMapEditor (노드 수집 + 미리보기)
- ACtionGame manifest.json 등록 완료, 컴파일 0에러

### SQCL 보관 — `_Archive_FTL_SpaceVehicle/` (28파일)
- **용도**: FTL 스타일 우주선 게임 전용 코드
- **나중에**: FTL 프로젝트 시작 시 해당 프로젝트로 직접 옮기면서 분류
- 포함 내용:
  - BuildingSystem (건물 배치/관리, Doozy 의존)
  - BuildingSystem/SpaceVehicleStructure (파이프/연료탱크/발전기/엔진/스러스터)
  - BuildingSystem/Editor (BuildingData, VoidRoom 인스펙터)
  - BasementTileMap.cs (산소/진공 방 시스템)
  - ConnectPartDestroyForMainFrame.cs (구조 무결성 BFS, MEC 의존)
  - SuctionPowerManager.cs (우주 진공 흡입)
  - VoidRoomInfoChecker.cs (산소 소비)
  - SideScrollMap/ (패럴렉스 스크롤)
  - TestFloodFill.cs (타일맵 BFS)

### 제거 — 3파일
- BasementLevelDesigner (빈 클래스)
- RespawnSystem 2파일 (GameEcosystem 대체)

---

## 이관 작업 완료. (2026-02-12)
