# Ch15 챌린지

단일 모나드 합성의 *벽* 과 변환기의 동기를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 손으로 짠 ReaderOption 의 Bind 읽기

`Types/ReaderOption.cs` 의 `Bind` — 두 층(Reader 의 env, Option 의 Some/None)을 *직접* 푸는 배관을 한 줄씩 읽고, "왜 이게 공짜로 안 생기는지" 를 설명하라.

**노리는 능력** — 모나드는 일반적으로 *합성되지 않는다* 는 사실을 코드로 체감한다. `Reader` 와 `Option` 각각은 모나드지만 겹친 것의 `Bind` 는 손으로 짜야 한다.

## 심화 챌린지 (선택)

### ② 다른 쌍을 직접 쌓아 보기

`Program.cs` 를 참고해 `StateOption` 또는 `ReaderEither` 같은 *다른 효과 쌍* 의 `Bind` 를 직접 짜 보라. 매번 *거의 같은 배관* 을 반복하게 됨을 느끼는 것이 핵심이다 — 그 반복이 4부 변환기 `ReaderT<Env, M, A>` 가 *내부 모나드 M 에 대해 자동 생성* 하는 대상이다.

**노리는 능력** — "효과 쌍마다 Bind 를 손으로 짜는 비용" 을 체감하고, 변환기가 왜 필요한지를 스스로 도출한다.

## 실행

```bash
dotnet run --project code/Part3-EffectMonads/Ch15-Why-Transformers/Ch15.csproj
```
