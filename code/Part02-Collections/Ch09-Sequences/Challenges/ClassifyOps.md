# 챌린지 ② — 시퀀스 연산 분류

다음 LINQ / 시퀀스 메서드가 각각 *어느 trait 의 자리* 인지 분류하라.
정답을 보기 전에 시그니처만 보고 종이에 답 + 근거를 적어라.

| 메서드 | 시그니처 (개념) | 어느 trait? |
|---|---|---|
| `Select(f)` | `Seq<A> → (A → B) → Seq<B>` | ? |
| `SelectMany(f)` | `Seq<A> → (A → Seq<B>) → Seq<B>` | ? |
| `Aggregate(seed, f)` | `Seq<A> → B → (B → A → B) → B` | ? |
| `First()` | `Seq<A> → A` | ? |
| `Where(p)` | `Seq<A> → (A → bool) → Seq<A>` | ? |

---

## 정답

| 메서드 | 어느 trait? | 근거 |
|---|---|---|
| `Select(f)` | **Functor** (`Map`) | `E<A> → E<B>`, 모양 보존, 변환 함수 `A → B` 가 Normal. |
| `SelectMany(f)` | **Monad** (`Bind`) | 변환 함수가 `A → E<B>` — 출력이 Elevated. 합성 회복. |
| `Aggregate(seed, f)` | **Foldable** (`FoldLeft`) | `E<A> → B` — 끌어내림. F 가 사라진다. |
| `First()` | **Foldable** (파생) | `E<A> → A`. `FoldRight` 로 유도되는 자유 함수. |
| `Where(p)` | *trait 아님* (Functor/Monad 동사 아님) | 원소 개수를 줄인다 → 모양 보존 위반. Functor 가 아니다. (MonoidK/Alternative 의 `Empty` + `Bind` 조합으로는 표현 가능 — 11장.) |

**핵심** — `Where` 가 Functor 가 아닌 이유가 중요하다. Functor 의 `Map` 은 *원소 개수를 바꾸지 않는다* (모양 보존). 필터는 개수를 바꾸므로 별도의 도구 (11장의 `Empty` + `Bind`) 가 필요하다.
