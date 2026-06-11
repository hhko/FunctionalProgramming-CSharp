# Ch14 챌린지

Elevated 값의 *선택·결합* 추상을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① Combine 과 Choose 는 같은가 다른가

`CombineVsChoose.md` — 시그니처가 같은 두 동사 `Combine` (SemigroupK) 과 `Choose` (Choice) 가 자료 타입에 따라 같기도 (`MyMaybe`) 다르기도 (`MySeq`) 함을 표로 정리한다.

**노리는 능력** — 시그니처가 같아도 *trait 이 약속하는 의미* 가 다름을 본다.

## 심화 챌린지 (선택)

### ② `guard` 헬퍼

`Guard.cs` — `guard(true) = Pure(unit)`, `guard(false) = Empty<Unit>()`. Applicative 의 `Pure` 와 MonoidK 의 `Empty` 두 능력을 동시에 쓰는 고전 헬퍼다. Monad 의 `Bind` 와 결합하면 LINQ `where` (조건 필터) 의 일반형이 된다.

**노리는 능력** — Alternative 가 단순히 "선택" 만이 아니라 *조건 분기 / 필터* 의 토대임을 본다.

## 실행

```bash
dotnet run --project code/Part2-Collections/Ch14-Alternative/Ch14.csproj
```
