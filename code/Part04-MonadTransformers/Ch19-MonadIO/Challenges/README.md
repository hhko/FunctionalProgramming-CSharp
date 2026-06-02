# Ch19 챌린지

`MonadIO` / `LiftIO` 와 IO 를 품는 변환기 스택을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① LiftIO 로 IO 를 스택 안으로

`Program.cs` 의 스택 — `ReaderT<int, IOF, A>` 에서 `LiftIO` 로 IO 부수 작용을 끌어올린다. `Run(env)` 는 IO 를 *조립만* 하고, 실제 부수 작용은 `io.Run()` 에서야 일어난다 (지연).

**노리는 능력** — 일반 `lift` 가 "바로 아래 한 층" 만 올리는 데 비해 `LiftIO` 는 *스택 맨 안쪽 IO* 를 어디서든 올림을 본다.

## 심화 챌린지 (선택)

### ② ReaderT<Env, IOF> = Eff<RT, A> 의 축소판

`ReaderT<Env, IOF, A>` 가 5부 `Eff<RT, A> = ReaderT<RT, IO, A>` 의 축소판임을 확인하라. 환경 주입(Reader) + 지연 부수 작용(IO) 이 한 스택에 있다.

**노리는 능력** — 4부 변환기가 5부 효과 시스템의 *골격* 임을 미리 본다. `Has<RT, …>` DI 는 이 위에 얹힌다.

## 실행

```bash
dotnet run --project code/Part4-MonadTransformers/Ch19-MonadIO/Ch19.csproj
```
