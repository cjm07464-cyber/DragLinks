# Drag Links! — Technical Architecture

> 문서 버전: 0.2  
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
- "타일이 사라졌다"는 사실만으로 사용/파괴 판정을 추론

## 2. 현재 Assets 구조

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

현재 `Scripts/`에 사용 중인 주요 책임:
- Board
- Data
- Input
- Linking
- Turn
- UI

향후:
- Formula
- Score
- Character
- Unique
- Stage
- Story
- Core/SceneFlow

등을 실제 구현 시점에 추가한다.

## 3. 프로젝트 루트 문서

Unity `Assets` 밖 프로젝트 루트:

```text
AGENTS.md
Docs/
Assets/
Packages/
ProjectSettings/
```

기획/기술 MD는 루트 `Docs/`를 Source of Truth로 사용한다.

## 4. Namespace

최상위:
`DragLinks`

필요에 따라:
- `DragLinks.Board`
- `DragLinks.Input`
- `DragLinks.Linking`
- `DragLinks.Character`
- `DragLinks.Stage`
- `DragLinks.UI`

과도하게 세분화하지 않는다.

## 5. 핵심 계층

### Domain / State
Unity View와 분리된 실제 게임 상태.

현재 예:
- `BoardState`
- `TileState`
- `BoardCoordinate`
- `DragPath`

향후:
- `CharacterRuntimeState`
- `ChainComboRuntimeState`
- `StageRuntime`
- `ScoreState`

가능하면 순수 C#.

### Resolver / Rule
입력을 받아 결과 계산.

현재 예:
- `DragRuleResolver`
- `ExpressionPathValidator`
- `BoardGravityResolver`
- `BoardRefillResolver`
- `LinkingDetector`
- `LinkingResolver`

향후:
- `ExpressionResolver`
- `ScoreCalculator`
- `ChainComboResolver`
- `HangaeunAbilityResolver`

### Controller / Orchestrator

현재:
- `GameplayActionController`
- `BoardBootstrap`

향후 필요에 따라:
- `TurnController`
- `StageController`
- `SceneLoader`

Controller는 계산 알고리즘 자체를 모두 소유하지 않는다.

### View / Presentation

현재:
- `BoardView`
- `TileView`

향후:
- `ScoreView`
- `LinkingView`
- `ChainComboView`

View 상태를 게임 규칙의 진실값으로 사용하지 않는다.

## 6. 현재 구현된 Board 책임

### `BoardState`
- Width / Height
- 좌표별 TileState
- 조회/배치/제거 Source of Truth

### `TileState`
숫자:
- TileKind.Number
- CurrentValue
- NumberIdentity
- HasGem 준비

연산자:
- TileKind.Operator
- OperatorType
- HasHammer 준비

### `BoardGenerator`
- 가중치 기반 타일 생성
- Seed 재현
- Refill과 동일 생성 규칙 공유

### `BoardGravityResolver`
- 빈칸 압축
- 이동 결과 반환

### `BoardRefillResolver`
- 빈칸 충당
- 생성 결과 반환

### `BoardView` / `TileView`
- BoardState 결과를 uGUI로 표현
- 직접 게임 규칙 계산 금지

## 7. 현재 구현된 Input 책임

### `DragController`
- 현재 DragPath 관리

### `DragRuleResolver`
- 8방향
- 타입 교대
- 재사용
- Backtrack
- MaxDragLength
- 자기 교차

### `ExpressionPathValidator`
- 현재 Number-start 유효 경로 형식

### `BoardInputController`
- TileView의 pointer 이벤트를 Drag 계층에 전달
- 선택 표현 제어
- 게임 규칙 Source of Truth 아님

## 8. 현재 구현된 LINKING 책임

### `LinkingDetector`
- 모든 행/열 검사
- 전부 Number인지
- 숫자값/Identity 무시

### `LinkingResult`
- Line 목록
- LineCount
- 중복 없는 제거 좌표

### `LinkingResolver`
- LINKING 탐지
- 제거
- 기존 Gravity/Refill 재사용
- Wave 반복
- `TotalLinkingLineCount`
- 1024 Wave 기술 안전장치

중요:
최신 기획에서 `TotalLinkingLineCount`는 **현재 연쇄 콤보 스택이 아니다.**

이는 해당 Linking Resolution이 새로 만들어낸:
`PendingComboTriggers 증가량`
으로 사용한다.

## 9. 최신 연쇄 콤보 기술 구조

캐릭터 전용 Runtime State에 최소한 다음 개념이 필요하다.

### Current Stack
예:
`CurrentChainComboStack`

- 턴을 넘어 유지
- 0~4 유지
- Pending 소비 시 +1
- 5스택 효과 발동 후 0

### Pending Combo Triggers
예:
`PendingComboTriggers`

- 아직 스택 효과로 정산하지 않은 LINKING Line 수
- Linking Resolution이 끝날 때 `+= TotalLinkingLineCount`
- 5스택 특수 보드 Resolution 중 새 LINKING이 발생해도 추가
- 정상 Action 종료 전 최종적으로 0까지 정산

### Chain Combo Resolver / Orchestrator
책임:
- Pending 하나 소비
- Stack 증가
- 해당 캐릭터 스택 효과 호출
- 5스택이면 일반 정산 일시 중지
- 5스택 보드 파괴/재정산을 Gameplay/Turn 오케스트레이터와 연결
- +S 완료 후 Pending 정산 재개

중요:
`LinkingDetector`가 한가은을 직접 알지 않는다.

## 10. 5스택의 오케스트레이션

5스택은 단순 Character Modifier가 아니다.

실제 타일을 파괴하므로:

```text
ChainCombo Resolver
↓
Hangaeun 5 Stack Result
↓
보석 파괴
↓
Board Gravity/Refill
↓
LinkingResolver
↓
새 Pending 추가
↓
+S
↓
ChainCombo 정산 재개
```

처럼 Turn/Gameplay 오케스트레이션이 필요하다.

한가은 Ability가 임의로 `BoardView`를 직접 움직이거나 Refill Coroutine을 시작하지 않는다.

## 11. Gameplay 판정 이벤트

새로운 유니크 확장을 위해 다음 판정을 분리한다.

- Operation
- Use
- Destroy
- ChangeIncrease

현재 예:
- Formula Number → Operation + Use
- LINKING Number → Operation + Use
- Hangaeun 5stack Gem Number → Destroy
- Hangaeun 4stack +n → ChangeIncrease
- Hangaeun 5stack +S → ChangeIncrease

상세:
`Docs/01_Gameplay/DragLinks_Judgements.md`

정확한 코드 명칭은 현재 구조에 맞게 결정한다.

## 12. Formula / Score 예정

### `ExpressionResolver`
- 실제 사칙연산 A 계산
- Board/UI 독립

### `ScoreCalculator`
- 약수 배율 B
- 최종 점수
- 순수 계산

### `OperatorFusionResolver`
- `++ → ×`
- `-- → ÷`
- 미정 조합 명시적 거절

## 13. Character 예정

### `CharacterDefinition`
정적 데이터.

### Character Runtime
턴을 넘어 유지되는 캐릭터 상태.

한가은:
- CurrentChainComboStack

### `HangaeunAbility`
- GameStart/EndTurn Gem/Hammer
- Formula Hammer/Gem
- 1~5 Chain Combo effects

공통 Linking/Board가 한가은을 직접 참조하지 않게 한다.

## 14. Presentation과 Logic 동기화

권장:

1. Logic 결과 확정
2. Result 객체 생성
3. View 애니메이션
4. 완료 후 다음 Phase

애니메이션이 게임 결과를 결정하게 만들지 않는다.

향후 5스택처럼 복잡한 흐름에서도 같은 원칙 유지.

## 15. 효과 Timing

향후 개념 예:
- OnGameStart
- OnTurnStart
- AfterExpression
- AfterScore
- BeforeTileConsume
- OnTileUsed
- OnTileDestroyed
- OnNumberIncreased
- AfterRefill
- OnLinkingResolved
- OnChainComboStep
- OnEndTurn

처음부터 거대한 범용 EventBus를 만들지는 않는다.

핵심 순서는 오케스트레이터가 명시적으로 관리하고, 유니크/캐릭터 구독이 실제로 필요해질 때 최소한의 이벤트 계층을 추가한다.

## 16. Mathcalibur에서 가져오지 않을 구조

- 수천 줄 규모 단일 BattleSceneController
- Tile View가 게임 데이터의 Source of Truth
- 숫자 CurrentValue만 보고 원래 Identity를 역추론
- Shop/UI/Stage/Board가 한 컨트롤러에 결합

가져올 아이디어:
- Drag Backtrack
- 제거
- Gravity
- Refill
- 반복 줄 검사
- 교차 Line 중복 제거
- 기술 안전장치
- 가중치 스폰

## 17. 테스트 전략

EditMode / 순수 C# 우선:
- BoardState
- BoardGenerator
- DragRuleResolver
- Gravity/Refill
- LinkingDetector/Resolver
- 향후 ChainComboRuntime/Resolver
- Judgement 이벤트
- ExpressionResolver
- ScoreCalculator

PlayMode:
- TileView 동기화
- 실제 Pointer Drag
- Turn 전체 처리
- 5스택 파괴 → Refill → LINKING → +S → Pending 재개
- 애니메이션/입력 잠금

상세:
`DragLinks_TestCases.md`

## 18. Assembly Definition

현재:
- Runtime asmdef
- EditMode Test asmdef

[현재안] 규모가 실제로 커질 때 Core/GamePlay/UI 등으로 추가 분리 여부를 검토한다.

처음부터 많은 asmdef로 쪼개지 않는다.

## 19. 의존 방향 요약

권장:

`Input → Gameplay/Turn Orchestrator → Domain/Resolver → Result → View`

Character/Unique/Stage는 명시된 Hook/판정으로 참여한다.

피해야 할 것:
- View가 게임 상태를 직접 결정
- LinkingDetector가 Hangaeun 스택을 소유
- Hangaeun Ability가 View/Coroutine을 직접 조종
- RemoveTile 호출만으로 Use/Destroy 의미를 자동 추론
