# Drag Links! — Data & Config

> 문서 버전: 0.2  
> 기준일: 2026-08-25

## 1. 목표

게임 밸런스와 콘텐츠가 늘어나도 코드 수정 없이 데이터 변경이 가능하도록 한다.

특히:
- 7×6 / 8×6
- 스폰 확률
- 목표 점수
- 제한 턴
- 캐릭터 능력 수치

를 적절한 Config/Definition에서 조정 가능하게 한다.

## 2. ScriptableObject와 Runtime State 분리

### Definition / Config
게임 원본 데이터.

예:
- `BoardConfig`
- `StageDefinition`
- `CharacterDefinition`
- `UniqueDefinition`
- `NumberPalette`

### Runtime State
플레이 중 계속 변하는 값.

예:
- 현재 점수
- 현재 턴
- 현재 보드
- CurrentValue
- Gem/Hammer
- 유니크 카운터
- 한가은 현재 연쇄 콤보 스택
- 현재 Action에서 아직 정산하지 않은 Pending Combo

[확정] ScriptableObject Definition 자체를 런타임 세이브 상태처럼 직접 변형하지 않는다.

## 3. BoardConfig

현재 구현/권장 개념:

- Width
- Height
- BaseMaxDragLength
- NumberCategoryWeight
- OperatorCategoryWeight
- NumberWeights[1..9]
- OperatorWeights[Add/Subtract/Multiply/Divide]

[미정] 최종 밸런스 값.

현재 7×6 및 3:1 Category는 테스트용 값이다.

Mathcalibur 가중치는 출발 레퍼런스로 사용할 수 있으나 최종값으로 취급하지 않는다.

## 4. Number Palette

숫자 1~9의 색을 데이터로 관리한다.

예:
- Identity 1 → Color
- ...
- Identity 9 → Color

향후:
- 패턴
- 테두리
- 색각 보조 아이콘

등 추가 가능.

[미정] 최종 RGB.

## 5. StageDefinition

권장:

- StageId
- TargetScore
- TurnLimit
- BoardConfig
- EnemyDefinition
- PlayerCharacter
- Background
- BGM
- Allowed mechanics

Stage_01 임시:
- TargetScore = 1000
- TurnLimit = 미정
- Reciprocal disabled

## 6. CharacterDefinition vs Character Runtime

### CharacterDefinition
정적:
- CharacterId
- DisplayName
- Portrait
- Mascot
- Weapon
- Ability reference/data

### Character Runtime
플레이 중 변화:
- CurrentChainComboStack
- 캐릭터별 카운터/게이지

한가은 CurrentStack은 턴을 넘어 유지되므로 Definition이 아니라 Runtime State다.

## 7. Pending Combo

`PendingComboTriggers` 계열 값은 **현재 Action을 해결하는 동안만 존재하는 작업 대기 상태**다.

- Linking Resolution LineCount만큼 증가
- 하나씩 소비
- 5스택 보드 재해결에서 새 LINKING이 생기면 증가
- 정상적인 Action 종료 전 0이 되어야 함

장기 저장/해금 데이터가 아니다.

## 8. 한가은 연쇄 콤보 수치

현재 확정 능력 수치:

- 1스택: Gem 최대 2
- 2스택: Gem 최대 5
- 3스택: Gem 최대 7
- 4스택: 현재 Gem 수 `n`, 모든 Gem Number `+n`
- 5스택: 비Gem Number 최대 11개 Gem화 후 전체 Gem Number 파괴, 값 합 `S`, 최종 Number 전체 `+S`

이 수치가 추후 밸런스로 변경될 가능성을 고려해 캐릭터 Ability Data로 분리할 수 있다.

단, **효과의 의미와 순서 자체는 코드 한 곳에서 명확히 보장**해야 한다.

## 9. UniqueDefinition

권장:
- UniqueId
- DisplayName
- Category
- Description
- Icon
- Rarity/Price — 추후
- Ability/Effect reference

유니크1 사용 카운터는 Runtime State.

## 10. Random

랜덤 예:
- 타일 스폰
- 한가은 EndTurn Gem 대상
- 한가은 EndTurn Hammer 대상
- 1/2/3/5스택 Gem 대상
- 적 1~100

[현재안] 공통 Random Provider/Seed 재현 방향을 유지한다.

목적:
- 버그 보드 재현
- QA
- 자동 테스트
- 밸런스 재현

## 11. Magic Number 금지

여러 클래스에 직접 반복해서 쓰지 않는다.

예:
- MaxDragLength 5
- Stage 목표 1000
- 적 2턴
- 망치 최대 Gem 3
- Chain Gem 2/5/7/11
- 안전 Wave 1024

단, 기술 안전장치와 게임 밸런스 수치를 구분한다.

## 12. Gameplay 판정은 데이터와 별개

`Use`, `Destroy`, `ChangeIncrease`는 단순 Config 값이 아니라 런타임 게임 이벤트/판정이다.

타일이 제거됐다는 이유로 자동으로 `Use=true`로 만들지 않는다.

상세:
`DragLinks_Judgements.md`

## 13. Save Data

[미정] 실제 세이브 파일 포맷.

향후 저장 후보:
- 스토리 진행
- 캐릭터 해금
- 유니크 해금
- 옵션
- 챌린지 기록

런 중 CurrentChainComboStack을 중간 저장할지 여부는 Save/Resume 기능 설계 시 결정한다.

## 14. Config 검증

예:
- Width/Height 유효 범위
- 스폰 가중치 합 0 방지
- 없는 NumberIdentity 방지
- Stage TargetScore 음수 방지
- 정의되지 않은 Operator Fusion 명시적 실패

[미정] 최소 보드 Width/Height.
