# C# 함수형 프로그래밍

> ***C# 으로 배우는 함수형 프로그래밍 — Wlaschin 의 Elevated World 비유와 LanguageExt v5 의 `K<F, A>` 직접 구현으로***

이 책은 .NET 개발자가 *Haskell / F# / Scala 의 함수형 어휘* 를 C# 어법으로 손에 잡고, *LanguageExt v5 의 내부 구현 방식* 을 직접 코드로 옮겨 보면서 함수형의 *어휘 + 동작 원리* 양쪽을 동시에 익히는 책입니다.

---

## 함수형이 처음 어려운 이유 — 어휘와 동작

함수형 프로그래밍을 처음 만나면 *두 가지 어려움* 이 동시에 옵니다.

- *어휘의 어려움* — `Functor`, `Monad`, `Applicative` 같은 이름이 *어디서 왔고 왜 그 모양인지* 시그니처만 봐서는 안 보입니다.
- *동작의 어려움* — 라이브러리 한 줄 호출 (`xs.Map(f)`) 안에 *무엇이 들어 있고 어떻게 작동하는지* 손에 잡히지 않습니다.

두 가지 어려움이 동시에 작동하므로 한쪽만 해결해서는 함수형이 손에 잡히지 않습니다. 이 책은 두 가지 어려움을 ***결정적인 두 축*** 으로 정면 돌파합니다.

---

## 결정적인 두 축

### 축 1 — Wlaschin 어법으로 함수형 *어휘* 를 바로 잡습니다

F# 커뮤니티의 Scott Wlaschin 이 *"Map and Bind and Apply, Oh My!"* 시리즈에서 정착시킨 ***Elevated World*** 비유를 그대로 가져옵니다. 이 책의 모든 추상은 한 동사 위에서 압축됩니다.

> *모든 값과 함수를 합성 가능한 Elevated World 로 lift 시키는 것.*

이 한 동사 (*lift*) 위에 Functor / Foldable / Applicative / Monad / Traversable 다섯 추상이 자리잡습니다. 독자가 새 추상을 만날 때마다 *그 화살표가 두 세계 (Normal / Elevated) 의 어디서 어디로 가는가* 만 보면 의미가 잡힙니다. 비유가 어휘를 정합니다.

### 축 2 — LanguageExt v5 의 `K<F, A>` 패턴으로 함수형 *동작 원리* 를 직접 구현합니다

비유만으로는 *내부 작동* 이 안 잡힙니다. 이 책은 라이브러리 한 줄 호출이 작동하기 위해 *내부에서* 어떤 trait / 어떤 타입 인코딩 / 어떤 dispatch 메커니즘이 필요한지를 ***손으로 직접 작성하면서*** 따라갑니다.

LanguageExt v5 의 결정적 발상인 ***`K<F, A>` 마커 인터페이스 + self-bound 제약 + `static abstract` 멤버*** 의 *3-tuple 패턴* (자료 / 태그 / trait) 이 어떻게 C# 의 한계 (Higher-Kinded Generics 미지원) 를 우회하면서 *컴파일러가 추상을 검증* 하게 만드는지 본문 코드로 봅니다.

- ***라이브러리 사용이 아니라 라이브러리의 구현 자체를 학습 도구로 씁니다.*** 처음부터 끝까지 *전체 `K<F, A>` 직접 구현* 으로 통일됩니다.
- 학습용 trait (`Functor<F>`, `Applicative<F>`, `Foldable<F>`, `Monad<F>`, `Traversable<F>`) 을 본문이 *처음부터* 정의하고, 학습용 자료 타입 (`MyList<A>`, `MyMaybe<A>`, `MyValidation<E, A>`) 에 *손으로* 부착하면서 추상의 내부 구조를 만집니다.
- 학습용 코드가 LanguageExt v5 의 공식 `Functor.Trait.cs` / `Applicative.Trait.cs` 와 시그니처 단계에서 정합 — 본문을 마치면 라이브러리 코드를 *익숙한 패턴의 변형* 으로 읽을 수 있습니다.

도구가 동작 원리를 정합니다.

### 두 축이 함께 작동하는 자리

| 축 | 무엇을 잡는가 | 책의 어느 자리 |
|---|---|---|
| **Wlaschin 어법** | 함수형의 *어휘* (왜 그 시그니처인가) | 각 Part 의 *비유 정착* 절 |
| **`K<F, A>` 직접 구현** | 함수형의 *동작 원리* (어떻게 작동하는가) | 각 Part 의 *trait 직접 구현* 절 |

비유가 *왜* 그런 trait 이 필요한지 정해 주고, 구현이 *어떻게* 그 trait 이 작동하는지 손에 잡아 줍니다. 어휘만 있으면 *추상이 공허* 해지고, 구현만 있으면 *코드가 미궁* 이 됩니다.

---

## 책의 목표

이 책을 끝까지 읽고 나면 독자는 다음을 갖게 됩니다.

1. **이해의 자신감** — Functor / Monad / IO 효과 / 컬렉션 / 동시성 / 스트리밍 같은 추상의 *내부 메커니즘* 을 일상의 말로 설명할 수 있습니다.
2. **선택의 자신감** — 도메인 문제 앞에서 *어느 추상이 적절한 자리* 인지 *시그니처만 보고* 판단하고 *조합* 할 수 있습니다.
3. **구현 + 읽기의 자신감** — LanguageExt v5 / Haskell / F# / Scala 의 함수형 코드를 *시그니처만 보고도* 읽어내고, *자기 손으로 비슷한 자료를 구현* 할 수 있습니다.
4. **실무의 자신감** — LanguageExt v5 의 효과 시스템 (`Eff` / `IO` / Schedule / Resource / OpenTelemetry) 으로 *실무 코드* 를 자신 있게 작성할 수 있습니다.
5. **테스트 자동화의 자신감** — xUnit + property-based + 합법칙 검증의 *함수형 테스트 표준* 을 자기 코드에 적용할 수 있습니다.

다섯 가지 자신감이 책의 *모든 Part* 가 빌드업하는 누적 결과입니다.

---

## 대상 독자

- **C# 11+ 를 쓰는 .NET 개발자** — `static abstract` 멤버와 self-bound 제약을 활용하므로 C# 11 이상이 전제입니다.
- **함수형의 어휘를 *처음* 만나는 입문자** — Haskell / F# 사전 지식 없이 C# 어법만으로 따라갈 수 있습니다.
- **LINQ / `Option` / `Task` 를 매일 쓰지만 *그 안 작동 원리* 가 안 잡히는 독자** — `Select` 가 사실 Functor 의 `Map`, `SelectMany` 가 Monad 의 `Bind` 였음을 시그니처 단계에서 봅니다.
- **Haskell / F# / Scala 의 어휘를 C# 어법으로 옮기고 싶은 독자** — 본문이 세 언어의 같은 발상을 C# 의 trait + `static abstract` 로 표현합니다.

---

## 목차

이 책은 *6개 Part* 로 구성됩니다. 현재 *Part 1* 이 진행 중이고, 나머지 Part 는 *순차적으로 추가* 됩니다. 이 책은 *모든 Part 가 `K<F, A>` 직접 구현 + Elevated World 비유* 두 축 위에서 자란다는 점이 차별점입니다.

| Part | 주제 | 상태 | 진입 |
|---|---|---|---|
| **Part 1. Foundations** | 함수형 기초 — 두 평행 세계 비유 정착 + Functor / Foldable / Applicative / Monad / Traversable + Validation 실전. 8개 추상 + 4분면 격자. | ***진행 중*** | [Part1-Foundations/README.md](./Part1-Foundations/README.md) |
| **Part 2. Effects and Composition** | 효과의 어휘와 결합 — 여러 Monad 의 효과 분류 + Monad Transformer + ReaderT / StateT / WriterT 패턴. | 예정 | — |
| **Part 3. Advanced Abstractions** | 고급 함수형 추상 — Free Monad / 트램펄린 / 광학 (Lens / Prism) / Bifunctor / Continuation. | 예정 | — |
| **Part 4. LanguageExt Effects System** | LanguageExt v5 의 효과 시스템 — `IO` / `Eff` + Runtime / Error + Fin / Schedule / Resource / `LanguageExt.Sys`. | 예정 | — |
| **Part 5. LanguageExt Library Depth** | LanguageExt 라이브러리의 폭과 깊이 — Live vs Test 런타임 / STM + Atom 동시성 / Pipes 스트리밍 / OpenTelemetry / Collections / 추가 Monad 카탈로그. | 예정 | — |
| **Part 6. Practical Projects** | 실무 프로젝트 — 카드 게임 (State Monad) / 신용카드 검증 (Applicative) / 뉴스레터 웹 서비스 (Eff + OTel) / 스트림 데이터 파이프라인 / 복합 효과 합성 / Blazor 함수형 UI. | 예정 | — |

---

## 저장소 구조

```
FunctionalProgramming-CSharp/
├── README.md                       : 이 문서 (책 전체 안내)
├── Part1-Foundations/              : 1부 본문 (진행 중)
│   ├── README.md                   : 1부 배경과 목표
│   ├── Ch01-Paradigm-Shift.md
│   ├── Ch02-Higher-Kinds.md
│   ├── Ch03-Functor.md
│   ├── Ch04-Applicative.md
│   ├── Ch05-Foldable.md
│   ├── Ch06-Monad.md               : 예정
│   ├── Ch07-Validation.md          : 예정
│   ├── Ch08-Traversable.md         : 예정
│   └── images/                     : 본문 SVG 도식
├── Part2-EffectsAndComposition     : 예정
├── Part3-AdvancedAbstractions      : 예정
├── Part4-LanguageExtEffectsSystem  : 예정
├── Part5-LanguageExtLibraryDepth   : 예정
├── Part6-PracticalProjects         : 예정
└── code/                           : 학습용 실행 코드
    ├── FunctionalProgramming-CSharp.slnx
    └── Part1-Foundations/
        ├── Ch01-Paradigm-Shift/
        ├── Ch02-HigherKinds/
        ├── Ch03-Functor/
        ├── Ch04-Applicative/
        ├── Ch05-Foldable/
        ├── Ch06-Monad/
        ├── Ch07-Validation/
        └── Ch08-Traversable/
```

본문 (`PartN-…/*.md`) 과 코드 (`code/PartN-…/*`) 가 *시그니처 단계에서 정합* 합니다. 본문을 읽으면서 해당 코드를 IDE 에서 직접 실행 / 수정해 볼 수 있습니다.

---

## 책의 사용법

### 처음부터 끝까지 읽기

함수형 프로그래밍이 처음이라면 *Part 1 → Part 2 → … → Part 6* 의 순서로 읽습니다. Part 1 에서 8개 추상을 차곡차곡 쌓고, Part 2 에서 그 추상으로 효과를 합성하고, Part 3 ~ Part 4 에서 고급 / 라이브러리 주제로 나아간 뒤, Part 5 ~ Part 6 에서 라이브러리 깊이와 실무 프로젝트로 손에 익힙니다.

각 Part 는 *앞 Part 의 결과물을 가정하지 않고도* 어느 정도 따라올 수 있도록 도입부에 회고를 두지만, 책 전체의 흐름은 *연속된 빌드업* 입니다.

### 필요한 부분만 골라 읽기

이미 함수형의 일부를 안다면 자신의 빈 곳만 골라 읽어도 좋습니다.

- *기본기 점검* — Part 1.
- *효과 합성을 더 깊이* — Part 2.
- *고급 함수형 주제 (Free / 광학 등)* — Part 3.
- *LanguageExt v5 효과 시스템* — Part 4.
- *라이브러리 깊이* — Part 5.
- *실무 적용 사례* — Part 6.

각 Part 의 첫 페이지 (`README.md`) 가 그 Part 의 흐름을 안내합니다.

---

## 책 코드 실행

본문에 등장하는 모든 코드는 `code/` 디렉토리에 Part / 챕터별로 정리되어 있습니다.

```bash
cd code
dotnet restore FunctionalProgramming-CSharp.slnx
dotnet build   FunctionalProgramming-CSharp.slnx
dotnet test    FunctionalProgramming-CSharp.slnx
```

각 챕터의 코드는 `code/PartN-…/ChNN-…/` 디렉토리에 독립적으로 들어 있어, 한 챕터만 따로 실행해 볼 수도 있습니다.

---

## 환경 요구사항

- **.NET SDK** — ***10.0 권장*** (코드 프로젝트의 `<TargetFramework>` 가 `net10.0` 으로 설정). 최소 .NET 8 SDK 가 필요 (C# 11 의 `static abstract` 멤버 지원).
- **C# 언어 버전** — ***14 권장*** (`<LangVersion>14</LangVersion>` 설정). 최소 C# 11 (`static abstract` 멤버 필수).
- **NuGet 패키지** — *Part 1 ~ Part 3 은 LanguageExt 의존 없이* 학습용 trait 을 직접 구현. *Part 4 이후* 부터 `LanguageExt.Core 5.0.0-beta-77+` 를 본격 사용.
- **테스트 도구** — xUnit + Shouldly.

---

## 함께 보는 자료

- ***Scott Wlaschin, "Map and Bind and Apply, Oh My!" 시리즈*** — [fsharpforfunandprofit.com/posts/elevated-world](https://fsharpforfunandprofit.com/posts/elevated-world/). 이 책의 *축 1 (Elevated World 어법)* 의 원전. F# 의 어휘로 정착된 비유를 이 책이 C# / `K<F, A>` 위에 다시 입힙니다.
- ***LanguageExt v5*** — [github.com/louthy/language-ext](https://github.com/louthy/language-ext). 이 책의 *축 2 (`K<F, A>` 직접 구현)* 의 기준. 본문의 학습용 trait 시그니처가 라이브러리의 `Functor.Trait.cs` / `Applicative.Trait.cs` 등과 정합합니다.
- ***Paul Louth (LanguageExt 저자) 의 블로그*** — <https://paullouth.com/>.
- ***Thinking Functionally wiki (LanguageExt)*** — <https://github.com/louthy/language-ext/wiki/Thinking-Functionally>.

---

## 진행 상태 (집필 중)

현재 *Part 1 (8개 장)* 의 1 ~ 5장이 완성되어 있고, 6 ~ 8장이 작성 중입니다. Part 2 이후는 *Part 1 마무리 후* 순차 추가됩니다. 본문의 사실 오류 / 한국어 표현 / 코드 정합성 문제는 issue 로 알려 주시면 검토합니다.

이제 [Part 1 — Foundations](./Part1-Foundations/README.md) 로 진입합니다.
