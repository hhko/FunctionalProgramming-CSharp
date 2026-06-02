# Ch33 챌린지

합법칙 검증의 일반화를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 재사용 가능한 법칙 모듈

`Tests/Laws.cs` — 1부의 챕터별 법칙 헬퍼를 *임의의 `Functor<F>`/`Monad<M>` 인스턴스* 를 받는 generic 모듈로 일반화했다. `Program.cs` 가 `MyListF` 에 대해 다섯 법칙을 모두 검사한다.

**노리는 능력** — 법칙이 *인스턴스의 계약* 임을, 그리고 검증을 한 번 작성해 모든 인스턴스에 재사용함을 본다.

## 심화 챌린지 (선택)

### ② 위반을 실제로 잡아내는가

`Challenges/BogusCheck.cs` — 일부러 항등 법칙을 어기는 `BogusListF` 를 같은 모듈로 검사하면 *위반* 으로 잡힌다. 검증이 통과만 하는 게 아니라 *틀린 구현을 거른다* 는 점을 확인한다.

**노리는 능력** — 좋은 법칙 테스트는 올바른 구현은 통과시키고 틀린 구현은 떨어뜨린다 (xUnit 으로 옮기면 `[Fact]` + `ShouldBeFalse()`).

## 실행

```bash
dotnet run --project code/Part9-FunctionalTesting/Ch33-Law-Verification/Ch33.csproj
```
