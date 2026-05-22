Card Game Buff System

이 레포지토리는 Unity 카드 게임의 캐릭터별 버프(Buff) 스크립트를 체계적으로 관리하고, 유니티에 임포트하기 쉽도록 자동으로 1개의 파일로 병합해주는 시스템입니다.

📁 폴더 및 파일 구조 (Directory Structure)

프로젝트를 깔끔하게 관리하기 위해, 분할된 캐릭터 버프 코드들은 모두 Buffs/ 폴더 내부에 위치하며, 병합된 최종 파일만 최상단(Root)에 생성됩니다.

📦 프로젝트 루트 (Root)
┣ 📂 Buffs/                 # 캐릭터별 버프 소스 코드 폴더
┃ ┣ 📜 BuffCase.cs          # 기본/공통 버프 클래스
┃ ┣ 📜 BuffCS_Anima.cs      # Anima 캐릭터 버프
┃ ┣ 📜 BuffCS_E_0.cs        # E_0 버프
┃ ┣ 📜 BuffCS_E_1.cs        # E_1 버프
┃ ┣ 📜 BuffCS_El.cs         # El 캐릭터 버프
┃ ┣ 📜 BuffCS_Hana.cs       # Hana 캐릭터 버프
┃ ┣ 📜 BuffCS_Hina.cs       # Hina 캐릭터 버프
┃ ┣ 📜 BuffCS_Kira.cs       # Kira 캐릭터 버프
┃ ┣ 📜 BuffCS_Manity.cs     # Manity 캐릭터 버프
┃ ┣ 📜 BuffCS_Prote.cs      # Prote 캐릭터 버프
┃ ┣ 📜 BuffCS_UG.cs         # UG 캐릭터 버프
┃ ┗ 📜 BuffCS_Whai.cs       # Whai 캐릭터 버프
┣ 📜 merge_buffs.py         # Buffs 폴더의 파일들을 합치는 파이썬 자동화 스크립트
┣ 📜 BuffCS.cs              # (⭐자동 생성됨) Unity로 한 번에 가져갈 최종 통합본 파일
┗ 📜 README.md              # 현재 설명서 파일


⚙️ 사용 및 자동화 방법

Buff Code Maker 웹 툴 사용:

Buff Code Maker 툴에서 Buffs/ 폴더 내부에 있는 각 캐릭터별 .cs 파일을 불러오고 편집합니다.

[코드 복사 및 GitHub 덮어씌우기] 버튼을 이용해 변경사항을 Buffs/ 폴더 안의 개별 파일들에 각각 업데이트합니다.

자동 병합 (GitHub Actions 활용 시):

개별 스크립트가 GitHub에 업데이트되면, 백그라운드에서 merge_buffs.py가 자동으로 실행됩니다.

파이썬 봇이 자동으로 Buffs/ 안의 모든 코드를 묶고, 중복된 using 문을 정리하여 BuffCS.cs 라는 1개의 최종 완성본을 최상위 폴더에 찍어냅니다.

Unity 적용:

여러분은 복잡한 여러 파일들을 신경 쓸 필요 없이, 항상 최신 상태로 유지되는 BuffCS.cs 파일 단 1개만 다운로드하여 유니티에 적용하시면 됩니다!
