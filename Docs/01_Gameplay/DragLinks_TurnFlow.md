# Drag Links! — Turn Flow

> 문서 버전: 0.2  
> 기준일: 2026-08-25

이 문서는 Drag Links!의 **처리 순서**를 정의한다.

특히 최신 한가은 연쇄 콤보는:
- LINKING 보드 해결
- 콤보 정산
- 5스택 보드 재해결
을 오갈 수 있으므로 순서를 명시적으로 관리한다.

## 1. 한 턴의 기준

[확정] 유효한 플레이어 행동 1회가 1턴을 소비한다.

유효 행동:
- 정상 수식 드래그
- 정상 연산자 합성

무효 행동:
- 미완성 수식
- 규칙에 맞지 않는 드래그
- 취소된 드래그

무효 행동은 턴을 소비하지 않는다.

## 2. 게임 시작 기본 순서

현재 기준:

1. Stage 데이터 로드
2. 보드 생성
3. 초기 무료 유니크 선택
4. 한가은 기본 능력 최초 발동
   - 보석이 아닌 숫자 중 무작위 1개에 보석
   - 망치가 아닌 연산자 중 무작위 1개에 망치
5. 첫 입력 가능 상태

[미정] 초기 보드에 LINKING이 존재할 경우 최초 능력 전에 자동 처리할지 여부.

## 3. 수식 행동 턴 — 큰 흐름

향후 완성 기준:

1. 입력 대기
2. 플레이어 드래그
3. 드래그 유효성 확정
4. 수식 계산
5. 점수 계산 및 누적
6. 한가은 수식 내 망치/보석 효과
7. 수식에 사용된 타일 사용/제거
8. Gravity
9. Refill
10. LINKING 보드 Resolution
11. 해당 Resolution의 LineCount를 Pending Combo에 추가
12. 연쇄 콤보 순차 정산
13. 5스택이 발생하면 특수 보드 Resolution 수행 후 정산 재개
14. Pending이 0이 될 때까지 반복
15. Stage/Enemy 턴 종료 기믹
16. 턴 종료
17. 한가은 기본 능력
18. 다음 턴 입력 가능

## 4. 연산자 합성 턴

향후 완성 기준:

1. 입력 대기
2. 연산자 2개 드래그
3. 유효 합성 판정
4. 첫 번째 연산자 제거
5. 두 번째 위치를 합성 결과로 변경
6. Gravity / Refill
7. LINKING 보드 Resolution
8. Pending 추가
9. 연쇄 콤보 순차 정산
10. 5스택 특수 Resolution 필요 시 처리
11. Pending 0
12. Stage/Enemy 턴 종료 기믹
13. 턴 종료
14. 한가은 기본 능력
15. 다음 턴 입력 가능

[확정] 연산자 합성 자체는 점수를 얻는 산술식이 아니다.

## 5. 기본 LINKING Resolution

```text
LINKING 탐색
↓
있음
↓
라인 타일 연산/사용 처리
↓
라인 합집합 제거
↓
Gravity
↓
Refill
↓
재탐색
```

없을 때까지 반복한다.

이 구간에서는 캐릭터 스택 효과를 중간중간 끼워 넣지 않는다.

현재 Resolution에서 발생한 각 Line은 이후 정산할 Pending 1개가 된다.

## 6. 연쇄 콤보 순차 정산

LINKING Resolution 종료:

```text
Pending += TotalLinkingLineCount
```

그 후:

```text
while Pending > 0:
    Pending -= 1
    CurrentStack += 1
    해당 스택 효과 실행
```

1~4스택은 보드를 물리적으로 제거하지 않으므로 일반적으로 바로 다음 Pending을 처리한다.

5스택은 별도 특수 흐름으로 전환한다.

## 7. 5스택 특수 흐름

5스택 도달 시:

1. 보석이 아닌 숫자 최대 11개 보석화
2. 모든 보석 숫자의 현재값 합 `S` 저장
3. 모든 보석 숫자 실제 파괴
   - 연산 X
   - 사용 X
   - 파괴 O
4. CurrentStack = 0
5. Gravity
6. Refill
7. LINKING Resolution 시작
8. 새 LINKING이 있으면 전부 연산/사용 처리
9. 새 Resolution의 TotalLinkingLineCount를 Pending에 추가
10. LINKING이 완전히 끝남
11. 현재 보드 모든 Number의 CurrentValue += S
12. 기존 Pending 정산 재개

중요:
`+S`는 보석 파괴 직후가 아니라 **그 파괴로 인해 발생한 모든 LINKING이 끝난 뒤** 적용한다.

따라서 새 LINKING으로 곧 사라질 숫자를 먼저 강화하지 않는다.

## 8. Pending 보존

5스택 때문에 보드 재정산이 발생해도 기존에 아직 처리하지 못한 Pending은 사라지지 않는다.

예:
- CurrentStack 4
- Pending 3
- 첫 Pending으로 5스택
- Pending 2 유지
- 보석 파괴 후 추가 LINKING 2 Line
- Pending 4
- +S 후 4개의 Pending을 0스택부터 계속 처리

## 9. 턴을 넘어 유지되는 값

[확정] Current Chain Combo Stack은 턴 종료 시 초기화하지 않는다.

예:
- 턴 종료 시 Stack=2
- 다음 턴 시작도 Stack=2

반면 `PendingComboTriggers`는 정상적인 턴 처리 종료 전에 모두 정산되어 0이 되는 것이 원칙이다.

## 10. 턴 종료 캐릭터 능력

한가은의 기본 능력은:
- LINKING
- 모든 Pending 연쇄 콤보 정산
- 5스택으로 파생된 추가 LINKING 및 +S

까지 전부 끝난 뒤 발동한다.

그 후 다음 턴.

## 11. 적 기믹 타이밍

[현재안] 적/스테이지의 N턴 기믹은 모든 연쇄 콤보 정산 후, 한가은의 턴 종료 기본 능력보다 먼저 처리하는 방향.

[미정] 동시에 여러 End Turn 효과가 존재할 경우 상세 Priority.

## 12. 목표 점수 도달 타이밍

[확정] 누적 점수가 목표 점수 이상이면 승리 조건 충족.

[미정] 수식 점수 획득 순간 승리 조건을 만족했을 때:
- 이후 망치/보석, LINKING, 연쇄 콤보를 끝까지 처리할지
- 즉시 클리어 흐름으로 전환할지

## 13. 입력 잠금

다음 자동 처리 중에는 입력을 잠근다.

- 점수
- 보석/망치
- 타일 제거
- Gravity/Refill
- LINKING
- 연쇄 콤보 정산
- 5스택 파괴 후 재정산
- 적 기믹

다음 턴 입력 가능 상태가 명시적으로 열릴 때만 드래그를 허용한다.

## 14. 기술 구현 상태/권장 Phase

현재 구현에는 `GameplayActionController`와 `ResolvingLinking` 계열 흐름이 존재한다.

향후 필요하면 다음 개념을 명시적으로 분리한다.

- Idle
- Dragging
- ResolvingAction
- ResolvingScore
- ResolvingCharacterFormulaEffect
- SettlingBoard
- ResolvingLinking
- ResolvingChainCombo
- ResolvingChainFiveStackBoard
- ResolvingStageEffect
- ResolvingEndTurn
- Transitioning

정확한 enum 이름은 코드 구조에 맞게 조정하되, **5스택의 중단/재개 흐름을 암묵적인 Coroutine 순서에만 의존시키지 않는다.**
