# Drag Links! — Technical Architecture

> 문서 버전: 0.1  
> 기준일: 2026-08-25  
> 대상: Unity/C# 구현

## 1. 목표

Drag Links!는 출시와 장기 확장을 전제로 한다.

따라서 다음 문제를 처음부터 피한다.

- 하나의 거대한 Battle Controller
- View가 게임 상태까지 소유
- 캐릭터 전용 능력이 Board 코드에 하드코딩
- Stage가 늘어날 때 Controller 복제
- 효과 순서가 Coroutine 작성 순서에 우연히 의존
- Config 값이 여러 스크립트에 하드코딩
- 테스트하기 어려운 MonoBehaviour 중심 계산

## 2. 권장 Assets 구조

`Assets/_Project/`

```text
Art/
Audio/
Prefabs/
Scenes/
ScriptableObjects/
Scripts/
Tests/
```

`Scripts/` 아래 권장:

```text
Core/
SceneFlow/
Board/
Input/
Formula/
Score/
Linking/
Turn/
Character/
Unique/
Stage/
Story/
UI/
Data/
Utils/
```

처음부터 비어 있는 하위 폴더를 과도하게 세분화할 필요는 없다.  
실제 클래스가 생길 때 위 책임 단위로 정리한다.

## 3. 프로젝트 루트 문서

Unity `Assets` 밖 프로젝트 루트:

```text
AGENTS.md
Docs/
Assets/
Packages/
ProjectSettings/
```

기획 MD는 `Assets/_Project/Docs`가 아니라 루트 `Docs/` 사용을 권장한다.

## 4. Namespace

권장 최상위 namespace:

`DragLinks`

예:
- `DragLinks.Board`
- `DragLinks.Gameplay`
- `DragLinks.Character`
- `DragLinks.Stage`
- `DragLinks.UI`

처음부터 지나치게 세밀한 namespace 계층을 만들 필요는 없다.

## 5. 핵심 계층

### Domain / State
Unity View와 분리된 실제 게임 상태.

예:
- `BoardState`
- `TileState`
- `DragPath`
- `StageRuntime`
- `ScoreState`

가능하면 순수 C#.

### Resolver / Rule
입력을 받아 결과를 계산.

예:
- `DragRuleResolver`
- `ExpressionResolver`
- `ScoreCalculator`
- `BoardGravityResolver`
- `LinkingDetector`
- `LinkingResolver`

### Controller / Orchestrator
시스템을 어떤 순서로 실행할지 지휘.

예:
- `TurnController`
- `StageController`
- `SceneFlowController`

Controller는 자신이 모든 계산을 직접 하지 않는다.

### View / Presentation
Sprite, Text, Animation, Effect, UI.

예:
- `BoardView`
- `TileView`
- `ScoreView`
- `LinkingView`

View의 표시 상태를 게임 규칙의 진실값으로 사용하지 않는다.

## 6. 권장 주요 클래스 책임

### Core / SceneFlow

#### `GameBootstrap`
- 게임 전역 초기화가 필요한 경우 진입점
- 실제 gameplay 규칙을 넣지 않음

#### `SceneLoader`
- 씬 전환 책임
- 씬 이름 문자열 분산 방지

### Board

#### `BoardState`
- 보드 크기
- 좌표별 `TileState`
- 타일 조회/배치의 진실값

#### `TileState`
기술 구현 예시:

숫자:
- `TileKind.Number`
- `CurrentValue`
- `NumberIdentity`
- `HasGem`
- Position/ID

연산자:
- `TileKind.Operator`
- `OperatorType`
- `HasHammer`
- Position/ID

중요:
- 숫자에 Hammer를 붙일 수 없게
- 연산자에 Gem을 붙일 수 없게
가능하면 타입/검증 구조로 방어한다.

#### `BoardGenerator`
- 초기 보드 생성
- 가중치 기반 숫자/연산자 생성
- Config 사용

#### `BoardGravityResolver`
- 빈칸에 대한 중력 결과 계산

#### `BoardRefillResolver`
- 새 타일 생성 및 빈칸 충당

#### `BoardView`
- BoardState를 화면에 표시
- 게임 규칙 계산 금지

#### `TileView`
- 둥근 사각형 Sprite
- 숫자/연산자 표시
- Gem/Hammer Overlay
- 선택/효과 연출

### Input / Drag

#### `BoardInputController`
- PC pointer/mouse 입력을 받아 DragController에 전달

#### `DragController`
- 현재 DragPath 관리
- 시작/추가/되돌리기/확정

#### `DragRuleResolver`
- 8방향 인접
- 최대 연결
- 다음 타일 타입
- 재사용 금지
- 모드별 규칙

#### `DragPath`
- 순서가 보존된 선택 타일 목록
- Formula / OperatorFusion Mode

### Formula / Score

#### `ExpressionValidator`
- 수식 형식 유효성

#### `ExpressionResolver`
- 사칙연산 결과 A 계산
- Board/UI를 몰라야 함

#### `OperatorFusionResolver`
- `++ → ×`
- `-- → ÷`
- 미정 조합은 명확히 거절

#### `ScoreCalculator`
- A의 유효 약수
- B
- 최종 점수
- 순수 계산 권장

### Linking

#### `LinkingDetector`
입력: `BoardState`

출력: `LinkingResult`

탐색:
- 전체 행
- 전체 열
- 전부 Number인지

숫자값/Identity는 보지 않음.

#### `LinkingResult`
권장 정보:
- 발견 라인 목록
- `LineCount`
- 실제 제거 대상 `UniqueTiles`
- Wave 정보가 필요하면 별도 Result

#### `LinkingResolver`
- Detector 호출
- 제거/중력/충당 반복을 TurnController와 협력
- 누적 연쇄 콤보 스택 계산
- 캐릭터 효과 자체는 실행하지 않음

### Turn

#### `TurnController`
프로젝트 gameplay의 중심 오케스트레이터.

책임:
- 입력 허용/잠금
- 현재 TurnPhase
- Action 확정
- Formula/Fusion 분기
- Score
- Character formula effect
- Board settle
- Linking
- Chain Combo callback
- Stage effect
- End Turn
- Next Turn

금지:
- 자체적으로 모든 계산 알고리즘 구현
- UI animation 코드를 대량 포함

### Character

#### `CharacterDefinition`
정적 캐릭터 데이터.

#### `ICharacterAbility`
캐릭터 능력의 공통 계약/Hook.

#### `HangaeunAbility`
- Start/End Turn 랜덤 Gem/Hammer
- Formula Gem/Hammer trigger
- Chain Combo 효과

Board/Linking이 한가은을 직접 참조하지 않게 한다.

### Stage

#### `StageDefinition`
ScriptableObject 권장.

예:
- Stage ID
- Target Score
- Turn Limit
- Board Config
- Enemy Definition
- Background/BGM reference

#### `StageController`
- 현재 Stage Runtime 관리
- 승패
- 턴 수
- Enemy Rule 연결

#### `EnemyRule`
스테이지 특수 기믹 인터페이스/전략.

## 7. Presentation과 Logic 동기화

권장 방향:

1. Logic이 먼저 결과 데이터 생성
2. View가 해당 결과를 애니메이션으로 표현
3. TurnController는 필요한 연출 완료를 기다림
4. 다음 Phase 진행

애니메이션이 게임 결과를 결정하게 만들지 않는다.

## 8. 효과 Timing

향후 유니크/캐릭터가 늘어날 것을 대비해 효과가 어느 타이밍에 반응하는지 명시할 수 있어야 한다.

개념 예:
- OnGameStart
- OnTurnStart
- AfterExpression
- AfterScore
- BeforeTileConsume
- AfterRefill
- OnLinking
- AfterLinking
- OnChainCombo
- OnEndTurn

정확한 Hook 세트는 실제 구현 규모에 맞춰 최소한부터 도입한다.

중요:
- 처음부터 복잡한 범용 EventBus를 과설계하지 않는다.
- 핵심 순서는 TurnController가 명시적으로 지휘한다.
- UI/Audio 알림에는 이벤트를 사용할 수 있다.

## 9. Mathcalibur에서 가져오지 않을 구조

- 수천 줄 규모의 단일 BattleSceneController 구조
- Tile View가 NumberValue 등 게임 데이터의 최종 진실값을 직접 소유
- 숫자 1→11 변화 후 원래 1인지 별도 예외 리스트로 추적하는 방식
- Shop/UI/Stage/Board가 한 컨트롤러에 결합된 구조

가져올 아이디어:
- Drag 경로 되돌리기
- 사용 타일 제거
- 중력
- 충당
- 반복 줄 검사
- 교차 라인의 타일 중복 제거
- 안전 반복 제한
- 가중치 기반 스폰

## 10. 테스트 전략

순수 C# 우선 테스트:
- ExpressionResolver
- ScoreCalculator
- DragRuleResolver
- LinkingDetector
- BoardGravityResolver

PlayMode:
- TileView 동기화
- Drag 실제 입력
- Turn 전체 처리
- 애니메이션/입력 잠금

상세 테스트:
`DragLinks_TestCases.md`

## 11. Assembly Definition

[현재안] 프로젝트가 실제로 커질 때 asmdef를 도입한다.

초기 후보:
- DragLinks.Core
- DragLinks.Gameplay
- DragLinks.UI

처음부터 많은 asmdef로 쪼개지 않는다.

## 12. 의존 방향 요약

권장:

`Input → Turn → Domain/Resolver → Result → View`

Character/Unique/Stage는 명시된 Hook을 통해 Turn 흐름에 참여한다.

피해야 할 것:
여러 View/Controller/Manager가 서로 양방향 참조하며 게임 상태를 직접 고치는 구조.
