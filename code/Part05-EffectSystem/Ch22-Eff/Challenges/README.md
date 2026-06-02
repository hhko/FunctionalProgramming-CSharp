# Ch22 챌린지

런타임 없는 효과(Eff)를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① Eff 의 지연 + 오류 포획

`Program.cs` — `Eff.Effect` 로 부수 작용을 조립하되 `Run` 전까지 실행되지 않고, 예외가 나면 자동으로 `Fail(Error)` 로 포획된다. `Bind` 는 첫 실패에서 단락한다.

**노리는 능력** — `Eff<A> ≈ IO<Fin<A>>` — 20장 IO 의 지연과 21장 Fin 의 오류 포획이 한 타입에 합쳐짐을 본다.

## 심화 챌린지 (선택)

### ② Catch 로 복구 파이프라인

`Program.cs` 의 Catch 예제 — 실패하는 Eff 를 `Catch` 로 받아 대체 Eff 로 복구한다 (재시도/폴백의 토대).

**노리는 능력** — 런타임 없이도 IO + 오류를 다루는 Eff 가 실무 효과의 최소 단위임을 본다. 런타임 주입은 23장에서.

## 실행

```bash
dotnet run --project code/Part5-EffectSystem/Ch22-Eff/Ch22.csproj
```
