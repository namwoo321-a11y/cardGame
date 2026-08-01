# Card Maker 사용법

`CardMaker.html`은 CardML을 Unity 밖에서 읽고 검증하는 GitHub Pages 도구다. 게임은 이 도구를 실행하지 않으며, Unity `InforM`은 Google Sheets에서 동기화한 카드 데이터만 읽는다.

## 작업 순서

1. **원본 불러오기**를 눌러 Google Apps Script 읽기 API에서 CardML을 가져온다.
2. 카드와 효과를 수정한다. 브라우저는 IndexedDB에 현재 초안과 최근 백업 10개를 보관한다.
3. `검증 결과`가 오류 0개인지 확인한다.
4. 선택한 시트를 TSV로 내보내거나, 변경 카드만 JSON으로 내보낸다.
5. TSV를 Google Sheets의 같은 열에 붙여 넣어 반영한다. 이 도구는 현재 Google 계정 쓰기 권한이나 비밀값을 보관하지 않는다.
6. 게임 시작 전 설정의 데이터 업데이트로 Unity에 새 데이터를 동기화한다.

## `[소멸]` 규칙

사용 후 소멸은 **카드 설명이나 Keyword가 아니라 실행 효과**다.

```text
description: 자신에게 보호 8
Keyword: 신속
effects1: shield:User:8
effects2: removeThis
```

- `removeThis`를 넣으면 해당 카드 인스턴스는 사용 뒤 `removedPile`로 이동한다.
- 덱을 다시 섞어도 `removedPile`의 카드는 돌아오지 않는다.
- 원본에 `description: ... [소멸]` 또는 `Keyword: 소멸`이 있으면 Card Maker는 초안에서 이를 지우고 `removeThis`로 옮긴다. 원본 Google Sheet는 사용자가 TSV를 반영하기 전까지 바뀌지 않는다.

## 안전한 확장 순서

새 효과를 추가할 때는 Unity의 `CardEffect` 구현과 `CardEffectParser`부터 작성한다. 그 뒤 `schemas/cardml-schema.json`에 최소 인자 수와 예시를 넣고 테스트를 추가한다. 마지막으로 Card Maker UI에서 해당 효과를 생성한다. 웹 도구만 먼저 새 문법을 허용하지 않는다.

## 저장 자동화의 다음 단계

Google Sheet에 직접 저장하려면 Google Identity Services OAuth와 Sheets API `batchUpdate`를 추가해야 한다. 이 저장소에는 OAuth Client ID나 쓰기 토큰을 넣지 않는다. 자동 저장은 별도 승인을 받아 OAuth 허용 원본, 충돌 검사, 재조회 검증을 모두 준비한 후 추가한다.
