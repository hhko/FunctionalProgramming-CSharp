# 11장 — NaturalTransformation

> 이 장에서 다룰 주제 — 4장 Functor 의 `map` 이 컨테이너 안의 값을 바꿨다면, NaturalTransformation 은 컨테이너 자체를 다른 컨테이너로 바꿉니다 (`K<F, A> → K<G, A>`, 값 타입 `A` 는 유지). 1부에서 모은 모든 어휘가 컨테이너 사이의 다리로 마무리되는 자리입니다.

## 학습 목표

- 값을 바꾸는 `map` 과 컨테이너를 바꾸는 NaturalTransformation 의 차이를 시그니처로 구분할 수 있습니다.
- `Transform : K<F, A> → K<G, A>` 를 `Natural<F, G>` trait 으로 직접 구현할 수 있습니다.
- 자연성 법칙이 왜 변환과 `map` 의 순서를 바꿔도 같은 결과를 보장하는지 설명할 수 있습니다.
- 9장 Traversable 의 `sequence` 가 사실 NaturalTransformation 의 한 형태였음을 설명할 수 있습니다.

---

## §11.1 목적 — 컨테이너 자체를 바꾸기

지금까지의 trait 은 모두 컨테이너를 고정한 채 안쪽을 다뤘습니다. 4장 Functor 의 `map` 은 `MyList<int>` 를 `MyList<string>` 으로 바꿨지만, 컨테이너는 여전히 `MyList` 였습니다. 안의 값만 변하고 컨테이너 종류는 그대로입니다.

그런데 컨테이너 종류 자체를 바꾸고 싶을 때가 있습니다. `MyList<int>` 의 첫 원소만 꺼내 `MyMaybe<int>` 로 옮기거나 (있으면 `Just`, 비어 있으면 `Nothing`), `MyMaybe<int>` 를 원소 0개 또는 1개의 `MyList<int>` 로 펼치는 변환입니다.

```text
headOrNone : MyList<a>  → MyMaybe<a>     값 a 는 그대로, 컨테이너만 List → Maybe
toList     : MyMaybe<a> → MyList<a>      값 a 는 그대로, 컨테이너만 Maybe → List
```

두 변환 모두 안의 값 타입 `a` 는 손대지 않습니다. 바뀌는 것은 컨테이너 (`F`) 자체입니다. 이것이 NaturalTransformation 입니다. Functor 의 `map` 과 방향이 직교합니다.

---

## §11.2 NaturalTransformation — 컨테이너를 옮기는 변환

NaturalTransformation 의 멤버는 변환 하나입니다.

```text
Transform : K<F, A> → K<G, A>
```

컨테이너 `F` 에 담긴 값을 컨테이너 `G` 로 옮깁니다. 값 타입 `A` 는 입력과 출력에서 같습니다. 컨테이너만 `F` 에서 `G` 로 바뀝니다.

`map` 과 나란히 두면 두 축이 직교한다는 것이 또렷합니다.

| | 무엇을 바꾸나 | 시그니처 | 고정되는 것 |
|---|---|---|---|
| **Functor `map`** | 컨테이너 안의 값 | `K<F, A> → K<F, B>` | 컨테이너 `F` |
| **NaturalTransformation** | 컨테이너 자체 | `K<F, A> → K<G, A>` | 값 타입 `A` |

`map` 은 컨테이너를 세로축으로 고정하고 값을 가로로 바꾸고, NaturalTransformation 은 값을 가로축으로 고정하고 컨테이너를 세로로 바꿉니다. 1부에서 다룬 trait 이 가로축의 어휘였다면, NaturalTransformation 은 세로축의 어휘입니다.

---

## §11.3 trait 직접 구현 — `Natural<F, G>`

마커 `K<F, A>` 를 입력으로 받아 `K<G, A>` 를 돌려주는 변환을 약속합니다. self-bound 가 없는 대신, 출발 컨테이너 `F` 와 도착 컨테이너 `G` 두 타입 인자를 받습니다.

```csharp
public interface Natural<out F, in G>
{
    // F 컨테이너의 값을 G 컨테이너로 옮깁니다. 값 타입 A 는 유지됩니다.
    static abstract K<G, A> Transform<A>(K<F, A> fa);
}
```

`Transform` 의 타입 매개변수는 `A` 하나뿐입니다. 변환이 값 타입과 무관하게 (`A` 가 무엇이든) 같은 방식으로 작동해야 한다는 뜻입니다. `MyList<int>` 든 `MyList<string>` 든 첫 원소를 꺼내는 방식은 똑같습니다. 이 무관함이 다음 절의 자연성 법칙으로 형식화됩니다. 이 시그니처는 LanguageExt v5 의 `Natural<out F, in G>` 와 정확히 정합합니다.

### `out F, in G` — source/target 의 카테고리적 비대칭

`out F, in G` 라는 variance 표기는 일반 C# variance 어법과 반대 방향으로 보입니다. `Transform` 의 시그니처만 보면 F 는 매개변수 (입력), G 는 반환 (출력) — 일반 variance 규칙으로는 `<in F, out G>` 가 자연스럽기 때문입니다. v5 가 반대로 표기하는 이유는 함수형 정통의 카테고리 이론을 따르기 때문입니다.

수학의 natural transformation 은 *Functor F: C → D* 와 *Functor G: C → D* 사이의 변환 `α: F ⇒ G`. 두 functor 의 카테고리적 위치가 비대칭입니다.

- **`F`** — source functor (변환의 출발) — `out F` 로 시각화
- **`G`** — target functor (변환의 도착) — `in G` 로 시각화

Haskell 의 `type Nat f g = forall a. f a -> g a` 에서 `f` 는 source, `g` 는 target. v5 의 `out F, in G` 는 이 source/target 비대칭을 C# variance 어휘로 카테고리적으로 박은 자리입니다.

### 어떻게 컴파일을 통과하는가 — K 마커의 contravariant 가 만든 variance 반전

`Transform` 의 시그니처만 보면 `<out F, in G>` 가 컴파일러 검사를 통과할 수 없어야 합니다. F 가 매개변수 자리에 등장하고 G 가 반환 자리에 등장하기 때문입니다. 그럼에도 v5 가 통과하는 핵심은 **`K<F, A>` 마커가 `K<in F, A>` 로 정의된다는 점** 입니다.

```csharp
// v5 의 K 마커 — F 가 contravariant
public interface K<in F, A> { }
```

`K<in F, A>` 의 `in F` (contravariant 표기) 가 variance 반전을 일으킵니다.

- `Transform` 매개변수 `K<F, A>` 안의 `F` — K 자체가 contravariant context, 그 안에서 F 가 매개변수 자리 → **contravariant × 매개변수 위치 = covariant** → `out F` 정합.
- `Transform` 반환 `K<G, A>` 안의 `G` — K 자체가 contravariant context, 그 안에서 G 가 반환 자리 → **contravariant × 반환 위치 = contravariant** → `in G` 정합.

즉 K 의 contravariant 표기가 두 단계 variance 반전을 만들어, 카테고리 이론의 source/target 어휘를 컴파일러 검사에 통과시킵니다. **v5 의 K 마커가 `K<in F, A>` 인 이유 중 하나가 정확히 이 자리** — Natural 같은 trait 의 카테고리적 어휘를 가능하게 합니다.

### family 의 카테고리적 비대칭 vs 단일 함수의 variance

*Natural<F, G>* 는 단일 함수가 아니라 모든 A 에 대해 동일하게 작동하는 함수의 family (Haskell 의 `forall a.` rank-2 polymorphism) 입니다. C# 의 함수 타입 `Func<in A, out B>` 의 단일 함수 input/output variance 와는 다른 의미를 가지며, *family 의 카테고리적 source/target 비대칭* 을 표기합니다.

C# 일반 variance 어법 (out=반환, in=매개변수) 에 익숙한 학습자에게는 처음 어색하지만, 함수형 정통의 카테고리 이론을 시그니처에 박는 v5 의 어법은 실무 코드에서 자연스럽게 만납니다. 본질은 그대로입니다 — F 컨테이너에서 G 컨테이너로 옮기는 변환, 값 A 는 유지.

---

## §11.4 예제 — MyList ↔ MyMaybe

두 변환을 직접 구현합니다. 먼저 리스트의 첫 원소를 꺼내는 변환입니다.

```csharp
// MyList → MyMaybe : 첫 원소가 있으면 Just, 없으면 Nothing
public sealed class ListToMaybe : Natural<MyListF, MyMaybeF>
{
    public static K<MyMaybeF, A> Transform<A>(K<MyListF, A> fa)
    {
        var list = (MyList<A>)fa;
        return list.IsEmpty
            ? MyMaybe<A>.Nothing
            : MyMaybe<A>.Just(list.Head);
    }
}
```

반대 방향은 `MyMaybe` 를 원소 0개 또는 1개의 리스트로 펼칩니다.

```csharp
// MyMaybe → MyList : Just 면 원소 1개, Nothing 이면 빈 리스트
public sealed class MaybeToList : Natural<MyMaybeF, MyListF>
{
    public static K<MyListF, A> Transform<A>(K<MyMaybeF, A> fa)
    {
        var maybe = (MyMaybe<A>)fa;
        return maybe.IsJust
            ? MyList<A>.Of(maybe.Value)
            : MyList<A>.Empty;
    }
}
```

```csharp
ListToMaybe.Transform(MyList<int>.Of(1, 2, 3));   // Just(1)
ListToMaybe.Transform(MyList<int>.Empty);         // Nothing
MaybeToList.Transform(MyMaybe<int>.Just(7));       // [7]
MaybeToList.Transform(MyMaybe<int>.Nothing);       // []
```

두 `Transform` 모두 값 `int` 를 건드리지 않습니다. 컨테이너만 옮깁니다. `map` 이 값을 변환하는 것과 정확히 직교합니다.

---

## §11.5 자연성 법칙 — 변환과 map 의 순서 무관

NaturalTransformation 이 진짜 자연 변환이려면 한 가지 법칙을 지켜야 합니다. 자연성 법칙입니다. 값을 먼저 변환하고 컨테이너를 옮기든, 컨테이너를 먼저 옮기고 값을 변환하든 결과가 같아야 합니다.

```text
Transform(F.Map(f, fa)) == G.Map(f, Transform(fa))
```

왼쪽은 출발 컨테이너 `F` 에서 `map(f)` 를 적용한 뒤 `G` 로 옮긴 것이고, 오른쪽은 먼저 `G` 로 옮긴 뒤 `G` 에서 `map(f)` 를 적용한 것입니다. 두 경로가 같은 결과를 낸다는 약속입니다.

`ListToMaybe` 로 확인하면, 리스트에 `f` 를 적용한 뒤 첫 원소를 꺼내든, 첫 원소를 꺼낸 뒤 `f` 를 적용하든 같은 `Just(f(head))` 가 나옵니다. 이 법칙이 성립해야 변환이 값과 무관하게 컨테이너 구조만 다룬다는 것이 보장됩니다. §11.3 에서 `Transform` 의 타입 매개변수가 `A` 하나뿐이었던 이유가 이것입니다.

---

## §11.6 sequence 는 NaturalTransformation 입니다

9장 Traversable 의 `sequence` 를 다시 봅니다. `sequence` 는 두 컨테이너의 층 순서를 뒤집었습니다.

```text
sequence : MyList<E<a>> → E<MyList<a>>
```

이 시그니처를 NaturalTransformation 의 눈으로 보면, 바깥에서 본 컨테이너가 `MyList∘E` 에서 `E∘MyList` 로 바뀌고 안의 값 `a` 는 그대로입니다. 즉 `sequence` 는 합성 컨테이너 `MyList<E<_>>` 를 합성 컨테이너 `E<MyList<_>>` 로 옮기는 자연 변환입니다. 9장에서 본 층 순서 뒤집기가 사실 이 장의 어휘로 다시 설명됩니다.

1부의 마지막 자리에서 앞의 어휘들이 한데 모입니다. Functor 의 `map` (가로축, 값 변환) 과 NaturalTransformation (세로축, 컨테이너 변환) 이 직교축을 이루고, Traversable 의 `sequence` 가 그 교차점에 놓입니다. 1부에서 모은 모든 도구가 컨테이너 사이의 다리로 마무리됩니다.

---

## §11.7 Q&A

> **Q1. `map` 과 NaturalTransformation 은 어떻게 다릅니까?**

`map` 은 컨테이너를 고정한 채 안의 값을 바꿉니다 (`K<F, A> → K<F, B>`). NaturalTransformation 은 값 타입을 고정한 채 컨테이너 자체를 바꿉니다 (`K<F, A> → K<G, A>`). 두 축이 직교합니다. `map` 은 가로 (값), NaturalTransformation 은 세로 (컨테이너) 입니다.

> **Q2. `Transform` 의 타입 매개변수가 왜 `A` 하나뿐입니까?**

변환이 값 타입과 무관하게 작동해야 하기 때문입니다. `MyList<int>` 든 `MyList<string>` 든 첫 원소를 꺼내는 방식은 똑같습니다. 값 `A` 가 무엇이든 같은 방식으로 컨테이너만 옮긴다는 제약이 타입 매개변수 `A` 하나로 표현되고, 자연성 법칙으로 형식화됩니다.

> **Q3. 자연성 법칙이 깨지면 어떻게 됩니까?**

변환이 값에 따라 다르게 작동한다는 뜻이 됩니다. 예를 들어 첫 원소를 꺼내되 값이 짝수일 때만 꺼낸다면, `map` 으로 값을 바꾼 뒤와 바꾸기 전의 결과가 달라져 법칙이 깨집니다. 자연성 법칙은 변환이 오직 컨테이너 구조만 다룬다는 보장입니다.

> **Q4. `sequence` 가 NaturalTransformation 이라는 말은 무슨 뜻입니까?**

`sequence : MyList<E<a>> → E<MyList<a>>` 를 보면 값 `a` 는 그대로이고 바깥 컨테이너가 `MyList<E<_>>` 에서 `E<MyList<_>>` 로 바뀝니다. 컨테이너만 옮기고 값을 유지하므로 NaturalTransformation 의 정의에 정확히 들어맞습니다. 9장의 층 순서 뒤집기가 이 장의 어휘로 다시 설명됩니다.

> **Q5. NaturalTransformation 이 1부의 마지막에 오는 이유는 무엇입니까?**

앞의 모든 어휘가 여기서 만나기 때문입니다. Functor 의 값 변환과 직교하는 컨테이너 변환을 보고, 9장 Traversable 의 `sequence` 가 그 한 형태였음을 확인합니다. 1부에서 한 컨테이너 안을 다루던 어휘가, 컨테이너 사이를 잇는 다리로 마무리되는 자리입니다.

> **Q6. `out F, in G` 라는 variance 표기는 왜 일반 C# 어법과 반대입니까?**

C# 의 일반 variance 어법은 `out` 이 반환 자리, `in` 이 매개변수 자리를 의미합니다. `Transform` 의 시그니처는 F 가 매개변수, G 가 반환이므로 일반 규칙으로는 `<in F, out G>` 가 자연스럽습니다. v5 가 반대로 표기하는 이유는 카테고리 이론의 source/target 비대칭을 어휘로 보존하기 때문입니다 — F 가 변환의 출발 (source), G 가 변환의 도착 (target). 이 표기가 컴파일을 통과하는 핵심은 **K 마커의 `K<in F, A>` contravariant 표기** 가 variance 반전을 만들기 때문입니다 — K 안의 F 가 contravariant 이라, Transform 매개변수 위치의 F 는 *contravariant × 매개변수 = covariant* 로 반전되어 `out F` 가 정합, 반환 위치의 G 는 *contravariant × 반환 = contravariant* 로 반전되어 `in G` 가 정합합니다. Haskell 의 `Nat f g = forall a. f a -> g a` 에서 f 가 source, g 가 target 인 카테고리적 어휘를 C# variance 로 옮긴 자리이고, K 마커의 contravariant 표기가 이를 가능하게 합니다. 자세한 풀이는 §11.3 의 *K 마커의 contravariant 가 만든 variance 반전* 절을 봅니다.

---

## §11.8 요약

- NaturalTransformation 은 컨테이너 자체를 다른 컨테이너로 옮기는 변환입니다 (`Transform : K<F, A> → K<G, A>`, 값 타입 `A` 유지).
- 4장 Functor 의 `map` (값 변환, 컨테이너 고정) 과 직교합니다. `map` 은 가로축, NaturalTransformation 은 세로축입니다.
- `Natural<out F, in G>` 는 출발 컨테이너 `F` (source) 와 도착 컨테이너 `G` (target) 두 인자를 받고, `Transform` 의 타입 매개변수는 값 `A` 하나뿐입니다. variance 표기 `out F, in G` 는 카테고리 이론의 source/target 비대칭을 시각화하며, K 마커의 `K<in F, A>` contravariant 표기가 variance 반전을 만들어 컴파일을 통과시킵니다. LanguageExt v5 와 정확히 정합.
- 자연성 법칙 (`Transform(map(f, fa)) == map(f, Transform(fa))`) 이 변환이 값과 무관하게 컨테이너 구조만 다룬다는 것을 보장합니다.
- 9장 Traversable 의 `sequence` 가 사실 NaturalTransformation 의 한 형태입니다. 1부의 모든 어휘가 컨테이너 사이의 다리로 마무리됩니다.
