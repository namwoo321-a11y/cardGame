import os

# 소스 파일들이 위치할 하위 폴더 이름
SOURCE_DIR = "Buffs"
# 병합 후 루트 경로에 생성될 최종 통합 파일
OUTPUT_FILE = "BuffCS.cs"

# 합칠 대상 파일 목록
TARGET_FILES = [
    "BuffCS.cs", "BuffCS_E_0.cs", "BuffCS_E_1.cs", "BuffCS_Anima.cs",
    "BuffCS_El.cs", "BuffCS_Hana.cs", "BuffCS_Hina.cs", "BuffCS_Kira.cs",
    "BuffCS_Manity.cs", "BuffCS_Prote.cs", "BuffCS_UG.cs", "BuffCS_Whai.cs"
]

def main():
    usings = set()
    combined_code = []

    for file_name in TARGET_FILES:
        # SOURCE_DIR(Buffs 폴더) 내부의 파일 경로로 연결
        file_path = os.path.join(SOURCE_DIR, file_name)
        
        if not os.path.exists(file_path):
            print(f"[Warning] {file_path} not found. Skipping...")
            continue
            
        with open(file_path, 'r', encoding='utf-8') as f:
            lines = f.readlines()
            
        for line in lines:
            clean_line = line.strip()
            # using 구문은 최상단 배치를 위해 set으로 중복없이 수집
            if clean_line.startswith('using ') and clean_line.endswith(';'):
                usings.add(clean_line)
            else:
                combined_code.append(line)

    # 최종 통합 코드 작성 (루트 폴더에 생성)
    with open(OUTPUT_FILE, 'w', encoding='utf-8') as out_file:
        # 1. Usings
        for using in sorted(list(usings)):
            out_file.write(using + '\n')
        out_file.write('\n')
        
        # 2. 본문
        out_file.writelines(combined_code)
        
    print(f"Successfully generated {OUTPUT_FILE}")

if __name__ == "__main__":
    main()
