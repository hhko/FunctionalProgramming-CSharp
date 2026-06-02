# Ch35 챌린지

효과 코드의 결정적 테스트를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① MemoryConsole 더블로 결정적 테스트

`Tests/EffectTests.cs` — 콘솔 입출력을 쓰는 `Greeter` 효과를, 실제 콘솔 대신 `MemoryConsole` 더블을 런타임에 주입해 검증한다. 입력은 큐, 출력은 리스트라 *부수 작용 없이 결정적* 으로 단언할 수 있다.

**노리는 능력** — 5부의 `Has<RT, IConsole>` DI 설계가 *테스트 용이성* 으로 회수됨을 본다. 같은 효과 코드가 Live/Memory 런타임 모두에서 동작한다.

## 심화 챌린지 (선택)

### ② 결정성

`EffectTests.DeterministicHolds` — 같은 입력에 같은 출력이 *항상* 나옴을 확인한다 (효과가 값으로 인코딩 + 런타임 주입 → 재현 가능).

**노리는 능력** — 효과 시스템(5부) + 테스트 더블(9부)이 결합해 "부수 작용 있는 코드도 순수 함수처럼 테스트" 됨을 본다. (xUnit: `[Fact]` + `con.Output.ShouldBe([...])`.)

## 실행

```bash
dotnet run --project code/Part9-FunctionalTesting/Ch35-Testing-Effects/Ch35.csproj
```
