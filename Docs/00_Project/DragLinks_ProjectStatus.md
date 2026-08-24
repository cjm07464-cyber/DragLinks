# Drag Links! — Project Status

> 기준일: 2026-08-25  
> 이 문서는 **현재 실제 프로젝트 상태**를 기록한다.

## 1. 현재 개발 단계

[확정] 신규 Unity 프로젝트 생성 완료.

[확정] 전작 Mathcalibur 프로젝트를 개조하지 않고 처음부터 새 프로젝트로 제작한다.

[확정] 이번 프로젝트는 단기 약식 프로토타입이 아니라 **출시와 장기 확장을 전제로 한 본 제작 프로젝트**다.

[현재안] 첫 번째 마일스톤은 `Stage_01`에서 Drag Links!의 핵심 게임 루프를 실제 플레이 가능한 상태로 만드는 것이다.

## 2. Unity 환경

| 항목 | 현재 상태 |
|---|---|
| Unity | 6000.3.9f1 |
| Template | Universal 3D |
| Render Pipeline | URP |
| Primary Platform | PC |
| Project Internal Name | DragLinks 권장 |
| Product Display Name | Drag Links! |

## 3. 현재 씬

`Assets/_Project/Scenes/`

- `TitleScene.unity`
- `MenuScene.unity`
- `StoryScene.unity`
- `Stage_01.unity`

[확정] `Stage_01`은 씬 이름이다.

[확정] 공통 게임 코드는 Stage 1 전용 구조로 만들지 않는다.

## 4. 현재 씬 Hierarchy 상태

각 씬에는 현재 Universal 3D 템플릿 기본 오브젝트만 존재한다.

- `Main Camera`
- `Directional Light`
- `Global Volume`

[현재안] 실제 시스템을 구현하기 전에 미래 예측만으로 빈 Root 오브젝트를 대량 생성하지 않는다.

## 5. 현재 Assets/_Project 구조

현재 확인된 기본 폴더:

- `Art`
- `Audio`
- `Docs`
- `Prefabs`
- `Scenes`
- `ScriptableObjects`
- `Scripts`
- `Settings`
- `Tests`

### 문서 위치 변경 권장

[현재안] `Assets/_Project/Docs`는 사용하지 않고 프로젝트 루트의 `Docs/`로 이동한다.

이유:
- Unity Asset Database에 기획 MD를 넣을 필요가 없음
- `.meta` 관리 불필요
- Codex/AI가 저장소 루트에서 문서를 찾기 쉬움
- 게임 자산과 개발 문서의 역할 분리

[현재안] 프로젝트 루트에 `AGENTS.md`를 둔다.

## 6. 아직 구현되지 않은 핵심

현재 대화 기준으로 다음 핵심 시스템의 본격 구현은 시작 전 단계다.

- BoardState / TileState
- 보드 생성
- PC 드래그
- 수식 계산
- 점수 계산
- 중력 / 충당
- LINKING
- 연쇄 콤보
- 한가은 보석/망치 능력
- Stage Rule
- Enemy Rule
- Unique

실제 구현 상태가 달라지면 이 문서를 갱신한다.

## 7. 다음 추천 작업

1. 루트 `AGENTS.md` 및 `Docs/` 배치
2. `Assets/_Project/Scripts` 하위 시스템 폴더 생성
3. `Assets/_Project/ScriptableObjects` 하위 데이터 폴더 생성
4. 기술 구조 확정
5. Board 데이터 계층부터 구현
6. 순수 계산 테스트 작성
7. Stage_01에서 보드 표현 연결

## 8. 문서 갱신 원칙

이 문서는 "기획상 무엇을 만들 것인가"가 아니라 **현재 실제로 어디까지 만들어졌는가**를 기록한다.

예:
- `BoardState 구현 완료`
- `7×6/8×6 테스트 중`
- `LINKING 기본 테스트 통과`
- `한가은 능력 미구현`

처럼 실제 개발 상태가 바뀔 때 갱신한다.
