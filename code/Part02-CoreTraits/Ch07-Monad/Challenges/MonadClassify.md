# 챌린지 ④ 정답 — 다섯 시그니처가 어느 trait 자리인지 분류

본문 §7.11.4 의 다섯 후보에 대한 정답과 근거.

먼저 **본인이 종이에 적은 답** 과 비교한다. 정답이 맞아도 **근거가 같은 자리를 짚었는지** 가 중요하다.

---

## 판단 기준 (요약)

본문 §7.10 경계표 + §7.2.1 `bind` 시그니처를 기반으로 두 가지를 차례로 묻는다.

1. 입력이 **Normal `a`** 이고 출력이 **Elevated `E<b>`** 인가? 그렇다면 World-crossing `a → E<b>` 로 **`bind` 의 자리**.
2. 입력·출력이 모두 Elevated (`E<a> → E<b>`) 면 **끌어올림** (Functor / Applicative), 출력만 Normal (`E<a> → b`) 이면 **끌어내림** (Foldable), 둘 다 Normal (`a → b`) 이면 추상 불필요한 평범한 함수.

`bind` 의 둘째 인자가 `a → E<b>` 라는 점이 핵심이다. 입력이 Elevated 거나 출력이 Normal 이면 `bind` 의 자리가 아니다.

---

## (a) `Parse : string → MyMaybe<int>`

**정답: ✓ `bind` 의 자리 (`a → E<b>`)**

| 기준 | 판정 |
|---|---|
| 입력이 Normal `a` 인가? | ✓ `string` |
| 출력이 Elevated `E<b>` 인가? | ✓ `MyMaybe<int>` |
| 시그니처 유형 | `a → E<b>` |

**근거** — 입력 `string` 은 Normal, 출력 `MyMaybe<int>` 는 Elevated. Normal 에서 출발해 Elevated 로 건너가는 World-crossing 함수다. 파싱은 실패할 수 있어 결과가 `MyMaybe` 이고, 이런 함수끼리의 합성을 되살리는 것이 `bind` 다.

**자리** — Monad 의 `bind` (7장). `Parse.Then(...)` 으로 다음 World-crossing 함수와 이을 수 있다.

---

## (b) `MapAge : E<a> → E<b>`

**정답: ✗ `bind` 의 자리 아님**

| 기준 | 판정 |
|---|---|
| 입력이 Normal `a` 인가? | **✗ Elevated `E<a>`** |
| 출력이 Elevated `E<b>` 인가? | ✓ `E<b>` |
| 시그니처 유형 | `E<a> → E<b>` |

**근거** — 입력이 Normal `a` 가 아니라 이미 Elevated `E<a>` 다. `bind` 의 둘째 인자는 `a → E<b>` 로 입력이 Normal 이어야 하므로 자리가 어긋난다. 컨테이너를 열지 않고 안의 값만 `a → b` 로 바꾸는 끌어올림이다.

**자리** — Functor 의 `map` (4장). 입력·출력 컨테이너가 같고 안의 값만 변환된다.

---

## (c) `Total : E<a> → b`

**정답: ✗ `bind` 의 자리 아님**

| 기준 | 판정 |
|---|---|
| 입력이 Normal `a` 인가? | **✗ Elevated `E<a>`** |
| 출력이 Elevated `E<b>` 인가? | **✗ Normal `b`** |
| 시그니처 유형 | `E<a> → b` |

**근거** — 입력은 Elevated `E<a>`, 출력은 Normal `b`. 컨테이너를 열어 한 값으로 압축하는 끌어내림이다. `bind` 는 결과가 다시 Elevated `E<b>` 인데, 여기는 Normal `b` 로 내려온다.

**자리** — Foldable 의 `fold` (6장). Elevated 를 Normal 의 한 값으로 끌어내린다.

---

## (d) `Apply : E<a → b> → E<a> → E<b>`

**정답: ✗ `bind` 의 자리 아님**

| 기준 | 판정 |
|---|---|
| 입력이 Normal `a` 인가? | **✗ 첫 인자가 Elevated 함수 `E<a → b>`** |
| 둘째 인자가 함수 `a → E<b>` 인가? | **✗ 값이 든 컨테이너 `E<a>`** |
| 시그니처 유형 | `E<a → b> → E<a> → E<b>` |

**근거** — 첫 인자가 이미 컨테이너 안에 든 함수 `E<a → b>` 이고, 둘째 인자도 함수가 아니라 값이 든 컨테이너 `E<a>` 다. 두 Elevated 값이 서로를 모르는 채 결합하는 독립 결합이다. `bind` 의 둘째 인자는 `a → E<b>` 함수라 앞에서 꺼낸 값으로 다음 효과를 만드는 의존 결합이고, 이 점이 둘을 가른다 (§7.4).

**자리** — Applicative 의 `apply` (5장). 독립 결합 (병렬).

---

## (e) `Double : a → b`

**정답: ✗ `bind` 의 자리 아님 — 끌어올림 대상인 평범한 함수**

| 기준 | 판정 |
|---|---|
| 입력이 Normal `a` 인가? | ✓ `a` |
| 출력이 Elevated `E<b>` 인가? | **✗ Normal `b`** |
| 시그니처 유형 | `a → b` |

**근거** — 입력도 출력도 모두 Normal 이다. 두 세계를 건너지 않으므로 함수형 추상이 필요 없는 평범한 함수다. 이 `a → b` 가 바로 Functor 의 `map` 이 Elevated 로 끌어올리는 대상이다. `bind` 가 다루는 `a → E<b>` 와는 출력이 Elevated 인지 아닌지로 갈린다.

**자리** — 추상 불필요. Functor 의 `map` 이 끌어올리는 평범한 함수 (`a → b`).

---

## 종합

| 후보 | 시그니처 | `bind` 자리? | 자리 |
|---|---|---|---|
| (a) Parse | `string → MyMaybe<int>` | ✓ | Monad 의 `bind` / `a → E<b>` (7장) |
| (b) MapAge | `E<a> → E<b>` | ✗ | Functor 의 `map` (4장) |
| (c) Total | `E<a> → b` | ✗ | Foldable 의 `fold` (6장) |
| (d) Apply | `E<a → b> → E<a> → E<b>` | ✗ | Applicative 의 `apply` (5장) |
| (e) Double | `a → b` | ✗ | 끌어올림 대상 (추상 불필요) |

다섯 후보 중 `bind` 의 자리는 (a) 하나뿐이다. 나머지 넷이 **왜 아닌가** 의 근거가 **각자 다른 자리** 에 있다. 입력이 Elevated 인가, 출력이 Normal 인가, 둘째 인자가 함수인가 값인가. 이 차이가 1부의 네 자리 (`a → b` / `a → E<b>` / `E<a> → b` / `E<a> → E<b>`) 를 가른다.

---

## 본문 어디로 돌아가야 하는가 (오답별 가이드)

- (a) Parse 가 막혔다면 — §7.1.2 의 `a → E<b>` 유형 정의와 §7.2.1 의 `bind` 시그니처를 다시 본다.
- (b) MapAge 가 막혔다면 — §7.10 경계표의 `Map` 행 (`E<a> → E<b>`, Functor) 을 다시 본다.
- (c) Total 이 막혔다면 — §7.10 경계표의 `Sum` / `Count` 행 (`E<a> → b`, Foldable) 을 다시 본다.
- (d) Apply 가 막혔다면 — §7.4 의 독립 결합 vs 의존 결합, §7.10 경계표의 `Apply` 행을 다시 본다.
- (e) Double 이 막혔다면 — §7.10 경계표의 `a → b` 행 (입력·출력 모두 Normal) 을 다시 본다.
