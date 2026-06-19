# 12 장. Sequences (실무 시퀀스에 부착하는 다섯 trait)

> **이 장의 목표** — 이 장을 읽고 나면 기초에서 toy 타입 `MyList` 에 부착했던 다섯 trait (Functor / Applicative / Monad / Foldable / Traversable) 이 실무 lazy 시퀀스 `MySeq` 에 글자 그대로 다시 붙는다는 것을 직접 구현으로 확인하고, LINQ 의 `Select` / `SelectMany` 가 사실 `map` 과 `bind` 의 다른 이름이었음을 시그니처 단계에서 읽을 수 있습니다. 계산을 미루는 lazy 가 무한 시퀀스까지 다루게 해 준다는 것, 표현이 전혀 다른 cons 리스트 `MyLst` 에도 같은 trait 이 그대로 붙는다는 것, 그리고 한 자료 타입에 적법한 Applicative 가 둘 (데카르트 곱과 짝 맞춤) 공존할 수 있다는 것까지 손에 쥡니다.

> **이 장의 핵심 어휘**
>
> - **`MySeq<A>`**: lazy 시퀀스. 기초의 `MyList` 가 가리키던 자리를 채우는 실무 시민으로, 계산을 끝까지 미룹니다
> - **lazy (계산 미룸)**: `Map` / `Bind` 가 결과를 즉시 만들지 않고, 값이 실제로 당겨질 때 한 원소씩 계산하는 방식. 무한 시퀀스도 안전하게 다룹니다
> - **`SeqF`**: `MySeq` 의 태그 타입. 하나가 `Monad` 와 `Traversable` 을 동시에 호스트합니다
> - **데카르트 곱 (`Apply`)**: 함수 시퀀스의 각 함수를 값 시퀀스의 각 값에 모두 적용 (`⊛` 는 `Apply` 기호. `[+1, ×10] ⊛ [1, 2]` = `[2, 3, 10, 20]`)
> - **zip (짝 맞춤)**: `i` 번째 함수를 `i` 번째 값에만 적용하는 두 번째 Applicative (`[2, 20]`)
> - **`SelectMany`**: C# 컴파일러가 `from-from-select` 를 변환해 호출하는 메서드. 곧 Monad 의 `bind`
> - **cons (`MyLst`)**: `Cons(Head, Tail)` / `Nil` 의 재귀 구조. 표현은 달라도 같은 trait 이 붙습니다

> 이 장을 마치면 할 수 있게 되는 것
> - [ ] toy `MyList` 와 실무 `MySeq` 의 차이가 trait 이 아니라 **평가 시점 (즉시 vs lazy)** 임을 설명할 수 있습니다.
> - [ ] lazy 가 큰 데이터·무한 시퀀스를 다루게 해 주는 이유를 한 줄로 말할 수 있습니다.
> - [ ] `MySeq` 에 Functor / Applicative / Monad / Foldable / Traversable 을 3-tuple 패턴으로 직접 부착할 수 있습니다.
> - [ ] `Sum` / `Count` / `Any` / `All` / `First` 같은 일상 함수가 `Foldable` 두 멤버 위에서 자란다는 것을 보일 수 있습니다.
> - [ ] LINQ `from x in xs from y in f(x) select …` 가 `Bind` 한 번으로 변환됨을 코드로 보일 수 있습니다.
> - [ ] `List<Maybe<a>>` 를 `Maybe<List<a>>` 로 뒤집는 `traverse` 와 `sequence` 로 "모두 성공해야 성공" 을 구현할 수 있습니다.
> - [ ] cons 구조 `MyLst` 에 같은 trait 이 붙는 것을 보고 추상이 표현이 아니라 시그니처에 달려 있음을 설명할 수 있습니다.
> - [ ] 같은 시퀀스에 데카르트 곱 `Apply` 와 zip `Apply` 두 Applicative 가 공존함을 보이고 둘을 구분할 수 있습니다.

---

## 12.1 이 장에서 다루는 것 — 기초 추상의 첫 실무 시민

기초에서 우리는 `MyList`, `MyMaybe`, `MyValidation` 같은 toy 자료 타입에 다섯 trait 을 손으로 부착했습니다. toy 타입은 추상이 **어떻게 작동하는지** 를 최소 골격으로 보여 주는 데 목적이 있었습니다. 매일의 코드에서 실제로 쓰는 자료 구조는 아니었습니다.

이 장은 4부의 출발점이고, 출발점의 질문은 하나입니다. **기초에서 손으로 만든 그 추상이 실무 컬렉션에서도 그대로 작동하는가.** 답을 먼저 말하면, 그대로 작동합니다. LanguageExt v5 의 `Seq` / `Lst` / `Map` 같은 실무 컬렉션은 우리가 기초에서 정의한 것과 **동일한** `Functor` / `Foldable` / `Traversable` / `Monad` 의 인스턴스이고, `MyList` 에 부착했던 것과 똑같은 3-tuple 패턴 (자료 / 태그 / trait) 이 그 안에 들어 있습니다.

이 장의 무대는 4장에서 본 두 평행 세계 그대로입니다. 시퀀스는 "여러 개일 수 있음" 효과를 인코딩한 Elevated World 의 시민이고, 안에 담기는 `a`, `b` 는 Normal World 의 값입니다. 1장 지도의 여섯 이동 가운데 시퀀스가 새 이동을 더하지는 않습니다. `map` (값 변환), `bind` (효과 잇기), `fold` (한 값으로 끌어내림), `traverse` (두 층 swap) 라는 이미 아는 자리들이 toy 가 아닌 실무 시민 위에서 똑같이 작동하는 것을 확인할 뿐입니다.

이 장은 도구를 처음부터 다시 설명하지 않습니다. 다섯 trait 의 정의와 법칙은 4장부터 9장까지에서 이미 손에 익혔으니, 여기서는 **그 trait 이 실무 시퀀스에 어떻게 부착되는지**, 그리고 toy 에는 없던 두 가지 새로운 사실 (계산을 미루는 lazy 평가, 한 타입에 공존하는 두 Applicative) 에 집중합니다.

---

## 12.2 왜 필요한가 — toy 타입은 실무 자료 구조가 아니다

기초의 `MyList` 는 안에 `List<A>` 를 들고, `Map` 을 호출하면 **그 자리에서 곧장** 새 리스트를 만들었습니다. 작은 예제에서는 아무 문제가 없습니다. 그런데 실무의 데이터는 두 가지 점에서 다릅니다.

첫째, 데이터가 큽니다. 백만 건의 로그를 `Map` 으로 변환하고 그중 처음 다섯 건만 본다면, 백만 건을 전부 변환한 뒤 다섯 건을 꺼내는 것은 낭비입니다. 둘째, 데이터가 무한할 수도 있습니다. "1, 2, 3, …" 처럼 끝이 없는 시퀀스는 즉시 평가하는 순간 영원히 끝나지 않습니다.

```csharp
// toy MyList — Map 이 곧장 새 리스트를 materialize (전부 계산)
var doubled = myList.Map(n => n * 2);   // 백만 건이면 백만 번 즉시 계산
```

기초에서 배운 trait 이 toy 에만 붙는 추상이었다면, 이 한계 앞에서 함수형 어휘는 실무로 넘어오지 못합니다. 그러나 trait 은 **시그니처가 약속하는 동작** 일 뿐, 그 동작을 **언제 계산하는가** 는 별개입니다. 같은 `Map` 시그니처를 즉시 계산이 아니라 **계산을 미루는** 방식으로 구현하면, 똑같은 trait 위에서 큰 데이터도 무한 데이터도 다룰 수 있습니다. 그 미루는 시퀀스가 이 장의 주인공 `MySeq` 입니다.

---

## 12.3 MySeq — 계산을 미루는 lazy 시퀀스

`MySeq<A>` 는 안에 `IEnumerable<A>` 를 들고, 모든 연산을 `yield` 기반 (LINQ) 으로 구현합니다. 핵심은 `IEnumerable` 이 **당겨질 때까지 계산하지 않는다** 는 점입니다.

```csharp
public sealed class MySeq<A>(IEnumerable<A> items) : K<SeqF, A>
{
    public IEnumerable<A> Items { get; } = items;
    public override string ToString() => $"[{string.Join(", ", Items)}]";
}
```

자료 타입 `MySeq<A>`, 태그 타입 `SeqF`, 공통 어휘 `K<SeqF, A>` 의 3-tuple 은 2장에서 본 그대로입니다. `MyList` 와 다른 점은 단 하나, 생성자가 받은 `IEnumerable` 을 즉시 `ToList()` 로 펼치지 않고 그대로 들고 있다는 것입니다.

> **흔한 함정** — lazy 가 trait 을 바꾼다는 오해입니다.
>
> lazy 는 `Map` / `Bind` 의 **시그니처를 한 글자도 바꾸지 않습니다**. `Map : (a → b) → E<a> → E<b>` 는 즉시 평가든 lazy 든 똑같습니다. 바뀌는 것은 그 변환을 **언제** 수행하는가뿐입니다. 그래서 toy `MyList` 에 통과한 Functor 법칙은 lazy `MySeq` 에서도 그대로 통과합니다.

### 12.3.1 lazy 가 무한 시퀀스까지 다룬다

lazy 의 진짜 위력은 **무한 시퀀스** 에서 드러납니다. `IEnumerable` 은 당겨질 때 한 원소씩 만들므로, 끝이 없는 시퀀스를 들고 있어도 앞에서 필요한 만큼만 계산하면 됩니다.

```text
끝없는 자연수 1, 2, 3, …  를 담은 MySeq 에
  Map(n => n * n) 을 걸면 → 1, 4, 9, …  (아직 한 번도 계산 안 함)
  앞에서 다섯 개만 당기면 → 1, 4, 9, 16, 25  (딱 다섯 번만 제곱)
```

즉시 평가하는 `MyList` 는 무한 시퀀스에 `Map` 을 거는 순간 영원히 멈추지 않습니다. lazy `MySeq` 는 같은 `Map` 시그니처로 무한 시퀀스를 안전하게 변환하고, 뒤에서 `fold` 나 `take` 가 필요한 만큼만 당깁니다. **시그니처는 같고 계산 시점만 미뤘을 뿐인데, 다룰 수 있는 데이터의 크기가 유한에서 무한으로 넓어집니다.**

> **외우지 않아도 됩니다** — lazy 의 구현 디테일 (`yield`, iterator 상태 기계) 을 지금 파고들 필요는 없습니다. 한 줄만 가져가면 됩니다. **lazy 는 "변환 규칙만 들고 있다가 당겨질 때 한 원소씩 계산" 한다.** 그래서 큰 데이터도 무한 데이터도 같은 trait 으로 다룹니다.

태그 타입 `SeqF` 는 하나가 `Monad` 와 `Traversable` 을 동시에 호스트합니다. Monad 는 Applicative 를, Applicative 는 Functor 를 상속하므로 (`Monad ⊃ Applicative ⊃ Functor`), `SeqF` 하나에 다섯 trait 의 멤버가 모두 들어갑니다.

```csharp
public sealed class SeqF : Monad<SeqF>, Traversable<SeqF>
{
    // 아래 절들에서 멤버를 하나씩 채웁니다.
}
```

---

## 12.4 다섯 trait 이 그대로 붙는다

### 12.4.1 Functor — 값만 변환하되, 계산은 미룬다

`Map` 은 4장에서 본 그대로 컨테이너 안의 값만 바꾸고 모양은 보존합니다. `MySeq` 에서는 `Select` 한 줄입니다.

```csharp
public static K<SeqF, B> Map<A, B>(Func<A, B> f, K<SeqF, A> fa) =>
    new MySeq<B>(fa.As().Items.Select(f));
```

`Select` 는 `IEnumerable` 의 lazy 연산이라, 이 `Map` 은 **새 값을 하나도 계산하지 않고** 변환 규칙만 품은 시퀀스를 곧장 돌려줍니다. 실제 곱셈은 결과를 출력하거나 `ToList` 로 당길 때 한 원소씩 일어납니다.

기초에서 익힌 세 가지 호출 어법이 그대로 쓰입니다.

```csharp
K<SeqF, int> nums = new MySeq<int>([1, 2, 3, 4, 5]);

K<SeqF, int> r1 = SeqF.Map<int, int>(n => n * 2, nums);      // ① trait 정적 멤버
K<SeqF, int> r2 = Monad.map<SeqF, int, int>(n => n * 2, nums); // ② 모듈 자유 함수
K<SeqF, int> r3 = nums.Map(n => n * 2);                       // ③ 확장 메서드
// 셋 다 [2, 4, 6, 8, 10] — 같은 결과
```

### 12.4.2 Applicative — Pure 와 Apply (데카르트 곱)

`Pure` 는 값 하나를 원소 하나짜리 시퀀스로 끌어올립니다. `Apply` 는 함수 시퀀스의 **각 함수** 를 값 시퀀스의 **각 값** 에 적용합니다. 두 시퀀스의 모든 조합을 만드는 데카르트 곱입니다.

```csharp
public static K<SeqF, A> Pure<A>(A value) =>
    new MySeq<A>([value]);

public static K<SeqF, B> Apply<A, B>(K<SeqF, Func<A, B>> mf, K<SeqF, A> ma)
{
    var fs = mf.As();
    var xs = ma.As();
    return new MySeq<B>(Go());

    IEnumerable<B> Go()
    {
        foreach (var f in fs.Items)
            foreach (var x in xs.Items)
                yield return f(x);
    }
}
```

함수 `[+1, ×10]` 을 값 `[1, 2]` 에 `Apply` 하면 함수마다 두 값에 적용한 결과가 이어붙습니다.

```
+1  을 [1, 2] 에  → [2, 3]
×10 을 [1, 2] 에  → [10, 20]
이어붙임           → [2, 3, 10, 20]
```

이 "모든 조합" 이 시퀀스 Apply 의 기본 의미입니다. 한 자리에 다른 의미의 Apply 도 둘 수 있는데, 그 두 번째 Applicative 는 뒤에서 다시 봅니다.

### 12.4.3 Monad — Bind, 그리고 LINQ 의 정체

`Bind` 는 각 원소를 시퀀스로 펼친 뒤 이어붙입니다. 7장에서 본 `MyMaybe` 의 단락과 `MyList` 의 펼침이 같은 `Bind` 의 두 얼굴이었듯, 시퀀스의 `Bind` 는 "한 입력이 여러 갈래로" 입니다.

```csharp
public static K<SeqF, B> Bind<A, B>(K<SeqF, A> ma, Func<A, K<SeqF, B>> f)
{
    var xs = ma.As();
    return new MySeq<B>(Go());

    IEnumerable<B> Go()
    {
        foreach (var x in xs.Items)
            foreach (var y in f(x).As().Items)
                yield return y;
    }
}
```

이 `Bind` 가 바로 LINQ 의 `SelectMany` 입니다. C# 컴파일러는 `from-from-select` 쿼리를 `SelectMany` 호출로 바꾸고, 우리의 `SelectMany` 는 `Bind` 위에서 자랍니다.

```csharp
// Bind 직접 호출 — 각 n 을 [n, n×10] 으로 펼침
K<SeqF, int> bound = nums.Bind(n => (K<SeqF, int>)new MySeq<int>([n, n * 10]));

// 똑같은 계산을 LINQ from-from-select 로
K<SeqF, int> viaLinq =
    from n in nums
    from m in (K<SeqF, int>)new MySeq<int>([n, n * 10])
    select m;

// 두 결과는 글자 그대로 같다 — [1, 10, 2, 20, 3, 30, 4, 40, 5, 50]
```

> **외우지 않아도 됩니다** — LINQ 를 함수형 어휘로 다시 외울 필요는 없습니다. 핵심 한 줄만 남기면 됩니다. **시퀀스에 대한 LINQ 쿼리는 Functor 의 `map` 과 Monad 의 `bind` 위에 서 있었다.** `Select` 는 `map`, `SelectMany` 는 `bind` 입니다.

### 12.4.4 Foldable — 일상의 함수들이 두 멤버 위에서 자란다

`Foldable` 은 6장에서 본 끌어내림입니다. 시퀀스의 여러 원소를 Normal World 의 한 값으로 접습니다. `FoldLeft` 는 앞에서 뒤로 lazy 시퀀스를 한 번만 순회합니다.

```csharp
public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<SeqF, A> fa)
{
    var acc = seed;
    foreach (var a in fa.As().Items)
        acc = f(acc, a);
    return acc;
}
```

여기서 6장의 핵심이 실무로 이어집니다. 매일 쓰는 `Sum` / `Count` / `Any` / `All` / `First` / `ToList` 가 모두 `FoldLeft` 와 `FoldRight` **두 멤버 위에서 자동으로 자랍니다**. 따로 구현할 필요가 없습니다.

| 일상 함수 | 두 멤버 위의 정의 | 의미 |
|---|---|---|
| `Count` | `FoldLeft((acc, _) => acc + 1, 0)` | 원소 개수 |
| `Any(p)` | `FoldRight((a, acc) => p(a) || acc, false)` | 하나라도 만족 |
| `All(p)` | `FoldRight((a, acc) => p(a) && acc, true)` | 모두 만족 |
| `IsEmpty` | `FoldRight((_, _) => false, true)` | 비었는가 |
| `First` | `FoldRight((a, _) => a, default)` | 첫 원소 |
| `ToList` | `FoldRight((a, acc) => [a, ..acc], [])` | 리스트로 |

```csharp
var sum     = nums.FoldLeft((acc, n) => acc + n, 0);   // 15
var count   = nums.Count();                            // 5
var anyEven = nums.Any(n => n % 2 == 0);               // true
var allPos  = nums.All(n => n > 0);                    // true
```

이것이 6장에서 본 "trait 한 정의가 N 개의 일반 함수를 공짜로 준다" 의 실무 모습입니다. `MySeq` 가 두 멤버만 구현하면 여섯 함수가 따라옵니다.

### 12.4.5 Traversable — 모두 성공해야 성공 (traverse 와 sequence)

`Traverse` 는 9장에서 본 층 swap 입니다. `List<Maybe<a>>` 처럼 겹친 두 Elevated 층의 순서를 뒤집어 `Maybe<List<a>>` 로 만듭니다. 실무에서 가장 쓸모 있는 자리는 "원소를 모두 변환했을 때, 하나라도 실패하면 전체 실패" 입니다.

> **미리보기입니다** — 아래 코드는 9장 `Traverse` 골격을 글자 그대로 옮긴 것입니다. 중첩 제네릭 `K<F, K<SeqF, B>>` 와 `Apply` 두 줄이 낯설어도, 새로 읽을 것은 `MySeq` 한 단어뿐입니다. 모양이 막히면 9장의 층 swap 그림을 떠올리면 됩니다.

```csharp
public static K<F, K<SeqF, B>> Traverse<F, A, B>(Func<A, K<F, B>> f, K<SeqF, A> ta)
    where F : Applicative<F>
{
    K<F, K<SeqF, B>> acc = F.Pure<K<SeqF, B>>(new MySeq<B>([]));

    foreach (var a in ta.As().Items.Reverse())
    {
        var fb = f(a);
        Func<B, Func<K<SeqF, B>, K<SeqF, B>>> prepend =
            head => tail => new MySeq<B>([head, .. tail.As().Items]);

        var liftedFn = F.Pure(prepend);
        var step1    = F.Apply<B, Func<K<SeqF, B>, K<SeqF, B>>>(liftedFn, fb);
        acc          = F.Apply<K<SeqF, B>, K<SeqF, B>>(step1, acc);
    }
    return acc;
}
```

`prepend` 은 머리 하나를 앞에 붙여 새 시퀀스를 만드는 함수 (`head → tail → [head, ..tail]`) 이고, `Apply` 두 번이 그 한 칸 붙이기를 Elevated 세계 안에서 수행합니다 (첫 `Apply` 가 `head` 자리, 둘째 `Apply` 가 `tail` 자리를 채웁니다). `Pure([])` 에서 시작해 뒤에서 앞으로 한 원소씩 붙여 나가는 이 골격은 9장의 `Traverse` 와 한 글자도 다르지 않습니다. 아래 `["1", "2", "3"]` 을 파싱하면 뒤에서 앞으로 한 칸씩 조립됩니다.

```
Pure([])     → Just([])
prepend(3)   → Just([3])
prepend(2)   → Just([2, 3])
prepend(1)   → Just([1, 2, 3])
```

문자열 시퀀스를 정수로 파싱하는 이 예에서, 두 번째 Elevated 세계 `F` 자리에 7장에서 본 `MyMaybe` 가 들어갑니다 (태그 타입 `MaybeF`).

```csharp
Func<string, K<MaybeF, int>> parse = s =>
    int.TryParse(s, out var v) ? new MyMaybe<int>.Just(v) : MyMaybe<int>.Nothing.Instance;

// ["1", "2", "3"] → Just([1, 2, 3])   — 모두 성공
// ["1", "x", "3"] → Nothing            — 하나라도 실패하면 전체 Nothing
```

하나가 `Nothing` 이면 `Apply` 규칙이 결과 전체를 `Nothing` 으로 만들어, 단락이 추가 코드 없이 따라옵니다. 9장에서 본 단락의 자동 전파 그대로입니다.

`Sequence` 는 `Traverse` 의 특수한 경우입니다. 이미 `List<Maybe<a>>` 모양이라 변환 함수가 필요 없을 때, `traverse` 에 항등 함수를 넣은 것이 `sequence` 입니다.

```csharp
// Sequence = Traverse id — 변환 없이 층만 뒤집음
static virtual K<F, K<T, A>> Sequence<F, A>(K<T, K<F, A>> tfa)
    where F : Applicative<F>
    => T.Traverse<F, K<F, A>, A>(x => x, tfa);

// [Just(1), Just(2), Just(3)] → Just([1, 2, 3])
```

`map f` 로 각 원소를 변환한 뒤 `sequence` 로 층을 뒤집는 두 단계가 곧 `traverse f` 입니다. 9장에서 본 `traverse f = sequence ∘ map f` 등식이 실무 시퀀스에서도 그대로 성립합니다.

---

## 12.5 표현이 달라도 같은 trait — cons 리스트 MyLst

`MySeq` 는 `IEnumerable` 을 백킹으로 썼습니다. 그런데 같은 trait 이 **표현이 전혀 다른** 자료에도 붙을까요. 챌린지의 `MyLst` 는 `IEnumerable` 이 아니라 재귀적 cons 구조입니다.

```csharp
public abstract record MyLst<A> : K<LstF, A>
{
    public sealed record Nil : MyLst<A> { public static readonly Nil Instance = new(); }
    public sealed record Cons(A Head, MyLst<A> Tail) : MyLst<A>;
}
```

`Cons(1, Cons(2, Cons(3, Nil)))` 처럼 머리 하나와 꼬리 리스트로 끝없이 이어지는 모양입니다. `IEnumerable` 과는 메모리 표현도 순회 방식도 완전히 다릅니다. 그런데도 같은 `Functor` / `Applicative` / `Monad` / `Foldable` 가 그대로 붙습니다.

```csharp
public sealed class LstF : Monad<LstF>, Foldable<LstF>
{
    public static K<LstF, B> Bind<A, B>(K<LstF, A> ma, Func<A, K<LstF, B>> f) =>
        MyLst<B>.FromEnumerable(
            ma.As().ToEnumerable().SelectMany(a => f(a).As().ToEnumerable()));
    // Map / Pure / Apply / FoldLeft / FoldRight 도 같은 시그니처로 부착
}
```

`Bind` 가 cons 구조에서 어떻게 도는지 한 줄씩 따라가 보면, 표현이 달라도 동작이 같음이 드러납니다.

```text
Cons(1, Cons(2, Nil)) 에 Bind(n => [n, n×10]) 를 걸면
  1 → [1, 10]  (Cons(1, Cons(10, Nil)))
  2 → [2, 20]  (Cons(2, Cons(20, Nil)))
  이어붙임 → Cons(1, Cons(10, Cons(2, Cons(20, Nil))))  = [1, 10, 2, 20]
```

`MySeq` 의 `Bind` 가 낸 `[1, 10, 2, 20]` 과 글자 그대로 같은 결과입니다. 그래서 어떤 Monad 든 받는 기초의 일반 함수가 `MySeq` 와 `MyLst` 양쪽에 한 줄도 고치지 않고 적용됩니다.

```csharp
var lstSum   = Foldable.foldLeft<LstF, int, int>((acc, n) => acc + n, 0, lst);
var lstBound = Monad.bind<LstF, int, int>(lst, n => LstF.Pure(n * 100));
```

여기서 이 장의 첫 결정적 통찰이 나옵니다. **추상은 자료의 표현이 아니라 시그니처가 약속하는 동작에 달려 있습니다.** `IEnumerable` 이든 cons 든, 시그니처의 약속만 지키면 같은 trait 의 시민입니다. toy 와 실무 사이에 경계가 처음부터 없었음을 보여 줍니다.

---

## 12.6 한 자료 타입에 두 Applicative — 데카르트 곱 vs zip

`MySeq.Apply` 는 데카르트 곱이었습니다. 그런데 시퀀스에는 **또 하나의 적법한 Applicative** 가 있습니다. 함수와 값을 `i` 번째끼리 짝지어 적용하는 zip 입니다.

```csharp
public sealed class ZipF : Functor<ZipF>
{
    public static K<ZipF, B> Map<A, B>(Func<A, B> f, K<ZipF, A> fa) =>
        new ZipSeq<B>(fa.As().Items.Select(f));

    // 함수와 값을 짝으로 맞춰 적용 (둘 중 짧은 길이까지)
    public static K<ZipF, B> Apply<A, B>(K<ZipF, Func<A, B>> mf, K<ZipF, A> ma) =>
        new ZipSeq<B>(mf.As().Items.Zip(ma.As().Items, (f, a) => f(a)));
}
```

같은 함수 `[+1, ×10]` 과 같은 값 `[1, 2]` 인데 두 Apply 의 결과가 다릅니다.

```text
데카르트 곱 Apply : [+1, ×10] ⊛ [1, 2]  =  [2, 3, 10, 20]   (모든 조합)
zip        Apply : [+1, ×10] ⊛ [1, 2]  =  [2, 20]           (i 번째끼리)
```

![시퀀스의 두 Applicative — 데카르트 곱과 zip](./images/Ch12-Sequences/01-two-applicatives.svg)

**그림 12-1. 한 시퀀스에 두 Applicative: 데카르트 곱 vs zip** — 같은 함수 시퀀스 `[+1, ×10]` 와 값 시퀀스 `[1, 2]` 에 두 `Apply` 가 서로 다른 결과를 냅니다. 왼쪽 데카르트 곱은 모든 조합 (`2×2 = 4` 개) 을, 오른쪽 zip 은 같은 자리끼리 짝지어 (`2` 개) 만듭니다. 같은 자료 타입 위에 적법한 Applicative 가 둘 공존할 수 있습니다.

이것이 두 번째 결정적 통찰입니다. **한 자료 타입에 서로 다른 추상이 둘 이상 살 수 있습니다.** Haskell 의 `[]` 와 `ZipList` 가 정확히 이 구도입니다. 어느 쪽이 옳은가가 아니라, 도메인이 "모든 조합" 을 원하는지 "짝 맞춤" 을 원하는지에 따라 고르는 선택입니다. 3장에서 같은 `int` 에 `Sum` 과 `Product` 두 Monoid 가 있었던 것과 같은 구도입니다.

> **흔한 함정** — 두 Applicative 의 `Pure` 가 같다는 오해입니다.
>
> 데카르트 곱의 `Pure(x)` 는 `[x]` 한 원소입니다. 그런데 zip 의 `Pure` 가 법칙을 지키려면 `[x, x, x, …]` 무한 반복이라야 합니다. 항등 법칙 `Apply(Pure(id), v) == v` 를 따져 보면 드러납니다. `Pure(id)` 가 `[id]` 한 원소면, 길이 2 인 `v` 와 zip 할 때 짧은 쪽 (1) 까지만 맞춰져 결과가 한 원소로 잘립니다 (`v` 와 다름). `Pure(id)` 가 무한 반복이라야 어떤 길이의 `v` 와도 모든 자리가 맞아 `v` 가 그대로 나옵니다. 그래서 학습용 `ZipSeq` 는 `Apply` 만 시연하고 `Pure` 는 두지 않습니다. 지금은 "Apply 의 의미가 둘" 이라는 것만 가져가면 충분합니다.

---

## 12.7 직접 해보기 — 챌린지

> **필수 ① — `MyLst` 에 두 법칙 검증.** cons 구조 `MyLst` 에 부착한 `Map` 이 항등 법칙 (`Map(x => x)` 가 원본) 과 합성 법칙 (`Map(g) ∘ Map(f) == Map(g ∘ f)`) 을 지키는지 종이에 먼저 적어 본 뒤 코드로 확인합니다. `IEnumerable` 백킹이 아닌데도 4장 Functor 의 두 법칙이 그대로 성립하는 이유를 한 문장으로 답해 봅니다.

> **필수 ② — LINQ 쿼리를 `Bind` 로 손번역.** `from x in xs from y in ys where x < y select (x, y)` 를 `Bind` 와 `Map` 호출로만 다시 적어 봅니다. `where` 절이 어떤 시퀀스 연산으로 바뀌는지 생각해 봅니다 (조건이 거짓인 원소가 빈 시퀀스로 펼쳐진다는 점이 힌트입니다).

> **심화 ③ — zip 의 `Pure` 는 왜 무한 반복인가.** zip Applicative 가 항등 법칙 `Apply(Pure(id), v) == v` 를 지키려면 `Pure(id)` 가 어떤 모양이어야 하는지, `[id]` 로는 왜 깨지는지 길이가 3 인 `v` 로 직접 따져 봅니다.

> **심화 ④ — 무한 시퀀스에 `traverse` 는 안전한가.** 유한 시퀀스의 `traverse` 는 모두 성공해야 성공이었습니다. 무한 시퀀스에 `traverse parse` 를 걸면 어떻게 되는지, 왜 `traverse` 는 `map` 과 달리 끝까지 당겨야 하는지 생각해 봅니다.

---

## 12.8 Elevated World 어휘로 다시 읽기

이 장에서 한 일을 1장의 두 평행 세계 어휘로 정리합니다. 시퀀스는 새 동사를 더하지 않았고, 이미 아는 자리들을 실무 시민 위에서 재확인했습니다.

| 이 장의 코드 | 1장 지도의 자리 | 한 줄 의미 |
|---|---|---|
| `MySeq.Map` | `E<a> → E<b>` (Functor, 4장) | 컨테이너 안 값만 변환, 모양 보존 |
| `MySeq.Apply` | `E<a> → E<b>` 의 다인자 확장 (Applicative, 5장) | 두 시퀀스의 모든 조합 |
| `MySeq.Bind` | `a → E<b>` 합성 되살리기 (Monad, 7장) | 한 입력이 여러 갈래로 (`SelectMany`) |
| `MySeq.FoldLeft` | `E<a> → b` 끌어내림 (Foldable, 6장) | 여러 원소를 한 값으로 (`Sum`/`Count`/`Any`) |
| `MySeq.Traverse` | 층 swap (Traversable, 9장) | `List<Maybe>` 를 `Maybe<List>` 로 |

`MyList` 에 붙였던 다섯 자리가 lazy `MySeq` 에 그대로 있고, cons `MyLst` 에도 그대로 있습니다. 표현이 달라도 시그니처가 같으면 같은 자리입니다.

---

## 12.9 Q&A — 자기 점검

> **Q1. toy `MyList` 와 실무 `MySeq` 의 진짜 차이는 무엇입니까?** (12.2절, 12.3절)
>
> trait 도 시그니처도 같습니다. 차이는 **평가 시점** 하나입니다. `MyList` 는 `Map` 호출 즉시 새 리스트를 만들고, `MySeq` 는 값이 당겨질 때까지 계산을 미룹니다 (lazy). 그래서 `MySeq` 는 큰 데이터와 무한 시퀀스를 다룰 수 있습니다.

> **Q2. lazy 가 무한 시퀀스를 다룰 수 있는 이유는 무엇입니까?** (12.3.1절)
>
> `IEnumerable` 이 당겨질 때 한 원소씩 만들기 때문입니다. 무한 시퀀스에 `Map` 을 걸어도 변환 규칙만 들고 있다가, 앞에서 다섯 개만 당기면 다섯 번만 계산합니다. 즉시 평가하는 `MyList` 는 무한 시퀀스에서 멈추지 않지만, lazy `MySeq` 는 같은 시그니처로 안전합니다.

> **Q3. `Sum` / `Count` / `Any` 는 따로 구현해야 합니까?** (12.4.4절)
>
> 아닙니다. `Foldable` 의 두 멤버 (`FoldLeft` / `FoldRight`) 만 구현하면 `Sum` / `Count` / `Any` / `All` / `First` / `ToList` 가 모두 그 위에서 자동으로 자랍니다. 6장에서 본 "두 멤버가 N 개의 일반 함수를 공짜로 준다" 의 실무 모습입니다.

> **Q4. LINQ 의 `Select` 와 `SelectMany` 는 함수형 어휘로 무엇입니까?** (12.4.3절)
>
> `Select` 는 Functor 의 `map`, `SelectMany` 는 Monad 의 `bind` 입니다. C# 컴파일러가 `from-from-select` 를 `SelectMany` 호출로 바꾸므로, 시퀀스에 대한 LINQ 쿼리는 사실 Functor + Monad 위에 서 있었습니다.

> **Q5. `traverse` 와 `sequence` 는 어떻게 다릅니까?** (12.4.5절)
>
> `sequence` 는 `traverse` 에 항등 함수를 넣은 특수한 경우입니다. 이미 `List<Maybe<a>>` 모양이면 변환 없이 층만 뒤집는 `sequence`, 원소를 변환하면서 뒤집으면 `traverse` 입니다. `traverse f = sequence ∘ map f` 가 성립합니다.

> **Q6. cons 구조 `MyLst` 에 같은 trait 이 붙는다는 사실이 왜 중요합니까?** (12.5절)
>
> 추상이 자료의 **표현** 이 아니라 **시그니처가 약속하는 동작** 에 달려 있음을 증명하기 때문입니다. `IEnumerable` 이든 cons 든, 약속만 지키면 같은 trait 의 시민입니다. toy 와 실무 사이에 경계가 없었다는 뜻입니다.

> **Q7. 데카르트 곱 `Apply` 와 zip `Apply` 는 어떻게 다릅니까?** (12.6절)
>
> 시그니처는 같고 의미가 다릅니다. 데카르트 곱은 함수와 값의 **모든 조합** 을, zip 은 **같은 자리끼리** 만 적용합니다. 한 자료 타입에 적법한 Applicative 가 둘 공존할 수 있다는 첫 사례입니다.

> **Q8. 같은 시퀀스에 두 Applicative 가 있다면 어느 것이 맞습니까?** (12.6절)
>
> 어느 쪽도 틀리지 않습니다. 도메인이 "모든 조합" 을 원하면 데카르트 곱, "짝 맞춤" 을 원하면 zip 을 고릅니다. 3장에서 같은 `int` 에 `Sum` 과 `Product` 두 Monoid 가 있었던 것과 같은 구도입니다.

---

## 12.10 요약

- **기초의 다섯 trait 은 toy 가 아니라 실무 컬렉션의 골격이었습니다.** `MyList` 에 부착한 Functor / Applicative / Monad / Foldable / Traversable 이 실무 lazy 시퀀스 `MySeq` 에 글자 그대로 다시 붙습니다 (12.1절, 12.4절).
- **toy 와 실무의 차이는 trait 이 아니라 평가 시점입니다.** `MyList` 는 즉시, `MySeq` 는 lazy 로 계산을 미뤄 큰 데이터와 무한 시퀀스를 다룹니다 (12.2절, 12.3절).
- **`Sum` / `Count` / `Any` 는 `Foldable` 두 멤버 위에서 공짜로 자랍니다.** 따로 구현하지 않습니다 (12.4.4절).
- **LINQ 의 `Select` / `SelectMany` 는 `map` / `bind` 의 다른 이름입니다.** 시퀀스 쿼리는 Functor + Monad 위에 서 있었습니다 (12.4.3절).
- **`traverse` 와 `sequence` 는 "모두 성공해야 성공" 을 단락 코드 없이 구현합니다.** `List<Maybe>` 를 `Maybe<List>` 로 뒤집습니다 (12.4.5절).
- **추상은 표현이 아니라 시그니처에 달려 있습니다.** cons 구조 `MyLst` 에도 같은 trait 이 그대로 붙습니다 (12.5절).
- **한 자료 타입에 두 Applicative 가 공존합니다.** 데카르트 곱과 zip 은 같은 시그니처, 다른 의미입니다 (12.6절).

이 장의 단일 목표는 하나였습니다. **기초에서 손으로 만든 추상이 실무 컬렉션에서 그대로 작동함을 코드로 확인한다.**

---

## 12.11 다음 장으로

시퀀스는 순서가 있는 컬렉션이었습니다. 다음 장은 키-값 컬렉션 `MyMap` 과 집합 `MySet` 으로 넘어갑니다. 거기서 `Functor` 의 `map` 이 키-값 컨테이너에서는 **값에만** 작용하고 키는 보존되는 자리를 보고, 더 나아가 **`Set` 에는 `Functor` 가 깔끔히 붙지 않는다** 는 경계 사례를 만납니다. trait 이 어디에 붙고 어디에 붙지 않는지, 그 경계를 보는 것이 다음 장입니다.

> **실무 디딤돌** — `MySeq` 의 lazy `Map` / `Bind` 는 후속 Part 의 스트리밍 처리 (`IAsyncEnumerable`, `Source`) 로 이어지는 디딤돌입니다. 계산을 미루는 시퀀스가 무한·비동기 데이터 흐름의 토대가 됩니다.
>
> **테스트 디딤돌** — 이 장의 Monad 세 법칙과 Foldable 일관성 검증은 11부의 property-based 테스트로 옮겨가, 손으로 고른 몇 입력이 아니라 임의 시퀀스 수백 건에 법칙을 자동으로 검증합니다.
