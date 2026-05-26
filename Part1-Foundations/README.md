# Part 1 — Foundations (기초)

> *책 전체의 정체성과 결정적인 두 축 (Wlaschin 어법 + `K<F, A>` 직접 구현) 안내는 [저장소 루트 README](../README.md) 를 참조하세요. 이 문서는 **1부 자체의 배경과 목표** 에 집중합니다.*

---

## 1부의 배경

함수형 추상은 카탈로그 식 학습 으로는 손에 안 잡힙니다. `Map`, `Apply`, `Bind`, `Fold`, `Traverse` 가 서로 무관해 보이는 함수들처럼 느껴지고, 시그니처도 이름도 제각각이라 어디서 시작해 어디로 끝나는지가 안 보입니다. 그래서 한 추상씩 배우면 다음 추상이 등장할 때 같은 학습이 처음부터 다시 일어납니다.

1부의 출발점은 그 학습 방식을 바꾸는 것입니다. **8개 추상이 사실 같은 격자 위의 다른 자리** 임을 책 전체를 관통하는 한 비유 (Elevated World) 위에서 정착시킵니다. 비유를 잡으면 새 추상이 익숙한 격자의 새 자리로 보이고, 학습이 누적 됩니다.

동시에 어떻게 작동하는가도 손에 잡혀야 합니다. 1부는 학습용 trait (`Functor<F>`, `Applicative<F>`, `Foldable<F>`, `Monad<F>`, `Traversable<F>`) 을 **`K<F, A>` 마커 + self-bound + `static abstract` 의 3-tuple 패턴** 으로 직접 구현 하면서 진행합니다. 라이브러리 한 줄 호출이 아니라 그 안 작동 원리가 코드로 만져집니다.

**1부의 8개 장이 두 축 (비유 + 직접 구현) 의 교집합 위에서 자란다는 것** 이 1부의 결정적 설계입니다.

---

## 1부의 목표

1부를 마치면 독자는 다음을 갖게 됩니다.

- 왜 함수형인가의 마음가짐 + 두 평행 세계 (Normal / Elevated) 비유의 진입.
- Higher Kinds (고차 카인드) — C# 에서 Elevated World 를 하나의 어휘 (`K<F, A>`) 로 묶는 우회 + 그것을 직접 구현한 3-tuple 패턴 (자료 / 태그 / trait).
- *Functor / `map`* — Normal World 의 함수 `a → b` 를 Elevated World 의 함수 `E<a> → E<b>` 로 끌어올리는 추상 + `Functor<F>` trait 의 직접 구현.
- *Foldable / `fold`* — Elevated World 안의 구조를 Normal World 의 한 값으로 끌어내리는 추상 + `Foldable<F>` 의 직접 구현.
- *Applicative / `pure` + `apply`* — Normal World 의 다인자 함수를 Elevated World 에서 차례로 적용 + `Lift2 / Lift3 / Lift4` 의 직접 구현.
- Validation 실전 — *applicative style* 누적 vs *monadic style* 단락. 같은 도메인에 두 어법이 어떻게 다른 결과를 내는지 직접 작성한 `MyValidation` 위에서 봅니다.
- *Monad / `bind`* — 출력 타입만 Elevated 한 함수 `a → E<b>` 를 `E<a> → E<b>` 로 끌어올리는 결정타 + Kleisli 합성 `>=>` + LINQ 의 직접 구현.
- *Traversable / `traverse` / `sequence`* — 두 Elevated 세계의 층 순서를 뒤집는 추상. Functor + Foldable + Applicative 의 합성임을 직접 구현으로 본 후 일반화.

8개 추상을 시그니처만 보고도 4분면 그림으로 그릴 수 있는 능력 + 어떤 새 자료 타입에든 trait 을 직접 부착할 수 있는 능력이 1부의 도달점입니다.

---

## 1부의 무대 — 두 평행 세계

8개 장이 모두 같은 무대 위에서 진행됩니다.

| 세계 | 시민 | 의미 |
|---|---|---|
| **Elevated World** (위) | `E<a>` — `Option<a>`, `List<a>`, `Result<E, a>`, `Task<a>` … | 컨테이너에 끌어올린 값입니다. 효과 (없을 수 있음 / 여러 개 / 실패 / 비동기 / 환경 의존) 를 타입에 인코딩합니다. |
| **Normal World** (아래) | `a`, `b` — `int`, `string`, `User`, `DateTime` … | 일상의 C# 값과 함수입니다. 효과 없는 평범한 어법입니다. |

두 평행 세계의 시민 사이를 오가는 함수가 정확히 네 가지 모양 으로만 가능합니다. 함수의 입력 / 출력이 두 세계 중 어디에 있느냐가 그 형태를 결정합니다.

| 함수 시그니처 | 자리 | 처리하는 trait |
|---|---|---|
| `a → b` | Normal → Normal | (추상 불필요 — C# 의 기본 함수 합성으로 충분) |
| `a → E<b>` | Normal → Elevated | **Monad** (6장 `bind`) — 합성 회복 |
| `E<a> → b` | Elevated → Normal | **Foldable** (5장 `fold`) — 끌어내림 |
| `E<a> → E<b>` | Elevated → Elevated | **Functor** (3장 `map`) — 끌어올림 |

이 네 자리 위에 5개 trait (Functor / Foldable / Applicative / Monad / Traversable) 이 자리잡습니다. 8개 장이 모두이 네 자리 중 어느 다리를 어떻게 놓는가의 이야기입니다.

---

## 1부의 학습 흐름

| 장 | 두 축에서의 자리 | 한 줄 |
|---|---|---|
| 1장 | 축 1 의 시작 (Wlaschin 어법) | 두 평행 세계 비유 도입 + 4가지 함수 유형 격자 |
| 2장 | 축 2 의 시작 (`K<F, A>` 직접 구현) | 3-tuple 패턴 + Higher Kinds 우회 + self-bound + `static abstract` |
| 3장 | 1인자 lift | Functor / `map` — Normal 함수 한 개를 Elevated 로 끌어올림 |
| 4장 | N 인자 lift | Applicative / `pure` + `apply` — 다인자 함수의 Elevated 적용 |
| 5장 | lower | Foldable / `fold` — Elevated 의 구조를 Normal 의 한 값으로 |
| 6장 | 합성 회복 | Monad / `bind` — `a → E<b>` 의 합성 가능성 회복 + Kleisli + LINQ |
| 7장 | 실전 | Validation — applicative 누적 vs monadic 단락 |
| 8장 | 최정상 | Traversable / `traverse` — 두 Elevated 의 층 순서 뒤집기 (세 추상의 합성) |

각 장은 **목적 (왜 이런 함수가 필요한가) → 기능 (목적을 달성하는 trait 의 약속) → 예제 (목적이 달성된 자리)** 의 3부 narrative arc 로 구성됩니다.

---

## 8개 장의 구성

### 1장 — [함수형 사고로의 전환](./Ch01-Paradigm-Shift.md)

코드 이전의 마음가짐입니다. 두 평행 세계라는 비유를 도입하고, 그 사이를 오가는 4가지 함수 유형 (`a → b`, `a → E<b>`, `E<a> → b`, `E<a> → E<b>`) 의 자리를 미리 봅니다. 이 4가지가 이후 2 ~ 8장에서 등장하는 모든 추상을 분류하고 이해하는 기준이 됩니다.

### 2장 — [Higher Kinds](./Ch02-Higher-Kinds.md)

기술적 출발점입니다. `K<F, A>` 마커가 어떻게 수많은 Elevated World (Option / List / Result) 를 하나의 어휘로 묶는지 봅니다. self-bound 제약과 `static abstract` 멤버는 C# 에서 Elevated World 추상을 컴파일러가 검증 하게 만드는 도구들입니다. 3-tuple 패턴 (자료 / 태그 / trait) 이 1부 전체의 모든 trait 구현의 공통 골격입니다.

### 3장 — [Functor / map](./Ch03-Functor.md)

1인자 lift. Normal World 의 함수 `a → b` 한 개를 Elevated World 의 `E<a> → E<b>` 로 끌어올립니다. 모양은 보존하고 안의 값만 변환 합니다. 학습용 `Functor<F>` trait 을 직접 정의하고 `MyList` / `MyMaybe` 두 자료 타입에 부착해 봅니다. Functor 두 법칙 (항등 + 합성) 이 왜 끌어올림의 자연스러움을 보장 하는지 다룹니다.

### 4장 — [Applicative / pure + apply](./Ch04-Applicative.md)

N 인자 lift. Normal World 의 다인자 함수 `(a, b, …) → r` 을 Elevated World 의 `E<a> → E<b> → … → E<r>` 로 끌어올립니다. `pure` 와 `apply` 두 멤버 위에서 `Lift2 / Lift3 / Lift4` 가 자라며, 3장의 1인자 lift 가 다인자로 확장되는 자리입니다. `MyMaybe` / `MyValidation` 에 직접 부착해 applicative 누적의 시그니처를 손에 잡습니다.

### 5장 — [Foldable / fold](./Ch05-Foldable.md)

반대 방향 (lower). Elevated World 안의 구조를 Normal World 의 한 값으로 끌어내립니다. `Sum`, `Count`, `All` 의 일반화입니다. `map` 이 끌어올림 이라면 `fold` 는 끌어내림. 학습용 `Foldable<F>` trait 을 직접 정의해 어떻게 한 번의 trait 정의 + N 번의 부착으로 N×M 비용이 N+M 으로 줄어드는지를 코드로 봅니다.

### 6장 — Monad / bind / Kleisli

의존 결합의 도구입니다. 출력 타입만 Elevated 인 함수 `a → E<b>` 는 직접 합성이 안 됩니다. `bind` 가 이를 `E<a> → E<b>` 로 끌어올려 합성을 가능하게 합니다. Kleisli 합성 `>=>` 로 Elevated World 의 정식 ∘ 가 완성됩니다. LINQ `from-from-select` 가 사실이 합성의 syntactic sugar 였음을 직접 구현한 `Monad<F>` 위에서 봅니다.

### 7장 — Validation 실전

이론과 실전을 잇는 자리입니다. 같은 회원가입 검증을 *applicative style* (모든 오류 누적) 과 *monadic style* (첫 오류 단락) 두 가지로 풀어 봅니다. 4장 Applicative 와 6장 Monad 가 도구로 모두 손에 잡힌 후라 두 어법의 차이가 시그니처 단계에서 자연스럽게 드러납니다.

### 8장 — Traversable / traverse / sequence

1부의 최정상 추상도입니다. `List<E<a>>` 를 `E<List<a>>` 로 옮겨 두 Elevated 세계의 층 순서를 뒤집습니다. `traverse` 가 Functor + Foldable + Applicative 의 합성임을 봅니다. `sequence = traverse id` 의 등식이 성립합니다. 1부의 모든 도구를 동시에 동원하는 자리입니다.

---

## 1부의 진입점

처음 읽는다면 [Ch01 — 함수형 사고로의 전환](./Ch01-Paradigm-Shift.md) 부터 시작합니다. Ch01 이 두 평행 세계 비유 + 4분면 격자를 정착시키며, 그 어휘 위에서 Ch02 의 `K<F, A>` 가 자연스럽게 등장합니다.
