# Part 1 — Foundations (함수형 사고와 기계장치)

> 기초 3부작 (Part 1 ~ 3) 의 첫 Part 입니다. Part 1 이 **무대와 도구** 를 세우고, [Part 2 — Core Traits](../Part02-CoreTraits/README.md) 가 **네 자리의 다리** 를, [Part 3 — Composition](../Part03-Composition/README.md) 이 **조합 · 실전 · 확장** 을 올립니다.

## Part 1 의 배경

함수형 추상은 카탈로그 식 학습으로는 손에 안 잡힙니다. `Map`, `Apply`, `Bind`, `Fold`, `Traverse` 가 서로 무관해 보이고, 시그니처도 이름도 제각각이라 어디서 시작해 어디로 끝나는지가 안 보입니다.

기초 3부작의 출발점은 그 학습 방식을 바꾸는 것입니다. **5 개 핵심 trait (Functor / Applicative / Foldable / Monad / Traversable) 이 사실 같은 4 가지 함수 유형의 다른 모습** 임을, 책 전체를 관통하는 한 비유 (Elevated World) 위에서 정착시킵니다. Part 1 은 그 **비유와 기계장치** 를 세우는 자리입니다.

- **두 평행 세계 비유 (축 1, Wlaschin 어법)** — Normal World 와 Elevated World, 그 사이를 오가는 4 가지 함수 유형. 이후 모든 추상을 분류하는 지도입니다.
- **`K<F, A>` 직접 구현 (축 2, language-ext v5)** — `K<F, A>` 마커 + self-bound + `static abstract` 의 3-tuple 패턴 (자료 / 태그 / trait). 라이브러리 한 줄 호출이 아니라 그 안 작동 원리를 코드로 만집니다.
- **Order 0 결합 (Monoid)** — `K<F, A>` 마커 없이 가장 단순한 형태로 self-bound + `static abstract` 패턴을 먼저 연습합니다.

## Part 1 의 무대 — 두 평행 세계

기초 3부작의 11 개 장이 모두 같은 무대 위에서 진행됩니다.

| 세계 | 시민 | 의미 |
|---|---|---|
| **Elevated World** (위) | `E<a>` — `Option<a>`, `List<a>`, `Result<E, a>`, `Task<a>` … | 컨테이너에 끌어올린 값. 효과 (없을 수 있음 / 여러 개 / 실패 / 비동기 / 환경 의존) 를 타입에 인코딩합니다. |
| **Normal World** (아래) | `a`, `b` — `int`, `string`, `User`, `DateTime` … | 일상의 C# 값과 함수. 효과 없는 평범한 어법입니다. |

두 세계 시민 사이를 오가는 함수는 정확히 네 모양으로만 가능합니다.

| 함수 시그니처 | 자리 | 처리하는 trait |
|---|---|---|
| `a → b` | Normal → Normal | (추상 불필요 — 기본 함수 합성으로 충분) |
| `a → E<b>` | Normal → Elevated | **Monad** (Part 2, 7장 `bind`) — 합성 되살리기 |
| `E<a> → b` | Elevated → Normal | **Foldable** (Part 2, 6장 `fold`) — 끌어내림 |
| `E<a> → E<b>` | Elevated → Elevated | **Functor** (Part 2, 4장 `map`) — 끌어올림 |

## Part 1 의 장 (Ch01 ~ 03)

### 1장 — [함수형 사고로의 전환](./Ch01-Paradigm-Shift.md)
코드 이전의 마음가짐입니다. 두 평행 세계 비유를 도입하고, 그 사이를 오가는 4 가지 함수 유형을 미리 봅니다. 이 4 가지가 이후 핵심 추상을 분류하고 이해하는 기준이 됩니다.

### 2장 — [Higher Kinds](./Ch02-Higher-Kinds.md)
기술적 출발점입니다. 객체지향 어법의 N×M 비용을 함수형의 N+M 비용으로 해소하는 발상을 잡고, C# 의 제약 (Order 2 미지원) 을 `K<F, A>` 마커 + self-bound + `static abstract` 두 우회 도구로 넘습니다. 3-tuple 패턴 (자료 / 태그 / trait) 이 이후 모든 trait 구현의 공통 골격입니다.

### 3장 — [Monoid / Semigroup](./Ch03-Monoid.md)
가장 단순한 trait 입니다. Normal World 의 두 값을 합치는 결합 (`Combine`) 과 항등원 (`Empty`). kind 가 `*` 인 Order 0 라 `K<F, A>` 마커 없이 self-bound + `static abstract` 만 먼저 익힙니다. Part 2 의 Order 1 trait 로 올라가는 디딤돌이며, 8장 Validation 의 오류 누적이 이 결합을 다시 만납니다. 첫 법칙이 등장하는 이 장에서 최소 property 검증 도구 (`ForAll`) 를 도입해, 기초의 모든 장이 법칙을 임의 입력으로 검증하는 출발점이 됩니다.

## Part 1 의 코드

본문 예제는 모두 `code/Part01-Foundations/` 에 실행 가능한 형태로 들어 있습니다 (외부 패키지 의존 0). 각 챕터는 `Traits/` · `Types/` · `Functions/` · `Tests/` · `Challenges/` · `Program.cs` 구성이며, 시그니처는 LanguageExt v5 의 공식 trait 와 정합합니다.

## 진입점

처음 읽는다면 [Ch01 — 함수형 사고로의 전환](./Ch01-Paradigm-Shift.md) 부터 시작합니다. Ch01 이 두 평행 세계 비유 + 4 가지 함수 유형을 정착시키며, 그 위에서 Ch02 의 `K<F, A>` 가 자연스럽게 등장합니다.
