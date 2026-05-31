# 3장. Monoid / Semigroup (Normal World 의 결합)

> **이 장의 주제**. 1장의 Normal World 에서 같은 타입의 값 둘을 한 값으로 합치는 결합과 그 결합의 단위원을 다룹니다. 합성 가능한 Elevated World 로 lift 의 **결합** 어휘가 가장 단순한 자리에서 등장하는 곳입니다. 추상의 자리는 컨테이너 없는 완성 타입 (`int`, `string`) 이라 kind 가 `*` 인 Order 0 trait 이고, 2장의 `K<F, A>` 마커가 필요 없는 가장 단순한 형태로 self-bound + `static abstract` 패턴을 손에 익힙니다.

> 이 장을 마치면 할 수 있게 되는 것
> - [ ] Semigroup (결합) 과 Monoid (결합 + 단위원) 의 차이를 시그니처 한 줄로 구분할 수 있습니다.
> - [ ] 결합 법칙과 항등 법칙이 시그니처가 약속 못 하는 두 자리임을 설명할 수 있습니다.
> - [ ] 같은 `int` 인데 `Sum` 과 `Product` 가 왜 서로 다른 Monoid 인지 답할 수 있습니다.
> - [ ] `Combine` 이 왜 instance, `Empty` 가 왜 `static abstract` 인지 어법 차이를 설명할 수 있습니다.
> - [ ] `K<F, A>` 마커가 Monoid 에는 왜 필요 없는지 kind 어휘로 답할 수 있습니다.
> - [ ] 어떤 Monoid 든 받는 일반 함수 (`FoldAll`) 를 한 번 정의하면 모든 Monoid 인스턴스에 자동 적용된다는 ROI 를 코드로 보여줄 수 있습니다.
> - [ ] 평균 / 뺄셈이 왜 Monoid 가 아닌지 시그니처가 아닌 법칙으로 판별할 수 있습니다.
> - [ ] Ch03 의 결합이 8장 Validation 의 오류 누적과 3부 Writer 의 로그 누적에서 어떻게 회수되는지 짚을 수 있습니다.

---

## §3.1 1장 / 2장 비유 회복 — Ch03 의 자리

1장에서 두 평행 세계 (Normal World 와 Elevated World) 를 봤고, 2장에서 Elevated World 의 수많은 시민을 하나의 어휘 (`K<F, A>`) 로 묶는 우회와 self-bound + `static abstract` 두 도구를 봤습니다. 이제 3장의 자리를 두 어휘로 짚습니다.

Monoid 는 **Normal World 안의 결합** 입니다. Elevated World 의 컨테이너 (`Option<a>`, `List<a>`) 가 아닌, 완성 타입 (`int`, `string`) 두 값을 한 값으로 합치는 자리입니다. 1장의 4 분면 격자 (`a → b` / `a → E<b>` / `E<a> → b` / `E<a> → E<b>`) 와 비교하면 Monoid 의 시그니처는 다음과 같습니다.

```text
Combine : a → a → a              (Normal → Normal → Normal)
```

세 자리 모두 Normal World 의 시민입니다. 1장의 4 분면 격자에 비추면 `a → b` 의 한 변형으로, **자연 합성** 자리에 살고 *World-crossing* 어휘가 등장하지 않습니다. 4 분면 격자 **바깥** 의 Order 0 자리라는 뜻이고, 4 분면의 추상 (Functor / Foldable / Monad / Traversable) 과는 결의 다른 trait 입니다.

| 자리 | trait | kind | 사는 World | 4 분면 격자 |
|---|---|---|---|---|
| Order 0 | **Monoid / Semigroup** (3 장) | `*` | Normal World 안 | 격자 **밖** |
| Order 1 | Functor / Foldable / Monad / Traversable (4 ~ 9 장) | `* → *` | Elevated World 위 | 격자 **위** |

이 자리 차이가 3 장의 단순함을 결정합니다. 2 장에서 정착시킨 `K<F, A>` 마커는 **Elevated World 의 컨테이너 자체를 가리키는 어휘** 였습니다. Monoid 는 컨테이너가 없으니 마커도 필요 없습니다. 자료 / 태그 / trait 3-tuple 패턴도 단순화됩니다 (자료가 trait 을 직접 구현, 태그 없음). self-bound + `static abstract` 두 도구만 **컨테이너 없는 가장 단순한 형태** 로 손에 익히는 자리입니다.

Ch03 의 자리를 한 줄로 정리합니다. **Normal World 의 결합을 trait 으로 정의한 Order 0 trait, 4 장 이후 Order 1 trait 으로 올라가는 디딤돌**. 격자 바깥의 자리이지만 1 부 전체에서 두 번 회수됩니다. 8 장 Validation 의 오류 누적, 3 부 Writer 의 로그 누적 모두 Monoid 의 결합으로 작동합니다 (§3.11 에서 짚습니다).

---

## §3.2 문제 배경 — 일상의 결합과 같은 코드의 반복

Normal World 에서 같은 타입의 값 여러 개를 하나로 합치는 일은 매일의 코드입니다. 숫자 목록의 합계, 문자열 조각의 이어붙이기, 여러 로그의 누적, 도형의 면적 합산, 유효성 검증의 결과 합치기가 모두 같은 모양입니다. 두 값을 받아 한 값을 돌려주는 함수 `a → a → a` 를 반복 적용하는 자리입니다.

명령형 어법으로 두 코드를 적어 봅니다.

```csharp
// 숫자 목록의 합계
int total = 0;
foreach (var n in numbers)
    total = total + n;                       // 0 에서 시작, + 로 합침

// 문자열 조각의 이어붙이기
string all = "";
foreach (var s in parts)
    all = all + s;                           // "" 에서 시작, + 로 합침
```

두 코드는 타입 (`int` / `string`) 과 합치는 연산 (`+` 의 두 가지 의미) 만 다를 뿐 **구조가 동일** 합니다. 시작값 (`0`, `""`) 이 있고, 두 값을 합치는 연산 (`+`) 이 있고, 시작값에서 출발해 차례로 합치는 누적이 있습니다. 두 코드가 같은 패턴을 **복사 + 타입만 교체** 한 자리입니다.

복사가 한 번이면 별 문제가 없습니다. 그런데 같은 패턴이 어디서 또 나타나는지 보면 함수형의 결정적 발상이 등장하는 자리가 잡힙니다.

```csharp
// 도형의 면적 합산
double area = 0.0;
foreach (var s in shapes)
    area = area + s.Area;                    // 0.0 에서 시작, + 로 합침

// 여러 리스트를 한 리스트로 이어붙이기
List<int> all = new();
foreach (var xs in lists)
    all.AddRange(xs);                        // 빈 리스트에서 시작, AddRange 로 합침

// 여러 검증 결과를 하나로 합치기
List<string> errors = new();
foreach (var result in results)
    errors.AddRange(result.Errors);          // 빈 리스트에서 시작, 오류 누적
```

다섯 자리가 모두 같은 패턴입니다. **시작값 + 두 값을 합치는 연산 + 차례로 누적**. 다만 **시작값** 과 **합치는 연산** 의 구체적 모양만 다릅니다.

| 도메인 | 시작값 | 합치는 연산 |
|---|---|---|
| 숫자 합계 | `0` | `a + b` |
| 곱셈 누적 | `1` | `a * b` |
| 문자열 이어붙이기 | `""` | `a + b` (이어붙이기) |
| 리스트 이어붙이기 | `[]` | `xs ++ ys` |
| 도형 면적 합산 | `0.0` | `a + b` |

같은 패턴이 다섯 자리에서 등장합니다. 명령형 어법은 **같은 발상을 다섯 번 복사**합니다. 새 도메인이 등장할 때마다 같은 구조의 `foreach` 가 또 등장하고, 빈 입력 처리 (시작값으로 그냥 두기) 도 매번 다시 적습니다. 같은 발상을 **한 번 정의 + 어디든 적용** 으로 묶을 수 없을까가 함수형의 발상이 등장하는 자리입니다.

> **흔한 함정** — 시작값을 **기본값** 으로만 보는 것입니다.
>
> 명령형 어법에 익숙해지면 `int total = 0;` 의 `0` 을 **그저 초기값** 으로 읽습니다. 그런데 `0` 은 단순한 초기값이 아닙니다. 합쳐도 상대를 바꾸지 않는 값 (`0 + n == n`) 이라는 **결합과의 약속** 이 그 자리에 있습니다. 곱셈의 시작값이 `0` 이 아니라 `1` 인 이유, 이어붙이기의 시작값이 `""` 인 이유가 모두 그 약속에서 나옵니다. 시작값은 **합치는 연산이 결정하는 약속** 입니다 (§3.5).

명령형 어법의 한 번 더의 약점은 **빈 입력의 안전성** 입니다. 위 코드들은 모두 `total = 0`, `all = ""` 처럼 **적절한 시작값** 을 안다는 가정에 의존합니다. 만약 합치는 함수가 다음처럼 **시작값을 받지 않는다면** 빈 입력은 어떻게 처리해야 할까요?

```csharp
// 시작값 없이 두 값만 받아 합치는 함수
int Combine(int a, int b) => a + b;

// 빈 목록을 합치려면?
int Sum(int[] xs) =>
    xs.Length == 0
        ? throw new InvalidOperationException("빈 목록은 합칠 수 없습니다")    // ✗ 빈 경우 분기
        : xs.Aggregate(Combine);
```

빈 경우를 **특별 취급** 하는 분기가 또 등장합니다. 빈 입력이 들어올 때마다 던지거나, `Length == 0` 분기를 매번 적거나, `null` 반환 같은 우회를 씁니다. 빈 경우가 **예외 자리** 가 되는 어법입니다.

문제 배경을 한 줄로 정리합니다. **같은 패턴의 반복 + 빈 경우의 특별 취급**. 두 비용을 한 번에 해결하는 자리가 함수형의 Monoid 입니다.

---

## §3.3 함수형의 발상 — 결합을 trait 으로

함수형의 발상은 §3.2 의 다섯 자리에서 **반복되는 구조** 를 trait 한 자리에 추상화합니다. 능력 (`Map`, `Bind`, `Fold` 같은 어휘) 이 객체에 사는 게 아니라 *trait* 에 살게 한 1 장 §1.5 의 결정적 도구를 결합 자리에 그대로 적용합니다.

```csharp
// 함수형 어법 (가상) — 결합 능력이 trait 한 자리에 산다
public interface Monoid<A>
{
    static A Empty { get; }              // 시작값 (타입의 단위원)
    A Combine(A rhs);                    // 두 값을 합치는 연산
}
```

같은 trait 한 정의로 §3.2 의 다섯 자리가 모두 같은 함수 호출이 됩니다.

```csharp
Monoid<int>.FoldAll([1, 2, 3]);            // Sum         → 6
Monoid<int>.FoldAll([2, 3, 4]);            // Product     → 24 (만약 곱셈 Monoid 라면)
Monoid<string>.FoldAll(["a", "b", "c"]);   // Concat      → "abc"
Monoid<int>.FoldAll([]);                   // 빈 목록     → Empty 가 답을 들고 있음
```

비용이 다섯 자리의 복사 + 빈 경우 분기 다섯 번에서 **trait 한 번 정의 + 자료마다 부착 한 번** 으로 줄어듭니다. 1 장 §1.7.2 의 trait 다형성이 결합 자리에서 작동하는 모양이고, 2 장 §2.2 의 N×M → N+M 회복 발상과 같은 어법입니다.

비용 절감의 핵심은 **결합 능력이 어디 사는가** 입니다. 명령형 어법에서는 결합 능력이 **함수의 본문** 에 살아 매번 새로 적었습니다. 함수형 어법에서는 결합 능력이 *trait* 에 살아 한 번 정의되면 어떤 자료 타입에든 부착됩니다. 능력의 자리가 함수 본문에서 trait 으로 옮겨가는 자리이고, 1 장 §1.5 의 세 패러다임의 한 차원 비교가 결합 어법에서 작동하는 사례입니다.

다만 시그니처만으로는 **결합** 이라는 의도가 완전히 표현되지 않습니다. `a → a → a` 시그니처를 만족하는 함수 중에는 **결합** 으로 부르기 곤란한 자리도 있습니다. 예를 들어 평균이나 뺄셈은 시그니처가 같지만 **결합 법칙** 을 깨고, 어떤 시작값이 항등인지도 정의되지 않습니다. 시그니처 외에 **두 약속 (법칙)** 이 따라붙어야 진짜 결합이 됩니다. 두 약속을 단계적으로 정착시키는 자리가 다음 두 절 (§3.4 Semigroup, §3.5 Monoid) 입니다.

---

## §3.4 결합의 약속 — Semigroup

먼저 **합치는 연산** 만 떼어 봅니다. 시작값 (단위원) 은 잠시 잊고, 두 값을 한 값으로 합치는 능력 자체에 이름을 붙입니다. 이 자리의 trait 이 **Semigroup** 입니다.

### 3.4.1 결합 연산이란 무엇인가

Semigroup 은 결합 연산 하나만 가진 trait 입니다. 멤버 한 줄로 표현됩니다.

```text
Combine : a → a → a              (수학적 시그니처)
a.Combine(b)  또는  a + b         (C# 어법 — 값의 instance 메서드 또는 + 연산자)
```

같은 타입 두 값을 받아 같은 타입 한 값을 돌려줍니다. 호출 어법은 `a.Combine(b)` 입니다. 왼쪽 값 `a` 가 자기 안의 결합 능력으로 오른쪽 값 `b` 와 합쳐집니다. `a + b` 는 `Combine` 의 syntactic sugar 로 같은 결과입니다 (§3.6.3 에서 자세히).

세 자리의 어휘를 정리합니다.

| 자리 | 의미 |
|---|---|
| 왼쪽 인자 `a` | 결합의 한쪽 피연산자 (값) |
| 오른쪽 인자 `a` | 결합의 다른쪽 피연산자 (값) |
| 반환값 `a` | 두 값이 합쳐진 결과 (같은 타입의 한 값) |

세 자리가 모두 **같은 타입 `a`** 라는 점이 핵심입니다. 두 값과 결과가 같은 World 안에 산다는 약속이고, Wlaschin 의 closure (닫혀 있음) 어휘로도 부르는 자리입니다. closure 가 깨지면 결과가 **다른 타입** 으로 빠져나가 `Combine` 을 또 호출할 수 없습니다.

예를 들어 두 문자열을 받아 **길이의 합** (`int`) 을 돌려주는 함수는 시그니처가 `string → string → int` 입니다. 결과가 `int` 라서 다음 결합 (`string` + `int`?) 이 막힙니다. Semigroup 시그니처가 아닙니다. 닫혀 있다는 closure 조건이 결합 어법의 첫 약속입니다.

### 3.4.2 결합 법칙

Semigroup 의 결합 연산에는 한 가지 법칙이 따릅니다. **결합 법칙 (associativity)** 입니다. 셋 이상의 값을 합칠 때 어느 쪽부터 묶어도 결과가 같아야 합니다.

```text
a.Combine(b).Combine(c) == a.Combine(b.Combine(c))
(a + b) + c             == a + (b + c)
```

Wikipedia 의 정의는 다음과 같습니다. *"(a • b) • c = a • (b • c)"*. 두 값을 묶고 한 번 더 묶는 자리에서 **묶는 순서** 에 결과가 의존하지 않는다는 약속입니다.

세 가지 결합 자리를 차례로 봅니다. 모두 결합 법칙이 성립합니다.

```text
(1 + 2) + 3                 == 1 + (2 + 3)               == 6        ✓ (덧셈)
(2 * 3) * 4                 == 2 * (3 * 4)               == 24       ✓ (곱셈)
("a" + "b") + "c"           == "a" + ("b" + "c")         == "abc"    ✓ (이어붙이기)
```

결합 법칙이 왜 중요한지는 **큰 입력의 병렬 처리** 에서 결정적으로 드러납니다. 백만 개 값을 합쳐야 한다고 합시다. 결합 법칙이 성립하면 입력을 절반으로 쪼개 두 코어가 각자 합친 뒤 마지막에 한 번 더 합쳐도 결과가 같습니다.

```text
순차:    a.Combine(b).Combine(c).Combine(d)
병렬:    a.Combine(b).Combine(c.Combine(d))    ← 좌우를 따로 합친 뒤 합침

         (1)  좌측 결합 — 코어 1: a.Combine(b)
         (2)  우측 결합 — 코어 2: c.Combine(d)
         (3)  최종 결합          : (코어 1 결과).Combine(코어 2 결과)
```

순차 결합과 병렬 결합이 **같은 결과** 를 낸다는 보장이 결합 법칙입니다. 이게 함수형 어법이 큰 입력의 효율적 처리에 자연스러운 한 이유이고, 분산 처리 / MapReduce / Spark 의 reducer 가 모두 **결합 가능한 연산** 을 가정하는 자리입니다.

Wlaschin 의 *Monoids in Practice* 시리즈에 실증 수치가 있습니다. 100 만 단어를 **문자열 이어붙이기 후 단어 수 세기** 로 처리하면 7955ms 가 걸리지만, 같은 입력을 *map-then-reduce* (작은 청크로 쪼개 각자 세고 결합) 로 처리하면 698ms 로 줄어듭니다. 11 배 빠른 속도가 **결합 법칙 한 줄의 수학적 결과** 입니다. 청크를 어떻게 묶든 같은 결과가 나온다는 보장이 있어야 **쪼개기 → 따로 처리 → 합치기** 가 안전한 최적화가 됩니다.

### 3.4.3 Semigroup 의 한계 — 빈 경우

Semigroup 한 추상만으로는 **빈 입력** 을 다룰 수 없습니다. 합칠 값이 하나도 없으면 `Combine` 을 호출할 짝이 없기 때문입니다.

```csharp
// 두 값을 받아 합치는 함수 — Semigroup 의 본문
int Combine(int a, int b) => a + b;

// 빈 목록을 합치려면?
int Sum(int[] xs) =>
    xs.Length == 0
        ? ???                            // ✗ 어떤 값을 돌려줘야 하나?
        : xs.Aggregate(Combine);
```

빈 입력의 답이 정해져 있지 않습니다. **시작값** 이 빠져 있기 때문입니다. 어떤 사람은 `0` 을 떠올리고, 어떤 사람은 예외를 떠올리고, 어떤 사람은 `null` 을 떠올립니다. trait 의 어휘 안에 **빈 경우의 답** 이 없으니 호출자가 매번 결정해야 하고, 결정이 다르면 같은 trait 의 두 인스턴스가 **빈 경우만 다른 결과** 를 냅니다.

빈 경우의 답을 trait 안에 함께 가두면 호출자가 결정할 일이 없습니다. 그 답이 **단위원 (identity element)** 이고, 그것을 가진 trait 이 다음 절의 Monoid 입니다.

---

## §3.5 단위원의 자리 — Monoid

Semigroup 에 단위원 하나를 더하면 Monoid 가 됩니다. 단위원은 **합쳐도 상대를 바꾸지 않는 값** 입니다. Wikipedia 의 인용은 다음과 같습니다. *"There exists an element e in S such that for every element a in S, the equalities e • a = a and a • e = a hold."* 어느 쪽에 합쳐도 상대가 그대로 남는 값이 단위원입니다.

### 3.5.1 단위원의 정의

Monoid 의 시그니처는 Semigroup 에 한 멤버를 더합니다.

```text
Combine : a → a → a              (Semigroup 에서 물려받음)
Empty   : a                       (단위원 — 타입의 한 자리에 박힌 값)
```

`Empty` 는 **어느 특정 원소 한 자리** 가 아니라 **타입 전체에 한 개 있는 값** 입니다. 덧셈의 `0`, 곱셈의 `1`, 이어붙이기의 `""`, 리스트 연결의 `[]` 가 모두 같은 자리에 옵니다. 어떤 연산으로 합치는가가 어떤 값이 단위원인가를 결정합니다.

| 도메인 | `Combine` | `Empty` |
|---|---|---|
| 덧셈 (`Sum`) | `a + b` | `0` |
| 곱셈 (`Product`) | `a * b` | `1` |
| 이어붙이기 (`Concat`) | `a + b` (string) | `""` |
| 리스트 연결 | `xs ++ ys` | `[]` |
| Boolean AND | `a && b` | `true` |
| Boolean OR | `a \|\| b` | `false` |
| 최댓값 (`Max`) | `Math.Max(a, b)` | `int.MinValue` |

같은 타입 (`int`) 인데 **합치는 연산이 다르면 단위원도 다르다** 는 점이 결정적입니다. `int` 의 덧셈 단위원은 `0`, 곱셈 단위원은 `1` 입니다. 같은 자료 타입이라도 **어떻게 합치는가** 가 **어떤 값이 항등인가** 를 결정합니다. C# 어법으로는 `Sum` / `Product` 같은 래퍼 타입으로 두 Monoid 를 구분합니다 (§3.8 에서 자세히).

### 3.5.2 항등 법칙

`Empty` 에는 **항등 법칙 (identity law)** 이 따릅니다. Haskell wiki 의 두 줄 인용은 다음과 같습니다. *"left identity: mempty <> x = x"* / *"right identity: x <> mempty = x"*. 단위원이 어느 쪽에 합쳐져도 상대가 그대로 남아야 합니다.

```text
Empty.Combine(a) == a            (좌 항등)
a.Combine(Empty) == a            (우 항등)
```

두 줄을 한 줄로 묶어 적으면 다음과 같습니다.

```text
Empty.Combine(a) == a == a.Combine(Empty)
Empty + a        == a == a + Empty
```

덧셈으로 검증해 봅시다.

```text
0 + 5  == 5  == 5 + 0            ✓ (덧셈의 좌·우 항등)
1 * 5  == 5  == 5 * 1            ✓ (곱셈의 좌·우 항등)
"" + "hi"  == "hi"  == "hi" + ""  ✓ (이어붙이기의 좌·우 항등)
```

항등 법칙은 단위원이 **결합 어법 안에서 보이지 않는 값** 이어야 한다는 약속입니다. 합쳐도 결과가 바뀌지 않으니 **합치는 사슬** 어디든 끼워도 결과가 그대로입니다.

```text
a.Combine(Empty).Combine(b).Combine(Empty).Combine(c)
== a.Combine(b).Combine(c)
== a + b + c
```

Empty 가 사슬 어디에 등장해도 빠진 것처럼 작동합니다. 이 성질이 다음 §3.5.3 의 **빈 경우의 안전** 의 근거입니다.

### 3.5.3 빈 경우의 안전

§3.4.3 에서 Semigroup 만으로 빈 입력의 답이 정해지지 않는 자리를 봤습니다. Monoid 의 `Empty` 가 그 답을 trait 의 어휘 안에 가둡니다.

```text
fold(Empty, Combine, [])         == Empty
fold(Empty, Combine, [a])        == a
fold(Empty, Combine, [a, b])     == a.Combine(b)
fold(Empty, Combine, [a, b, c])  == a.Combine(b).Combine(c)
```

빈 목록의 결과가 `Empty` 입니다. 호출자가 **예외** / *null* / **기본값** 같은 자기 결정을 내릴 필요가 없습니다. trait 의 약속 안에 빈 경우의 답이 이미 있고, 그 답이 항등 법칙을 만족하니 합치는 사슬과 **완벽히 정합** 합니다.

```csharp
// Monoid 가 빈 경우의 답을 들고 있어 분기가 사라진다
M FoldAll<M>(IEnumerable<M> items) where M : Monoid<M>
{
    var acc = M.Empty;                   // 빈 목록이면 그대로 반환됨
    foreach (var x in items)
        acc = acc.Combine(x);            // 첫 항이 Empty + x == x 로 자연스럽게 시작
    return acc;
}
```

`xs.Length == 0` 같은 분기가 없습니다. `acc = M.Empty` 한 줄이 빈 경우의 답을 미리 들고 있고, 항등 법칙이 첫 합칠 자리에서 `Empty + x == x` 로 작동해 **시작값이 결과를 오염시키지 않는다** 는 보장을 줍니다.

§3.2 의 명령형 어법의 두 비용 (반복 + 빈 경우 분기) 이 한 자리에서 해소된 자리입니다. 결합 능력은 trait 한 정의로 묶었고, 빈 경우 분기는 단위원 한 자리에 사라졌습니다. Monoid 의 두 멤버 (`Combine` + `Empty`) 가 두 비용에 각각 답하는 어법입니다.

---

## §3.6 직접 구현 — self-bound + `static abstract` 패턴

이론을 코드로 옮깁니다. 2 장에서 정착시킨 self-bound + `static abstract` 두 도구를 Monoid 에 적용합니다. Order 0 자리라 가장 단순한 형태입니다.

### 3.6.1 Semigroup trait 정의

먼저 결합 능력만 가진 Semigroup 부터 봅니다.

```csharp
// Semigroup — 결합 연산을 가진 값의 trait (Order 0, kind `*`)
public interface Semigroup<A>
    where A : Semigroup<A>
{
    A Combine(A rhs);                                          // 값의 결합 (instance)

    // + 는 Combine 의 syntactic sugar — 모든 구현체가 자동으로 받는다.
    static virtual A operator +(A lhs, A rhs) => lhs.Combine(rhs);
}
```

세 자리를 차례로 봅니다.

**`where A : Semigroup<A>`** — self-bound 제약. 2 장 §2.10.1 에서 봤던 자기 자신이 자기 trait 을 구현한다는 약속입니다. `A` 자리에 들어갈 타입이 **Semigroup 의 구현체임** 을 컴파일러에 보장합니다. 일반 함수 안에서 `a + b` 같은 호출이 가능한 자리를 만드는 도구이고, 4 장 이후 모든 trait 의 공통 골격입니다.

**`A Combine(A rhs)`** — instance 메서드. 결합 능력이 **값에 묶인다** 는 어법입니다. 호출 모양은 `lhs.Combine(rhs)` — 왼쪽 값 `lhs` 가 자기 안의 결합 능력으로 오른쪽 값 `rhs` 와 합쳐집니다. 4 장 이후 모든 trait 이 `static abstract` 인 것과 대비되는 자리이고, 왜 그렇게 다른지는 §3.10 에서 자세히 짚습니다.

**`static virtual A operator +`** — default 연산자. `+` 가 모든 Semigroup 구현체에 자동 제공되는 자리입니다. 본문은 한 줄 — `lhs.Combine(rhs)` 의 위임. 즉 **`+` 는 `Combine` 의 syntactic sugar** 입니다. 구현체가 `Combine` 한 메서드만 정의하면 `+` 가 자동으로 작동합니다. Haskell 의 `<>` 연산자가 C# 의 `+` 로 옮겨오는 자리입니다.

`[Pure]` 같은 attribute 는 LanguageExt v5 정통 본문에 등장하지만 학습용 코드에서는 생략합니다. 의미는 같습니다. 결합 연산이 부작용 없는 순수 함수임을 약속하는 자리입니다.

### 3.6.2 Monoid trait 정의 — Semigroup 상속

Monoid 는 Semigroup 을 상속하고 단위원 한 줄을 더합니다.

```csharp
// Monoid — Semigroup + 단위원 Empty (Order 0, kind `*`)
public interface Monoid<A> : Semigroup<A>
    where A : Monoid<A>
{
    static abstract A Empty { get; }                           // 타입의 단위원 (static abstract)
}
```

두 자리를 봅니다.

**`Monoid<A> : Semigroup<A>`** — trait 상속. Monoid 가 Semigroup 의 모든 능력 (`Combine` + default `+`) 을 물려받고 한 멤버를 더한다는 어법입니다. 2 장 §2.12 에서 봤던 trait 들 사이의 상속 관계가 처음 등장하는 자리이고, 5 장 Applicative (Functor 의 자식), 7 장 Monad (Applicative 의 자식) 같은 1 부의 다른 trait 사슬의 원형입니다.

**`static abstract A Empty { get; }`** — 타입의 단위원. 결합 능력은 **값에 묶인다** 면 단위원은 **타입에 한 개 박힌다** 는 어법입니다. `Sum` 타입의 단위원이 `Sum(0)`, `Product` 타입의 단위원이 `Product(1)` 같이 **각 타입마다 하나** 입니다. 어느 인스턴스가 들고 있는 게 아니라 타입 자체가 들고 있는 자리라 `static abstract` 가 자연스럽습니다 (§3.6.3 에서 자세히).

`where A : Monoid<A>` 의 self-bound 가 `Semigroup<A>` 의 그것과 짝을 이룹니다. 일반 함수 안에서 `M.Empty` 같은 호출 (타입의 능력) 과 `a.Combine(b)` 같은 호출 (값의 능력) 두 가지를 모두 가능하게 하는 자리입니다.

두 trait 의 시그니처는 **LanguageExt v5 의 `Semigroup<A>` / `Monoid<A> : Semigroup<A>` 와 정확히 정합** 합니다 (`LanguageExt.Core/Traits/Semigroup/Semigroup.cs`, `Monoid/Monoid.cs`).

### 3.6.3 `Combine` 은 instance, `Empty` 는 `static abstract` — 어법은 의미를 따라간다

같은 trait 안에서 두 멤버의 어법이 다릅니다. 왜 그런지가 Ch03 의 결정적 통찰입니다.

**`Combine` 이 instance 인 이유** — 결합은 **값 둘 사이의 연산** 입니다. 수학으로 적으면 `(ℤ, +)` 의 `5 + 3` — 두 원소 `5 ∈ ℤ` 와 `3 ∈ ℤ` 가 자기 안의 능력으로 합쳐지는 자리입니다. 원소 (값) 가 능력의 주인이라는 의미가 코드의 **instance 메서드** 어법에 그대로 옮겨옵니다. `a.Combine(b)` 호출에서 **왼쪽 값 `a`** 가 자기 능력으로 **오른쪽 값 `b`** 와 합쳐지는 자리입니다.

**`Empty` 가 `static abstract` 인 이유** — 단위원은 **타입에 한 개 박힌 값** 입니다. 어느 특정 인스턴스 (`Sum(5)` 의 `5`) 가 들고 있는 게 아니라 **`Sum` 타입 전체** 가 한 자리에 보관하는 값입니다. 인스턴스마다 다른 `Empty` 가 있을 수 없으니 **타입의 멤버** 어법이 자연스럽고, C# 어법으로는 `static` 입니다. 거기에 **각 구현체가 자기 `Empty` 를 정의해야 한다** 는 약속을 `abstract` 가 추가합니다.

| 멤버 | 의미 | 능력의 주인 | C# 어법 | 호출 모양 |
|---|---|---|---|---|
| `Combine` | 결합 연산 | 값 (`5`, `"hi"`) | instance | `a.Combine(b)`, `a + b` |
| `Empty` | 단위원 | 타입 (`Sum`, `Concat`) | `static abstract` | `Sum.Empty`, `M.Empty` |

같은 trait 안에서도 **추상의 자리** (값 vs 타입) 가 어법을 결정합니다. 한 자리는 **값의 능력** 이라 instance, 다른 자리는 **타입의 능력** 이라 static. 어법은 의미를 따라갑니다.

### 3.6.4 `K<F, A>` 마커가 왜 없는가

2 장의 Functor 시그니처와 Monoid 시그니처를 나란히 비교하면 결정적 차이가 보입니다.

```csharp
// Functor — Order 1 trait (Elevated World 의 컨테이너 위)
public interface Functor<F> where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> fa);
    //                       ──┬───                ──┬───
    //                    K<F, B> 자리           K<F, A> 자리
    //                    "F 안에 B 가 있다"     "F 안에 A 가 있다"
}

// Monoid — Order 0 trait (Normal World 의 값)
public interface Monoid<A> : Semigroup<A> where A : Monoid<A>
{
    static abstract A Empty { get; }
    //               ─┬─
    //               A 자리 (단순 완성 타입 — K<F, A> 마커 없음)
}
```

**Monoid 의 시그니처에는 `K<F, A>` 가 등장하지 않습니다.** 그 이유는 Monoid 가 **완성 타입** (`int`, `string`) 에 대한 추상이기 때문입니다. 끌어올릴 컨테이너 (`Option<a>`, `List<a>`) 가 없으니 **컨테이너의 안쪽을 가리키는 어휘** 도 필요 없습니다. 2 장 §2.9 에서 정착시킨 `K<F, A>` 의 결정적 동기 (C# 이 `* → *` 의 type constructor 를 일급으로 못 다룬다는 Order 2 의 미지원 우회) 자체가 Monoid 에는 적용되지 않습니다.

| trait | kind | 자료 타입 어법 | `K<F, A>` |
|---|---|---|---|
| Monoid | `*` (Order 0) | `Sum`, `Concat` (완성 타입) | 불필요 |
| Functor | `* → *` (Order 1) | `MyList<a>`, `MyMaybe<a>` (컨테이너) | 필요 |

Monoid 의 자료 타입 어법도 단순화됩니다. 2 장 §2.13 의 3-tuple 패턴 (자료 / 태그 / trait 의 세 조각) 에서 **태그** 가 빠집니다. `Sum` 자체가 자료 타입이면서 동시에 **trait 의 구현체** 입니다. 태그 타입 (`MyListF` 같은 빈 클래스) 도 없고 `K<F, A>` 마커도 없습니다. 자료가 trait 을 직접 구현하는 가장 단순한 부착 패턴입니다.

```csharp
// 3-tuple 패턴 (Order 1 — Ch02 §2.13)
public sealed class MyList<A>   : K<MyListF, A> { ... }     // 자료
public sealed class MyListF     : Functor<MyListF> { ... }  // 태그 (Map 의 호스트)
public interface  Functor<F>    where F : Functor<F> { ... } // trait

// 1-tuple 패턴 (Order 0 — Ch03)
public readonly record struct Sum(int Value) : Monoid<Sum>  // 자료가 trait 을 직접 구현
{
    public static Sum Empty => new(0);
    public Sum Combine(Sum rhs) => new(Value + rhs.Value);
}
```

조각이 **3 개에서 1 개로 줄어든** 자리입니다. Order 0 의 단순함이 코드의 부착 패턴까지 단순화합니다. 이 단순함이 3 장이 4 장 (Functor) 앞에 오는 이유이고, self-bound + `static abstract` 두 도구를 **컨테이너 없는 가장 단순한 형태로 먼저 익히는** 자리입니다. 4 장에서 같은 도구에 `K<F, A>` 마커 한 겹과 태그 타입을 더해 Order 1 trait 으로 올라갑니다.

---

## §3.7 두 법칙 검증 — 결합 + 항등

Monoid 가 진짜 Monoid 이려면 두 법칙을 지켜야 합니다. 시그니처만으로는 강제되지 않고 **구현이 약속하는 성질** 입니다. C# 컴파일러는 `Combine` 의 시그니처 (`A → A → A`) 와 `Empty` 의 시그니처 (`A`) 만 검증하지, 두 법칙이 성립하는지는 검증하지 못합니다. 사람이 약속하는 자리입니다.

두 법칙을 코드로 검증할 수 있습니다.

```csharp
// Monoid 두 법칙 검증 — 결합 + 항등 (instance + static 어법)
public static class MonoidLaws
{
    // 결합 법칙: a.Combine(b).Combine(c) == a.Combine(b.Combine(c))
    public static bool AssociativityHolds<M>(M a, M b, M c)
        where M : Monoid<M> =>
        a.Combine(b).Combine(c)!.Equals(a.Combine(b.Combine(c)));

    // 좌 항등: Empty.Combine(a) == a
    public static bool LeftIdentityHolds<M>(M a)
        where M : Monoid<M> =>
        M.Empty.Combine(a)!.Equals(a);

    // 우 항등: a.Combine(Empty) == a
    public static bool RightIdentityHolds<M>(M a)
        where M : Monoid<M> =>
        a.Combine(M.Empty)!.Equals(a);
}
```

검증 함수의 시그니처를 봅니다. **`where M : Monoid<M>`** 한 줄이 **어떤 Monoid 든** 받아들이는 자리입니다. `M.Empty` 는 타입의 능력 (static), `a.Combine(b)` 는 값의 능력 (instance). 두 호출이 한 함수 안에서 자기 자리에 정확히 놓입니다. 어떤 자료 타입이 Monoid 만 만족하면 이 검증 함수 세 개가 **공짜로** 적용되는 자리이고, 2 장 §2.14 의 **어떤 Functor 든 받는 일반 함수** 와 같은 ROI 입니다.

검증을 호출해 봅니다.

```csharp
// 덧셈 Monoid 의 두 법칙 검증
var (s1, s2, s3) = (new Sum(2), new Sum(3), new Sum(5));
MonoidLaws.AssociativityHolds(s1, s2, s3);      // true
MonoidLaws.LeftIdentityHolds(s1);               // true
MonoidLaws.RightIdentityHolds(s1);              // true

// 곱셈 Monoid 의 결합 법칙
var (p1, p2, p3) = (new Product(2), new Product(3), new Product(4));
MonoidLaws.AssociativityHolds(p1, p2, p3);      // true

// 이어붙이기 Monoid 의 결합 법칙
var (c1, c2, c3) = (new Concat("x"), new Concat("y"), new Concat("z"));
MonoidLaws.AssociativityHolds(c1, c2, c3);      // true
```

세 자료 타입의 두 법칙이 모두 `true`. 진짜 Monoid 라는 약속이 코드로 검증된 자리입니다.

> **흔한 함정** — **시그니처만 만족하면 Monoid** 라고 결론 내리는 것입니다.
>
> 평균 함수 `average : double → double → double` 은 시그니처가 Semigroup 처럼 보입니다. 그러나 결합 법칙이 깨집니다. `average(average(2, 4), 8) == 5` 인데 `average(2, average(4, 8)) == 4` 입니다. 묶는 순서에 결과가 달라지면 Semigroup 이 아닙니다.
>
> 뺄셈도 마찬가지입니다. `(10 - 5) - 3 == 2` 인데 `10 - (5 - 3) == 8` 입니다. 결합 법칙이 깨지면 Monoid 도 아닙니다. **시그니처는 필요조건이지 충분조건이 아닙니다**. 두 법칙 (결합 + 항등) 의 실제 성립이 진짜 Monoid 의 약속입니다.

> **평균을 Monoid 로 만드는 패턴**. 평균은 직접 Monoid 가 아니지만 **자료의 모양을 바꾸면** Monoid 가 됩니다. 평균 값 한 개 (`double`) 가 아니라 **`{total, count}` 두 자리의 자료** 로 누적하면 됩니다.
>
> ```csharp
> public readonly record struct Avg(int Total, int Count) : Monoid<Avg>
> {
>     public static Avg Empty => new(0, 0);
>     public Avg Combine(Avg rhs) => new(Total + rhs.Total, Count + rhs.Count);
>     public double Value => Count == 0 ? 0.0 : (double)Total / Count;
> }
> ```
>
> `Total` 과 `Count` 각각이 덧셈 Monoid 라 둘의 짝도 결합 법칙을 만족합니다. 단위원은 `(0, 0)`. 마지막 단계에 `.Value` 로 평균을 계산하면 **결합 가능한 자료의 누적** + **최종 추출** 어법으로 평균이 Monoid 어법 안에 들어옵니다. Wlaschin 의 정리는 다음과 같습니다. *"design the type to be a monoid first, then compute the desired result at the end."* 결합 어법 안에 못 들어오는 자리는 **자료의 모양을 바꿔** 끌어들이는 자리입니다.

두 법칙이 함께 성립하면 **`FoldAll` 이 어떻게 묶든, 빈 경우가 와도 항상 같은 답을 낸다** 는 보장이 생깁니다. 결합 법칙이 순서의 자유를, 항등 법칙이 빈 경우의 안전을 책임집니다. 큰 입력을 병렬로 쪼개 따로 합친 뒤 마지막에 한 번 더 합치는 분산 처리의 안전성도 두 법칙에서 옵니다.

5 장 Applicative 의 다섯 법칙, 4 장 Functor 의 두 법칙, 7 장 Monad 의 세 법칙처럼 1 부의 모든 trait 에 **법칙** 이 따라붙습니다. Monoid 의 두 법칙이 그 **법칙 사다리** 의 첫 단입니다. 시그니처가 약속 못 하는 성질을 사람이 약속하고, 그 약속이 코드의 자유로운 조합 / 최적화 / 검증을 가능하게 합니다.

---

## §3.8 더 많은 Monoid — Sum / Product / Concat / Boolean

같은 Monoid 패턴이 다양한 자료 타입에 부착되는 모양을 봅니다. 각 자리에서 `Empty` 와 `Combine` 이 무엇인지가 결합 어법을 결정합니다.

### 3.8.1 Sum — 덧셈

가장 친숙한 자리입니다. 정수의 덧셈으로 합치고 단위원은 `0` 입니다.

```csharp
// 덧셈 Monoid — Empty 는 0, Combine 은 +
public readonly record struct Sum(int Value) : Monoid<Sum>
{
    public static Sum Empty => new(0);
    public Sum Combine(Sum rhs) => new(Value + rhs.Value);

    // operator + 명시 — 구체 타입에서 직접 호출 가능하게
    public static Sum operator +(Sum lhs, Sum rhs) => lhs.Combine(rhs);
}
```

`readonly record struct` 가 **값 타입의 불변 어법** 입니다. C# 의 합성 어법 (`==`, `with`, `Deconstruct`) 을 함께 받는 자리이고, 1 부 전체에서 값 타입 자료의 표준 모양입니다.

`Empty` 는 `Sum(0)` 입니다. 어떤 `Sum(n)` 과 합쳐도 그대로 남습니다 (`Sum(0).Combine(Sum(5)) == Sum(5)`). `Combine` 은 안의 `Value` 끼리 더한 새 `Sum`. 두 자리가 덧셈의 단위원 `0` 과 결합 `+` 의 직접 표현입니다.

`operator +` 명시 한 줄에 대한 세부를 짚습니다. Semigroup 의 `static virtual A operator +` default 는 **generic 제약 context 안에서만** 직접 호출됩니다 (`where M : Semigroup<M>` 안의 `a + b`). 구체 타입 `Sum + Sum` 직접 호출이 가능하려면 자료 타입에 `public static Sum operator +` 한 줄을 명시해 default 를 override 해야 합니다. v5 의 default 발상은 보존하되 구체 타입 호출의 편의성을 더한 자리입니다.

### 3.8.2 Product — 곱셈

같은 `int` 인데 합치는 연산만 바뀝니다. 곱셈으로 합치면 단위원이 `1` 입니다.

```csharp
// 곱셈 Monoid — Empty 는 1, Combine 은 *
public readonly record struct Product(int Value) : Monoid<Product>
{
    public static Product Empty => new(1);
    public Product Combine(Product rhs) => new(Value * rhs.Value);

    public static Product operator +(Product lhs, Product rhs) => lhs.Combine(rhs);
}
```

`Sum` 과 `Product` 가 **같은 `int` 를 감싸지만 다른 Monoid** 라는 점이 결정적입니다. 두 trait 인스턴스 사이에 **어느 쪽이 진짜 `int` 의 Monoid 인가** 의 답이 없습니다. 둘 다 진짜이고, **어떤 연산으로 합칠지를 호출자가 선택** 합니다.

```csharp
FoldAll(new[] { new Sum(2), new Sum(3), new Sum(4) });          // Sum(9)  — 덧셈
FoldAll(new[] { new Product(2), new Product(3), new Product(4) }); // Product(24) — 곱셈
```

같은 `[2, 3, 4]` 가 `Sum` 으로 합치면 `9`, `Product` 로 합치면 `24` 입니다. 자료 타입 (`Sum` vs `Product`) 이 **합치는 방식의 선택** 을 표현하는 자리이고, Haskell 의 `newtype Sum a` / `newtype Product a` 어법이 C# 의 **래퍼 struct** 로 옮겨오는 자리입니다.

곱셈 Monoid 의 `operator +` 본문이 **덧셈이 아니라 곱셈** 임에 주의합니다. `+` 는 Semigroup 의 결합 sugar 이지 **산술 + 가 아닙니다**. `Product(2) + Product(3) == Product(6)` 이고, 안의 `int` 끼리는 곱셈으로 합쳐집니다. 이 어법이 처음에는 어색해 보이지만, **trait 의 `+` 가 결합의 일반 어휘** 라는 점이 손에 잡히면 자연스러워집니다.

### 3.8.3 Concat — 문자열 이어붙이기

문자열에도 같은 패턴이 적용됩니다.

```csharp
// 문자열 이어붙이기 Monoid — Empty 는 "", Combine 은 + (string)
public readonly record struct Concat(string Value) : Monoid<Concat>
{
    public static Concat Empty => new("");
    public Concat Combine(Concat rhs) => new(Value + rhs.Value);

    public static Concat operator +(Concat lhs, Concat rhs) => lhs.Combine(rhs);
}
```

`Empty` 는 빈 문자열 `""`. 어떤 문자열과 이어붙여도 그대로 남습니다 (`"".Combine("hi") == "hi"`). `Combine` 은 `string + string` 의 이어붙이기. 자리가 **덧셈에서 이어붙이기** 로 옮겼을 뿐 패턴은 같습니다.

```csharp
FoldAll(new[] { new Concat("hello"), new Concat(" "), new Concat("world") });
// → Concat("hello world")

FoldAll(Array.Empty<Concat>());
// → Concat("") — 빈 목록도 단위원이 답을 들고 있음
```

여러 자료 / 두 어휘 / 한 패턴. Sum, Product, Concat 세 자료 타입이 모두 **같은 두 멤버 (`Empty` + `Combine`)** 를 정의하고, 같은 일반 함수 (`FoldAll`) 가 셋 모두에 자동 적용됩니다. trait 한 정의 + 부착 N 번이 N×M 비용을 N+M 으로 줄이는 자리입니다 (1 장 §1.7.2 의 N+5 비용 재현).

### 3.8.4 Boolean — AND / OR

Boolean 에는 두 가지 Monoid 가 있습니다. AND 로 합치면 단위원이 `true`, OR 로 합치면 단위원이 `false` 입니다.

```csharp
// AND Monoid — Empty 는 true, Combine 은 &&
public readonly record struct All(bool Value) : Monoid<All>
{
    public static All Empty => new(true);
    public All Combine(All rhs) => new(Value && rhs.Value);

    public static All operator +(All lhs, All rhs) => lhs.Combine(rhs);
}

// OR Monoid — Empty 는 false, Combine 은 ||
public readonly record struct Any(bool Value) : Monoid<Any>
{
    public static Any Empty => new(false);
    public Any Combine(Any rhs) => new(Value || rhs.Value);

    public static Any operator +(Any lhs, Any rhs) => lhs.Combine(rhs);
}
```

`All.Empty == true` 의 이유는 다음과 같습니다. **모두 참** 의 빈 합은 **참** 입니다 (vacuous truth, 합칠 게 없으니 거짓이 될 수가 없습니다). LINQ 의 `xs.All(p)` 가 빈 컬렉션에 대해 `true` 를 돌려주는 자리와 정합합니다.

`Any.Empty == false` 의 이유는 다음과 같습니다. **하나라도 참** 의 빈 합은 **거짓** 입니다 (vacuous falsity, 합칠 게 없으니 참인 게 없습니다). LINQ 의 `xs.Any(p)` 가 빈 컬렉션에 대해 `false` 를 돌려주는 자리와 정합합니다.

`Sum` / `Product` 처럼 같은 `bool` 인데 **합치는 연산이 다르면 단위원도 다르다** 는 패턴이 또 반복됩니다. Wlaschin 의 정리는 다음과 같습니다. *"Boolean AND (identity: true) and OR (identity: false)"*. 두 자리의 단위원이 LINQ 의 `All` / `Any` 의 빈 경우 동작과 정확히 맞물립니다.

### 3.8.5 같은 타입, 다른 Monoid — 어떻게 구분하나

`Sum` / `Product`, `All` / `Any` 처럼 같은 자료 타입에 두 가지 결합 어법이 있을 수 있다는 점이 흥미롭습니다. C# 어법에서는 **서로 다른 자료 타입** (`Sum` vs `Product`) 으로 **어떤 결합인지** 를 표현합니다. Haskell 의 `newtype Sum a` / `newtype Product a` 어법이 C# 의 **래퍼 struct** 로 옮겨오는 자리입니다.

| 자료 타입 | 안의 타입 | `Empty` | `Combine` |
|---|---|---|---|
| `Sum` | `int` | `0` | `+` (덧셈) |
| `Product` | `int` | `1` | `*` (곱셈) |
| `Concat` | `string` | `""` | `+` (이어붙이기) |
| `All` | `bool` | `true` | `&&` (AND) |
| `Any` | `bool` | `false` | `\|\|` (OR) |
| `Max` (확장) | `int` | `int.MinValue` | `Math.Max` |
| `Min` (확장) | `int` | `int.MaxValue` | `Math.Min` |

`Max` / `Min` 같은 자리도 정의 가능합니다. `int.MinValue` 가 어떤 `int` 와의 `Max` 에서 **상대를 그대로 남기는** 자리이고 (`Max(int.MinValue, 5) == 5`), 결합 법칙도 성립합니다 (`Max(Max(a, b), c) == Max(a, Max(b, c))`). 결합 가능한 자리는 어디든 Monoid 가 등장하는 셈입니다.

이 풍부함이 Monoid 의 ROI 를 결정합니다. 같은 패턴 (trait 한 번 정의 + 자료마다 부착) 으로 **모든 결합 어법** 을 한 어휘 안에 묶을 수 있습니다. Wlaschin 의 정리 한 줄을 그대로 인용합니다. *"whenever you encounter terms like 'sum,' 'product,' or 'concatenation,' you've likely found a monoid waiting to simplify your design"*.

### 3.8.6 비 Monoid 를 Monoid 로 만드는 세 패턴

§3.7 의 **흔한 함정** 에서 평균 / 뺄셈이 Monoid 가 아니라는 자리를 봤습니다. 그런데 **자료의 모양을 바꿔** Monoid 의 어휘 안으로 끌어들이는 패턴이 있습니다. Wlaschin 의 *Monoids in Practice* 시리즈가 세 패턴으로 정리합니다.

**첫 번째 패턴** — closure 회복 (list 로 감싸기). 두 값을 합쳤더니 결과가 **다른 타입** 으로 빠져나가는 자리. 두 `char` 를 합치면 `string` 이 되어 **char 의 Monoid** 가 안 되지만, **`char` 를 `[char]` 리스트로 감싸면** 두 리스트의 이어붙이기 (`['a'] ++ ['b'] == ['a', 'b']`) 가 closure 를 만족합니다. Wlaschin 의 정리는 다음과 같습니다. *"anything can be put into a list, and lists (with concatenation) are always monoids."* 리스트 이어붙이기가 **모든 자료를 Monoid 화하는 만능 도구** 인 자리이고, 8 장 Validation 의 **오류 리스트** 도 같은 패턴입니다.

**두 번째 패턴** — associativity 회복 (동사 → 명사). 결합 법칙이 깨지는 자리. **연산을 수행하지 말고** 연산을 **자료 구조로 표현** 하면 됩니다. `'a' - 'b'` (문자열에서 빼기) 가 결합 법칙을 깨면, **`CharsToRemove(['b'])` 같은 자료 구조로 빼기 의도를 표현** 하고 최종 단계에서 한 번에 적용하면 됩니다. **동사 (함수) 를 명사 (자료 구조) 로** 바꾸는 자리이고, 6 장 Foldable 의 *defer evaluation* 패턴과 같은 발상입니다.

**세 번째 패턴** — identity 회복 (Option/None 으로 단위원 추가). 단위원이 **원래 자료** 안에 없는 자리. 양의 정수의 덧셈은 `Combine` 은 만족하지만 **항상 양수** 라 `0` (덧셈 단위원) 이 자료 안에 없습니다. `int option` (= `Option<int>`) 으로 감싸 **`None` 을 단위원** 으로 쓰면 됩니다. Wlaschin 의 정리는 다음과 같습니다. *"any time we need an identity which is outside the normal set of values, we can use Option.None to represent it."* 단위원이 **외부에서 주입** 되는 자리이고, 2 부 `Alternative` / `MonoidK` (13 장) 의 **`Option` 의 단위원이 `None`** 어법과 정합합니다.

세 패턴을 한 표로 정리합니다.

| 깨지는 약속 | 해결 도구 | 예 | 회수 자리 |
|---|---|---|---|
| closure (`a → b` 가 다른 타입) | list 로 감싸기 (`a → [a]`) | `char → [char]` | 8 장 Validation 의 **오류 리스트** |
| associativity (묶는 순서 의존) | 동사 → 명사 (자료 구조화) | **빼기** → `CharsToRemove` | 6 장 Foldable 의 *defer evaluation* |
| identity (단위원 없음) | Option / None 으로 단위원 주입 | 양의 정수 → `int option` | 2 부 `Alternative` / `MonoidK` (13 장) |

§3.7 의 평균 자리에서 본 **`{total, count}` 패턴** 도 같은 발상의 변형입니다. 평균이 Monoid 가 아닌 자리에서, **자료의 모양을 Monoid 가 되는 형태로 바꿔** 누적합니다. **자료의 모양이 Monoid 어법을 결정한다** 는 결정적 통찰이 세 패턴의 공통점입니다.

이 세 패턴이 1 부 / 2 부 / 3 부의 다양한 자리에서 회수됩니다. **Validation 의 오류 리스트** (1 패턴), **Writer 의 로그 누적** (1 패턴), **Alternative 의 `None` 단위원** (3 패턴), **Foldable 의 defer 어법** (2 패턴) 모두 같은 어휘의 변형입니다. Monoid 가 **결합 어법의 원형** 이면, 세 패턴은 **Monoid 어휘를 모든 자리로 확장하는 도구** 입니다.

---

## §3.9 일반 함수 — `FoldAll` (Foldable 의 디딤돌)

§3.8 의 다양한 자료 타입에 같은 누적 함수가 적용되는 모양을 한 자리에서 보겠습니다. 어떤 Monoid 든 받는 일반 함수입니다.

```csharp
// 어떤 Monoid M 이든 목록을 하나로 접는 자유 함수
public static class FoldAll
{
    public static M Of<M>(IEnumerable<M> items)
        where M : Monoid<M>
    {
        var acc = M.Empty;                   // 타입의 단위원 (static)
        foreach (var x in items)
            acc = acc.Combine(x);            // 값의 결합 능력 (instance)
        return acc;
    }
}
```

함수의 시그니처 한 줄을 봅니다. **`M Of<M>(IEnumerable<M> items) where M : Monoid<M>`**. `M` 자리에 어떤 Monoid 든 들어오고, 같은 함수 본문이 그대로 작동합니다. 본문이 **두 호출 (`M.Empty`, `acc.Combine(x)`)** 외에 **자료 타입에 대한 어떤 가정도 하지 않는다** 는 점이 핵심입니다.

같은 함수가 다섯 자리에서 자동 동작합니다.

```csharp
FoldAll.Of(new[] { new Sum(1), new Sum(2), new Sum(3) });
// → Sum(6)

FoldAll.Of(new[] { new Product(2), new Product(3), new Product(4) });
// → Product(24)

FoldAll.Of(new[] { new Concat("a"), new Concat("b"), new Concat("c") });
// → Concat("abc")

FoldAll.Of(new[] { new All(true), new All(true), new All(false) });
// → All(false)

FoldAll.Of(Array.Empty<Sum>());
// → Sum(0) — 빈 목록도 안전
```

다섯 자료 타입에 **같은 함수 한 줄 호출**. trait 한 정의 + 자료마다 부착이 만드는 ROI 의 가장 명확한 실증 자리입니다. 1 장 §1.7.2 의 **trait 부착 한 번에 컨테이너의 모든 시민이 trait 의 모든 능력을 자동으로 가집니다** 가 Monoid 에서 작동하는 모양이고, 2 장 §2.14 의 `FunctorOps.Run` 의 Order 0 자리 원형입니다.

`FoldAll` 의 자리는 6 장 Foldable 의 디딤돌이기도 합니다. Foldable 은 **컨테이너 (`E<a>`) 안의 구조를 Normal World 의 한 값으로 끌어내리는** 추상이고, 그 끌어내림의 출발점이 **어떤 값으로 시작해 무엇으로 합칠 것인가** 의 결정입니다. `FoldAll` 이 그 결정을 **Monoid 의 `Empty` + `Combine`** 으로 묶은 자리이고, 6 장에서 같은 발상에 **컨테이너 모양** (`E<a>`) 을 추가하면 Foldable 의 시그니처로 자연스럽게 진화합니다.

```text
Ch03 FoldAll  : Monoid<M> 의 [M] → M               (Normal World 안에서)
Ch06 fold     : Monoid<M> 의 E<M> → M              (Elevated World 의 E<a> 에서 Normal 의 M 으로)
```

`FoldAll` 의 자료 타입이 `IEnumerable<M>` 에서 **임의의 컨테이너 `E<M>`** 으로 일반화되는 자리입니다. 3 장의 결합 어법 + 6 장의 컨테이너 어법이 만나 **Foldable 의 통합 시그니처** 가 등장합니다.

> **3-tuple 의 ROI 의 Order 0 원형** — 새 자료 타입을 만들 때 `Empty` + `Combine` 두 멤버만 정의하면 `FoldAll` / 두 법칙 검증 / `+` 연산자가 **공짜로** 따라옵니다. 6 장 Foldable 에서 같은 ROI 가 컨테이너 위에서 더 큰 규모로 작동합니다 (4 멤버 정의 + 30+ 자유 함수 공짜).

---

## §3.10 어법 비대칭 — Order 0 의 instance, Order 1 의 `static abstract`

Monoid 와 다음 장의 Functor 를 어법으로 나란히 두면 두 trait 의 자리가 또렷해집니다.

| trait | kind | 능력의 주인 | C# 어법 | `K<F, A>` | 자료 어법 |
|---|---|---|---|---|---|
| **Semigroup / Monoid** (3 장) | `*` (Order 0) | 값 자체 (`5`, `"hello"`) | instance `a.Combine(b)` + `+` | 불필요 | `struct Sum : Monoid<Sum>` (1-tuple) |
| **Functor / Foldable / Monad / Traversable** (4 ~ 9 장) | `* → *` (Order 1) | 컨테이너 모양 (`MyList`, `MyMaybe`) | `static abstract F.Map(...)` | 필요 | 자료 + 태그 + trait (3-tuple) |

**Order 0 trait 의 능력의 주인은 값입니다.** `5` 가 자기 안에 결합 능력을 들고 다니고, `"hello"` 가 자기 안에 이어붙이기 능력을 들고 다닙니다. 원소가 가진 능력이라 instance 어법이 자연스럽고, `lhs + rhs` 의 syntactic sugar 까지 그 안에서 자연스럽게 자랍니다.

**Order 1 trait 의 능력의 주인은 컨테이너 모양입니다.** `MyList<int>` 라는 특정 값 이 Map 능력을 들고 있는 게 아니라 **MyList 라는 컨테이너 종류** 가 Map 을 어떻게 하는지 정의합니다. 컨테이너 자체가 능력의 주인이라 **타입의 능력** 어법 (`static abstract`) 이 자연스럽고, `K<F, A>` 마커가 **컨테이너 안쪽** 을 가리키는 어휘로 등장합니다.

| 자리 | 값의 능력 | 타입의 능력 |
|---|---|---|
| 호출 모양 | `a.Combine(b)` | `F.Map(f, fa)` |
| C# 어법 | instance method | `static abstract` |
| 추상의 자리 | 값 (`5`, `"hello"`) | 컨테이너 (`MyList`, `MyMaybe`) |
| 예 | `new Sum(2).Combine(new Sum(3))` | `MyListF.Map(plus1, xs)` |
| 학습 자리 | 3 장 Monoid | 4 ~ 9 장 모든 trait |

같은 self-bound 패턴 안에서 **추상의 자리가 다르면 어법이 다릅니다**. 4 장 이후 모든 trait 이 `static abstract` 인 이유는 컨테이너 위 추상이라 **타입의 능력** 으로 정의되어야 하기 때문이고, 3 장의 Monoid 만 instance + `+` 인 이유는 Order 0 의 **값의 능력** 이라 instance 어법이 자연스럽기 때문입니다.

**LanguageExt v5 의 정통 어법도 같은 비대칭을 따릅니다**. 책의 학습용 trait 시그니처가 v5 의 `LanguageExt.Core/Traits/Semigroup/Semigroup.cs` 와 `Monoid/Monoid.cs` 와 정확히 정합한 자리이고, **Order 가 어법을 결정한다** 는 공통의 설계 결정이 라이브러리와 학습용 코드 양쪽에 반영된 자리입니다.

세 동기로 정리합니다. Order 0 의 instance + `+` 어법이 다음 셋을 동시에 만족하는 자리이기 때문입니다.

- (a) **Haskell `<>` 어법 보존** — `a <> b` 가 C# 의 `a + b` 로. 함수형 어법이 OO 의 **instance 메서드 + operator overload** 안에 자연스럽게 살아 있는 자리입니다.
- (b) **대수 구조 정합** — `(ℤ, +)` 의 `5 ∈ ℤ` 가 덧셈 능력의 주인이라는 수학의 어법이 *instance method* 로 그대로 옮겨오는 자리.
- (c) **`+` 연산자와의 1:1 짝지음** — `lhs + rhs` 의 자연스러운 구현이 `lhs.Combine(rhs)`. `static virtual A operator +` 의 default 한 줄이 두 어법을 자동으로 잇습니다.

3 장의 Order 0 어법에 4 장에서 `K<F, A>` 마커 한 겹과 static dispatch 가 더해지면 Order 1 어법으로 올라갑니다. Monoid 가 self-bound 패턴의 **컨테이너 없는 원형** 이라면, Functor 는 거기에 **컨테이너 모양 + static dispatch** 를 씌운 첫 사례입니다.

---

## §3.11 Ch03 의 자리 — 후속 회수

3 장의 Monoid 가 격자 바깥의 Order 0 trait 임에도 1 부 전체에서 두 번 / 2 부에서 한 번 회수됩니다. 이 회수 자리들이 **3 장의 자리** 를 결정합니다.

### 3.11.1 8 장 Validation — 오류 누적 (1 부 안)

8 장에서 회원가입 같은 다중 검증을 다룹니다. 두 검증 스타일이 등장합니다.

- **applicative style** — 모든 오류 누적 (다섯 필드에 다섯 오류가 있으면 다섯 모두 보여 줌)
- **monadic style** — 첫 오류 단락 (첫 실패에서 멈춤)

*applicative style* 의 핵심 도구가 **오류들의 결합** 이고, 그 결합이 정확히 Monoid 입니다. 보통은 **오류 리스트** (`List<string>`) 의 Monoid 가 사용되고, 빈 리스트가 단위원, 리스트 이어붙이기가 결합입니다.

```text
검증 1 의 오류:  ["이름은 비어 있을 수 없습니다"]
검증 2 의 오류:  ["이메일 형식이 잘못됐습니다"]
검증 3 의 오류:  ["비밀번호는 8 자 이상이어야 합니다"]

세 결과를 Monoid 결합으로:
  Combine(Combine(검증 1, 검증 2), 검증 3)
   == ["이름은 비어 있을 수 없습니다", "이메일 형식이 잘못됐습니다", "비밀번호는 8 자 이상이어야 합니다"]
```

세 검증의 오류가 **Monoid 결합** 한 자리로 모입니다. 항등 법칙이 **오류가 없는 검증** (빈 리스트 = `Empty`) 의 안전성을 책임지고, 결합 법칙이 **검증 순서의 자유** 를 책임집니다.

### 3.11.2 3 부 Writer — 로그 누적 (3 부)

3 부의 Writer Monad 는 **값과 로그를 함께 들고 다니는 효과** 입니다. 함수가 한 결과 값을 돌려주면서 **부작용처럼 보이는 로그** 를 함께 누적합니다.

```csharp
// 가상 어법 — Writer 의 핵심 자리
Writer<W, A> f();           // W 가 로그의 타입, A 가 결과의 타입

// 두 Writer 의 결합:
//   값 A → 함수 f 의 새 A 로 변환
//   로그 W → 함수 f 의 새 로그와 Monoid 결합으로 누적
```

`W` 자리에 Monoid 가 들어옵니다. 로그를 어떻게 합칠지가 Monoid 의 결합으로 결정되고, **로그가 없는 자리** 의 답이 단위원으로 자연스럽게 처리됩니다. 보통은 `List<LogEntry>` 같은 자료 타입의 Monoid 가 사용됩니다.

### 3.11.3 2 부 `SemigroupK` / `MonoidK` — Elevated World 의 결합

2 부에서 Monoid 가 Elevated World 로 끌어올려진 자리도 만납니다. `SemigroupK` / `MonoidK` 는 **컨테이너 끼리의 결합** 어휘입니다. `List<a>` 두 개를 이어붙이는 결합 (`xs ++ ys`) 이나, `Option<a>` 두 개 중 **값이 있는 쪽을 고르는** 결합 같은 자리들이 모두 `SemigroupK` / `MonoidK` 의 인스턴스입니다.

`SemigroupK` 의 시그니처는 다음과 같습니다.

```text
Combine : K<F, A> → K<F, A> → K<F, A>
```

3 장의 `Combine : a → a → a` 의 **완성 타입 자리에 `K<F, A>` 마커가 들어간 자리** 입니다. Order 0 의 결합 어법이 Order 1 의 컨테이너 자리로 올라간 자리이고, 2 부에서 본격적으로 다룹니다 (13 장).

### 3.11.4 한 줄 회수

Ch03 의 Monoid 가 **Normal World 안의 결합 한 자리** 이지만, 그 자리에서 정착시킨 어휘 (`Combine` + `Empty` + 두 법칙) 가 1 부의 Validation, 3 부의 Writer, 2 부의 `MonoidK` 세 자리에서 회수됩니다. 또 self-bound + `static abstract` 두 도구를 **컨테이너 없는 가장 단순한 형태** 로 손에 익히는 자리이기도 합니다. 4 장 Functor 가 그 도구에 **컨테이너 모양 + `K<F, A>` 마커** 를 더해 Order 1 으로 올라가는 자리입니다.

---

## §3.12 Q&A — 자기 점검

> **Q1. Semigroup 과 Monoid 의 차이는 무엇입니까?** (§3.4, §3.5)

Semigroup 은 결합 연산 `Combine` 한 멤버만 가집니다. Monoid 는 거기에 단위원 `Empty` 를 더한 trait 입니다. 차이는 빈 경우를 다룰 수 있는가입니다. Semigroup 은 합칠 값이 최소 하나는 있어야 하지만, Monoid 는 `Empty` 가 빈 경우의 답을 쥐고 있어 값이 하나도 없어도 안전합니다. 시그니처로는 한 멤버 차이, 의미로는 **빈 경우** 의 어휘를 trait 안에 가두느냐의 차이입니다.

> **Q2. Monoid 의 두 법칙은 무엇입니까? 시그니처와 어떤 관계입니까?** (§3.7)

두 법칙은 다음과 같습니다. **결합 법칙**은 `(a + b) + c == a + (b + c)` 로, 묶는 순서가 결과를 바꾸지 않습니다. **항등 법칙**은 `Empty + a == a == a + Empty` 로, 단위원이 어느 쪽에 합쳐져도 상대를 바꾸지 않습니다. 두 법칙은 **시그니처가 약속 못 하는 성질** 입니다. C# 컴파일러는 `Combine` 의 `A → A → A` 시그니처만 검증하지 결합 법칙이 성립하는지는 검증하지 못합니다. 시그니처는 필요조건, 두 법칙은 충분조건입니다.

> **Q3. 같은 `int` 인데 왜 `Sum` 과 `Product` 를 따로 만듭니까?** (§3.5.1, §3.8)

`int` 를 합치는 방법이 하나가 아니기 때문입니다. 덧셈으로 합치면 단위원이 `0`, 곱셈으로 합치면 단위원이 `1` 입니다. 어떤 연산으로 합치는가가 Monoid 를 결정하므로, 같은 타입이라도 **연산이 다르면 다른 Monoid** 입니다. C# 어법에서는 `Sum` / `Product` 같은 래퍼 struct 로 두 Monoid 를 구분합니다. Haskell 의 `newtype Sum a` / `newtype Product a` 어법이 C# 의 래퍼 struct 로 옮겨오는 자리입니다.

> **Q4. 왜 Monoid 에는 `K<F, A>` 마커가 없습니까?** (§3.6.4)

Monoid 는 완성 타입 (`int`, `string`) 에 직접 붙는 Order 0 trait 이기 때문입니다 (kind `*`). `K<F, A>` 마커는 `MyList<a>` 같은 컨테이너 (Order 1, kind `* → *`) 의 안쪽 값을 C# 어법에서 가리키기 위한 우회였습니다. Monoid 에는 끌어올릴 컨테이너 자체가 없으니 마커도 필요 없습니다. 그래서 자료 타입의 부착도 3-tuple (자료 / 태그 / trait) 이 아닌 **자료가 trait 을 직접 구현** 하는 1-tuple 로 단순화됩니다.

> **Q5. 왜 `Combine` 은 instance 메서드, `Empty` 는 `static abstract` 입니까?** (§3.6.3, §3.10)

두 멤버의 능력의 주인이 다르기 때문입니다. `Combine` 은 **값 둘 사이의 연산** 입니다. `(ℤ, +)` 의 `5 + 3` 처럼 값 `5` 가 자기 안에 결합 능력을 들고 있는 자리입니다. **값이 능력의 주인** 이라 instance 메서드가 자연스럽습니다. `Empty` 는 **타입 전체에 한 자리에 박힌 단위원** 입니다. 어느 특정 인스턴스가 들고 있는 게 아니라 **타입** 이 들고 있는 값입니다. **타입이 능력의 주인** 이라 `static abstract` 가 자연스럽습니다. 같은 trait 안에서도 어법은 의미를 따라갑니다.

> **Q6. 평균이나 뺄셈은 왜 Monoid 가 아닙니까?** (§3.7 흔한 함정)

결합 법칙이 깨지기 때문입니다. 뺄셈은 `(10 - 5) - 3 == 2` 인데 `10 - (5 - 3) == 8` 이라 묶는 순서에 결과가 달라집니다. 평균도 `average(average(2, 4), 8) == 5` 인데 `average(2, average(4, 8)) == 4` 라 마찬가지입니다. `Combine` 의 시그니처 `a → a → a` 는 만족해도, 결합 법칙을 지키지 못하면 Monoid 가 아닙니다. 시그니처는 필요조건이지 충분조건이 아닙니다.

> **Q7. `static virtual A operator +` 한 줄이 무엇을 합니까?** (§3.6.1, §3.8.1)

Semigroup 의 default 연산자입니다. 본문이 한 줄, `lhs.Combine(rhs)` 의 위임입니다. 모든 Semigroup 구현체가 `+` 연산자를 **공짜로** 제공받습니다. 즉 **`+` 는 `Combine` 의 syntactic sugar** 입니다. 구현체가 `Combine` 한 메서드만 정의하면 `+` 가 자동으로 작동합니다. 다만 **generic 제약 context 안에서만** 직접 호출되므로 (`where M : Semigroup<M>` 안의 `a + b`), 구체 타입에서 `Sum + Sum` 같이 직접 쓰려면 구현체에서 `public static Sum operator +` 한 줄을 명시해 default 를 override 합니다.

> **Q8. `FoldAll` 의 시그니처와 6 장 Foldable 의 `fold` 는 어떻게 이어집니까?** (§3.9)

`FoldAll` 의 시그니처는 `IEnumerable<M> → M where M : Monoid<M>` 입니다. **Monoid 의 [M] 을 M 으로 접는** 함수입니다. 6 장 Foldable 의 `fold` 는 같은 발상에 **컨테이너 일반화** 가 더해진 자리로, 시그니처가 `K<F, M> → M where M : Monoid<M>` 입니다. `IEnumerable<M>` 이 **임의의 컨테이너 `E<M>`** 으로 일반화되는 자리이고, 3 장의 결합 어법 + 6 장의 컨테이너 어법이 만나 Foldable 의 통합 시그니처가 등장합니다. `FoldAll` 은 Foldable 의 Order 0 원형입니다.

> **Q9. Boolean 의 `All` 과 `Any` 의 단위원은 왜 그렇게 결정됩니까?** (§3.8.4)

빈 합의 답 (vacuous answer) 이 단위원입니다. `All.Empty == true` 인 이유는 **모두 참** 의 빈 합이 **참** 이기 때문입니다 (합칠 게 없으니 거짓이 될 수가 없음). `Any.Empty == false` 인 이유는 **하나라도 참** 의 빈 합이 **거짓** 이기 때문입니다 (참인 게 없음). 두 단위원이 LINQ 의 `xs.All(p)` / `xs.Any(p)` 가 빈 컬렉션에 대해 돌려주는 값과 정확히 정합합니다. 항등 법칙이 LINQ 어법의 빈 경우 안전성의 수학적 근거이기도 합니다.

> **Q10. Monoid 가 1 부의 다른 trait 과 어떻게 이어집니까?** (§3.11)

세 자리에서 회수됩니다. **8 장 Validation** 의 *applicative style* 에서 오류 리스트를 합치는 자리, **3 부 Writer Monad** 에서 로그 `W` 를 누적하는 자리, **2 부 `SemigroupK` / `MonoidK`** 에서 같은 결합을 Elevated World 의 컨테이너 위로 끌어올리는 자리입니다. Order 0 의 결합 어휘 한 정의가 1 부 / 2 부 / 3 부의 세 자리에서 회수되는 셈입니다. 또 self-bound + `static abstract` 두 도구를 **컨테이너 없는 가장 단순한 형태** 로 익히는 디딤돌이기도 합니다 (4 장이 같은 도구에 컨테이너 + `K<F, A>` 를 더해 Order 1 으로 올라감).

> **Q11. 결합 법칙이 왜 큰 입력의 병렬 처리에 결정적입니까?** (§3.4.2)

결합 법칙이 **묶는 순서의 자유** 를 보장하기 때문입니다. 백만 개 값을 합쳐야 할 때, 입력을 절반으로 쪼개 두 코어가 각자 합친 뒤 마지막에 한 번 더 합쳐도 결과가 **순차 결합과 같다** 는 보장이 결합 법칙입니다. `(a · b) · c == a · (b · c)` 가 성립하므로 `((a·b)·c)·d` 와 `(a·b)·(c·d)` 가 같습니다. 분산 처리 / MapReduce / Spark 의 reducer 가 모두 **결합 가능한 연산** 을 가정하고, 그 가정의 수학적 근거가 Monoid 의 결합 법칙입니다.

> **Q12. 같은 자료 타입에 두 가지 Monoid 가 있을 때 (예: `int` 의 `Sum` / `Product`), 어떻게 호출자가 어느 쪽을 선택합니까?** (§3.8.5)

C# 어법에서는 **서로 다른 자료 타입** 으로 두 Monoid 를 구분합니다. `Sum` 과 `Product` 가 같은 `int` 를 감싸지만 **서로 다른 struct** 라서 컴파일러가 두 trait 의 인스턴스를 별개로 인식합니다. `FoldAll(new[] { new Sum(2), new Sum(3) })` 와 `FoldAll(new[] { new Product(2), new Product(3) })` 가 호출 자리에서 어느 Monoid 인지가 결정됩니다. Haskell 의 `newtype Sum a` / `newtype Product a` 가 같은 도구이고, C# 의 **래퍼 struct** 는 그 직접 표현입니다.

---

## §3.13 3 장 요약

- **Normal World 안의 결합** — Monoid 는 완성 타입 (`int`, `string`) 두 값을 한 값으로 합치는 Order 0 trait 입니다 (kind `*`). 1 장의 4 분면 격자 **바깥** 의 Order 0 자리이고, Elevated World 의 컨테이너 (`E<a>`) 가 등장하지 않습니다.
- **Semigroup + 단위원 = Monoid** — Semigroup 은 결합 연산 `Combine` 한 멤버만 가집니다. Monoid 는 거기에 단위원 `Empty` 를 더해 **빈 경우의 답** 을 trait 안에 가둡니다. 두 멤버 (`Combine` + `Empty`) 가 §3.2 의 두 비용 (반복 + 빈 경우 분기) 에 각각 답하는 어법입니다.
- **두 법칙** — 결합 법칙 (`(a + b) + c == a + (b + c)`) + 항등 법칙 (`Empty + a == a == a + Empty`). 시그니처가 약속 못 하는 성질을 **구현이 약속** 하는 자리이고, 시그니처는 필요조건, 두 법칙은 충분조건입니다. 평균 / 뺄셈 같은 자리는 시그니처는 만족해도 결합 법칙이 깨져 Monoid 가 아닙니다.
- **`K<F, A>` 마커 없는 단순한 부착** — Order 0 자리라 컨테이너 어휘 (`K<F, A>`) 가 필요 없습니다. 2 장의 3-tuple 패턴 (자료 / 태그 / trait) 도 **자료가 trait 을 직접 구현** 하는 1-tuple 로 단순화됩니다. self-bound + `static abstract` 두 도구를 **컨테이너 없는 가장 단순한 형태** 로 손에 익히는 디딤돌입니다.
- **`Combine` 은 instance, `Empty` 는 `static abstract`** — 능력의 주인이 **값** 인 자리는 instance, **타입** 인 자리는 `static abstract`. 같은 trait 안에서도 어법은 의미를 따라갑니다. 이 비대칭이 4 장 이후 모든 Order 1 trait 이 `static abstract` 인 어법의 원형 (Order 0 의 반례) 이기도 합니다.
- **`+` 는 `Combine` 의 syntactic sugar** — Semigroup 의 `static virtual A operator +` 한 줄이 모든 구현체에 `+` 연산자를 자동 제공합니다. Haskell 의 `<>` 어법이 C# 의 `+` 어법으로 옮겨오는 자리이고, 함수형의 이항 연산자가 OO 의 **instance 메서드 + operator overload** 안에 자연스럽게 자라는 모양입니다.
- **`Sum` vs `Product`** — 같은 타입, 다른 Monoid — 같은 `int` 를 감싸도 합치는 연산이 다르면 다른 Monoid 입니다. 덧셈의 단위원은 `0`, 곱셈의 단위원은 `1`. **어떻게 합치는가** 가 **어떤 값이 단위원인가** 를 결정합니다. C# 어법으로는 래퍼 struct 로 두 Monoid 를 구분합니다.
- **`FoldAll`** — Foldable 의 디딤돌 — 어떤 Monoid 든 받는 일반 함수 한 정의가 모든 Monoid 인스턴스에 자동 적용됩니다. 1 장 §1.7.2 의 trait 부착 ROI 의 Order 0 원형이고, 6 장 Foldable 의 `fold` 가 **임의의 컨테이너 `E<M>`** 으로 일반화하는 출발점입니다.
- **세 자리 회수** — 3 장의 결합이 1 부 8 장 Validation (오류 누적), 3 부 Writer (로그 누적), 2 부 `SemigroupK` / `MonoidK` (컨테이너 결합) 세 자리에서 회수됩니다. 격자 바깥의 Order 0 trait 이지만 **모든 결합 어법의 원형** 이라 1 부 / 2 부 / 3 부 전체에 그림자가 닿습니다.

이 장에서 결합 어법을 손에 잡았습니다. self-bound + `static abstract` 두 도구를 **컨테이너 없는 가장 단순한 형태** 로 익혔고, 어떤 자료 타입에든 두 멤버 (`Empty` + `Combine`) 를 정의하면 일반 함수 (`FoldAll`) 가 자동 따라오는 ROI 도 봤습니다. 다만 3 장의 결합은 **Normal World 안의 자리** 였습니다. Elevated World 의 시민 (`Option<a>`, `MyList<a>`) 위로 결합 어법을 끌어올리는 자리가 4 장부터 본격적으로 등장합니다.

---

## §3.14 다음 장으로

4 장은 1 부의 결정적 자리입니다. **함수형의 본질, 즉 합성 가능한 Elevated World 로 lift** (§1.6.1) 의 첫 trait 인 Functor 입니다. Normal World 의 함수 `a → b` 한 개를 Elevated World 의 함수 `E<a> → E<b>` 로 끌어올리는 자리이고, 3 장의 self-bound + `static abstract` 두 도구에 **컨테이너 모양** (`K<F, A>` 마커) + *static dispatch* 가 더해져 Order 1 trait 으로 올라갑니다.

3 장에서 익힌 패턴 (`where A : Monoid<A>` + `static abstract A Empty` + instance `Combine`) 의 어법이 4 장에서 (`where F : Functor<F>` + `static abstract K<F, B> Map(...)`) 형태로 옮겨갑니다. **값 자리** 가 **컨테이너 모양 자리** 로 올라가고, instance 어법이 `static abstract` 로 통일됩니다. 두 어법의 비대칭이 §3.10 에서 짚은 **Order 0 vs Order 1 의 어법 차이** 의 가장 명확한 사례입니다.

준비가 됐다면 [4 장 — Functor / map](./Ch04-Functor.md) 으로 넘어갑니다.
