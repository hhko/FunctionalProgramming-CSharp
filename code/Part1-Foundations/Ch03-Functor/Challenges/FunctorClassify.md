# 챌린지 ② 정답 — Functor 인지 아닌지 분류

[본문 §3.9.2](../../../../Part1-Foundations/Ch03-Functor.md#392-챌린지--functor-인지-아닌지-분류하기-필수) 의 다섯 후보에 대한 정답과 근거.

먼저 *본인이 종이에 적은 답* 과 비교한다. 정답이 맞아도 *근거가 같은 자리를 짚었는지* 가 중요하다.

---

## 판단 기준 (요약)

본문 §3.8.4 + §3.2.3 의 정의를 기반으로 네 가지를 차례로 묻는다.

1. *컨테이너 종류* 가 입출력에서 같은가?
2. *원소 개수* 가 입출력에서 같은가?
3. *각 원소에 변환 함수 `f : a → b`* 가 적용되는가? (시그니처에 `f` 인자가 등장하는가)
4. 시그니처 자체가 *`E<a> → E<b>`* 모양인가, 아니면 *③ `E<a> → b`* / *② `a → E<b>`* / *기타* 인가?

네 기준 모두 *예* 면 Functor 의 `Map`. 하나라도 *아니오* 면 *어느 trait* 의 자리를 §3.8.2 표에서 찾는다.

---

## (a) `Reverse : List<A> → List<A>`

**정답: ✗ Functor 의 `Map` 아님**

| 기준 | 판정 |
|---|---|
| 컨테이너 종류 같은가? | ✓ List → List |
| 원소 개수 같은가? | ✓ 길이 보존 |
| 변환 함수 `f` 가 시그니처에 있는가? | **✗ 없음** |
| `E<a> → E<b>` 모양인가? | 시그니처는 비슷하지만 *변환 함수가 없으니 `Map` 의 자리가 아님* |

**근거** — `Reverse` 의 시그니처는 `List<A> → List<A>` 로 *Functor 의 `Map` 시그니처와 닮았지만 변환 함수 `f` 인자가 없다*. 즉 *원소를 변환하는 함수* 가 없는 자리는 `Map` 이 아니다. `Reverse` 는 *원소를 다른 위치로 이동* 시키는 *순서 변경 연산* 으로, *Functor 의 모양 보존* (원소가 *제자리에서* 변환) 약속과도 다르다.

**자리** — 표준 trait 으로 분류하기 어려운 *Sequence 특화 연산*. 본문 §3.8.2 표의 *(컨테이너 특화)* 행에 해당.

---

## (b) `Distinct : List<A> → List<A>`

**정답: ✗ Functor 의 `Map` 아님**

| 기준 | 판정 |
|---|---|
| 컨테이너 종류 같은가? | ✓ List → List |
| 원소 개수 같은가? | **✗ 중복 제거로 줄어듦** |
| 변환 함수 `f` 가 시그니처에 있는가? | ✗ 없음 |
| `E<a> → E<b>` 모양인가? | 모양은 비슷하지만 *원소 개수 보존 위반* |

**근거** — `Distinct` 는 *원소 개수* 가 감소한다 (중복 제거). 본문 §3.8.1 표의 *Filter 행* 과 같은 사유 — *컨테이너 종류는 같지만 원소 개수가 줄어든다*.

**자리** — `Filter` 와 같은 자리. *Filterable* (9장).

---

## (c) `Average : List<int> → double`

**정답: ✗ Functor 의 `Map` 아님**

| 기준 | 판정 |
|---|---|
| 컨테이너 종류 같은가? | **✗ List → (없음)** |
| 원소 개수 같은가? | ✗ N → 1 |
| 변환 함수 `f` 가 시그니처에 있는가? | ✗ 없음 |
| `E<a> → E<b>` 모양인가? | **✗ ③ 유형 `E<a> → b`** |

**근거** — 시그니처가 `List<int> → double` 로 *Elevated 입력 → Normal 출력*. 1장의 *③ 유형* (`E<a> → b`) 정확히. *컨테이너가 사라지고 한 값으로 압축* 되는 패턴은 `fold` 의 자리.

**자리** — *Foldable* (4장).

---

## (d) `Lookup : int → Option<User>`

**정답: ✗ Functor 의 `Map` 아님**

| 기준 | 판정 |
|---|---|
| 컨테이너 종류 같은가? | **✗ (없음) → Option** |
| 원소 개수 같은가? | ✗ (입력에 컨테이너 자체가 없음) |
| 변환 함수 `f` 가 시그니처에 있는가? | (시그니처 형태 자체가 다름) |
| `E<a> → E<b>` 모양인가? | **✗ ② 유형 `a → E<b>`** |

**근거** — 시그니처가 `int → Option<User>` 로 *Normal 입력 → Elevated 출력*. 1장의 *② 유형* (`a → E<b>`) — *World-crossing function*. Functor 의 `Map` 은 *④ 유형* 만 만든다. ② 유형의 합성은 `bind` 가 푼다 (본문 §3.8 끝의 8장 §6.2.3 참조).

**자리** — *Monad 의 `bind`* (8장).

---

## (e) `Replace : (List<A>, A) → List<A>`

**정답: ✗ Functor 의 `Map` 자체는 아니지만, *Functor 위에 자라난 파생 함수*. 즉 *Map 을 호출해 구현 가능*.**

| 기준 | 판정 |
|---|---|
| 컨테이너 종류 같은가? | ✓ List → List |
| 원소 개수 같은가? | ✓ 같음 |
| 변환 함수 `f` 가 시그니처에 있는가? | **✗ `f` 자리에 *값* `A` 가 있음** |
| `E<a> → E<b>` 모양인가? | 시그니처가 *2 인자* 라 `Map` 과 다름 |

**근거** — `Replace(xs, newValue)` 가 *모든 원소를 `newValue` 로 교체* 한다고 보면 — 이는 *상수 람다 `_ => newValue`* 를 `Map` 에 넘기는 것과 같다.

```csharp
public static K<F, A> Replace<F, A>(K<F, A> fa, A newValue)
    where F : Functor<F>
=>
    F.Map<A, A>(_ => newValue, fa);
```

즉 `Replace` 는 *Functor 의 `Map` 위에 자라난 파생 함수* 다. 본문 §3.5.3 의 `MapTwice` / `MapThenMap` 과 같은 *trait 위의 일반 함수* 자리.

**자리** — *Functor 의 `Map` 자체* 는 아니지만 *Functor 위에 자유롭게 정의 가능*. 분류상으로는 *Functor 의 파생 함수* 로 본다.

---

## 종합

| 후보 | Functor 의 Map? | 자리 |
|---|---|---|
| (a) Reverse | ✗ | Sequence 특화 (Filterable / Foldable / Functor 모두 아님) |
| (b) Distinct | ✗ | Filterable (9장) |
| (c) Average | ✗ | Foldable / ③ 유형 (4장) |
| (d) Lookup | ✗ | Monad bind / ② 유형 (8장) |
| (e) Replace | ✗ (그러나 Map 으로 정의됨) | Functor 의 파생 함수 (§3.5.3) |

다섯 후보 모두 `Map` 자체는 아니지만 *왜 아닌가* 의 근거가 *각자 다른 자리* 에 있다. 이 *각자 다른 자리* 가 1부의 8 개 trait (Functor / Foldable / Applicative / Traversable / Monad / …) 가 *왜 따로 존재해야 하는가* 의 답이다.

---

## 본문 어디로 돌아가야 하는가 (오답별 가이드)

- (a) Reverse 가 막혔다면 — §3.2.1 의 `Map` 시그니처 (`Func<A, B> f` 가 *인자* 라는 점) 와 §3.2.3 의 ③ container operation 시각을 다시 본다.
- (b) Distinct 가 막혔다면 — §3.8.1 표의 *Filter / Take* 행과 §3.8.4 *좁은 약속* 을 다시 본다.
- (c) Average 가 막혔다면 — §3.1.2 의 ③ 유형 정의를 본다.
- (d) Lookup 이 막혔다면 — §3.1.2 의 ② 유형 정의 + §3.8 끝의 8장 §6.2.3 미리보기를 본다.
- (e) Replace 가 막혔다면 — §3.5 *어떤 Functor 든 받는 일반 함수* 절을 본다. *`Map` 위에 함수를 자라게 하는 패턴*.
