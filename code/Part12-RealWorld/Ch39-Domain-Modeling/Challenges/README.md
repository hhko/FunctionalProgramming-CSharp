# Ch39 챌린지

도메인 모델링 + 검증 파이프라인을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 잘못된 상태를 표현 불가능하게

`Types/Domain.cs` — `Username`/`Email`/`Age` 는 private 생성자 + 스마트 생성자(Validation 반환)로 *불변식을 타입 경계에 가둔다*. `User` 가 존재하면 세 필드가 모두 유효함이 보장된다. `Registration.Register` 가 세 필드를 동시 검증해 *모든 오류를 누적* 한다 (applicative).

**노리는 능력** — 1부 Validation 의 applicative 누적이 실무 도메인 검증으로 이어짐을 본다 (monadic 단락과 대비 — 모든 오류를 한 번에 보고).

## 심화 챌린지 (선택)

### ② 주문 검증

`Order.cs` — 수량/가격/할인율을 `Map3` 로 동시 검증해 오류를 누적한다.

**노리는 능력** — 강타입 도메인 + applicative 누적을 새 도메인에 적용한다.

## 실행

```bash
dotnet run --project code/Part10-RealWorld/Ch39-Domain-Modeling/Ch39.csproj
```
