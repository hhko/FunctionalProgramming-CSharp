# Part 4 — Monad Transformers (세계를 쌓다 — 모나드 변환기)

## 6부의 배경

5부 마지막에서 부딪힌 벽은 분명했습니다. 실전 코드는 환경 + 상태 + 오류 + IO 처럼 여러 효과를 동시에 요구하는데, 모나드는 일반적으로 합성되지 않습니다. `Reader<Env, Option<a>>` 의 `bind` 를 직접 짜 보면 두 층을 매번 손으로 풀어야 해서 추상이 무너집니다.

6부의 출발점은이 문제의 정식 해법인 **모나드 변환기 (monad transformer)** 입니다. 변환기는 하나의 효과 층을 임의의 내부 모나드 `M` 위에 쌓는 도구입니다.

- `ReaderT<Env, M, A>` — 환경 효과를 내부 모나드 `M` 위에 얹음.
- `StateT<S, M, A>` / `WriterT<W, M, A>` — 상태 / 로그 효과를 내부 `M` 위에.
- `OptionT<M, A>` / `EitherT<L, M, A>` — 부재 / 오류 효과를 내부 `M` 위에.

핵심 동사는 `lift` 입니다. 내부 모나드의 계산을 한 층 위로 끌어올려 쌓인 스택 안에서 쓸 수 있게 만듭니다. 그리고 `MonadIO<M>.LiftIO` 는 IO 효과를 스택 맨 안쪽에서 끌어올리는 특수한 lift 로, 7부의 효과 시스템으로 이어지는 다리입니다.

**여러 Elevated World 를 하나의 스택으로 쌓아 올리는 것** 이 6부의 결정적 설계입니다. 이것이 곧 7부 `Eff<RT, A> = ReaderT<RT, IO, A>` 의 정체입니다.

---

## 6부의 목표

6부를 마치면 독자는 다음을 갖게 됩니다.

- 모나드가 일반적으로 합성되지 않는 이유와, 변환기가 그것을 푸는 방식을 시그니처 단계에서 설명.
- `ReaderT<Env, M, A>` / `StateT<S, M, A>` / `WriterT<W, M, A>` 의 직접 구현 — 5부의 단일 모나드를 임의의 내부 `M` 위로 일반화.
- `OptionT<M, A>` / `EitherT<L, M, A>` 의 직접 구현 — 부재 / 오류를 내부 효과 위에 얹기.
- `lift` 와 `MonadIO<M>.LiftIO<A>(IO<A>)` 의 차이 — 일반 lift 와 IO 의 특수 lift.
- 변환기 스택을 시그니처만 보고 읽어내고, `Eff<RT, A>` 가 `ReaderT<RT, IO, A>` 임을 예감하는 능력 (7부 다리).

여러 효과를 한 스택에 쌓고 그 스택을 시그니처로 읽어내는 능력이 6부의 도달점입니다.

---

## 6부의 무대 — 쌓인 Elevated World

| 변환기 | 시그니처 | 얹는 효과 | 내부 모나드 |
|---|---|---|---|
| **ReaderT** | `ReaderT<Env, M, A>` | 환경 의존 | 임의의 `M` |
| **StateT** | `StateT<S, M, A>` | 상태 스레딩 | 임의의 `M` |
| **WriterT** | `WriterT<W, M, A>` | 로그 누적 | 임의의 `M` |
| **OptionT** | `OptionT<M, A>` | 없을 수 있음 | 임의의 `M` |
| **EitherT** | `EitherT<L, M, A>` | 실패 가능 | 임의의 `M` |

기초의 두 평행 세계 비유가 여러 층으로 확장됩니다. 변환기마다 "이 층은 무슨 효과인가 / 내부 `M` 은 무엇인가" 두 질문만 던지면 스택이 읽힙니다.

| 동사 | 시그니처 자리 | 의미 |
|---|---|---|
| `lift` | `M<a> → T<M, a>` | 내부 모나드 계산을 한 층 위로 |
| `LiftIO` | `IO<a> → M<a>` | IO 효과를 스택 맨 안쪽에서 끌어올림 |

---

## 6부의 학습 흐름

| 장 | 두 축에서의 자리 | 한 줄 |
|---|---|---|
| 19장 | 변환기 발상 | 합성의 벽 + `lift` — 효과 층을 내부 `M` 위에 쌓는 발상 |
| 20장 | 상태 · 환경 변환기 | `ReaderT` / `StateT` / `WriterT` 의 직접 구현 |
| 21장 | 오류 · 부재 변환기 | `OptionT` / `EitherT` — 실패를 내부 효과 위에 |
| 22장 | IO 의 특수 lift | `MonadIO<M>` / `LiftIO` — 7부 효과 시스템으로의 다리 |

---

## 4개 장의 구성

### 19장 — 변환기 발상 & lift

5부의 합성의 벽을 정면으로 코드로 부딪칩니다. 두 모나드를 단순 중첩하면 `bind` 가 왜 합성되지 않는지를 본 뒤, 변환기 `T<M, A>` 가 한 효과 층 + 내부 모나드 `M` 으로 문제를 푸는 구조를 봅니다. 핵심 동사 `lift : M<a> → T<M, a>` 를 직접 구현합니다.

### 20장 — ReaderT · StateT · WriterT

5부의 단일 효과 모나드를 내부 `M` 위로 일반화합니다. `ReaderT<Env, M, A>` 가 `Env → M<A>` 임을, `StateT<S, M, A>` 가 `S → M<(A, S)>` 임을 직접 구현으로 봅니다. 5부의 `Readable` / `Stateful` / `Writable` trait 이 변환기에서도 그대로 부착됨을 확인합니다.

### 21장 — OptionT · EitherT

부재 (`OptionT<M, A> = M<Option<A>>`) 와 오류 (`EitherT<L, M, A> = M<Either<L, A>>`) 를 내부 효과 위에 얹습니다. 기초의 `MyMaybe` / `MyValidation` 이 비동기 / 환경 의존과 결합되는 자리를 직접 구현하며, 오류 처리가 어떻게 스택의 한 층이 되는지 봅니다.

### 22장 — MonadIO & LiftIO

`MonadIO<M> : Monad<M>` trait 과 `static abstract K<M, A> LiftIO<A>(IO<A> ma)` 를 직접 구현합니다. 일반 `lift` 와 달리 `LiftIO` 는 스택 맨 안쪽의 IO 효과를 어디서든 끌어올립니다. 이 한 멤버가 7부 `Eff<RT, A>` 가 IO 를 품는 메커니즘이며, 6부와 7부를 잇는 다리입니다.

---

## 6부의 코드

본문 예제는 모두 `code/Part06-MonadTransformers/` 에 실행 가능한 형태로 들어 있습니다 (외부 패키지 의존 0).

```bash
dotnet run --project code/Part06-MonadTransformers/Ch19-Transformer-Idea/Ch19.csproj
dotnet run --project code/Part06-MonadTransformers/Ch20-ReaderT-StateT-WriterT/Ch20.csproj
dotnet run --project code/Part06-MonadTransformers/Ch21-OptionT-EitherT/Ch21.csproj
dotnet run --project code/Part06-MonadTransformers/Ch22-MonadIO/Ch22.csproj
```

코드 예제 요약 — Ch19 은 `OptionT<ManyF, int>` (비결정성 + 실패) 로 `lift` 와 첫 변환기를, Ch20 은 `ReaderT<int, OptionF>` (18장 ReaderOption 이 공짜로) 와 `StateT<Stack, OptionF>` (상태 + 실패) 를, Ch21 은 `EitherT<string, ManyF>` 로 실패에 이유를 남기는 것과 스택 순서의 의미를, Ch22 는 `ReaderT<int, IOF>` 에서 `LiftIO` 로 IO 를 끌어올리며 `Run(env)` 가 IO 를 조립만 하고 `io.Run()` 에서야 부수 작용이 일어나는 지연을 보입니다. 이 스택이 곧 7부 `Eff<RT, A> = ReaderT<RT, IO, A>` 의 축소판입니다. 모든 변환기는 모나드 세 법칙 검증을 통과합니다.

학습용 변환기는 LanguageExt v5 의 표현 (`ReaderT = Func<Env, K<M,A>>`, `OptionT = K<M, Option<A>>` 등) 과 정합합니다. 단, `MonadIO` 는 라이브러리의 런타임 "LiftIOMaybe" 방식 대신 내부 M 을 `MonadIO<M>` 로 제약하는 컴파일타임 안전 버전으로 단순화했습니다.

---

## 6부의 진입점

5부를 마쳤다면 Ch19 — 변환기 발상 & lift 부터 시작합니다. 합성의 벽을 코드로 먼저 부딪혀야 변환기가 왜 필요한지가 손에 잡힙니다.
