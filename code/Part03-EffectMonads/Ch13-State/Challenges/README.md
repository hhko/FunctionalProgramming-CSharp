# Ch13 챌린지

상태 스레딩 효과 모나드(State)를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 고유 ID 생성기

`FreshId.cs` — 가변 카운터 필드 없이 `State<int, _>` 로 고유 ID 를 만든다. `Fresh` 는 현재 카운터를 반환하고 상태를 +1 하며, 여러 `Fresh` 를 LINQ 로 이으면 카운터가 *자동으로 실려* 매번 다른 ID 가 나온다.

**노리는 능력** — `Bind` 가 *새 상태를 다음 단계로 자동 전달* 하는 것을 본다. 명령형의 `int counter++` 가 순수 함수로 표현된다.

## 심화 챌린지 (선택)

### ② Get / Put 으로 누산기

`Program.cs` 의 누산 예제 — `Modify` 로 상태를 변형하고 `Gets` 로 일부만 뽑는다. 같은 State 계산을 *다른 초기 상태* 로 Run 하면 결과가 달라진다 (상태가 인자가 아니라 효과임).

**노리는 능력** — `Get`/`Put`/`Modify`/`Gets` 네 멤버가 가변 상태를 *순수하게* 대체함을 본다.

## 실행

```bash
dotnet run --project code/Part3-EffectMonads/Ch13-State/Ch13.csproj
```
