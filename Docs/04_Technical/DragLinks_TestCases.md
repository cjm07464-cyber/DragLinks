# Drag Links! — Core Test Cases

> 문서 버전: 0.3  
> 기준일: 2026-08-25

이 문서는 출시용 프로젝트에서 최소한 검증해야 할 핵심 규칙을 정리한다.

# A. Drag

## A-01 숫자 시작 기본
`1 → + → 2`

기대:
- 유효 수식 경로
- 드래그 길이 3
- 현재 구현에서는 유효 Action 처리 가능

## A-02 숫자→숫자 직접 연결
기대:
- 두 번째 숫자 선택되지 않음

## A-03 대각선
기대:
- 타입 규칙이 맞으면 4개 대각선 모두 연결 가능

## A-04 재사용
`A → B → C` 후 A 재진입.

기대:
- 불가

## A-05 되돌리기
`A → B → C` 후 B로 이동.

기대:
- C 선택 취소
- Path=A,B

## A-06 최대 길이
기본 MaxDragLength=5.

기대:
- 5개까지
- 6번째 불가

## A-07 미완성 수식
`1 → +` Release.

기대:
- 무효
- 보드 변화 없음

# B. Board Settle

## B-01 단일 빈칸 Gravity
기대:
- 위 타일이 아래로 압축
- 좌표 정상

## B-02 다중 빈칸
기대:
- 순서 유지
- 아래쪽부터 채움

## B-03 Refill
기대:
- Gravity 후 모든 빈칸 신규 타일로 채움

# C. LINKING

## C-01 서로 다른 숫자값 한 행
한 행 전체 Number.

기대:
- LINKING 1 Line
- CurrentValue/Identity 무관

## C-02 Operator 포함
기대:
- 해당 라인 불성립

## C-03 한 열 전체 Number
기대:
- LINKING 1 Line

## C-04 대각선
대각선 전체 Number.

기대:
- LINKING 아님

## C-05 가로/세로 교차
가로 1 + 세로 1.

기대:
- LineCount=2
- 교차 타일 실제 제거 좌표 1회

## C-06 연속 Wave
Wave1=2 Line, Refill 후 Wave2=1 Line.

기대:
- `TotalLinkingLineCount=3`

중요:
- 3은 CurrentChainComboStack이 아니라 Pending 증가량

## C-07 0 Line
기대:
- Resolution 즉시 종료

## C-08 입력 잠금
LINKING 해결 중.

기대:
- 새 Drag 불가
- 종료 후 입력 복귀

# D. Gameplay Judgements

## D-01 수식 숫자
정상 수식에서 숫자 타일 제거.

기대:
- Operation O
- Use O
- Destroy X

## D-02 LINKING 숫자
LINKING으로 숫자 제거.

기대:
- Operation O
- Use O
- Destroy X

## D-03 5스택 보석 숫자
한가은 5스택으로 Gem Number 제거.

기대:
- Operation X
- Use X
- Destroy O

## D-04 4스택 +n
기대:
- ChangeIncrease O
- Use/Destroy 없음
- NumberIdentity 유지

## D-05 5스택 +S
기대:
- 최종 Number에 ChangeIncrease
- NumberIdentity 유지

# E. Number Identity

## E-01 값 상승
NumberIdentity=1, CurrentValue=1에 +10.

기대:
- CurrentValue=11
- Identity=1 유지
- 색 유지

## E-02 유니크1 조건
Identity=1, CurrentValue=2140 타일을 수식/LINKING에서 사용.

기대:
- 사용 카운트 포함

## E-03 5스택 파괴
Identity=1 Gem Number가 5스택으로 파괴.

기대:
- 유니크1 사용 카운트 증가 없음

# F. Hangaeun 기본 능력

## F-01 GameStart
기대:
- 비Gem Number 중 1개 Gem
- 비Hammer Operator 중 1개 Hammer

## F-02 기존 특수 상태 제외
기대:
- 이미 Gem/Hammer 후보 제외

## F-03 유지
기대:
- 사용/제거되지 않은 기존 특수 상태 유지

# G. Hangaeun 수식 Gem/Hammer

## G-01 Hammer + Gem
기대:
- 수식/점수 후 Gem/Hammer 효과 발동

## G-02 주변 Number → 전역 Gem 시도
Gem 3×3 주변의 유효 Number가 3개.

기대:
- Number 숫자값 직접 +1 없음
- 현재 보드 Number 대상 Gem 부여 시도 3회

## G-03 주변 Operator
기대:
- Gem 주변 일반 Operator 자신에게 Hammer 부여

## G-04 Drag Path 주변 판정 제외
기대:
- 현재 수식 경로 타일은 3×3 주변 판정 대상에서 제외

## G-05 이미 Hammer
기대:
- 주변 Operator가 이미 Hammer면 중복 상태 없음

## G-06 새 특수 즉시 재폭발
기대:
- 새로 Gem/Hammer가 된 타일이 즉시 추가 발동하지 않음

## G-07 복수 Hammer
기대:
- Drag 순서상 첫 Hammer만 트리거

## G-08 Gem 최대 3
기대:
- 한 Hammer가 같은 수식의 Gem 최대 3개 처리

## G-09 Gem 4개 이상
기대:
- 우선순위 [미정]
- 임의 확정 금지

## G-10 전역 Gem 랜덤 후보
기대:
- 기존 Gem 제외 여부 / 수식 경로 제외 여부는 문서 상태 태그에 따름
- `[미정]`을 테스트 편의상 임의 확정하지 않음
# H. Persistent Chain Combo

## H-01 1 Line, Stack 0
초기:
- Stack=0
- Pending=1

기대:
- Pending 소비
- Stack 0→1
- 1스택 효과
- 최종 Stack=1

## H-02 스택 턴 유지
턴 종료 Stack=2.

기대:
- 다음 턴 시작 Stack=2

## H-03 여러 Pending 순차 효과
초기:
- Stack=2
- Pending=2

기대:
1. 2→3, 3스택 효과
2. 3→4, 4스택 효과
- 최종 Stack=4

## H-04 1스택
기대:
- 비Gem Number 최대 2개 Gem

## H-05 2스택
기대:
- 최대 5개 Gem

## H-06 3스택
기대:
- 최대 7개 Gem

## H-07 4스택
보드 Gem Number 수=4.

예:
- 값 11, 8, 20, 4

기대:
- 각 Gem Number +4
- 결과 15,12,24,8
- 비Gem Number 변화 없음
- Gem 상태 유지
- Stack=4 유지

## H-08 5스택 기본
초기:
- Stack=4
- Pending=1
- 5스택 신규 Gem 최대 11개 부여 가능

기대:
- 4→5
- 신규 Gem 부여
- 전체 Gem Number 파괴
- Stack=0
- 파괴는 Use/Operation 아님
- Gravity/Refill

## H-09 5스택 S 계산
파괴 직전 Gem 값:
11, 8, 20, 4

기대:
- S=43

## H-10 5스택 +S 타이밍
Gem 파괴 → Refill 후 LINKING 발생.

기대:
- +43 즉시 적용하지 않음
- LINKING 먼저 완전히 해결
- 최종 보드 Number 각각 +43

## H-11 5스택 추가 LINKING Pending
초기:
- Stack=4
- Pending=2

첫 Pending으로 5스택.
5스택 보드 재정산 중 새 LINKING 2 Line.

기대:
- 5스택 진입 직후 Stack=0
- 기존 남은 Pending=1
- 새 Line 2 → Pending=3
- +S
- Pending 3 순차 정산
- 최종 Stack=3

## H-12 한 Action에서 5스택 여러 번
충분한 Pending과 추가 LINKING 존재.

기대:
- 첫 5스택 후 0
- 다시 1→2→3→4→5 가능
- "6스택" 상태는 만들지 않음

## H-13 후보 부족
1/2/3/5스택에서 비Gem Number가 요구 개수보다 적음.

기대:
- 가능한 후보까지만 처리
- 오류 없음

# I. Unique 1

## I-01 Formula Use
Identity 1 Number가 수식으로 사용.

기대:
- UseCount +1

## I-02 LINKING Use
Identity 1 Number가 LINKING으로 제거.

기대:
- UseCount +1

## I-03 5stack Destroy
Identity 1 Gem Number가 5스택으로 파괴.

기대:
- UseCount 변화 없음

## I-04 CurrentValue 무관
Identity=1, CurrentValue=2140 사용.

기대:
- UseCount +1

# J. Formula

## J-01 우선순위
`3 + 4 × 2`
기대 A=11

## J-02 동일 우선순위
`8 ÷ 4 × 2`
기대 A=4

## J-03 0
`3 - 3`
기대:
- A=0
- 점수=0

## J-04 최종 음수
`1 - 3`

기대:
- [미정]
- 임의 효과 금지

# K. Score

## K-01 A=24
유효 약수 1,2,3,4,6,8.

기대:
- 추가 18%
- B=1.18
- Final=28.32

## K-02 반올림
- 28.324 → 28.32
- 28.325 → 28.33

## K-03 표시
103.10 → 103.1

# L. Operator Fusion

## L-01 `+ +`
기대:
- 첫 Operator 제거
- 두 번째 위치 ×
- 1턴

## L-02 `- -`
기대:
- 첫 Operator 제거
- 두 번째 위치 ÷
- 1턴

## L-03 미정 조합
`× ×`

기대:
- Unsupported
- 임의 결과 금지

# M. Turn Flow

## M-01 일반 수식
기대 큰 순서:
수식 → 점수 → 한가은 수식 효과 → 수식 타일 Use/Remove → Gravity/Refill → LINKING Resolution → Pending 정산 → EndTurn

## M-02 5스택 개입
기대:
Pending 정산 → 5스택 → Destroy → Gravity/Refill → LINKING → Pending 추가 → +S → Pending 정산 재개

## M-03 무효 수식
기대:
- TurnCount 증가 없음
- Enemy N턴 카운트 증가 없음

## M-04 처리 중 입력
기대:
- 입력 불가

# N. Stage_01

## N-01 목표 점수
누적 999 → 2점.

기대:
- 누적 1001
- 승리 조건 충족

## N-02 2턴 적 기믹
[현재안] 유효 행동 2회.

기대:
- 적 기믹 시점 도달
- 감소 대상은 [미정]

# O. 미정 규칙 테스트 정책

`[미정]` 규칙:
- 임의 Expected Result 작성 금지
- Pending/Ignore/Unsupported
- 기획 확정 후 본 테스트로 승격
