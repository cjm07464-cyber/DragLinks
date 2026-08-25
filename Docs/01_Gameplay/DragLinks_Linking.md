# Drag Links! — LINKING & Persistent Chain Combo

> 문서 버전: 0.2  
> 기준일: 2026-08-25

## 1. 개념 구분

### 연쇄 보너스 / LINKING

타일 제거 후 중력과 충당이 진행된 보드에서 **가로 또는 세로 한 줄 전체가 숫자 타일**이 되어 자동으로 제거되는 현상.

게임 연출 표기:
**LINKING!**

### LINKING Line

조건을 만족한 가로 1행 또는 세로 1열 하나.

### LINKING Wave

한 번의 탐색 시점에서 동시에 발견된 LINKING Line들의 묶음.

### 연쇄 콤보 스택

한가은의 캐릭터 Runtime State.

[확정]
- 턴이 끝나도 유지된다.
- 0에서 시작한다.
- LINKING Line에 의해 순차적으로 1씩 상승한다.
- 5스택 효과 발동 후 0으로 초기화된다.

### 콤보 정산 대기 횟수

LINKING Line은 발생 즉시 한가은 효과를 실행하지 않는다.

현재 보드의 LINKING을 먼저 모두 해결한 뒤, 발견한 **LineCount만큼 정산 대기 횟수**를 쌓는다.

이 대기 횟수를 하나씩 소비하면서 현재 연쇄 콤보 스택을 +1 하고 해당 단계 효과를 순차 발동한다.

## 2. LINKING 조건

[확정] 한 행 전체가 숫자 타일이면 LINKING 1 Line.

[확정] 한 열 전체가 숫자 타일이면 LINKING 1 Line.

[확정] 숫자값은 서로 달라도 된다.

[확정] 고유 숫자 속성/색도 서로 달라도 된다.

즉, 판정은 "같은 숫자"가 아니라 **타일 종류가 모두 Number인가**를 본다.

[확정] 대각선은 LINKING 라인 판정에 사용하지 않는다.

## 3. LINKING으로 제거되는 타일 판정

[확정] LINKING으로 실제 제거되는 숫자 타일은:

- 연산 판정 O
- 사용 판정 O
- 파괴 판정 X

따라서 유니크1 같은 사용 횟수 조건에 포함된다.

상세:
`DragLinks_Judgements.md`

## 4. 한 Wave 처리

1. 현재 보드에서 모든 LINKING Line을 찾는다.
2. LineCount를 기록한다.
3. 발견된 각 Line을 연쇄 보너스로 표시한다.
4. Line에 포함된 타일의 합집합을 실제 제거 대상으로 만든다.
5. 해당 타일을 연산/사용 처리 후 제거한다.
6. Gravity.
7. Refill.
8. 다시 LINKING 탐색.

## 5. 교차 LINKING

[확정] 가로와 세로 LINKING이 교차할 수 있다.

예:
- 가로 1 Line
- 세로 1 Line
- 중앙 타일 공유

결과:
- `LineCount = 2`
- 교차 타일은 실제 제거 좌표에 한 번만 존재

따라서 **콤보 정산 대기 횟수는 +2**가 된다.

[현재안] 교차 타일의 사용 이벤트는 실제 제거 기준으로 한 번만 기록한다.

## 6. 기본 LINKING Resolution

플레이어 Action의 최초 제거/Gravity/Refill 이후:

```text
LINKING 탐색
↓
있음
↓
해당 Line 타일 사용/제거
↓
Gravity
↓
Refill
↓
다시 탐색
```

을 LINKING이 없을 때까지 반복한다.

예:
- Wave 1: 2 Line
- Wave 2: 1 Line
- Wave 3: 0

이 Resolution에서 새로 얻는 콤보 정산 대기 횟수:
`3`

중요:
`3`은 **현재 연쇄 콤보 스택 그 자체가 아니다.**

예를 들어 이전 턴부터 현재 스택이 2였다면 이 3회는 이후:
- 2→3
- 3→4
- 4→5

를 순차적으로 처리할 수 있다.

## 7. 연쇄 콤보 스택 유지

[확정] 연쇄 콤보 스택은 LINKING Resolution 또는 턴이 끝나도 초기화되지 않는다.

예:
- 현재 0
- 이번 Resolution LineCount 2
- 순차 정산 → 1스택 효과 → 2스택 효과
- 최종 현재 스택 2
- 턴 종료
- 다음 턴에도 2 유지

다음 턴에서 LINKING 1 Line:
- 2→3
- 3스택 효과 발동
- 현재 스택 3 유지

## 8. 순차 정산

보드 LINKING이 완전히 끝난 뒤:

```text
PendingComboTriggers > 0 ?
↓ YES
Pending -1
CurrentStack +1
해당 스택 효과 실행
↓
다음 Pending 처리
```

[확정] 중간 스택 효과를 건너뛰지 않는다.

예:
현재 스택 2, Pending 3:
1. 2→3 → 3스택 효과
2. 3→4 → 4스택 효과
3. 4→5 → 5스택 효과 → 스택 0

## 9. 5스택의 특수 처리

5스택은 실제 보석 숫자 타일을 파괴하므로 보드에 빈칸이 생긴다.

따라서 일반 1~4스택과 달리 **콤보 정산 흐름을 잠시 중단**한다.

상세 순서:

1. 5스택 도달
2. 5스택 효과의 신규 보석 부여
3. 현재 모든 보석 숫자의 파괴 직전 현재값 합 `S` 저장
4. 모든 보석 숫자 실제 파괴
5. 현재 연쇄 콤보 스택 0으로 초기화
6. Gravity
7. Refill
8. LINKING 탐색
9. LINKING이 있으면 일반 LINKING Resolution처럼 모두 사용/제거/Gravity/Refill
10. 이 과정에서 발견한 LineCount를 `PendingComboTriggers`에 추가
11. LINKING이 더 이상 없으면 현재 보드의 모든 숫자 타일 현재값 `+S`
12. 이후 남아 있는 `PendingComboTriggers` 정산을 재개

## 10. 5스택 예시

초기:
- CurrentStack = 4
- Pending = 2

첫 Pending:
- 4→5
- 5스택 발동
- Stack = 0
- Pending = 1

보석 파괴 후 Refill로 LINKING 2 Line 발생:
- LINKING을 먼저 전부 해결
- Pending = 기존 1 + 신규 2 = 3

LINKING 종료 후:
- 최종 현재 Number 전체 +S

정산 재개:
- 0→1
- 1→2
- 2→3

최종:
- CurrentStack = 3
- Pending = 0

## 11. 한 Action에서 5스택 여러 번 가능

[확정] 충분한 Pending이 존재하면 한 번의 플레이어 Action 처리 안에서도 5스택을 여러 번 발동할 수 있다.

각 5스택은:
- 보석 파괴
- 보드 재정산
- 추가 LINKING 수집
- +S
- 남은 Pending 정산

순서를 독립적으로 수행한다.

5스택 도달 후 "6스택"으로 올라가지 않는다.

## 12. 한가은 효과 요약

상세는 `Docs/02_Characters/DragLinks_Hangaeun.md`.

- 1스택 → 비보석 숫자 최대 2개 보석
- 2스택 → 비보석 숫자 최대 5개 보석
- 3스택 → 비보석 숫자 최대 7개 보석
- 4스택 → 보석 개수 `n`, 모든 현재 보석 숫자 각각 `+n`
- 5스택 → 비보석 숫자 최대 11개 보석 → 모든 보석 실제 파괴 → 보드 재정산 → 파괴값 합 `S`를 최종 현재 숫자 전체 각각 `+S` → 스택 0

## 13. 공통 LINKING과 캐릭터 효과 분리

`LinkingDetector` / `LinkingResolver`가 알아야 하는 것:
- Line 목록
- LineCount
- 중복 없는 제거 좌표
- Wave
- 전체 Resolution의 `TotalLinkingLineCount`

한가은 전용 시스템이 알아야 하는 것:
- CurrentStack
- PendingComboTriggers
- 1~5스택 효과
- 5스택 특수 중단/재개 흐름

공통 `LinkingDetector`에 한가은 효과를 하드코딩하지 않는다.

현재 구현의 `TotalLinkingLineCount`는 계속 유용하지만, 최신 설계에서는 **Pending 증가량**으로 사용한다.

## 14. 초기 보드

[미정] 게임 시작 직후 이미 LINKING 라인이 존재하는 보드를 허용할지 여부.

현재 구현은 게임 시작 즉시 자동 LINKING하지 않으며, 유효 Action 이후 전체 보드를 검사한다.

최종 기획 확정 전까지 초기 보드 규칙을 임의 변경하지 않는다.

## 15. 안전장치

현재 구현에는 1024 Wave 기술 안전장치가 있다.

이 값은:
- 게임 밸런스상 최대 연쇄 수가 아님
- 정상 플레이의 콤보 스택 상한이 아님
- 비정상 무한루프 보호용

안전장치가 발동하면 명확한 로그를 남긴다.
