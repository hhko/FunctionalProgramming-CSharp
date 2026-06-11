# Ch20 챌린지

상태·환경 변환기(ReaderT/StateT/WriterT)를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① StateT<Stack, Option> — 상태 + 실패

`Stack.cs` — `push` 는 상태만 바꾸고 `pop` 은 빈 스택이면 *실패(None)* 한다. 상태 스레딩(StateT)과 실패(내부 Option)가 한 스택에서 동시에 작동한다.

**노리는 능력** — 3부의 단일 `State` 모나드가 *내부 모나드 M 위로 일반화* 되어, 상태와 또 다른 효과(여기선 실패)를 함께 다룸을 본다.

## 심화 챌린지 (선택)

### ② ReaderT<Env, Option> = 15장 ReaderOption (공짜로)

`Program.cs` 의 ReaderT 예제 — 15장에서 *손으로* 짠 `ReaderOption` 이 `ReaderT<Env, OptionF, A>` 의 *특수 사례* 로 공짜로 나온다. 내부 M 을 Option 으로 고정하면 같은 동작이다.

**노리는 능력** — 변환기가 15장의 수동 배관을 *내부 M 에 대해 자동 생성* 함을 확인한다. WriterT 도 동일한 패턴(출력 W 를 M 위에 누적)으로 따라온다.

## 실행

```bash
dotnet run --project code/Part4-MonadTransformers/Ch20-ReaderT-StateT-WriterT/Ch20.csproj
```
