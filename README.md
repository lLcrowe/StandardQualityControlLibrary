# Standard Quality Control Library (SQCL)

품질관리를 위한 Unity C# 유틸리티 라이브러리

## 개요

SQCL은 Unity 게임 개발에 사용되는 재사용 가능한 시스템 및 유틸리티 모음입니다. Assembly Definition 기반 모듈로 구성되어 있어 프로젝트에 필요한 모듈만 선택적으로 포함할 수 있습니다.

## 모듈

| 모듈 | 설명 |
|------|------|
| **lLcroweUtil** | 핵심 유틸리티 — 수학 헬퍼, 벡터 연산, 타일맵 유틸, 우선순위 큐, 이벤트 큐 등 |
| **ASMCode/AchievementFunc** | 업적 및 잠금해제 추적, 자동/수동 잠금해제 및 알림 UI |
| **ASMCode/DBBase** | Google Sheets CSV 연동을 통한 불변 게임 데이터 관리 |
| **ASMCode/InPutKeySystem** | 커스터마이징 가능한 입력 키 바인딩 및 리매핑 (레거시 & 새 Input System 지원) |
| **ASMCode/LocalizingSystem** | 다국어 로컬라이징, 런타임 언어 전환 (TextMeshPro & UI) |
| **ASMCode/Physics** | 스프링 물리 유틸리티 — 탄성 운동, 감쇠, 물리 기반 애니메이션 |
| **ASMCode/ThirdpartyAssetScript** | 서드파티 에셋 연동 래퍼 (탐색 인터랙션, 사운드 경로 감지) |
| **ASMCode/UILogic** | 재사용 가능한 UI 로직 — 옵션 메뉴, 확인 창, 초상화 카드 컴포넌트 |
| **ContentCode** | 게임 콘텐츠 시스템 — 점수 관리, 비주얼 오브젝트, 지형/농장, 시그널 브로드캐스팅 |
| **CustomAtrribute** | 커스텀 C# 어트리뷰트 (`ButtonMethod`, `SceneName`, `Tag`) — 인스펙터 기능 확장 |
| **CustomReportSystem** | Google Forms 연동 버그 리포트 시스템 |
| **Editor** | 에디터 도구 — 커스텀 인스펙터, 컴파일 분석기, 빌드 헬퍼, 씬뷰 유틸 |
| **EditorComponent** | 재사용 가능한 에디터 GUI 컴포넌트 |
| **Interface** | 인터랙션 시스템 및 게임 규칙 공용 인터페이스 정의 |
| **ObjectPoolSystem** | 제네릭 오브젝트 풀링 — 동적 컴포넌트 재활용으로 성능 최적화 |
| **Pathfinding** | A* Pathfinding 래퍼 — 포메이션 (Rect, Circle, SemiCircle, Charge) 및 RVO |
| **SoundSystem** | 오디오 관리 — 사운드 풀링, 태그 기반 정리, 거리 기반 재생 |
| **UI/Dialogue** | 노드 기반 대화 시스템 — 애니메이션 텍스트, 대화 DB, NPC 인터랙션 |
| **UI/Quest** | 퀘스트 관리 — 퀘스트 일지 UI, 미션 체커, 보상 지급, 비주얼 노드 편집 |
| **UI/Notice** | 알림 표시 — 설정 가능한 애니메이션, 방향성 이동, 무한 스크롤 |
| **UnityDOTS** | DOTS 연동 — Job System 헬퍼, 엔티티 유틸, CPU 코어 감지, 스레드 세이프 연산 |

## 설치

1. 이 레포를 Unity 프로젝트의 `Assets` 폴더(또는 하위 폴더)에 클론하거나 다운로드합니다:
   ```
   git clone https://github.com/lLcrowe/StandardQualityControlLibrary.git
   ```
2. Unity가 Assembly Definition(`.asmdef`) 파일을 자동 감지하여 각 모듈을 컴파일합니다.
3. 프로젝트의 Assembly Definition에서 원하는 모듈을 참조하면 됩니다.

## 요구사항

- **Unity** 2020.3 이상 (LTS 권장)
- **선택사항:** Doozy UI (BuildingSystem 아카이브 코드용)

## 네임스페이스

모든 핵심 코드는 `lLCroweTool` 네임스페이스 아래에 있습니다:

```csharp
using lLCroweTool;
```

하위 네임스페이스:
- `lLCroweTool.QC.EditorOnly` — 에디터 전용 유틸리티
- `lLCroweTool.Achievement` — 업적 시스템
- `lLCroweTool.ClassObjectPool` — 오브젝트 풀 시스템
- `lLCroweTool.DataBase` — 데이터베이스 유틸리티
- `lLCroweTool.InputKey` — 입력 키 시스템
- `lLCroweTool.Sound` — 사운드 시스템
- `lLCroweTool.DialogueSystem` — 대화 시스템
- `lLCroweTool.QuestSystem` — 퀘스트 시스템
- `lLCroweTool.NoticeDisplay` — 알림 표시
- `lLCroweTool.DOTS` — Unity DOTS 유틸리티

## 라이선스

이 프로젝트는 **MIT License**로 배포됩니다.
자세한 내용은 [LICENSE](LICENSE) 파일을 참고하세요.
