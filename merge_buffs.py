import os

# 합칠 대상 파일 목록
TARGET_FILES = [
    "BuffCase.cs", "BuffCS_E_0.cs", "BuffCS_E_1.cs", "BuffCS_Anima.cs",
    "BuffCS_El.cs", "BuffCS_Hana.cs", "BuffCS_Hina.cs", "BuffCS_Kira.cs",
    "BuffCS_Manity.cs", "BuffCS_Prote.cs", "BuffCS_UG.cs", "BuffCS_Whai.cs"
]
OUTPUT_FILE = "BuffCS.cs"

def main():
    usings = set()
    combined_code = []

    for file_name in TARGET_FILES:
        if not os.path.exists(file_name):
            print(f"[Warning] {file_name} not found. Skipping...")
            continue
            
        with open(file_name, 'r', encoding='utf-8') as f:
            lines = f.readlines()
            
        for line in lines:
            clean_line = line.strip()
            # using 구문은 최상단 배치를 위해 set으로 중복없이 수집
            if clean_line.startswith('using ') and clean_line.endswith(';'):
                usings.add(clean_line)
            else:
                combined_code.append(line)

    # 최종 코드 작성
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
