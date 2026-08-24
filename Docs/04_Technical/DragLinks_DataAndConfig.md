# Drag Links! — Data & Config

> 문서 버전: 0.1  
> 기준일: 2026-08-25

## 1. 목표

게임 밸런스와 콘텐츠가 늘어나도 코드 수정 없이 데이터 변경이 가능하도록 한다.

특히 현재 미정인:
- 7×6 / 8×6
- 스폰 확률
- 목표 점수
- 제한 턴

은 반드시 데이터로 교체 가능해야 한다.

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
- 현재 보드 타일
- 현재 숫자값
- 현재 보석/망치 상태
- 유니크 카운터

[확정] ScriptableObject Definition 자체를 런타임 세이브 상태처럼 직접 변형하지 않는다.

## 3. BoardConfig

권장 필드 개념:

- Width
- Height
- BaseMaxDragLength
- NumberCategoryWeight
- OperatorCategoryWeight
- NumberWeights[1..9]
- OperatorWeights[Add/Subtract/Multiply/Divide]

[미정] 정확한 초기 값.

[임시] Mathcalibur 최종 구현의 값은 첫 테스트용 참고값으로만 사용.

## 4. Number Palette

숫자 1~9의 색을 데이터로 관리하는 것을 권장한다.

예:
- Identity 1 → Color
- ...
- Identity 9 → Color

향후:
- 패턴
- 테두리
- 색각 보조 아이콘

등이 추가될 수 있다.

[미정] 최종 RGB.

## 5. StageDefinition

권장 개념:

- StageId
- TargetScore
- TurnLimit
- BoardConfig
- EnemyDefinition
- PlayerCharacter
- Background reference
- BGM reference
- Allowed mechanics

Stage_01 임시:
- TargetScore = 1000
- TurnLimit = 미정
- Reciprocal disabled

## 6. CharacterDefinition

정적 표시/참조 정보 중심.

예:
- CharacterId
- DisplayName
- Portrait
- Mascot
- Weapon
- Ability type/reference

능력의 런타임 카운터는 Definition에 저장하지 않는다.

## 7. UniqueDefinition

권장:
- UniqueId
- DisplayName
- Category
- Description
- Icon
- Rarity/Price — 추후
- Ability/Effect reference

유니크1의 "1을 5회 사용" 같은 카운터는 Runtime State에 둔다.

## 8. Random

Drag Links!에는 랜덤이 많다.

예:
- 타일 스폰
- 한가은 턴 종료 보석 선정
- 한가은 턴 종료 망치 선정
- 연쇄 콤보 보석 대상 선정
- 적 1~100 랜덤

[현재안] 랜덤 생성 경로를 공통화하고 테스트 가능한 Seed를 지원하는 방향을 권장한다.

목적:
- 버그 보드 재현
- QA
- 자동 테스트
- 밸런스 재현

## 9. Magic Number 금지

다음 값을 여러 클래스에 직접 숫자로 쓰지 않는다.

예:
- 5 최대 드래그
- 1000 목표 점수
- 2턴 적 기믹
- 3개 보석 파괴
- 연쇄 콤보 2/5/7/9/11

규칙의 성격에 따라 Config, Character Ability Data, Stage Data 중 적절한 위치에서 관리한다.

## 10. Save Data

[미정] 실제 세이브 파일 포맷.

향후 저장 대상 후보:
- 스토리 진행
- 캐릭터 해금
- 유니크 해금
- 옵션
- 챌린지 기록

현재 Board Runtime을 영구 세이브할지 여부는 미정.

## 11. Config 검증

예:
- Width/Height <= 0 방지
- 스폰 가중치 합 0 방지
- 없는 숫자 Identity 방지
- Stage TargetScore 음수 방지
- 정의되지 않은 Operator Fusion 결과는 명시적으로 실패 처리
