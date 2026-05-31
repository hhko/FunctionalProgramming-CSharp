# C# 함수형 프로그래밍

> **C# 으로 배우는 함수형 프로그래밍 — Wlaschin 의 Elevated World 비유와 LanguageExt v5 의 `K<F, A>` 직접 구현으로**

이 책은 .NET 개발자가 Haskell / F# / Scala 의 함수형 어휘를 C# 어법으로 손에 잡고, LanguageExt v5 의 내부 구현 방식을 직접 코드로 옮겨 보면서 함수형의 어휘 + 동작 원리 양쪽을 동시에 익히는 책입니다.

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
3. **구현 + 읽기의 자신감** — LanguageExt v5 / Haskell / F# / Scala 의 함수형 코드를 시그니처만 보고도 읽어내고, 자기 손으로 비슷한 자료를 구현할 수 있습니다.
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

이 책은 10 개 Part 로 구성됩니다. **10 개 Part 의 실행 코드 (`code/`) 가 모두 구현** 되어 있고, 이 책은 모든 Part 가 **`K<F, A>` 직접 구현과 Elevated World 비유** 두 축 위에서 자란다는 점이 차별점입니다.

Part 1 ~ 4 는 함수형의 어휘와 동작 원리를 쌓습니다 (Foundations → 실제 컬렉션 → 효과 모나드 → 변환기 순). Part 5 ~ 8 은 부작용 (side-effect) 을 순수하게 다루는 고급 주제를 직접 구현합니다. 효과를 값으로 인코딩하고 (IO / Eff), 견고하게 다루고 (Schedule / Resource), 안전하게 동시 처리하고 (STM), 메모리 안전하게 스트리밍합니다. Part 9 ~ 10 은 함수형 테스트 표준과 실무 예제로 마무리합니다.

| Part | 주제 | 상태 |
|---|---|---|
| **[Part 1. Foundations](./Part01-Foundations/README.md)** | 함수형 기초 — 두 평행 세계 비유 정착 + Functor / Foldable / Applicative / Monad / Traversable + Validation 실전. 5개 trait + 4 가지 함수 유형. | **진행 중** |
| **Part 2. Collections** | 불변 컬렉션 — Part 1 의 toy 추상을 실제 `Seq` / `Map` / `HashMap` 에 적용. `Alternative` / `SemigroupK` / `MonoidK` (선택과 결합). | 예정 |
| **Part 3. Effect Monads** | 효과를 담는 모나드 — `Reader` / `State` / `Writer` 와 `Readable` / `Stateful` / `Writable` trait. 효과를 타입으로 인코딩. | 예정 |
| **Part 4. Monad Transformers** | 세계를 쌓다 — `ReaderT` / `StateT` / `WriterT` / `OptionT` / `EitherT` + `MonadIO`. 여러 효과를 한 스택에. | 예정 |
| **Part 5. Effect System** | IO 와 효과 런타임 — `IO<A>` DSL 인코딩 + `EnvIO`, `Error` / `Fin` / `Fallible`, `Eff<A>`, `Eff<RT, A> = ReaderT<RT, IO, A>`. | 예정 |
| **Part 6. Robust Effects** | 견고한 효과 — `Schedule` (재시도 / 반복), `Resource` / bracket (자원 수명), `Activity` (OpenTelemetry 관측성). | 예정 |
| **Part 7. Concurrency** | 동시성 — `Atom` (CAS), `STM` / `Ref` (트랜잭션 메모리), `AtomHashMap` / `VectorClock` (동시 컬렉션 · 인과성). | 예정 |
| **Part 8. Streaming** | 스트리밍 — `StreamT` (효과적 스트림), Pipes `Producer` / `Consumer` / `Pipe`, `Conduit` 실전 파이프라인. | 예정 |
| **Part 9. Functional Testing** | 함수형 테스트 표준 — 합법칙 검증 (xUnit), property-based 테스트, 테스트 더블 (`MemoryConsole`) 로 `Eff<RT>` 결정성 테스트. | 예정 |
| **Part 10. Real-world** | 실무 예제 — 도메인 모델링 + 검증 파이프라인, `Eff<RT>` + `LanguageExt.Sys` + `Has` DI 애플리케이션, 종합 capstone. | 예정 |

---

## 저장소 구조

```
FunctionalProgramming-CSharp/
├── README.md                       : 이 문서 (책 전체 안내)
├── LanguageExt-Concept-Map.md      : LanguageExt v5 기능 ↔ 함수형 개념 ↔ 책 커버리지 매핑표
├── Part01-Foundations/             : 1부 본문 (진행 중)
│   ├── README.md                   : 1부 배경과 목표
│   ├── Ch01-Paradigm-Shift.md
│   ├── Ch02-Higher-Kinds.md
│   ├── Ch03-Monoid.md
│   ├── Ch04-Functor.md
│   ├── Ch05-Applicative.md
│   ├── Ch06-Foldable.md
│   ├── Ch07-Monad.md               : 예정
│   ├── Ch08-Validation.md          : 예정
│   ├── Ch09-Traversable.md         : 예정
│   ├── Ch10-Bifunctor.md
│   ├── Ch11-NaturalTransformation.md
│   └── images/                     : 본문 SVG 도식
├── Part02-Collections/             : 2부 (예정) — 불변 컬렉션 — 추상의 실전 적용
├── Part03-EffectMonads/            : 3부 (예정) — 효과를 담는 모나드 (Reader / State / Writer)
├── Part04-MonadTransformers/       : 4부 (예정) — 세계를 쌓다 — 모나드 변환기
├── Part05-EffectSystem/            : 5부 (예정) — IO 와 효과 런타임 (IO / Eff)
├── Part06-RobustEffects/           : 6부 (예정) — 견고한 효과 (Schedule / Resource / Observability)
├── Part07-Concurrency/             : 7부 (예정) — 동시성 (Atom / STM / Ref)
├── Part08-Streaming/               : 8부 (예정) — 스트리밍 (StreamT / Pipes / Conduit)
├── Part09-FunctionalTesting/       : 9부 (예정) — 함수형 테스트 표준
├── Part10-RealWorld/               : 10부 (예정) — 실무 예제
└── code/                           : 학습용 실행 코드
    ├── FunctionalProgramming-CSharp.slnx
    ├── Part01-Foundations/         : 11 챕터 모두 완성 (빌드·실행 가능)
    │   ├── Ch01-Paradigm-Shift/
    │   ├── Ch02-HigherKinds/
    │   ├── Ch03-Monoid/
    │   ├── Ch04-Functor/
    │   ├── Ch05-Applicative/
    │   ├── Ch06-Foldable/
    │   ├── Ch07-Monad/
    │   ├── Ch08-Validation/
    │   ├── Ch09-Traversable/
    │   ├── Ch10-Bifunctor/
    │   └── Ch11-NaturalTransformation/
    ├── Part02-Collections/         : 예정 (Ch12 ~ Ch14)
    ├── Part03-EffectMonads/        : 예정 (Ch15 ~ Ch18)
    ├── Part04-MonadTransformers/   : 예정 (Ch19 ~ Ch22)
    ├── Part05-EffectSystem/        : 예정 (Ch23 ~ Ch26)
    ├── Part06-RobustEffects/       : 예정 (Ch27 ~ Ch29)
    ├── Part07-Concurrency/         : 예정 (Ch30 ~ Ch32)
    ├── Part08-Streaming/           : 예정 (Ch33 ~ Ch35)
    ├── Part09-FunctionalTesting/   : 예정 (Ch36 ~ Ch38)
    └── Part10-RealWorld/           : 예정 (Ch39 ~ Ch42)
```

> **총 42 챕터 구성** — 1부 11 챕터 (신규 Ch03 Monoid / Ch10 Bifunctor / Ch11 NaturalTransformation 추가) + 2 ~ 10 부 31 챕터 (각 2 ~ 4 챕터). 1부 코드는 11 챕터 모두 완성, 본문은 8 장 완성 (Ch01 / 02 / 03 / 04 / 05 / 06 / 10 / 11), 3 장 (Ch07 / 08 / 09) 예정.

본문 (`PartN-…/*.md`) 과 코드 (`code/PartN-…/*`) 가 시그니처 단계에서 정합합니다. 본문을 읽으면서 해당 코드를 IDE 에서 직접 실행 / 수정해 볼 수 있습니다.

---

## 책 코드 실행

본문에 등장하는 모든 코드는 `code/` 디렉토리에 Part / 챕터별로 정리되어 있습니다.

```bash
cd code
dotnet build FunctionalProgramming-CSharp.slnx          # 전체 프로젝트 빌드

# 각 챕터는 독립 실행 가능한 콘솔 데모 — 데모가 모든 법칙/검증 결과를 출력
dotnet run --project Part01-Foundations/Ch04-Functor/Ch04.csproj
dotnet run --project Part01-Foundations/Ch11-NaturalTransformation/Ch11.csproj
```

각 챕터 코드는 `code/Part01-Foundations/ChNN-…/` (1부는 완성, 2 ~ 10부는 예정) 에 독립적으로 들어 있어 한 챕터만 따로 실행해 볼 수 있습니다. 검증은 `Tests/` 의 콘솔 `bool` 헬퍼로 이뤄지며, 9부 (Ch36 ~ Ch38) 에서 이를 xUnit + Shouldly / property-based 표준으로 옮기는 법을 다룹니다. 각 챕터의 `Challenges/` 에는 직접 해보기 정답 코드가 들어 있습니다.

---

## 환경 요구사항

- **.NET SDK** — **10.0 권장** (코드 프로젝트의 `<TargetFramework>` 가 `net10.0` 으로 설정). 최소 .NET 8 SDK 가 필요 (C# 11 의 `static abstract` 멤버 지원).
- **C# 언어 버전** — **14 권장** (`<LangVersion>14</LangVersion>` 설정). 최소 C# 11 (`static abstract` 멤버 필수).
- **테스트 도구** — 9부에서 실무 표준 (xUnit + Shouldly, property-based: CsCheck / FsCheck) 으로 옮기는 법을 다룹니다.

---

## 함께 보는 자료

- **Scott Wlaschin, "Map and Bind and Apply, Oh My!" 시리즈** — [fsharpforfunandprofit.com/posts/elevated-world](https://fsharpforfunandprofit.com/posts/elevated-world/). 이 책의 축 1 (Elevated World 어법) 의 원전입니다. F# 의 어휘로 정착된 비유를 이 책이 C# / `K<F, A>` 위에 다시 입힙니다.
- **LanguageExt v5** — [github.com/louthy/language-ext](https://github.com/louthy/language-ext). 이 책의 축 2 (`K<F, A>` 직접 구현) 의 기준. 본문의 학습용 trait 시그니처가 라이브러리의 `Functor.Trait.cs` / `Applicative.Trait.cs` 등과 정합합니다.
- **Paul Louth (LanguageExt 저자) 의 블로그** — <https://paullouth.com/>.
- **Thinking Functionally wiki (LanguageExt)** — <https://github.com/louthy/language-ext/wiki/Thinking-Functionally>.
