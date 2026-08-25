# Drag Links! 문서 인덱스

> 문서 버전: 0.3  
> 기준일: 2026-08-25

이 문서는 Drag Links! 프로젝트의 **문서 지도**다.  
새로운 개발자나 AI가 프로젝트 맥락을 전혀 모르는 상태에서도 필요한 문서만 빠르게 찾을 수 있도록 한다.

## 1. 문서 우선순위

규칙이나 설명이 서로 충돌할 경우 다음 순서를 기준으로 확인한다.

1. `Docs/00_Project/DragLinks_Main.md`
2. 해당 시스템의 최신 전용 문서
3. `Docs/00_Project/DragLinks_Decisions.md`
4. `Docs/04_Technical/` 아래의 기술 문서
5. `Docs/04_Technical/DragLinks_TestCases.md`
6. `Docs/99_Archive/`의 과거 자료
7. Mathcalibur 레거시 자료

실제 개발 중 최신 기획 결정이 아직 문서에 반영되지 않았음이 확인되면 **문서를 먼저 갱신한 뒤 구현과 맞춘다.**

## 2. 프로젝트 전체를 처음 파악할 때

1. `00_Project/DragLinks_Main.md`
2. `00_Project/DragLinks_ProjectStatus.md`
3. `00_Project/DragLinks_SceneFlow.md`
4. `00_Project/DragLinks_Decisions.md`
5. `00_Project/DragLinks_OpenQuestions.md`
6. `01_Gameplay/DragLinks_TurnFlow.md`
7. 필요한 시스템 문서

## 3. 문서 목록

### 00_Project

| 문서 | 내용 |
|---|---|
| `DragLinks_Main.md` | 게임 정체성, 핵심 규칙, 전체 방향, 용어 |
| `DragLinks_ProjectStatus.md` | 현재 Unity 프로젝트 상태와 구현 단계 |
| `DragLinks_SceneFlow.md` | Title/Menu/Story/Stage 씬 역할과 흐름 |
| `DragLinks_Decisions.md` | 중요한 기획/기술 결정과 변경 이유 기록 |
| `DragLinks_OpenQuestions.md` | 구현 전 확인이 필요한 미정/충돌 가능 항목 |
| `DragLinks_DocsChangelog.md` | 문서 세트 버전별 핵심 변경 요약 |

### 01_Gameplay

| 문서 | 내용 |
|---|---|
| `DragLinks_Board.md` | 보드, 타일, 숫자 속성, 생성, 제거, 중력, 충당 |
| `DragLinks_Drag.md` | PC 드래그, 8방향 연결, 되돌리기, 수식/연산자 합성 입력 |
| `DragLinks_FormulaAndScore.md` | 사칙연산, 수식 결과, 약수 배율, 점수 |
| `DragLinks_Linking.md` | 연쇄 보너스, LINKING!, 지속형 연쇄 콤보 정산 |
| `DragLinks_Judgements.md` | 연산/사용/파괴/변화 판정과 이벤트 의미 |
| `DragLinks_TurnFlow.md` | 한 턴의 전체 처리 순서와 5스택 특수 흐름 |

### 02_Characters

| 문서 | 내용 |
|---|---|
| `DragLinks_CharacterSystem.md` | 플레이어블 캐릭터 공통 구조와 확장 방향 |
| `DragLinks_Hangaeun.md` | 한가은의 보석/망치 및 1~5스택 능력 |

### 03_Content

| 문서 | 내용 |
|---|---|
| `DragLinks_Stage.md` | 스테이지 규칙, Stage_01, 적 기믹, 향후 요소 |
| `DragLinks_Story.md` | 세계관, S.A.V.E, Link, 스토리 톤 |
| `DragLinks_Unique.md` | 유니크/수치형 성장과 사용 판정 예시 |

### 04_Technical

| 문서 | 내용 |
|---|---|
| `DragLinks_Architecture.md` | 스크립트 역할 배분, 의존 관계, 최신 연쇄 콤보 구조 |
| `DragLinks_DataAndConfig.md` | ScriptableObject, Config, Runtime State, 데이터 원칙 |
| `DragLinks_TestCases.md` | 시스템별 필수 테스트 케이스 |

### 99_Archive

과거 규칙이나 폐기된 문서를 보관한다.  
현재 구현의 근거로 직접 사용하지 않는다.

## 4. 작업별 최소 참조 문서

### 보드/타일
- `DragLinks_Board.md`
- `DragLinks_Architecture.md`
- `DragLinks_DataAndConfig.md`

### 드래그
- `DragLinks_Drag.md`
- `DragLinks_TurnFlow.md`

### 수식/점수
- `DragLinks_FormulaAndScore.md`
- `DragLinks_Judgements.md`
- `DragLinks_TestCases.md`

### LINKING
- `DragLinks_Linking.md`
- `DragLinks_Judgements.md`
- `DragLinks_TurnFlow.md`

### 한가은
- `DragLinks_CharacterSystem.md`
- `DragLinks_Hangaeun.md`
- `DragLinks_Linking.md`
- `DragLinks_Judgements.md`
- `DragLinks_TurnFlow.md`

### 유니크
- `DragLinks_Unique.md`
- `DragLinks_Judgements.md`

### 씬 이동
- `DragLinks_SceneFlow.md`
- `DragLinks_Architecture.md`

### Stage_01
- `DragLinks_Stage.md`
- `DragLinks_TurnFlow.md`

## 5. 문서 작성 언어

- 본문: 한국어
- 코드 식별자 및 파일명: 영어
- 동일 문서의 영문 번역본을 별도로 만들지 않는다.
- 중요한 게임 용어는 실제 게임 표기와 동일하게 유지한다.
