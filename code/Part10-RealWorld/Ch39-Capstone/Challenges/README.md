# Ch39 챌린지 — 종합 capstone

책 전체의 도구를 한 흐름에 모은다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 주문 접수 서비스 (전 Part 결합)

`OrderService.cs` — 주문 요청 배치를 처리한다:
- **도메인 검증** (1부/36장 `Validation` applicative 누적) — id/amount 동시 검증.
- **효과 + 능력 기반 DI** (5부/37장 `Eff<RT>` + `Has<RT, IConsole>`, `Has<RT, IStore>`) — 저장 + 로그.
- **테스트 더블** (9부) — `MemoryConsole`/`MemoryStore` 로 결정적 검증.

유효 주문은 저장+승인 로그, 무효는 거부 로그. 승인 건수를 돌려준다.

**노리는 능력** — 따로 익힌 추상들이 *새 추상 없이* 한 실전 코드로 합성됨을 본다. 1부의 한 동사("모든 값과 함수를 Elevated World 로 lift") 가 실무 규모에서 합성된다.

## 심화 챌린지 (선택)

### ② 라이브 런타임 + LanguageExt 샘플 읽기

`AppRT` 의 `IConsole`/`IStore` 를 실제 콘솔/DB 구현으로 바꾸면 같은 서비스 코드가 운영에서 동작한다. 마지막으로 LanguageExt v5 의 `Samples/`(Newsletter·CardGame·BlazorApp)를 펼쳐 *이 책에서 손으로 만든 패턴의 변형* 으로 읽어 보라.

**노리는 능력** — 학습용 구현과 실무 라이브러리가 *같은 골격* 임을 확인하며 책을 마무리한다.

## 실행

```bash
dotnet run --project code/Part10-RealWorld/Ch39-Capstone/Ch39.csproj
```
