# C# 함수형 프로그래밍

> **C# 으로 배우는 함수형 프로그래밍 — Wlaschin 의 Elevated World 비유와 LanguageExt v5 의 `K<F, A>` 직접 구현으로**

이 책은 .NET 개발자가 Haskell / F# / Scala 의 함수형 어휘를 C# 어법으로 손에 잡고, LanguageExt v5 의 내부 구현 방식을 직접 코드로 옮겨 보면서 함수형의 어휘와 동작 원리 양쪽을 동시에 익히는 책입니다.

---

## 함수형이 처음 어려운 이유 — 어휘와 동작

함수형 프로그래밍을 처음 만나면 두 가지 어려움이 동시에 옵니다.

- **어휘의 어려움** — `Functor`, `Monad`, `Applicative` 같은 이름이 어디서 왔고 왜 그 모양인지 시그니처만 봐서는 안 보입니다.
- **동작의 어려움** — 라이브러리 한 줄 호출 (`xs.Map(f)`) 안에 무엇이 들어 있고 어떻게 작동하는지 손에 잡히지 않습니다.

두 가지 어려움이 동시에 작동하므로 한쪽만 해결해서는 함수형이 손에 잡히지 않습니다. 이 책은 두 가지 어려움을 **결정적인 두 축**으로 정면 돌파합니다.

---

## 결정적인 두 축

### 축 1 — Wlaschin 어법으로 함수형 어휘를 바로 잡습니다

F# 커뮤니티의 Scott Wlaschin 이 *"Map and Bind and Apply, Oh My!"* 시리즈에서 정착시킨 **Elevated World** 비유를 그대로 가져옵니다. 이 책의 모든 추상은 한 동사 위에서 압축됩니다.

> 모든 값과 함수를 합성 가능한 Elevated World 로 lift 시키는 것.

이 한 동사 (*lift*) 위에 Functor / Foldable / Applicative / Monad / Traversable 다섯 추상이 자리잡습니다. 독자가 새 추상을 만날 때마다 그 화살표가 두 세계 (Normal / Elevated) 사이에서 어느 방향으로 움직이는지만 보면 의미가 잡힙니다. 비유가 어휘를 정합니다.

### 축 2 — LanguageExt v5 의 `K<F, A>` 패턴으로 함수형 동작 원리를 직접 구현합니다

비유만으로는 내부 작동이 안 잡힙니다. 이 책은 라이브러리 한 줄 호출이 작동하기 위해 내부에서 어떤 trait / 어떤 타입 인코딩 / 어떤 dispatch 메커니즘이 필요한지를 **손으로 직접 작성하면서** 따라갑니다.

LanguageExt v5 의 핵심 발상인 **`K<F, A>` 마커 인터페이스 + self-bound 제약 + `static abstract` 멤버**의 3-tuple 패턴 (자료 / 태그 / trait) 이 어떻게 C# 의 한계 (Higher-Kinded Generics 미지원) 를 우회하면서 컴파일러가 추상을 검증하게 만드는지 본문 코드로 봅니다.

- **라이브러리 사용이 아니라 라이브러리의 구현 자체를 학습 도구로 씁니다.** 처음부터 끝까지 전체 `K<F, A>` 직접 구현으로 통일됩니다.
- 학습용 trait (`Functor<F>`, `Applicative<F>`, `Foldable<F>`, `Monad<F>`, `Traversable<F>`) 을 본문이 처음부터 정의하고, 학습용 자료 타입 (`MyList<A>`, `MyMaybe<A>`, `MyValidation<E, A>`) 에 손으로 부착하면서 추상의 내부 구조를 만집니다.
- 학습용 코드가 LanguageExt v5 의 공식 `Functor.Trait.cs` / `Applicative.Trait.cs` 와 시그니처 단계에서 정합합니다. 본문을 마치면 라이브러리 코드를 익숙한 패턴의 변형으로 읽을 수 있습니다.

도구가 동작 원리를 잡아 줍니다.

### 두 축이 함께 작동하는 방식

| 축 | 목적 | 본문 위치 |
|---|---|---|
| **Wlaschin 어법** | 함수형의 어휘 (왜 그 시그니처인가) | 각 Part 의 비유 정착 절 |
| **`K<F, A>` 직접 구현** | 함수형의 동작 원리 (어떻게 작동하는가) | 각 Part 의 trait 직접 구현 절 |

비유가 왜 그런 trait 이 필요한지 정해 주고, 구현이 어떻게 그 trait 이 작동하는지 손에 잡아 줍니다. 어휘만 있으면 추상이 공허해지고, 구현만 있으면 코드가 미궁이 됩니다.

---

## 책의 목표

이 책을 끝까지 읽고 나면 독자는 다음을 갖게 됩니다.

1. **이해의 자신감** — Functor / Monad / IO 효과 / 컬렉션 / 동시성 / 스트리밍 같은 추상의 내부 메커니즘을 일상의 말로 설명할 수 있습니다.
2. **선택의 자신감** — 도메인 문제 앞에서 어느 추상이 적절한지 시그니처만 보고 판단하고 조합할 수 있습니다.
3. **구현과 읽기의 자신감** — LanguageExt v5 / Haskell / F# / Scala 의 함수형 코드를 시그니처만 보고도 읽어내고, 자기 손으로 비슷한 자료를 구현할 수 있습니다.
4. **실무의 자신감** — LanguageExt v5 의 효과 시스템 (`Eff` / `IO` / Schedule / Resource / OpenTelemetry) 으로 실무 코드를 자신 있게 작성할 수 있습니다.
5. **테스트 자동화의 자신감** — xUnit + property-based + 합법칙 검증의 함수형 테스트 표준을 자기 코드에 적용할 수 있습니다.

다섯 가지 자신감은 책의 모든 Part 를 따라가며 누적되는 결과입니다.

---

## 대상 독자

- **C# 11+ 를 쓰는 .NET 개발자** — `static abstract` 멤버와 self-bound 제약을 활용하므로 C# 11 이상이 전제입니다.
- **함수형의 어휘를 처음 만나는 입문자** — Haskell / F# 사전 지식 없이 C# 어법만으로 따라갈 수 있습니다.
- **LINQ / `Option` / `Task` 를 매일 쓰지만 그 안 작동 원리가 안 잡히는 독자** — `Select` 가 사실 Functor 의 `Map`, `SelectMany` 가 Monad 의 `Bind` 였음을 시그니처 단계에서 봅니다.
- **Haskell / F# / Scala 의 어휘를 C# 어법으로 옮기고 싶은 독자** — 본문이 세 언어의 같은 발상을 C# 의 trait + `static abstract` 로 표현합니다.

---

## 목차

이 책은 12 개 Part 로 구성됩니다. **Part 1 ~ 12 의 본문과 실행 코드 (`code/`) 가 구현되어 다듬는 중**이고, 이 책은 모든 Part 가 **`K<F, A>` 직접 구현과 Elevated World 비유** 두 축 위에서 자란다는 점이 차별점입니다.

Part 1 ~ 3 (기초) 은 함수형의 어휘와 동작 원리를 쌓습니다 (사고·기계장치 → 핵심 trait → 조합·확장). Part 4 ~ 6 은 실제 컬렉션 / 효과 모나드 / 변환기로 이어지고, Part 7 ~ 10 은 부수 효과 (side-effect) 를 순수하게 다루는 고급 주제를 직접 구현합니다. 효과를 값으로 인코딩하고 (IO / Eff), 견고하게 다루고 (Schedule / Resource), 안전하게 동시 처리하고 (STM), 메모리 안전하게 스트리밍합니다. Part 11 ~ 12 는 함수형 테스트 표준과 실무 예제로 마무리합니다.

| Part | 주제 |
|---|---|
| **[Part 1. Foundations](./Part01-Foundations/README.md)** | 함수형 사고와 기계장치 — 두 평행 세계 비유 + Higher Kinds (`K<F, A>` + 3-tuple) + Monoid (Order 0). 4 가지 함수 유형(`a -> b`, `a -> E<b>`, `E<a> -> b`, `E<a> -> E<b>`) 의 지도. (Ch01 ~ 03) |
| **[Part 2. Core Traits](./Part02-CoreTraits/README.md)** | 네 자리의 다리 — Functor (`map`) / Applicative (`pure`+`apply`) / Foldable (`fold`) / Monad (`bind`). 4 가지 함수 유형 위 끌어올림 · 끌어내림 · 합성. (Ch04 ~ 07) |
| **[Part 3. Composition](./Part03-Composition/README.md)** | 조합 · 실전 · 확장 — Validation 누적 vs 단락 + Traversable 층 swap (세 trait 합성) + Bifunctor / NaturalTransformation 확장. (Ch08 ~ 11) |
| **[Part 4. Collections](./Part04-Collections/README.md)** | 불변 컬렉션 — 기초 toy 추상을 실무 컬렉션에 적용. lazy `Seq` (5 trait) · `Map` 값 Functor·Foldable·Traversable · `Set` Foldable 경계 · `Alternative` / `MonoidK` (고르기 vs 모으기). (Ch12 ~ 14) |
| **[Part 5. Effect Monads](./Part05-EffectMonads/README.md)** | 효과를 담는 모나드 — `Reader` / `State` / `Writer` 와 `Readable` / `Stateful` / `Writable` trait. 효과를 타입으로 인코딩하고, 단일 모나드의 합성 한계로 6부 변환기의 다리를 놓음. (Ch15 ~ 18) |
| **[Part 6. Monad Transformers](./Part06-MonadTransformers/README.md)** | 세계를 쌓다 — `ReaderT` / `StateT` / `WriterT` / `OptionT` / `EitherT` + `MonadIO` / `LiftIO`. 여러 효과를 한 스택에, 바깥 효과 하나당 변환기 하나 (`lift`). 18장 합성 한계 → 7부 `Eff<RT, A> = ReaderT<RT, IO, A>` 다리. (Ch19 ~ 22) |
| **[Part 7. Effect System](./Part07-EffectSystem/README.md)** | IO 와 효과 런타임 — `IO<A>` DSL 노드 + 트램폴린 스택 안전 · `EnvIO` 취소, `Error` / `Fin` / `Fallible` (예외를 값으로), `Eff<A>`, **`Eff<RT, A> = ReaderT<RT, IO, A>`** + `Has<RT, Trait>` 능력 DI. (Ch23 ~ 26) |
| **[Part 8. Robust Effects](./Part08-RobustEffects/README.md)** | 견고한 효과 — `Schedule` (재시도·반복, union/intersect 합성), `Resource` / `bracket` (예외에도 해제, LIFO), `Activity` / `Tracer` (분산 추적, 결과 불변 횡단 관심사). 셋 다 7부 효과 위에 조합으로 얹힘. (Ch27 ~ 29) |
| **[Part 9. Concurrency](./Part09-Concurrency/README.md)** | 동시성 — `Atom` (CAS 로 락 없는 원자적 갱신, 순수 함수라 충돌 시 재시도 안전), `STM` / `Ref` (여러 참조를 한 트랜잭션으로, 낙관적 커밋·전체 재시도), `AtomHashMap` / `VectorClock` (불변 맵을 CAS 로 감싼 동시 컬렉션 · happens-before 인과성). 상태 변화를 순수 함수로 표현해 충돌을 재적용으로 푼다. (Ch30 ~ 32) |
| **[Part 10. Streaming](./Part10-Streaming/README.md)** | 스트리밍 — `StreamT` (효과를 품은 lazy 스트림, 당긴 만큼만 계산·메모리 안전), Pipes `Producer` / `Consumer` / `Pipe` (당김 기반 합성·역압), `Conduit` + `bracket` (자원 안전 ETL 파이프라인). 기초의 합성 어휘가 무한·대용량으로 확장됩니다. (Ch33 ~ 35) |
| **[Part 11. Functional Testing](./Part11-FunctionalTesting/README.md)** | 효과·전문성 테스트 — 효과 코드 결정적 테스트 (`Sys.Test` / `MemoryConsole`), 동시·스트리밍·자원 효과, property-based 심화. 법칙 검증은 각 trait 장에서 `ForAll` 로 수행. (Ch36 ~ 38) |
| **[Part 12. Real-world](./Part12-RealWorld/README.md)** | 실무 예제 — 따로 익힌 도구를 한 코드로 합성. 강타입 도메인 + Validation 누적 검증, `Eff<RT>` + `Has` 다중 능력 DI 앱, 재시도·자원·스트림·동시 집계 파이프라인, 종합 capstone (책을 마치며). (Ch39 ~ 42) |

---

## 저장소 구조

```
FunctionalProgramming-CSharp/
├── README.md                           : 이 문서 (책 전체 안내)
│
├── Part01-Foundations/                 : 1부 (진행 중) — 함수형 사고와 기계장치
│   ├── Ch01-Paradigm-Shift.md          : 함수형 사고로의 전환 (두 평행 세계)
│   ├── Ch02-Higher-Kinds.md            : Higher Kinds (K<F, A> 직접 구현)
│   └── Ch03-Monoid.md                  : Monoid / Semigroup (Order 0 결합)
│
├── Part02-CoreTraits/                  : 2부 (진행 중) — 네 자리의 다리
│   ├── Ch04-Functor.md                 : Functor / map (1인자 lift)
│   ├── Ch05-Applicative.md             : Applicative / pure · apply (N인자 lift)
│   ├── Ch06-Foldable.md                : Foldable / fold (끌어내림)
│   └── Ch07-Monad.md                   : Monad / bind (World-crossing 합성)
│
├── Part03-Composition/                 : 3부 (진행 중) — 조합 · 실전 · 확장
│   ├── Ch08-Validation.md              : Validation (applicative 누적 vs monadic 단락)
│   ├── Ch09-Traversable.md             : Traversable / traverse · sequence (층 swap)
│   ├── Ch10-Bifunctor.md               : Bifunctor / BiMap (2-인자 변환)
│   └── Ch11-NaturalTransformation.md   : NaturalTransformation (컨테이너 교체)
│
├── Part04-Collections/                 : 4부 (진행 중) — 불변 컬렉션, 추상의 실전 적용
│   ├── Ch12-Sequences.md               : Sequences / lazy MySeq (5 trait) · MyLst (cons) · 두 Applicative
│   ├── Ch13-Maps-and-Sets.md           : Maps & Sets / MyMap (값 Functor·Foldable·Traversable) · MySet (Foldable 경계)
│   └── Ch14-Alternative.md             : Alternative & MonoidK / Choose (고르기) vs Combine (모으기)
│
├── Part05-EffectMonads/                : 5부 (진행 중) — 효과를 담는 모나드
│   ├── Ch15-Reader.md                  : Reader / 환경 의존 (Readable: Asks · Ask · Local)
│   ├── Ch16-State.md                   : State / 상태 스레딩 (Stateful: Get · Put · Modify · Gets)
│   ├── Ch17-Writer.md                  : Writer / 누적 로그 (Writable: Tell · Listen · Pass, Monoid 재방문)
│   └── Ch18-Why-Transformers.md        : 왜 변환기가 필요한가 (단일 모나드 합성 한계 → 6부 다리)
│
├── Part06-MonadTransformers/           : 6부 (진행 중) — 세계를 쌓다, 모나드 변환기
│   ├── Ch19-Transformer-Idea.md        : 변환기 발상 & lift (첫 변환기 OptionT, MonadT.Lift)
│   ├── Ch20-ReaderT-StateT-WriterT.md  : ReaderT · StateT · WriterT (5부 효과를 내부 M 위로 일반화)
│   ├── Ch21-OptionT-EitherT.md         : OptionT · EitherT (부재 None vs 이유 있는 오류 Left)
│   └── Ch22-MonadIO.md                 : MonadIO & LiftIO (IO 의 특수 lift, 7부 효과 시스템 다리)
│
├── Part07-EffectSystem/                : 7부 (진행 중) — IO 와 효과 런타임
│   ├── Ch23-IO.md                      : IO / 지연 효과 (DSL 노드 + 트램폴린 스택 안전 + EnvIO 취소)
│   ├── Ch24-Error-Fin-Fallible.md      : Error · Fin · Fallible (예외를 값으로, 첫 실패 단락)
│   ├── Ch25-Eff.md                     : Eff / 런타임 없는 효과 (IO + 오류, Choose · Finally)
│   └── Ch26-Eff-Runtime.md             : Eff<RT, A> = ReaderT<RT, IO, A> (6부 변환기 회수 + Has DI)
│
├── Part08-RobustEffects/               : 8부 (진행 중) — 견고한 효과
│   ├── Ch27-Schedule.md                : Schedule / 재시도·반복 (Duration 스트림, union · intersect 합성)
│   ├── Ch28-Resource.md                : Resource & bracket / 자원 수명 (예외에도 해제, LIFO)
│   └── Ch29-Observability.md           : Observability / Activity · Tracer (분산 추적, 결과 불변)
│
├── Part09-Concurrency/                 : 9부 (진행 중) — 동시성
│   ├── Ch30-Atom.md                    : Atom / CAS 원자성 (순수 갱신이라 재시도 안전, 8 스레드 무손실)
│   ├── Ch31-STM.md                     : STM & Ref / 트랜잭션 메모리 (낙관적 커밋·전체 재시도, all-or-nothing)
│   └── Ch32-Concurrent-Collections.md  : 동시 컬렉션 & 인과성 (AtomHashMap CAS · VectorClock 부분 순서)
│
├── Part10-Streaming/                   : 10부 (진행 중) — 스트리밍
│   ├── Ch33-StreamT.md                 : StreamT / 효과적 lazy 스트림 (Pull 기반 · 당긴 만큼만 효과)
│   ├── Ch34-Pipes.md                   : Pipes / Producer · Consumer · Pipe (당김 합성 · 역압)
│   └── Ch35-Conduit.md                 : Conduit & 실전 파이프라인 (자원 안전 ETL · bracket 결합)
│
├── Part11-FunctionalTesting/           : 11부 (진행 중) — 효과·전문성 테스트
│   ├── Ch36-Effect-Testing.md          : 효과 코드의 결정적 테스트 (런타임 교체 · MemoryConsole)
│   ├── Ch37-Concurrent-Streaming.md    : 동시·스트리밍·자원 효과 테스트
│   └── Ch38-Property-Based.md          : property-based 심화 + 테스트 아키텍처
│
└── Part12-RealWorld/                   : 12부 (진행 중) — 실무 예제, 도구의 합성
    ├── Ch39-Domain-Modeling.md          : 강타입 도메인 + Validation 누적 (잘못된 상태를 표현 불가능하게)
    ├── Ch40-Effectful-Application.md    : Eff<RT> + Has 다중 능력 DI (부수 효과 격리 · 두 런타임)
    ├── Ch41-Streaming-Pipeline.md       : 재시도 · 자원 · 스트림 · 동시 집계 한 파이프라인
    └── Ch42-Capstone.md                 : 종합 capstone — 검증·효과·테스트 한 서비스 (책을 마치며)
```

본문 (`Part…/*.md`) 과 코드 (`code/Part…/Ch…/*`) 가 시그니처 단계에서 정합합니다. 본문을 읽으면서 해당 코드를 IDE 에서 직접 실행 / 수정해 볼 수 있습니다 (위 트리에서는 코드 계층을 생략했습니다).

---

## 책 코드 실행

본문에 등장하는 모든 코드는 `code/` 디렉토리에 Part / 챕터별로 정리되어 있습니다.

```bash
cd code
dotnet build FunctionalProgramming-CSharp.slnx          # 전체 프로젝트 빌드

# 각 챕터는 독립 실행 가능한 콘솔 데모 — 데모가 모든 법칙/검증 결과를 출력
dotnet run --project Part02-CoreTraits/Ch04-Functor/Ch04.csproj
dotnet run --project Part03-Composition/Ch11-NaturalTransformation/Ch11.csproj
```

각 챕터 코드는 `code/PartNN-…/ChNN-…/` 에 독립적으로 들어 있어 한 챕터만 따로 실행해 볼 수 있습니다. 검증은 `Tests/` 의 콘솔 `bool` 헬퍼로 이뤄지며 (각 trait 장이 3장 3.10.6절의 `ForAll` 로 법칙을 임의 입력에 검증), 11부 (Ch36 ~ Ch38) 에서 효과 코드의 결정적 테스트와 xUnit + Shouldly / property-based 심화로 옮기는 법을 다룹니다. 각 챕터의 `Challenges/` 에는 직접 해보기 정답 코드가 들어 있습니다.

---

## 환경 요구사항

- .NET SDK — **10.0 권장** (코드 프로젝트의 `<TargetFramework>` 가 `net10.0` 으로 설정). 최소 .NET 8 SDK 가 필요 (C# 11 의 `static abstract` 멤버 지원).
- C# 언어 버전 — **14 권장** (`<LangVersion>14</LangVersion>` 설정). 최소 C# 11 (`static abstract` 멤버 필수).
- 테스트 도구 — 11부에서 실무 표준 (xUnit + Shouldly, property-based: CsCheck / FsCheck) 으로 옮기는 법을 다룹니다.

---

## 함께 보는 자료

- Scott Wlaschin, "Map and Bind and Apply, Oh My!" 시리즈 — [fsharpforfunandprofit.com/posts/elevated-world](https://fsharpforfunandprofit.com/posts/elevated-world/). 이 책의 축 1 (Elevated World 어법) 의 원전입니다. F# 의 어휘로 정착된 비유를 이 책이 C# / `K<F, A>` 위에 다시 입힙니다.
- LanguageExt v5 — [github.com/louthy/language-ext](https://github.com/louthy/language-ext). 이 책의 축 2 (`K<F, A>` 직접 구현) 의 기준. 본문의 학습용 trait 시그니처가 라이브러리의 `Functor.Trait.cs` / `Applicative.Trait.cs` 등과 정합합니다.
- Paul Louth (LanguageExt 저자) 의 블로그 — <https://paullouth.com/>.
- Thinking Functionally wiki (LanguageExt) — <https://github.com/louthy/language-ext/wiki/Thinking-Functionally>.
