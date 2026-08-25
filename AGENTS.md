# Drag Links! — Codex / AI 작업 지침

> 문서 버전: 0.3  
> 기준일: 2026-08-25  
> 프로젝트 성격: **출시를 목적으로 처음부터 새로 제작하는 Unity PC 캐주얼 퍼즐 게임**

## 1. 프로젝트 정체성

이 저장소는 **Drag Links!**라는 신규 게임 프로젝트다.

- 전작 **Mathcalibur의 리메이크나 코드 개조 프로젝트가 아니다.**
- Mathcalibur는 보드, 드래그, 수식 처리, 중력/충당, 자동 줄 처리 등 일부 아이디어와 시행착오를 참고하는 **레거시 레퍼런스**다.
- Drag Links!의 최신 규칙과 구현 방향은 반드시 `Docs/` 아래 문서를 기준으로 판단한다.
- 레거시 코드나 과거 문서와 Drag Links! 문서가 충돌하면 **Drag Links! 문서가 우선**이다.
- 문서에 `[미정]`으로 적힌 규칙을 AI가 임의로 확정하지 않는다.

## 2. 작업 전에 읽을 문서

모든 작업에서 먼저 `Docs/INDEX.md`를 읽는다.

구현 중 미정 판정이 필요한 경우:
- `Docs/00_Project/DragLinks_OpenQuestions.md`
- `Docs/00_Project/DragLinks_Decisions.md`

도 확인한다.

그다음 현재 작업과 직접 관련된 문서만 추가로 읽는다.

예:
- 보드 작업 → `DragLinks_Board.md`, `DragLinks_Architecture.md`
- 드래그 작업 → `DragLinks_Drag.md`, `DragLinks_TurnFlow.md`
- 점수 작업 → `DragLinks_FormulaAndScore.md`
- LINKING 작업 → `DragLinks_Linking.md`, `DragLinks_Judgements.md`
- 한가은 능력 → `DragLinks_Hangaeun.md`, `DragLinks_Linking.md`, `DragLinks_Judgements.md`
- 유니크 작업 → `DragLinks_Unique.md`, `DragLinks_Judgements.md`
- 스테이지 작업 → `DragLinks_Stage.md`

불필요한 문서를 전부 읽어 컨텍스트를 낭비하지 않는다.

## 3. 문서와 코드의 언어

- 기획 및 기술 설명 문서는 **한국어**를 기본으로 한다.
- 클래스명, 메서드명, enum, 필드명, namespace 등 **코드 식별자는 영어**를 사용한다.
- 동일 내용을 영어와 한국어로 중복 작성하지 않는다.
- 코드 식별자 예시는 `CurrentValue`, `NumberIdentity`, `TurnPhase`처럼 영어로 표기할 수 있다.
- 사람이 검토해야 하는 게임 규칙의 본문은 한국어를 유지한다.

## 4. 현재 기술 환경

- Engine: Unity **6000.3.9f1**
- Render Pipeline: **Universal Render Pipeline (Universal 3D)**
- Primary Platform: **PC**
- Project namespace 권장: `DragLinks`
- 현재 씬:
  - `TitleScene`
  - `MenuScene`
  - `StoryScene`
  - `Stage_01`

## 5. 현재 구현 상태 요약

현재 `Stage_01`에서 다음 핵심 기반이 구현 및 실제 플레이 확인되었다.

- Config 기반 보드 생성 및 UI 표시
- `BoardState` / `TileState`와 View 분리
- 8방향 Number-start Drag
- Number → Operator → Number 교대
- Backtrack
- 최대 연결 길이
- 자기 교차 방지
- 유효 경로 제거
- Gravity
- Refill
- LINKING 탐지
- 교차 LINKING의 라인 수/중복 제거 분리
- LINKING 반복 해결
- 처리 중 입력 잠금
- **1-C-R 지속형 연쇄 콤보 Runtime 기반**
- `ChainComboRuntimeState`
- `ChainComboResolver`
- Persistent `CurrentStack`
- `PendingComboTriggers`
- 5스택 중단/재개를 위한 한 단계 정산 API

아직 구현되지 않은 주요 기능:
- 실제 산술 계산
- 점수
- 연산자 합성
- 한가은 Gem/Hammer 실제 보드 효과
- 연쇄 콤보 1~5스택 실제 효과
- 유니크
- 적 기믹
- 완전한 턴 카운트

상세 현황은 `Docs/00_Project/DragLinks_ProjectStatus.md`를 따른다.

## 6. 핵심 아키텍처 원칙

1. **게임 상태와 화면 표현을 분리한다.**
   - `TileState`와 `TileView`를 같은 책임으로 만들지 않는다.
   - UI/Sprite 상태를 게임 규칙의 진실값으로 사용하지 않는다.

2. **거대한 단일 Controller를 만들지 않는다.**
   - 보드, 수식, 점수, LINKING, 캐릭터 능력, 스테이지 기믹을 하나의 MonoBehaviour에 몰아넣지 않는다.

3. **공통 게임 규칙과 캐릭터 전용 능력을 분리한다.**
   - LINKING은 공통 보드 규칙이다.
   - 보석/망치 및 연쇄 콤보 단계별 효과는 한가은의 캐릭터 능력이다.

4. **처리 순서를 명시적으로 관리한다.**
   - 턴 진행은 `GameplayActionController`, 향후 `TurnController` 또는 동등한 오케스트레이션 계층이 총괄한다.
   - 각 Resolver는 자신이 맡은 계산만 수행한다.
   - 효과 순서를 우연한 `Update()` 호출 순서나 오브젝트 배치 순서에 맡기지 않는다.

5. **밸런스와 콘텐츠 데이터를 하드코딩하지 않는다.**
   - 보드 크기, 스폰 가중치, 목표 점수, 제한 턴 등은 데이터/Config로 교체할 수 있게 만든다.
   - 현재 보드 크기는 7×6과 8×6 중 미정이므로 특정 크기를 코드에 고정하지 않는다.

6. **순수 계산 로직은 테스트 가능하게 만든다.**
   - 수식 계산, 점수 계산, LINKING 탐색, Gravity 등은 가능하면 MonoBehaviour에 의존하지 않는 순수 C# 계층으로 구현한다.

7. **ScriptableObject 원본 데이터와 런타임 상태를 분리한다.**
   - Definition/Config를 플레이 중 누적 상태 저장소처럼 직접 변형하지 않는다.
   - 한가은의 연쇄 콤보 현재 스택처럼 턴을 넘어 유지되는 값은 Runtime State에 둔다.
   - 현재 `ChainComboRuntimeState` / `ChainComboResolver` 기반이 구현되어 있다.

## 7. 중요한 현재 게임 규칙 요약

- 숫자 타일은 기본 1~9의 9가지 고유 숫자 속성을 가진다.
- 숫자 타일의 현재 표시값이 변해도 일반적으로 고유 숫자 속성과 색은 변하지 않는다.
- 현재 기획에서 고유 숫자 속성을 바꾸는 예외는 유니크9다.
- 숫자 타일에만 **보석 속성**을 부여할 수 있다.
- 연산자 타일에만 **망치 속성**을 부여할 수 있다.
- 드래그는 8방향이며 기본 최대 연결 수는 5타일이다.
- 숫자로 시작하면 숫자→연산자→숫자 순서를 지킨다.
- 연산자로 시작하면 현재는 2개 연산자 합성 행동으로 처리한다.
- `+ + → ×`, `- - → ÷`만 현재 확정이다.
- 가로 또는 세로 한 줄 전체가 숫자 타일이면, 숫자값이나 색에 관계없이 **연쇄 보너스(LINKING!)**가 발생한다.
- LINKING으로 실제 제거되는 숫자 타일은 **연산 + 사용 판정**이다.
- 교차 LINKING은 라인 수는 각각 세되, 교차 타일은 실제로 한 번만 제거한다.
- 한가은의 연쇄 콤보 스택은 **턴이 끝나도 초기화되지 않고 유지**된다.
- LINKING 라인 수는 연쇄 콤보의 "정산 대기 횟수"로 추가된다.
- LINKING이 모두 끝난 뒤 대기 횟수를 하나씩 소비하며 스택을 1씩 올리고 해당 단계 효과를 순차 발동한다.
- 5스택 효과가 발동하면 스택은 0으로 초기화된다.
- 5스택의 보석 제거는 실제 **파괴**이며 연산/사용 판정이 아니다.
- 5스택 파괴 후 Refill로 LINKING이 생기면 그 LINKING을 먼저 모두 처리하고, 이후 파괴된 보석 숫자값 합 `S`를 현재 모든 숫자 타일에 `+S` 한다.

상세 규칙은 각 시스템 문서를 읽는다.

## 8. 판정 이벤트를 섞지 않는다

`연산`, `사용`, `파괴`, `변화`는 서로 다른 판정이다.

현재 핵심 예:

| 원인 | 연산 | 사용 | 파괴 |
|---|:---:|:---:|:---:|
| 수식에 실제 사용된 숫자 | O | O | X |
| LINKING으로 제거된 숫자 | O | O | X |
| 한가은 5스택으로 파괴된 보석 숫자 | X | X | O |

상세 정의는 `Docs/01_Gameplay/DragLinks_Judgements.md`를 따른다.

## 9. 코드 수정 시 절차

1. 관련 문서를 읽는다.
2. 현재 코드 구조를 확인한다.
3. 문서와 코드가 충돌하면 임의로 선택하지 않는다.
4. 최신 문서의 확정 규칙을 우선하되, 큰 구조 변경이 필요하면 이유를 명시한다.
5. 구현 후 관련 테스트를 추가하거나 갱신한다.
6. 구현 결과가 기획 규칙을 변경했다면 관련 MD를 반드시 갱신한다.
7. `[미정]`인 사항을 구현 편의상 새 규칙으로 확정하지 않는다.

## 10. 금지 사항

- Mathcalibur 코드를 대량 복사해 새 프로젝트의 중심 구조로 사용하는 것
- `Stage_01`이라는 씬 이름을 이유로 공통 시스템을 `Stage01Controller` 같은 전용 구조로 묶는 것
- 숫자의 현재값만 보고 고유 숫자 속성을 추론하는 것
- 보석/망치 여부를 Sprite 상태만 보고 판단하는 것
- LINKING 처리 중 한가은 전용 효과를 `LinkingDetector`에 하드코딩하는 것
- `TotalLinkingLineCount`를 그대로 "현재 연쇄 콤보 스택"으로 취급하는 것
- 5스택의 보석 파괴를 LINKING/수식 사용과 같은 판정으로 처리하는 것
- 문서에 없는 규칙을 "자연스러워 보여서" 임의로 확정하는 것
- 테스트 편의를 위해 출시 구조와 다른 임시 전용 아키텍처를 핵심 코드에 고착시키는 것

## 11. 문서 상태 태그

- `[확정]` : 현재 기준 규칙
- `[현재안]` : 현재 방향으로 사용하지만 테스트에 따라 조정 가능
- `[임시]` : 프로토타입/밸런스 검증용 값
- `[미정]` : 아직 결정하지 않은 항목

AI는 이 태그의 의미를 유지해야 한다.
