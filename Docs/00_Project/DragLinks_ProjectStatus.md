# Drag Links! — Project Status

> 문서 버전: 0.2  
> 기준일: 2026-08-25  
> 이 문서는 **현재 실제 프로젝트 상태**를 기록한다.

## 1. 현재 개발 단계

[확정] 신규 Unity 프로젝트 생성 완료.

[확정] 전작 Mathcalibur 프로젝트를 개조하지 않고 처음부터 새 프로젝트로 제작한다.

[확정] 이번 프로젝트는 단기 약식 프로토타입이 아니라 **출시와 장기 확장을 전제로 한 본 제작 프로젝트**다.

[확정] 첫 Gameplay 기반 마일스톤 1-A / 1-B / 1-C가 실제 Editor 테스트까지 완료되었다.

현재 단계는:
> **공통 Board/Drag/LINKING 기반 완료 → 최신 한가은 연쇄 콤보 규칙 반영 준비**

상태다.

## 2. Unity 환경

| 항목 | 현재 상태 |
|---|---|
| Unity | 6000.3.9f1 |
| Template | Universal 3D |
| Render Pipeline | URP |
| Primary Platform | PC |
| Project Internal Name | DragLinks |
| Product Display Name | Drag Links! |

## 3. 현재 씬

`Assets/_Project/Scenes/`

- `TitleScene.unity`
- `MenuScene.unity`
- `StoryScene.unity`
- `Stage_01.unity`

[확정] `Stage_01`은 씬 이름이다.

[확정] 공통 게임 코드는 Stage 1 전용 구조로 만들지 않는다.

## 4. Stage_01 현재 Hierarchy/화면 기반

현재 Stage_01에는 Universal 3D 기본 오브젝트와 Gameplay UI 기반이 있다.

기본:
- `Main Camera`
- `Directional Light`
- `Global Volume`

Gameplay:
- `GameplayCanvas`
  - `BoardRoot`
  - `HudRoot`
  - `OverlayRoot`
- `EventSystem`
- Board 표시/Bootstrap 관련 GameObject

[현재안] 보드는 화면 중앙 하단에 크게 배치하고, 향후 좌측에는 플레이어 캐릭터, 우측에는 적 캐릭터를 표시하는 구도를 사용한다.

[현재안] UI는 1920×1080 기준 `Scale With Screen Size`를 사용해 4K에서도 비율을 유지하는 방향으로 테스트 중이다.

## 5. 문서 위치

프로젝트 루트:

```text
AGENTS.md
Docs/
Assets/
Packages/
ProjectSettings/
```

[확정] 기획/기술 MD는 Unity `Assets` 밖 프로젝트 루트의 `Docs/`를 Source of Truth로 사용한다.

## 6. 구현 완료 — 1-A Board Foundation

현재 구현 및 확인 완료:

- `BoardConfig`
- `BoardGenerationSettings`
- `TileState`
- `BoardState`
- `BoardGenerator`
- `BoardBootstrap`
- `BoardView`
- `TileView`
- Runtime/Test asmdef
- EditMode 테스트

핵심 결과:
- Config 기반 7×6 테스트 보드 생성
- Width/Height 변경 가능 구조
- Number/Operator 생성
- `CurrentValue` / `NumberIdentity` 분리
- UI View와 게임 상태 분리
- Seed 기반 재현 가능 생성
- uGUI 기반 타일 표시

## 7. 구현 완료 — 1-B Drag / Remove / Gravity / Refill

현재 구현 및 실제 플레이 확인 완료:

- `BoardCoordinate`
- `DragPath`
- `DragRuleResolver`
- `ExpressionPathValidator`
- `DragController`
- `BoardInputController`
- `BoardGravityResolver`
- `BoardRefillResolver`
- Board settle 결과 구조
- `GameplayActionController`

동작 확인:
- 8방향 드래그
- Number → Operator → Number 교대
- 잘못된 타입 연결 차단
- 최대 연결 길이
- Backtrack
- 자기 교차 방지
- 유효 경로 Release
- 선택 타일 제거
- Gravity
- Refill
- 무효 수식은 보드 미변경
- 처리 후 입력 복귀

## 8. 구현 완료 — 1-C LINKING

현재 구현 및 실제 플레이 확인 완료:

- `LinkingDetector`
- `LinkingResult`
- `LinkingLine`
- `LinkingResolver`
- `LinkingResolutionResult`
- `LinkingWaveResult`
- deterministic Refill 테스트를 위한 최소 생성 인터페이스

현재 동작:
- 전체 Number 행 탐색
- 전체 Number 열 탐색
- 숫자값/Identity와 무관한 LINKING
- 가로/세로 교차 라인 별도 카운트
- 교차 타일 실제 제거 중복 방지
- LINKING 제거 → Gravity → Refill → 재검사
- 여러 Wave 반복
- `TotalLinkingLineCount` 결과 제공
- 처리 중 입력 잠금
- 1024 Wave 기술 안전장치

중요:
현재 코드의 `TotalLinkingLineCount`는 최신 기획에서 **현재 연쇄 콤보 스택 그 자체가 아니다.**

앞으로는:
> 해당 Linking Resolution에서 새로 얻은 **콤보 정산 대기 횟수**

로 사용한다.

## 9. 구현 완료 — 지속형 연쇄 콤보 Runtime 기반

현재 코드 반영 완료:

- `ChainComboRuntimeState`: 턴을 넘어 유지되는 `CurrentStack` / `PendingComboTriggers`
- `ChainComboResolver`: Pending 한 개 단위 정산 API
- `ChainComboStepResult`: 활성화된 1~5스택 단계 결과
- `ChainComboSettlementResult`: 한 Action의 단계 순서와 최종 Runtime 상태
- `GameplayActionController`: LINKING LineCount Pending 등록 및 정산 연결
- 5스택 후 0 초기화, 남은 Pending 재개 및 한 Action 내 복수 5스택 기반
- Gameplay Action 사이 동일 Runtime State 유지 테스트

현재 기반에 반영된 확정 규칙:
- 연쇄 콤보 현재 스택은 턴을 넘어 유지
- LINKING 라인 하나당 콤보 정산 대기 횟수 +1
- LINKING 보드 해결을 먼저 끝낸 후 대기 횟수를 순차 정산
- 스택 증가 때마다 1/2/3/4/5 단계 발생 결과 생성
- 5스택 발동 후 현재 스택은 0

아직 실제 효과가 미구현인 최신 확정 규칙:

- 5스택은 보석 숫자를 실제 파괴
- 5스택 파괴로 생긴 Refill/LINKING은 다시 보드 해결
- 해당 추가 LINKING도 새로운 콤보 정산 대기 횟수로 누적
- 추가 LINKING 종료 후 파괴된 보석 값 합 `S`만큼 최종 현재 숫자 타일 전체 +S
- 이후 남은 콤보 정산 대기 횟수 처리 재개

상세:
- `Docs/01_Gameplay/DragLinks_Linking.md`
- `Docs/02_Characters/DragLinks_Hangaeun.md`
- `Docs/01_Gameplay/DragLinks_TurnFlow.md`

## 10. 아직 구현되지 않은 핵심

- 한가은 Gem/Hammer 실제 기능
- 연쇄 콤보 1~5스택 실제 보드 효과
- 실제 산술 계산
- 점수 계산
- Operator Fusion
- TurnCount
- Stage 목표 점수/승패
- Stage_01 적 기믹
- Unique
- Story/Shop 본 구현
- 타일 낙하/파괴/LINKING 실제 애니메이션

## 11. 현재 테스트용 밸런스

[임시]
- 보드: 7×6
- 기본 최대 드래그: 5
- Number : Operator 카테고리 가중치: 현재 테스트용 3 : 1
- 실제 최종 스폰 가중치는 미정

Mathcalibur의 가중치를 출발 참고값으로 사용할 수 있으나 Drag Links!의 8방향 Drag와 LINKING 빈도 때문에 반드시 별도 재조정한다.

## 12. 다음 추천 작업

1. 판정 이벤트(`연산/사용/파괴/변화`) 데이터 모델 준비
2. Gem/Hammer 기본 능력 구현
3. 한가은 1~5스택 실제 능력과 5스택 중단/보드 재정산/재개 연결
4. 실제 수식 계산 및 점수 시스템

실제 구현 순서는 기능 의존성을 보고 조정 가능하다.

## 13. 문서 갱신 원칙

이 문서는 "기획상 무엇을 만들 것인가"가 아니라 **현재 실제로 어디까지 만들어졌는가**를 기록한다.

기능이 완료되거나 설계가 크게 바뀔 때 마일스톤 단위로 갱신한다.
