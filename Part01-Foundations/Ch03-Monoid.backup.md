# 3장 — Monoid / Semigroup

> 이 장에서 다룰 주제 — Normal World 의 두 값을 하나로 합치는 결합과 그 결합의 단위원을, kind 가 `*` 인 Order 0 trait 으로 정의합니다. `K<F, A>` 마커가 필요 없는 가장 단순한 trait 이라, 2장에서 본 self-bound + `static abstract` 패턴을 여기서 작은 규모로 먼저 손에 익힙니다.

## 학습 목표

- Semigroup (결합) 과 Monoid (결합 + 단위원) 의 차이를 시그니처로 구분할 수 있습니다.
- `Combine` 과 `Empty` 두 멤버를 self-bound + `static abstract` 패턴으로 직접 구현할 수 있습니다.
- 결합 법칙과 항등 법칙이 왜 빈 경우까지 안전한 누적을 보장하는지 설명할 수 있습니다.
- Monoid 가 Order 0 trait 임을, 4장 이후의 Order 1 trait (Functor 계열) 과 kind 로 구분할 수 있습니다.

---

## §3.1 목적 — 여러 값을 하나로 안전하게 합치기

Normal World 에서 같은 타입의 값 여러 개를 하나로 합치는 일은 흔합니다. 숫자 목록의 합계, 문자열 조각의 이어붙이기, 여러 로그의 누적이 모두 같은 모양입니다. 두 값을 받아 한 값을 돌려주는 함수 `a → a → a` 를 반복 적용하는 것입니다.

```csharp
// 숫자 합계
int total = 0;
foreach (var n in numbers) total = total + n;   // 0 에서 시작, + 로 합침

// 문자열 이어붙이기
string all = "";
foreach (var s in parts) all = all + s;          // "" 에서 시작, + 로 합침
```

두 코드는 타입과 합치는 연산만 다를 뿐 구조가 같습니다. 시작값 (`0`, `""`) 이 있고, 두 값을 합치는 연산 (`+`) 이 있습니다. 이 공통 구조를 추상으로 떼어내면, 합치는 방법을 한 번만 정의하고 어떤 누적에든 재사용할 수 있습니다. 그 추상이 Monoid 입니다.

핵심은 시작값의 역할입니다. 합계의 `0`, 이어붙이기의 `""` 는 단순한 초기값이 아니라 **합쳐도 상대를 바꾸지 않는 값** 입니다 (`0 + n == n`, `"" + s == s`). 이 성질 덕분에 빈 목록도 안전하게 다룰 수 있습니다. 합칠 것이 하나도 없으면 시작값을 그대로 돌려주면 됩니다.

---

## §3.2 Semigroup — 결합

먼저 합치는 연산만 떼어냅니다. 두 값을 하나로 합치는 능력을 가진 타입을 Semigroup 이라 합니다. 멤버는 결합 연산 하나입니다.

```text
Combine : a → a → a              (수학적 시그니처)
a.Combine(b)  또는  a + b         (C# 어법 — 값의 instance 메서드 또는 + 연산자)
```

같은 타입 두 값을 받아 같은 타입 한 값을 돌려줍니다. 호출 어법은 `a.Combine(b)` — 왼쪽 값 `a` 가 자기 안의 결합 능력으로 오른쪽 값 `b` 와 합쳐집니다. `a + b` 는 `Combine` 의 syntactic sugar 로 같은 결과입니다. `int` 의 덧셈, `string` 의 이어붙이기, 리스트의 연결이 모두 이 시그니처를 만족합니다.

이 연산에는 한 가지 약속이 따릅니다. **결합 법칙** 입니다. 셋 이상을 합칠 때 어느 쪽부터 묶어도 결과가 같아야 합니다.

```text
a.Combine(b).Combine(c) == a.Combine(b.Combine(c))
(a + b) + c             == a + (b + c)
```

`(1 + 2) + 3` 과 `1 + (2 + 3)` 이 같고, `("a" + "b") + "c"` 와 `"a" + ("b" + "c")` 가 같습니다. 이 약속이 있어야 여러 값을 순서만 지키면 어떻게 묶든 안심하고 합칠 수 있습니다. 앞쪽 절반과 뒤쪽 절반을 병렬로 따로 합친 뒤 마지막에 한 번 더 합쳐도 결과가 같다는 보장이 여기서 나옵니다.

Semigroup 만으로는 빈 경우를 다룰 수 없습니다. 합칠 값이 하나도 없으면 `Combine` 을 호출할 짝이 없기 때문입니다. 시작값이 빠져 있습니다.

---

## §3.3 Monoid — 단위원을 더하다

Semigroup 에 단위원 하나를 더하면 Monoid 가 됩니다. 단위원은 합쳐도 상대를 바꾸지 않는 값입니다.

```text
Combine : a → a → a     (Semigroup 에서 물려받음)
Empty   : a             (단위원)
```

`Empty` 에는 **항등 법칙** 이 따릅니다. 어느 쪽에 합쳐도 상대가 그대로 남아야 합니다.

```text
Empty.Combine(a) == a == a.Combine(Empty)
Empty + a        == a == a + Empty
```

덧셈의 `Empty` 는 `0`, 이어붙이기의 `Empty` 는 `""`, 리스트 연결의 `Empty` 는 빈 리스트입니다. 단위원이 생기면 §3.1 의 누적이 깔끔해집니다. 시작값을 `Empty` 로 두고, 목록을 돌며 `Combine` 으로 합치면, 빈 목록일 때 자연스럽게 `Empty` 가 결과가 됩니다.

```text
fold(Empty, Combine, [])        == Empty
fold(Empty, Combine, [a])       == a
fold(Empty, Combine, [a, b, c]) == Combine(Combine(Combine(Empty, a), b), c)
```

빈 목록을 특별 취급하는 분기가 사라집니다. 단위원이 빈 경우의 답을 이미 쥐고 있기 때문입니다.

---

## §3.4 trait 직접 구현 — Order 0 의 instance 어법

2장에서 본 self-bound 패턴을 Monoid 에 적용합니다. 먼저 Semigroup 입니다.

```csharp
// Order 0 trait — 값의 결합 능력
public interface Semigroup<A>
    where A : Semigroup<A>
{
    A Combine(A rhs);                                          // 값의 결합 (instance)

    // + 는 Combine 의 syntactic sugar — 모든 구현체가 자동으로 받는다.
    static virtual A operator +(A lhs, A rhs) => lhs.Combine(rhs);
}
```

핵심은 `Combine` 이 instance 메서드 (`A Combine(A rhs)`) 라는 점입니다. 호출 모양은 `lhs.Combine(rhs)` — 왼쪽 피연산자가 자기 안의 결합 능력으로 오른쪽과 합쳐집니다. `static virtual A operator +` 한 줄이 모든 Semigroup 구현체에 `+` 연산자를 자동 제공합니다. **instance method 가 기반, `+` 가 syntactic sugar** 입니다.

Monoid 는 Semigroup 을 상속하고 단위원만 더합니다.

```csharp
public interface Monoid<A> : Semigroup<A>
    where A : Monoid<A>
{
    static abstract A Empty { get; }                           // 타입의 단위원 (static)
}
```

`Empty` 는 **타입에 하나뿐인 단위원** 이라 `static abstract` 입니다. `Combine` 은 값의 능력이라 instance, `Empty` 는 타입의 능력이라 static — 같은 trait 안에서도 자리에 따라 어법이 다릅니다.

이 시그니처는 LanguageExt v5 의 `Semigroup<A>` / `Monoid<A> : Semigroup<A>` 와 정확히 정합합니다.

### 왜 Combine 은 instance, Empty 는 static 인가

수학의 대수 구조 관점에서 자연스럽습니다.

- `Combine : a → a → a` — 원소 둘 사이의 연산. 값 `a` 자체가 결합 능력의 주인입니다 (`(ℤ, +)` 의 `5 ∈ ℤ` 가 덧셈 능력의 주인).
- `Empty : a` — 타입의 한 자리에 박힌 단위원. 어느 특정 원소가 아니라 타입 전체에 한 개 있는 값입니다 (덧셈의 `0`, 곱셈의 `1`).

instance method 는 원소가 가진 능력, static 멤버는 타입이 가진 능력 — 어법이 의미를 따라갑니다. 또 `lhs.Combine(rhs)` 의 자연스러운 sugar 인 `lhs + rhs` 가 Haskell 의 `<>` 어법 (`a <> b`) 을 C# 으로 그대로 옮긴 자리이기도 합니다.

### `K<F, A>` 마커가 없는 이유

2장의 Functor 와 비교하면 결정적 차이가 하나 보입니다. **여기에는 `K<F, A>` 마커가 없습니다.** Monoid 는 완성 타입 하나 (`int`, `string`) 에 대한 추상이라 kind 가 `*` 인 Order 0 trait 입니다. 끌어올릴 컨테이너 `E<a>` 가 없으니 `K<F, A>` 도 필요 없습니다. 그래서 self-bound 패턴의 가장 단순한 형태가 됩니다. 타입 인자가 `A` 하나뿐이고, 그마저 완성 타입입니다.

이 단순함이 3장이 4장 앞에 오는 이유입니다. self-bound 패턴을 컨테이너 없는 자리에서 먼저 익히고, 4장에서 같은 패턴에 `K<F, A>` 마커를 더해 Order 1 trait (Functor) 로 올라갑니다.

---

## §3.5 예제 — Sum / Product / Concat

세 가지 완성 타입에 Monoid 를 부착합니다. 같은 `int` 라도 합치는 방법이 다르면 다른 Monoid 입니다.

```csharp
// 덧셈 Monoid — Empty 는 0
public readonly record struct Sum(int Value) : Monoid<Sum>
{
    public static Sum Empty => new(0);
    public Sum Combine(Sum rhs) => new(Value + rhs.Value);
    public static Sum operator +(Sum lhs, Sum rhs) => lhs.Combine(rhs);
}

// 곱셈 Monoid — Empty 는 1
public readonly record struct Product(int Value) : Monoid<Product>
{
    public static Product Empty => new(1);
    public Product Combine(Product rhs) => new(Value * rhs.Value);
    public static Product operator +(Product lhs, Product rhs) => lhs.Combine(rhs);
}

// 문자열 이어붙이기 Monoid — Empty 는 ""
public readonly record struct Concat(string Value) : Monoid<Concat>
{
    public static Concat Empty => new("");
    public Concat Combine(Concat rhs) => new(Value + rhs.Value);
    public static Concat operator +(Concat lhs, Concat rhs) => lhs.Combine(rhs);
}
```

`Sum` 과 `Product` 는 같은 `int` 를 감싸지만 단위원이 다릅니다. 덧셈의 단위원은 `0`, 곱셈의 단위원은 `1` 입니다. 어떤 값이 단위원인가는 어떤 연산으로 합치는가가 결정합니다.

각 타입이 자기 안 에 결합 연산을 들고 다니므로 호출 어법이 자연스럽게 두 가지입니다.

```csharp
new Sum(1).Combine(new Sum(2))                     // Sum(3)
new Sum(1) + new Sum(2)                            // Sum(3) — 같은 결과
new Sum(1) + new Sum(2) + new Sum(3)               // Sum(6) — 좌결합 자동
```

Haskell 의 `<>` (`a <> b <> c`) 어법이 C# 의 `+` 어법으로 그대로 옮겨오는 자리입니다. 함수형의 이항 연산자가 OO 의 *instance 메서드 + operator overload* 안에 자연스럽게 살아 있습니다. 구체 타입에 명시한 `operator +` 한 줄이 `Combine` 의 sugar 로 작동합니다.

> **세부 — `static virtual operator +` 와 명시 `operator +`** — Semigroup `<A>` 의 `static virtual A operator +` default 는 *generic 제약 context 안에서만* 직접 호출됩니다 (예: `where M : Semigroup<M>` 안의 `a + b`). 구체 타입 `Sum + Sum` 직접 호출이 가능하려면 각 자료 타입에 `public static Sum operator +` 한 줄을 명시해 default 를 override 해야 합니다. v5 의 default 발상은 보존하되 구체 타입 호출의 편의성을 더한 자리입니다.

세 Monoid 를 같은 누적 함수 하나로 접습니다.

```csharp
// 어떤 Monoid M 이든 목록을 하나로 접는 자유 함수
public static M FoldAll<M>(IEnumerable<M> items)
    where M : Monoid<M>
{
    var acc = M.Empty;                       // 타입의 단위원 (static)
    foreach (var x in items)
        acc = acc.Combine(x);                // 값의 결합 능력 (instance)
    return acc;
}
```

```csharp
FoldAll(new[] { new Sum(1), new Sum(2), new Sum(3) });        // Sum(6)
FoldAll(new[] { new Product(2), new Product(3) });            // Product(6)
FoldAll(new[] { new Concat("a"), new Concat("b") });          // Concat("ab")
FoldAll(Array.Empty<Sum>());                                  // Sum(0) — 빈 목록도 안전합니다.
```

`M.Empty` 는 타입의 능력으로 호출 (`M.Empty`), `acc.Combine(x)` 는 값의 능력으로 호출. 두 어법이 한 함수 안에서 자기 자리에 정확히 놓입니다. `FoldAll` 은 어떤 Monoid 인지 모릅니다. `Empty` 에서 시작해 `Combine` 으로 합칠 뿐입니다. trait 한 개를 만족하면 이 누적이 공짜로 따라옵니다.

---

## §3.6 두 법칙 — 결합 + 항등

Monoid 가 진짜 Monoid 이려면 두 가지 법칙을 지켜야 합니다. 시그니처만으로는 강제되지 않고, 구현이 약속하는 성질입니다.

**첫 번째 법칙 — 결합 법칙.** 셋 이상을 합칠 때 묶는 순서가 결과를 바꾸지 않습니다.

```text
a.Combine(b).Combine(c) == a.Combine(b.Combine(c))
(a + b) + c             == a + (b + c)
```

**두 번째 법칙 — 항등 법칙.** 단위원은 어느 쪽에 합쳐도 상대를 바꾸지 않습니다.

```text
Empty.Combine(a) == a == a.Combine(Empty)
Empty + a        == a == a + Empty
```

두 법칙이 함께 성립하면 `FoldAll` 이 어떻게 묶든, 빈 경우가 와도 항상 같은 답을 낸다는 보장이 생깁니다. 결합 법칙이 순서의 자유를, 항등 법칙이 빈 경우의 안전을 책임집니다. 이 둘은 함수형 코드가 큰 입력을 병렬로 쪼개 따로 합친 뒤 마지막에 한 번 더 합치는 최적화의 근거이기도 합니다.

> **흔한 함정** — 평균 (average) 은 Monoid 가 아닙니다. `average(average(a, b), c)` 와 `average(a, average(b, c))` 가 다르기 때문입니다. 결합 법칙이 깨지면 Monoid 가 아닙니다. 합치는 연산이 항상 Monoid 인 것은 아니라는 점을 기억합니다.

---

## §3.7 Order 0 의 instance, Order 1 의 static abstract — 어법은 추상 거리를 따라간다

Monoid 와 다음 장의 Functor 를 어법으로 나란히 두면 두 trait 의 자리가 또렷해집니다.

| trait | kind | 능력의 주인 | C# 어법 | `K<F, A>` |
|---|---|---|---|---|
| **Semigroup / Monoid** (3장) | `*` (Order 0) | 값 자체 (`5`, `"hello"`) | instance `a.Combine(b)` + operator `+` | 불필요 |
| **Functor / Foldable / Monad / Traversable** (4~9장) | `* → *` (Order 1) | 컨테이너 모양 (`MyList`, `MyMaybe`) | static abstract `F.Map(...)` / `F.Bind(...)` | 필요 |

**Order 0 trait 의 능력의 주인은 값입니다.** `5` 가 자기 안에 결합 능력을 들고 다니고, `"hello"` 가 자기 안에 이어붙이기 능력을 들고 다닙니다. 원소가 가진 능력이라 instance 어법이 자연스럽습니다.

**Order 1 trait 의 능력의 주인은 컨테이너 모양입니다.** `MyList<int>` 라는 특정 값 이 Map 능력을 들고 있는 게 아니라 *MyList 라는 컨테이너 종류* 가 Map 을 어떻게 하는지 정의합니다. 타입이 가진 능력이라 static abstract 어법이 자연스럽습니다.

같은 self-bound 패턴 안에서 추상의 자리가 다르면 어법이 다릅니다. 4장 이후 모든 trait 이 `static abstract` 인 이유는 컨테이너 위 추상이라 타입의 능력으로 정의되어야 하기 때문이고, 3장의 Monoid 만 instance + operator `+` 인 이유는 Order 0 의 값의 능력이라 instance 어법이 자연스럽기 때문입니다.

이 어법 비대칭은 LanguageExt v5 의 정통 어법이고, 함수형의 세 동기가 한 자리에 모인 설계입니다.

- (a) **Haskell `<>` 어법 보존** — `a <> b` 가 C# 의 `a + b` 로.
- (b) **대수 구조 정합** — 원소가 가진 능력 (`(ℤ, +)` 의 `5 ∈ ℤ` 가 결합 능력의 주인).
- (c) **`+` 연산자와의 1:1 짝지음** — `lhs + rhs` 의 자연스러운 구현이 `lhs.Combine(rhs)`.

3장의 Order 0 어법에 4장에서 `K<F, A>` 마커 한 겹과 static dispatch 가 더해지면 Order 1 어법으로 올라갑니다. Monoid 가 self-bound 패턴의 컨테이너 없는 원형이라면, Functor 는 거기에 컨테이너 모양 + static dispatch 를 씌운 첫 사례입니다.

이 결합 연산은 1부 뒤에서 두 번 회수됩니다. 8장 Validation 에서 여러 오류를 누적하는 자리가 오류 리스트의 Monoid 결합이고, 3부 Writer (17장) 에서 로그를 누적하는 자리가 출력 `W` 의 Monoid 결합입니다. 2부에서는 같은 결합을 Elevated World 로 끌어올린 `SemigroupK` / `MonoidK` (13장) 를 만납니다.

---

## §3.8 Q&A

> **Q1. Semigroup 과 Monoid 의 차이는 무엇입니까?**

Semigroup 은 결합 연산 `Combine` 만 가집니다. Monoid 는 거기에 단위원 `Empty` 를 더한 것입니다. 차이는 빈 경우를 다룰 수 있는가입니다. Semigroup 은 합칠 값이 최소 하나는 있어야 하지만, Monoid 는 `Empty` 가 빈 경우의 답을 쥐고 있어 값이 하나도 없어도 안전합니다.

> **Q2. 왜 Monoid 에는 `K<F, A>` 마커가 없습니까?**

Monoid 는 `int` 나 `string` 같은 완성 타입에 직접 붙는 Order 0 trait 이기 때문입니다 (kind `*`). `K<F, A>` 마커는 `MyList<a>` 처럼 컨테이너 (Order 1, kind `* → *`) 의 안쪽 값을 C# 에서 가리키기 위한 우회였습니다. Monoid 에는 끌어올릴 컨테이너가 없으니 마커도 필요 없습니다.

> **Q3. 같은 `int` 인데 왜 `Sum` 과 `Product` 를 따로 만듭니까?**

`int` 를 합치는 방법이 하나가 아니기 때문입니다. 덧셈으로 합치면 단위원이 `0`, 곱셈으로 합치면 단위원이 `1` 입니다. 어떤 연산으로 합치는가가 Monoid 를 결정하므로, 같은 타입이라도 연산이 다르면 다른 Monoid 입니다. C# 에서는 `Sum` / `Product` 같은 래퍼 타입으로 둘을 구분합니다.

> **Q4. 평균이나 뺄셈은 왜 Monoid 가 아닙니까?**

결합 법칙이 깨지기 때문입니다. 뺄셈은 `(a - b) - c` 와 `a - (b - c)` 가 다르고, 평균도 묶는 순서에 따라 결과가 달라집니다. `Combine` 의 시그니처 `a → a → a` 는 만족해도, 결합 법칙을 지키지 못하면 Monoid 가 아닙니다. 시그니처는 필요조건이지 충분조건이 아닙니다.

> **Q5. Monoid 가 1부의 다른 trait 과 어떻게 이어집니까?**

self-bound 패턴을 컨테이너 없이 먼저 익히는 자리입니다. 4장 Functor 는 같은 self-bound 패턴에 `K<F, A>` 마커와 static dispatch 를 더해 Order 1 로 올라갑니다. 또 Monoid 의 결합은 8장 Validation 의 오류 누적과 3부 Writer (17장) 의 로그 누적으로 회수되고, 2부의 `SemigroupK` / `MonoidK` (13장) 로 Elevated World 까지 끌어올려집니다.

> **Q6. 왜 Semigroup / Monoid 만 instance + operator 이고, Functor 부터는 static abstract 입니까?**

추상의 자리가 달라서입니다. Semigroup / Monoid 는 Order 0 trait — 값 자체가 결합 능력의 주인입니다. `5 + 3` 처럼 값과 값 사이의 연산이라 instance 어법이 자연스럽고, `+` 연산자가 그 syntactic sugar 입니다. Functor 부터는 Order 1 trait — 컨테이너 모양의 능력이라 컨테이너 자체 (타입) 가 능력의 주인이 됩니다. `MyList<int>.Map(...)` 의 `Map` 은 특정 리스트가 가진 능력이 아니라 *MyList 라는 모양이 어떻게 lift 하는가* 의 정의이므로 static dispatch 가 자연스럽습니다. LanguageExt v5 도 같은 비대칭을 따릅니다 — Order 가 어법을 결정합니다.

---

## §3.9 요약

- Semigroup 은 결합 연산을 instance 메서드 (`a.Combine(b)`) 로 가진 trait입니다. `static virtual operator +` 한 줄이 모든 구현체에 `+` 연산자를 syntactic sugar 로 자동 제공합니다. 결합 법칙을 약속.
- Monoid 는 Semigroup 에 단위원 `Empty` 를 더합니다. `Empty` 는 타입의 단위원 이라 static, `Combine` 은 값의 결합 이라 instance — 같은 trait 안에서도 어법이 의미를 따라갑니다.
- Monoid 는 완성 타입에 붙는 Order 0 trait (kind `*`) 이라 `K<F, A>` 마커가 필요 없습니다. self-bound 패턴의 가장 단순한 형태입니다.
- Order 0 의 instance + operator 어법은 4장 이후 Order 1 trait 의 static abstract 어법과 의도된 비대칭 입니다. 추상의 자리 (값 vs 컨테이너) 에 어법이 맞춰지고, LanguageExt v5 의 정통 어법과 정합합니다.
- 같은 타입이라도 합치는 연산이 다르면 다른 Monoid 입니다 (`Sum` vs `Product`).
- 이 결합은 8장 Validation 의 오류 누적, 3부 Writer (17장) 의 로그 누적, 2부 `MonoidK` (13장) 로 1부 전체에 회수됩니다.
