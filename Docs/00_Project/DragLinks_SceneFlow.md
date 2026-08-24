# Drag Links! — Scene Flow

> 문서 버전: 0.1  
> 기준일: 2026-08-25

## 1. 현재 씬 목록

1. `TitleScene`
2. `MenuScene`
3. `StoryScene`
4. `Stage_01`

## 2. 기본 흐름

현재 기본 스토리 플레이 흐름:

`TitleScene → MenuScene → StoryScene → Stage_01`

[미정] Stage_01 종료 후 정확한 복귀 경로.

[미정] 챌린지 모드 전용 씬을 새로 둘지, Stage 씬을 재사용할지.

## 3. TitleScene

역할:
- 게임 최초 진입
- 타이틀 표시
- MenuScene 진입
- 게임 전역 초기화가 필요할 경우 진입점 제공

원칙:
- 실제 보드/스테이지 규칙을 넣지 않는다.
- Title 전용 로직과 게임 전역 초기화를 구분한다.

## 4. MenuScene

현재 계획:
- Story Mode 진입
- Challenge Mode 진입
- Settings
- Exit

[미정] 메뉴 세부 UI와 옵션 구성.

[미정] Challenge Mode의 실제 목적지 씬.

## 5. StoryScene

역할:
- 한국 고등학교 및 S.A.V.E 중심 스토리 진행
- 대화
- 사건 소개
- Link 진입 연출
- 스테이지 진입 전 서사 연결

[미정] 실제 스토리 데이터 포맷과 대화 시스템.

## 6. Stage_01

`Stage_01`은 첫 실제 퍼즐 플레이 씬이다.

중요:
- 씬 이름은 `Stage_01`이지만 보드/수식/점수/LINKING/턴 시스템은 **범용 구조**로 제작한다.
- `Stage01Controller`, `Stage01Board`처럼 핵심 시스템을 Stage 1 전용으로 복제하지 않는다.
- Stage 1 고유 목표 점수, 적 기믹, 배경 등은 Stage 데이터로 분리하는 방향을 사용한다.

## 7. 씬 Hierarchy 원칙

현재 네 씬 모두 기본 상태:

- Main Camera
- Directional Light
- Global Volume

원칙:
- 실제 기능이 생기기 전까지 미래 예상만으로 빈 GameObject 구조를 강제하지 않는다.
- 시스템 구현 시 필요한 Root를 목적에 맞게 추가한다.

## 8. SceneLoader 원칙

[현재안]
- 씬 로딩 책임은 개별 UI 버튼이나 게임플레이 클래스에 흩뿌리지 않는다.
- 공통 `SceneLoader` 또는 동등한 SceneFlow 계층을 둔다.
- Scene 이름 문자열을 여러 코드에 직접 하드코딩하지 않는 방법을 사용한다.

[미정]
- Addressables 사용 여부
- Additive Scene 구조 사용 여부
- Persistent Bootstrap Scene 사용 여부
