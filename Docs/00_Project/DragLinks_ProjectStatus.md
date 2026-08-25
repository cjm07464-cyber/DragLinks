# Drag Links! — Project Status

> 문서 버전: 0.3  
> 기준일: 2026-08-25  
> 이 문서는 **현재 실제 프로젝트 상태**를 기록한다.

## 1. 현재 개발 단계

[확정] 신규 Unity 프로젝트 생성 완료.

[확정] 전작 Mathcalibur 프로젝트를 개조하지 않고 처음부터 새 프로젝트로 제작한다.

[확정] 이번 프로젝트는 단기 약식 프로토타입이 아니라 **출시와 장기 확장을 전제로 한 본 제작 프로젝트**다.

[확정] Gameplay 기반 마일스톤 1-A / 1-B / 1-C가 완료되었다.

[확정] **1-C-R 지속형 연쇄 콤보 Runtime 기반 구현도 완료되었다.**

현재 단계는:
> **Board/Drag/LINKING + Persistent Chain Combo Runtime 기반 완료 → 2-A 한가은 Gem/Hammer Foundation 준비**

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
  - `PlayerVisualRoot`
    - `PlayerVideo` (`RawImage`)
  - `HudRoot`
  - `OverlayRoot`
- `EventSystem`
- Board 표시/Bootstrap 관련 GameObject

Player presentation 준비:
- `PlayerVisualRoot`에 `VideoPlayer`
- `Assets/_Project/Video/RenderTextures/RT_PlayerCharacter`
- RenderTexture → RawImage 연결용 Hierarchy/Inspector 수동 세팅 완료
- Idle/Attack 전환 C#은 아직 미구현

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

## 9. 구현 완료 — 1-C-R Persistent Chain Combo Runtime

[확정] 최신 지속형 연쇄 콤보 Runtime 기반이 코드에 반영되었다.

구현:
- `ChainComboRuntimeState`
- `ChainComboResolver`
- `ChainComboSettlementResult`
- 관련 EditMode 테스트
- `GameplayActionController` 연결
- `TurnPhase.ResolvingChainCombo`

현재 Runtime 의미:
- `CurrentStack`: 0~4, Gameplay Action을 넘어 유지
- `PendingComboTriggers`: 아직 정산하지 않은 LINKING Line 수
- LINKING Resolution 후 `Pending += TotalLinkingLineCount`
- `TryResolveNextStep()` 한 호출당 Pending 1개 정산
- 4→5 시 `ActivatedStack=5` 결과 생성 후 Runtime Stack 즉시 0
- 향후 실제 5스택 효과에서 외부 Orchestrator가 중단/보드 재정산/재개 가능

현재는 1~5스택 **실제 Gem/Destroy/+n/+S 효과는 아직 구현하지 않았다.**

Codex 완료 보고 시점에는 마지막 Asset Import 이후 Unity EditMode `Run All`을 다시 실행하지 않은 상태였다는 검증 메모가 있었으나, 구현 상태 자체는 완료로 기록한다.

## 10. 아직 구현되지 않은 핵심

- 한가은 Gem/Hammer 실제 기능
- 실제 1~5스택 보드 효과
- 5스택 실제 보석 파괴 및 +S
- 실제 산술 계산
- 점수 계산
- Operator Fusion
- TurnCount
- Stage 목표 점수/승패
- Stage_01 적 기믹
- Unique
- Story/Shop 본 구현
- 타일 낙하/파괴/LINKING 실제 애니메이션
- CharacterVideoController 기반 Idle/Attack 자동 전환

## 11. 현재 테스트용 밸런스

[임시]
- 보드: 7×6
- 기본 최대 드래그: 5
- Number : Operator 카테고리 가중치: 현재 테스트용 3 : 1
- 실제 최종 스폰 가중치는 미정

Mathcalibur의 가중치를 출발 참고값으로 사용할 수 있으나 Drag Links!의 8방향 Drag와 LINKING 빈도 때문에 반드시 별도 재조정한다.

## 12. 다음 추천 작업

1. **2-A 한가은 Gem/Hammer Foundation**
   - Number만 Gem
   - Operator만 Hammer
   - Game Start / 완전한 Turn End 후 기본 부여
   - Runtime 상태 유지 / Refill 기본 특수 없음
   - 임시 시각 표시
2. 2-B 수식 Hammer + Gem 기본 능력
3. 2-C 실제 1~5 Chain Combo 효과
4. 실제 수식 계산 및 점수
5. Operator Fusion / Stage / Unique

Player 영상 Presentation은 별도 작은 작업으로:
- `CharacterVideoController`
- Idle loop
- Valid Gameplay Action → Attack 1회
- Attack 종료 → Idle
를 연결할 수 있다.

## 13. 문서 갱신 원칙

이 문서는 "기획상 무엇을 만들 것인가"가 아니라 **현재 실제로 어디까지 만들어졌는가**를 기록한다.

기능이 완료되거나 설계가 크게 바뀔 때 마일스톤 단위로 갱신한다.
