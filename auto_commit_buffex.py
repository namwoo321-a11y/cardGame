#!/usr/bin/env python3
"""
BuffEx.cs 자동 커밋/푸시 스크립트

이 스크립트는 BuffEx.cs 파일의 변경사항을 감지하여
자동으로 git에 커밋하고 푸시합니다.

사용 방법:
    python auto_commit_buffex.py [--check] [--force]
    
옵션:
    --check     : 현재 git 상태만 확인
    --force     : 변경사항이 없어도 강제 커밋
"""

import os
import sys
import subprocess
import argparse
from datetime import datetime
from pathlib import Path

# 설정값
BUFFEX_FILE = "BuffEx.cs"
COMMIT_MESSAGE = "[Auto] BuffEx.cs updated"
REMOTE_NAME = "origin"
BRANCH_NAME = "main"

class BuffExAutoCommitter:
    def __init__(self, project_root=None):
        self.project_root = project_root or os.getcwd()
        self.buffex_path = os.path.join(self.project_root, BUFFEX_FILE)
        
    def run_command(self, cmd, check=True):
        """명령어를 실행하고 결과를 반환합니다."""
        try:
            result = subprocess.run(
                cmd,
                shell=True,
                cwd=self.project_root,
                capture_output=True,
                text=True
            )
            
            if check and result.returncode != 0:
                print(f"❌ 명령어 실행 실패: {cmd}")
                print(f"오류: {result.stderr}")
                return None
                
            return result.stdout.strip()
        except Exception as e:
            print(f"❌ 예외 발생: {e}")
            return None
    
    def is_git_repo(self):
        """git 저장소 여부를 확인합니다."""
        return os.path.isdir(os.path.join(self.project_root, ".git"))
    
    def buffex_exists(self):
        """BuffEx.cs 파일 존재 여부를 확인합니다."""
        return os.path.isfile(self.buffex_path)
    
    def get_git_status(self):
        """git 상태를 확인합니다."""
        output = self.run_command("git status --porcelain", check=False)
        return output if output else ""
    
    def has_buffex_changes(self):
        """BuffEx.cs 파일의 변경사항 여부를 확인합니다."""
        status = self.run_command(f"git status --porcelain {BUFFEX_FILE}", check=False)
        return bool(status) if status else False
    
    def check_status(self):
        """현재 상태를 확인하고 출력합니다."""
        print("\n" + "="*60)
        print("📊 BuffEx 자동 커밋 상태 확인")
        print("="*60)
        
        if not self.is_git_repo():
            print("❌ 오류: Git 저장소가 아닙니다.")
            return False
        
        if not self.buffex_exists():
            print(f"❌ 오류: {BUFFEX_FILE}을 찾을 수 없습니다.")
            return False
        
        print(f"✅ BuffEx 파일 경로: {self.buffex_path}")
        
        # Git 상태 확인
        status = self.get_git_status()
        if not status:
            print("✅ 변경사항: 없음")
            return False
        
        print("\n📝 Git 상태:")
        for line in status.split('\n'):
            if line:
                print(f"  {line}")
        
        # BuffEx.cs 변경사항 확인
        if self.has_buffex_changes():
            print(f"\n⚠️  {BUFFEX_FILE}에 변경사항이 있습니다!")
            return True
        else:
            print(f"\n✅ {BUFFEX_FILE}에 변경사항이 없습니다.")
            return False
    
    def stage_buffex(self):
        """BuffEx.cs 파일을 스테이징합니다."""
        print(f"\n📌 {BUFFEX_FILE} 스테이징 중...")
        output = self.run_command(f"git add {BUFFEX_FILE}")
        if output is not None or self.run_command(f"git diff --cached {BUFFEX_FILE}", check=False):
            print(f"✅ {BUFFEX_FILE} 스테이징 완료")
            return True
        return False
    
    def commit_changes(self):
        """변경사항을 커밋합니다."""
        print(f"\n💾 커밋 중: {COMMIT_MESSAGE}")
        
        # 커밋 메시지에 타임스탬프 추가
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        full_message = f"{COMMIT_MESSAGE}\n\nTimestamp: {timestamp}"
        
        cmd = f'git commit -m "{COMMIT_MESSAGE}" -m "Timestamp: {timestamp}"'
        output = self.run_command(cmd, check=False)
        
        if output and ("changed" in output or "insertion" in output):
            print("✅ 커밋 완료")
            return True
        elif "nothing to commit" in output:
            print("⚠️  커밋할 변경사항이 없습니다.")
            return False
        else:
            print(f"❌ 커밋 실패: {output}")
            return False
    
    def push_changes(self):
        """변경사항을 원격 저장소에 푸시합니다."""
        print(f"\n🚀 {REMOTE_NAME}/{BRANCH_NAME}에 푸시 중...")
        cmd = f"git push {REMOTE_NAME} {BRANCH_NAME}"
        output = self.run_command(cmd, check=False)
        
        if output and not ("error" in output.lower() or "fatal" in output.lower()):
            print(f"✅ 푸시 완료")
            print(output)
            return True
        else:
            print(f"❌ 푸시 실패: {output}")
            return False
    
    def view_log(self, count=5):
        """최근 커밋 로그를 확인합니다."""
        print(f"\n📜 최근 {count}개 커밋:")
        print("="*60)
        output = self.run_command(f"git log --oneline -n {count}", check=False)
        if output:
            print(output)
        else:
            print("커밋 로그가 없습니다.")
        print("="*60)
    
    def full_update(self, force=False):
        """전체 업데이트 프로세스를 실행합니다."""
        print("\n" + "="*60)
        print("🔄 BuffEx 자동 업데이트 시작")
        print("="*60)
        
        # 사전 확인
        if not self.is_git_repo():
            print("❌ Git 저장소가 아닙니다.")
            return False
        
        if not self.buffex_exists():
            print(f"❌ {BUFFEX_FILE}을 찾을 수 없습니다.")
            return False
        
        # 상태 확인
        has_changes = self.has_buffex_changes()
        if not has_changes and not force:
            print(f"✅ {BUFFEX_FILE}에 변경사항이 없습니다. 업데이트를 건너뜁니다.")
            self.view_log()
            return True
        
        if force:
            print("⚠️  강제 커밋 모드 활성화")
        
        # 스테이징
        if not self.stage_buffex():
            print("❌ 스테이징 실패")
            return False
        
        # 커밋
        if not self.commit_changes():
            if not force:
                print("❌ 커밋 실패")
                return False
        
        # 푸시
        if not self.push_changes():
            print("⚠️  푸시 실패 (로컬 커밋은 완료됨)")
            return False
        
        print("\n" + "="*60)
        print("✅ 모든 업데이트 완료!")
        print("="*60)
        
        self.view_log()
        return True


def main():
    parser = argparse.ArgumentParser(
        description="BuffEx.cs 자동 커밋/푸시 스크립트",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
예시:
  python auto_commit_buffex.py          # 자동 커밋/푸시
  python auto_commit_buffex.py --check  # 상태만 확인
  python auto_commit_buffex.py --force  # 강제 커밋
        """
    )
    
    parser.add_argument("--check", action="store_true", help="현재 상태만 확인")
    parser.add_argument("--force", action="store_true", help="변경사항이 없어도 강제 커밋")
    parser.add_argument("--log", type=int, default=5, help="표시할 커밋 개수 (기본값: 5)")
    
    args = parser.parse_args()
    
    # 프로젝트 루트 찾기
    project_root = os.path.dirname(os.path.abspath(__file__))
    
    committer = BuffExAutoCommitter(project_root)
    
    try:
        if args.check:
            # 상태만 확인
            has_changes = committer.check_status()
            committer.view_log(args.log)
            sys.exit(0 if has_changes else 1)
        else:
            # 전체 업데이트 프로세스
            success = committer.full_update(force=args.force)
            sys.exit(0 if success else 1)
    
    except KeyboardInterrupt:
        print("\n\n⚠️  사용자가 중단했습니다.")
        sys.exit(1)
    except Exception as e:
        print(f"\n❌ 예상치 못한 오류: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
