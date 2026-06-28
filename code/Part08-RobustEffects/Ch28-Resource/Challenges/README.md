# Ch28 챌린지

자원 수명 관리(Resource/bracket)를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① bracket 의 예외 안전성

`Program.cs` 의 bracket 예제 — 사용 중 예외가 나도 `release` 가 *반드시* 실행됨을 확인한다 (try/finally 의 함수형 캡처).

**노리는 능력** — `using` 의 함수형·효과적 대응을 본다. 획득–사용–해제가 한 값으로 묶여 누수가 구조적으로 차단된다.

## 심화 챌린지 (선택)

### ② 다중 자원의 LIFO 해제

`Resources` — 열린 순서 A→B 면 닫히는 순서는 B→A (중첩 자원의 올바른 정리). 예외에도 모두 해제된다.

**노리는 능력** — 7부 `EnvIO` 가 자원을 운반하던 그림(23장)과 연결해, 효과 실행 환경이 자원 수명을 관리함을 이해한다.

## 실행

```bash
dotnet run --project code/Part08-RobustEffects/Ch28-Resource/Ch28.csproj
```
