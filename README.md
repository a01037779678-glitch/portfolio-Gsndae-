# 그슨대

한국 설화를 모티브로 한 3D 공포 퍼즐 게임입니다.
5인 팀 프로젝트로 개발했으며, 게임 진행 상태 관리, Save/Load,
플레이어 상호작용, Scene 전환 및 시스템 통합을 담당했습니다.

## Project Info
- Engine: Unity
- Language: C#
- Team: 5명
- Platform: Windows PC
- Genre: Horror / Puzzle
- Development: 2026.05 ~ 진행 중

## My Role
- 게임 진행 및 상태 관리
- Save / Load 시스템
- 플레이어 및 오브젝트 상호작용
- Scene 전환 및 객체 생명주기 관리
- 시스템 통합 및 오류 수정

## Core Systems

### 1. Game State Management
- House 진행 상태 관리
- 제단 및 분기 상태 관리
- Scene 이동 후 진행 상태 유지

### 2. Save / Load
- SaveData 기반 게임 상태 저장 및 복원
- 다시하기 시 저장 당시 인벤토리 상태 복원
- 진행 분기 및 엔딩 조건 상태 유지

### 3. Interaction System
- 플레이어 입력 기반 상호작용
- 문 / 아이템 / 퍼즐 오브젝트 처리
- 인터페이스 기반 공통 상호작용 구조

## Troubleshooting

### Retry 이후 인벤토리 상태 불일치
Player가 DontDestroyOnLoad로 유지되면서 런타임 인벤토리 상태가 남아
저장 데이터와 실제 플레이 상태가 불일치하는 문제가 발생했습니다.

RestoreSlots()를 통해 다시하기 시 SaveData 기준으로
인벤토리를 재구성하도록 수정했습니다.

### Retry 이후 카메라 상태 복원 오류
카메라 재탐색 과정에서 실제 플레이어 카메라 참조와 Transform이
정상적으로 복원되지 않는 문제가 발생했습니다.

PlayerController.cameraTransform을 기준으로
Parent / LocalPosition / LocalRotation을 저장하고 복원하도록 수정했습니다.

## Collaboration
- GitHub Branch 기반 협업
- 기능 단위 Commit / Push
- Main Merge 후 통합 테스트
- 병합 과정에서 발생하는 충돌 및 시스템 연동 오류 수정

## Source Code

프로젝트에서 직접 구현한 주요 시스템 코드를 기능별로 정리했습니다.

- [Game State Management](./Scripts/GameState)
  - 게임 진행 상태
  - House / 제단 / 분기 상태 관리

- [Save / Load](./Scripts/SaveLoad)
  - SaveData 기반 게임 상태 저장 및 복원
  - JSON 저장 / 로드
  - 인벤토리 저장 상태 관리

- [Interaction System](./Scripts/Interaction)
  - IInteractable 기반 공통 상호작용 구조
  - 문 / 아이템 / 퍼즐 오브젝트 처리

- [Troubleshooting](./Scripts/Troubleshooting)
  - 다시하기 시 인벤토리 상태 복원
  - 카메라 Transform 복원
  - Scene 재진입 시 플레이어 상태 처리
 
## Screenshots
<!-- 추후 게임 스크린샷 추가 -->

## Gameplay Video
<!-- 추후 영상 링크 추가 -->

## Portfolio
<!-- 추후 포트폴리오 PDF 링크 추가 -->
