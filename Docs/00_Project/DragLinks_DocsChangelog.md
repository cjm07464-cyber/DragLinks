# Drag Links! — Docs Changelog

## v0.2 — 2026-08-25

### 핵심 변경

- 연쇄 콤보를 **턴마다 초기화되는 최종 스택 방식**에서 **턴을 넘어 유지되는 지속형 스택 방식**으로 변경
- LINKING Line 수를 `PendingComboTriggers` 개념으로 정의
- LINKING 보드 해결 후 Pending을 하나씩 순차 정산하는 흐름 확정
- 5스택에서 실제 보석 숫자 타일을 **파괴**하도록 변경
- 5스택 파괴는 연산/사용이 아니라 파괴 판정임을 확정
- LINKING으로 제거되는 숫자 타일은 연산/사용 판정임을 확정
- 4스택 효과를 "현재 Gem 수 n만큼 모든 Gem Number +n"으로 변경
- 5스택 효과를 "최대 11 Gem 부여 → 모든 Gem 실제 파괴 → 값 합 S → 보드 재정산 → 최종 Number 전체 +S"로 변경
- 5스택 파괴 후 발생한 LINKING도 Pending에 추가하고, +S 후 남은 Pending 정산을 재개하는 흐름 확정
- 6스택 개념 폐기
- `DragLinks_Judgements.md`, `DragLinks_Decisions.md` 추가
- 1-A/1-B/1-C 실제 구현 현황을 ProjectStatus에 반영

### 아직 미정

- 5스택 강제 파괴가 일반 3×3 보석 폭발 효과까지 일으키는지
- 시작 보드 LINKING 처리
- LINKING 1회당 전체 숫자 +1 과거 규칙 유지 여부
- 교차 LINKING 교차점의 사용 카운트 최종 확정
