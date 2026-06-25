# 14 장. Alternative & MonoidK (선택과 결합)

> **이 장의 목표** — 이 장을 읽고 나면 기초에 없던 두 새 추상을 직접 구현할 수 있습니다. 여러 Elevated 후보 중 하나를 고르는 `Choose` (Choice), 두 Elevated 값을 합치는 `Combine` 과 그 항등원 `Empty` (SemigroupK / MonoidK) 입니다. `Combine` 과 `Choose` 가 시그니처는 똑같은데 의미가 다르다는 것을, 시퀀스에서는 둘이 갈라지고 (`concat` vs 첫 비어있지 않은 쪽) Maybe 에서는 우연히 일치한다는 (둘 다 첫 `Just`) 대비로 봅니다. 3장의 `Monoid` 가 어떻게 Elevated World 로 올라가는지, `Choose` 가 fallback (`x ?? value`) 과 파서 결합자의 토대가 되는지, 그리고 그 약속을 고정하는 세 법칙까지 손에 쥡니다.

> **이 장의 핵심 어휘**
>
> - **`Choose`**: 두 Elevated 후보 중 성공하는 쪽을 고르는 연산 (`K<F, a> → K<F, a> → K<F, a>`)
> - **`Combine`**: 두 Elevated 값을 합치는 연산. 같은 시그니처지만 "고르기" 가 아니라 "모으기"
> - **`SemigroupK` / `MonoidK`**: 3장 `Semigroup` / `Monoid` 를 Elevated World 로 끌어올린 trait. `MonoidK` 는 항등원 `Empty` 를 더합니다
> - **`Alternative`**: `Choice` 와 `Applicative` 를 상속하고 `Empty` 를 직접 선언하는 trait. "여러 시도 중 하나, 전부 실패하면 `Empty`"
> - **`OneOf` / `option`**: 여러 후보 중 첫 성공 / 실패하면 기본값으로 떨어지는 fallback (`x ?? value`)
> - **`guard`**: 조건이 참이면 `Pure(unit)`, 거짓이면 `Empty`. 조건 필터·`where` 절의 토대

> 이 장을 마치면 할 수 있게 되는 것
> - [ ] `SemigroupK` / `MonoidK` 를 직접 정의해 두 Elevated 값을 합치고 항등원을 둘 수 있습니다.
> - [ ] 3장 `Monoid` 의 결합이 Elevated World 로 올라간 형태가 `MonoidK` 임을 시그니처로 설명할 수 있습니다.
> - [ ] `Choice` 의 `Choose` 로 여러 후보 중 하나를 고르고, `option` 으로 fallback 을 구현할 수 있습니다.
> - [ ] `Combine` 과 `Choose` 가 시퀀스에서는 다르고 Maybe 에서는 같은 이유를 설명할 수 있습니다.
> - [ ] `Alternative` 가 `Choice` + `Applicative` + `Empty` 의 합류임을 보이고 `OneOf` 를 구현할 수 있습니다.
> - [ ] Alternative 의 세 법칙 (좌 zero / 우 zero / 좌 catch) 으로 "첫 성공" 의 약속을 검증할 수 있습니다.
> - [ ] `guard` 가 조건 분기·필터의 토대이고, `Choose` 가 파서 결합자의 토대임을 설명할 수 있습니다.

---

## 14.1 이 장에서 다루는 것 — 기초에 없던 두 동사


시작하기 전에 이 장 내내 쓸 말 몇 개를 짧게 다시 꺼내 둡니다. 1부에서 본 어휘라 이미 손에 익었겠지만, 잊었다고 가정하고 한 줄씩 상기합니다. 부담 없이 읽고 넘어가도 됩니다.

- **Normal World** 는 `int`, `string` 같은 평범한 값과 함수가 사는 아래 층입니다. **Elevated World** 는 그 값을 컨테이너 한 겹으로 감싼 값들이 사는 위 층입니다. `MyMaybe<int>` (없을 수 있음), `MySeq<int>` (여러 개일 수 있음) 가 그 시민입니다. 이 책은 어떤 Elevated 컨테이너든 `E<a>` 로 적습니다. `K<F, a>` 는 그 `E<a>` 를 C# 코드로 옮긴 표기입니다. `F` 가 컨테이너의 종류, `a` 가 안에 든 값의 타입입니다.
- **trait** 은 능력을 객체가 아니라 타입에 부착해 정의하는 자리입니다. C# 의 interface 와 같은 발상이라고 보면 됩니다. `MySeq` 가 어떤 trait 을 부착하면 그 trait 의 능력을 공짜로 얻습니다.
- **끌어올림 (lift)** 은 Normal World 의 값이나 함수를 Elevated World 로 올리는 일입니다. 1부에서 본 함수형의 본질이 바로 이 끌어올림이었습니다.
- **Monoid** 는 3장에서 본 trait 입니다. 같은 타입 두 값을 한 값으로 합치는 연산 (`Combine`) 과, 합쳐도 상대를 바꾸지 않는 값 (`Empty`, 항등원) 을 가집니다. `int` 의 덧셈과 `0` 이 그 예였습니다.

이 네 단어 (Elevated, trait, 끌어올림, Monoid) 가 이 장의 토대입니다. 지금 다 외우지 않아도 됩니다. 본문에서 다시 만날 때마다 그 자리에서 또 풀어 드립니다.


여기까지 4부는 기초의 trait 이 실무 컬렉션에 그대로 붙는다는 것을 확인했습니다 (시퀀스), 그리고 trait 이 붙는 자리에 경계가 있다는 것을 보았습니다 (맵·집합). 이 장은 한 걸음 더 나아가, **기초에 없던 새 추상 두 개** 를 더합니다.

첫째는 **선택** 입니다. 여러 Elevated 후보 중 성공하는 쪽 하나를 고르는 `Choose` 입니다. 파서가 "이 규칙 아니면 저 규칙" 을 시도하거나, "값이 있으면 그 값, 없으면 대체값" 을 고르는 자리의 일반형입니다. 둘째는 **결합** 입니다. 두 Elevated 값을 하나로 합치는 `Combine` 과 그 항등원 `Empty` 입니다. 두 시퀀스를 이어붙이거나, 여러 조각을 모으는 자리의 일반형입니다.

이 두 동사는 1장에서 그린 지도에 새 화살표를 그리지 않습니다. 잠깐 그 지도를 떠올려 봅니다. 1장에서 두 세계를 오가는 함수가 네 가지 모양이라고 했습니다. `a → b` (그대로), `a → E<b>` (올라감), `E<a> → b` (내려옴), `E<a> → E<b>` (위에서 위로) 였습니다. `map` 은 안의 값 하나를 바꾸는 도구였고, `bind` 는 효과를 이어 붙이는 도구였습니다.

이 장의 두 동사는 그 네 자리 중 어디에도 새로 들어가지 않습니다. 둘 다 모양이 같습니다. 이미 Elevated 인 값 둘을 받아 Elevated 값 하나를 냅니다.

```
E<a> → E<a> → E<a>
────   ────   ────
첫째    둘째    결과
(모두 Elevated 시민)
```

`map` 이 값 하나를 바꾸고 `bind` 가 효과 둘을 사슬로 이었다면, `Choose` 와 `Combine` 은 이미 위 층에 있는 두 값을 받아 하나로 **합칩니다**. 새 세계로 건너가는 게 아니라, 같은 위 층 안에서 두 값을 다루는 동사입니다. 그래서 1장 지도에 새 화살표가 필요 없습니다.

그리고 이 결합은 새로 만들어진 것이 아닙니다. 3장에서 본 `Monoid` 가 Normal World 의 값을 합쳤다면, 이 장의 `MonoidK` 는 **같은 발상을 Elevated World 로 끌어올린** 것입니다. 기초의 어휘가 한 층 위에서 다시 나타납니다.

이 장에 trait 이 넷 (`Choice`, `SemigroupK`, `MonoidK`, `Alternative`) 나오지만, 진짜 외울 것은 한 문장입니다. **`Combine` 은 모으기, `Choose` 는 고르기, 둘은 시그니처가 같다.** 나머지는 이 한 문장의 변주입니다.

---

## 14.2 왜 필요한가 — "고르기" 와 "모으기" 가 막히는 자리

여러 후보 중 첫 성공을 고르는 일은 명령형에서 if-else 연쇄가 됩니다.

```csharp
// 세 곳에서 차례로 사용자를 찾아, 처음 찾은 것을 쓴다 — 명령형
User? FindUser(int id)
{
    var fromCache = cache.Get(id);
    if (fromCache != null) return fromCache;
    var fromDb = db.Get(id);
    if (fromDb != null) return fromDb;
    var fromApi = api.Get(id);
    if (fromApi != null) return fromApi;
    return null;
}
```

이 코드를 천천히 읽어 봅니다. 후보가 셋 (`cache`, `db`, `api`) 인데, 각 후보마다 `var x = …; if (x != null) return x;` 두 줄이 똑같이 복사됩니다. 후보가 넷이 되면 그 두 줄이 한 벌 더 붙고, 다섯이 되면 또 한 벌 붙습니다.

무엇이 불편한지 한 문장으로 짚으면 이렇습니다. "여러 후보를 차례로 시도하고 처음 성공한 것을 쓴다" 는 발상은 하나인데, 코드는 그 발상을 후보 수만큼 베껴 적고 있습니다. 발상은 하나, 코드는 N 벌입니다.

3장에서 똑같은 종류의 불편을 봤습니다. 거기서는 "여러 값을 하나로 합친다" 는 발상을 도메인마다 베껴 적는 것이 문제였고, 그 베끼기를 `Monoid` 라는 trait 한 정의로 거둬들였습니다. 여기서도 같은 일을 합니다. "첫 성공을 고른다" 는 발상을 코드 구조가 담아내지 못하는 것이 불편의 정체이고, 이 장의 `Choose` 가 그 발상을 한 자리로 거둬들입니다.

두 컬렉션을 합치는 일도 보겠습니다. 여러 시퀀스를 하나로 이어붙이는 코드는 보통 이런 모양입니다.

```csharp
// 여러 시퀀스를 하나로 이어붙이기 — 명령형
var all = new List<int>();           // 빈 것에서 시작
foreach (var part in parts)
    all.AddRange(part);              // 하나씩 모은다
```

빈 것에서 시작해 하나씩 모은다. 이 패턴은 3장에서 본 `Monoid` 그대로입니다. 거기서도 `0` (빈 것) 에서 시작해 값을 하나씩 더했습니다. 모양이 똑같습니다.

그런데 한 가지가 다릅니다. 3장의 `Monoid` 는 `int`, `string` 같은 Normal World 의 평범한 값만 합쳤습니다 (`int` 셋을 더해 `int` 하나). 지금 합치려는 것은 `MySeq<int>` 같은 컨테이너 자체입니다 (시퀀스 둘을 이어 시퀀스 하나). 합칠 대상이 Normal 값이 아니라 Elevated 시민입니다.

그래서 3장의 결합을 그대로 쓸 수 없습니다. 결합을 한 층 위, Elevated World 로 끌어올려야 합니다. 그 끌어올린 결합이 이 장의 두 번째 추상입니다.

이 두 자리 (여러 후보 중 고르기, 여러 컨테이너 모으기) 를 trait 으로 묶는 것이 이 장의 두 새 추상입니다.

---

## 14.3 SemigroupK / MonoidK — 기초 Monoid 의 Elevated 판

3장을 잠깐 되짚습니다. `Semigroup` 은 같은 타입 두 값을 한 값으로 합치는 능력이었습니다 (`A → A → A`, 예: `int` 둘을 더해 `int` 하나). `Monoid` 는 거기에 항등원 `Empty` 를 더한 것이었습니다 (`int` 의 항등원은 덧셈의 `0`).

이 장의 `SemigroupK` 와 `MonoidK` 는 정확히 그 두 trait 을 한 층 위로 끌어올린 것입니다. 합치는 대상이 Normal 값 `A` 에서 Elevated 시민 `K<M, A>` 로 바뀝니다.

끝에 붙은 `K` 한 글자가 그 "한 층 위" 를 가리킵니다. `K` 는 Kind 의 머리글자이고, 2장에서 본 `K<F, A>` 의 그 `K` 와 같은 어원입니다. 이름에 `K` 가 붙으면 "완성 타입 `A` 가 아니라 컨테이너 `K<M, A>` 를 다룬다" 는 표지로 읽으면 됩니다. `Semigroup` 에 `K` 가 붙어 `SemigroupK`, `Monoid` 에 `K` 가 붙어 `MonoidK` 입니다. 이름만 봐도 "3장 그것의 Elevated 판" 이라는 신호입니다.

```csharp
// 3장 Semigroup (Normal):    A      → A      → A
// 이 장 SemigroupK (Elevated): K<M, A> → K<M, A> → K<M, A>
public interface SemigroupK<M> where M : SemigroupK<M>
{
    static abstract K<M, A> Combine<A>(K<M, A> lhs, K<M, A> rhs);
}

public interface MonoidK<M> : SemigroupK<M> where M : MonoidK<M>
{
    static abstract K<M, A> Empty<A>();
}
```


두 인터페이스를 한 줄씩 읽어 봅니다. `SemigroupK<M>` 은 `Combine` 하나만 요구합니다. 두 컨테이너 `lhs` 와 `rhs` 를 받아 하나로 합쳐 돌려줍니다. `MonoidK<M>` 은 `SemigroupK<M>` 을 상속하면서 `Empty` 를 하나 더 얹습니다. 인자 없이 "빈 컨테이너" 를 돌려주는 자리입니다. 3장에서 `Semigroup` 위에 `Monoid` 가 `Empty` 를 얹은 그 계단과 글자 그대로 같은 모양입니다.

`where M : MonoidK<M>` 이라는 제약이 낯설다면 2장을 떠올립니다. "이 trait 을 부착하는 타입은 자기 자신을 `M` 자리에 넣는다" 는 self-bound 약속이었습니다. 지금은 이 줄이 trait 을 C# 으로 적는 정해진 틀이라는 것만 알면 충분합니다.


![3장 Monoid 가 Elevated World 로 — MonoidK](./images/Ch14-Alternative/01-monoid-to-monoidk.svg)

**그림 14-1. `Monoid` 의 결합이 Elevated World 로 올라간 `MonoidK`** — 아래 Normal World 의 `Combine : a → a → a` (3장, 예: `int` 덧셈) 가 가운데 끌어올림으로 위 Elevated World 의 `Combine : K<M, a> → K<M, a> → K<M, a>` (예: 두 시퀀스 이어붙이기) 가 됩니다. 항등원도 `0` 같은 Normal 값에서 빈 시퀀스 같은 Elevated 값 `Empty` 로 함께 올라갑니다. 3장의 결합 발상이 한 층 위에서 그대로 반복됩니다.

`MyMaybe` 에 부착하면 `Empty` 는 `Nothing`, `Combine` 은 좌 편향 (왼쪽이 `Just` 면 왼쪽) 입니다.

```csharp
public static K<MaybeF, A> Empty<A>() => MyMaybe<A>.Nothing.Instance;
public static K<MaybeF, A> Combine<A>(K<MaybeF, A> lhs, K<MaybeF, A> rhs) =>
    lhs.As() is MyMaybe<A>.Just ? lhs : rhs;
```

이제 `MonoidK` 의 법칙을 봅니다. 외울 것은 없습니다. 3장에서 본 그 두 법칙이 한 층 위에서 똑같이 반복될 뿐입니다.

- **좌 항등원**: `Combine(Empty, x)` 는 `x` 입니다. 빈 것을 왼쪽에 합쳐도 상대가 그대로 남습니다.
- **우 항등원**: `Combine(x, Empty)` 도 `x` 입니다. 빈 것을 오른쪽에 합쳐도 마찬가지입니다.
- **결합법칙**: 셋 이상을 합칠 때 어느 쪽부터 묶어도 결과가 같습니다. `Combine(Combine(a, b), c)` 와 `Combine(a, Combine(b, c))` 가 같은 값입니다.

3장에서 `0` 이 덧셈 사슬에서 보이지 않는 값이었던 것과 똑같습니다. 여기서는 `Empty` (빈 시퀀스, `Nothing`) 가 결합 사슬에서 보이지 않는 값입니다. 어디에 끼워도 결과를 바꾸지 않습니다.

```
Combine(Nothing, Just 5)  = Just 5   (좌 항등원: Empty 가 왼쪽이면 오른쪽)
Combine(Just 5, Nothing)  = Just 5   (우 항등원: Empty 가 오른쪽이면 왼쪽)
```

---

## 14.4 Choice — 여러 후보 중 하나를 고른다

이제 두 번째 동사 "고르기" 로 넘어갑니다. `Choice` 라는 trait 의 `Choose` 가 그 동사입니다. 두 Elevated 후보 중 성공하는 쪽을 고릅니다. 왼쪽이 성공이면 왼쪽, 왼쪽이 실패면 오른쪽입니다.

여기서 잠깐 멈춰 시그니처를 보면 흥미로운 일이 일어납니다. `Choose` 의 시그니처가 앞 절 `Combine` 과 글자 하나 다르지 않습니다.

```csharp
public interface Choice<F> where F : Choice<F>
{
    static abstract K<F, A> Choose<A>(K<F, A> fa, K<F, A> fb);
}
```

`MyMaybe` 에서 `Choose` 는 왼쪽이 `Just` 면 왼쪽, 아니면 오른쪽입니다. 이 동작이 곧 "값이 있으면 그 값, 없으면 대체값" 의 일반형입니다.

```csharp
public static K<MaybeF, A> Choose<A>(K<MaybeF, A> fa, K<MaybeF, A> fb) =>
    fa.As() is MyMaybe<A>.Just ? fa : fb;
```

앞에서 본 명령형 `FindUser` 의 if-else 연쇄를 떠올립니다. 후보마다 두 줄씩 복사되던 그 코드가 `Choose` 하나로 줄어듭니다.

다만 한 단계를 거쳐야 합니다. `Choose` 는 `K<F, A>` 위에서 동작하므로, 세 조회 결과 `User?` 를 먼저 `MyMaybe` 로 끌어올려야 합니다. `cache`, `db`, `api` 가 각각 `K<MaybeF, User>` 가 되면 (값이 있으면 `Just`, 없으면 `Nothing`), "캐시 아니면 DB 아니면 API" 가 한 줄로 적힙니다.

```csharp
// 셋을 모두 끌어올린 뒤 — 캐시 → DB → API 순으로 첫 성공
K<MaybeF, User> found = cache.Choose(db).Choose(api);
```

읽는 방식은 이렇습니다. `cache.Choose(db)` 가 "캐시가 있으면 캐시, 없으면 DB" 를 고르고, 거기에 다시 `.Choose(api)` 가 "앞이 비어 있으면 API" 를 잇습니다. 후보가 넷이 되면 `.Choose(넷째)` 한 토막만 붙이면 됩니다. 명령형에서 두 줄씩 복사되던 것이 여기서는 한 토막입니다.

---

## 14.5 같은 시그니처, 다른 의미 — Combine vs Choose

`Combine` 과 `Choose` 는 시그니처가 똑같습니다. 둘 다 `K<F, a>` 둘을 받아 `K<F, a>` 하나를 냅니다. 그런데 **의미는 다릅니다.** `Combine` 은 둘을 **모두 모으고**, `Choose` 는 둘 중 **하나만 고릅니다.** 시퀀스에서 이 차이가 선명하게 드러납니다.

```csharp
public sealed class SeqF : MonoidK<SeqF>, Alternative<SeqF>
{
    // Combine — concat. 둘을 모두 모은다.
    public static K<SeqF, A> Combine<A>(K<SeqF, A> lhs, K<SeqF, A> rhs) =>
        new MySeq<A>([.. lhs.As().Items, .. rhs.As().Items]);

    // Choose — 첫 비어있지 않은 쪽. 하나만 고른다.
    public static K<SeqF, A> Choose<A>(K<SeqF, A> fa, K<SeqF, A> fb) =>
        fa.As().Items.Count > 0 ? fa : fb;
}
```

같은 두 시퀀스 `[1, 2]` 와 `[3, 4]` 인데 결과가 다릅니다.

```text
Combine([1, 2], [3, 4])  =  [1, 2, 3, 4]   (둘 다 모음 — concat)
Choose ([1, 2], [3, 4])  =  [1, 2]         (첫 비어있지 않은 쪽 — 하나만)
Choose ([],     [3, 4])  =  [3, 4]         (왼쪽이 비어 → 오른쪽)
```

![Combine 과 Choose 의 의미 차이 — 시퀀스](./images/Ch14-Alternative/02-combine-vs-choose.svg)

**그림 14-2. 같은 시그니처, 다른 의미: `Combine` vs `Choose`** — 같은 두 시퀀스 `[1, 2]` 와 `[3, 4]` 에 `Combine` 은 둘을 모두 모아 `[1, 2, 3, 4]` 를, `Choose` 는 첫 비어있지 않은 쪽 `[1, 2]` 만 냅니다. `K<F, a> → K<F, a> → K<F, a>` 라는 똑같은 시그니처 위에 "모으기" 와 "고르기" 라는 별개의 추상이 삽니다.


> **흔한 함정** — 시그니처가 같으니 둘 중 하나만 구현하면 된다는 오해입니다.
>
> `Combine` 과 `Choose` 는 시그니처가 같아 코드만 보면 한쪽을 다른 쪽에 베껴 써도 컴파일이 됩니다. 그러나 의미가 다르므로 둘은 별개로 구현해야 합니다. v5 도 자료 타입에 따라 둘을 따로 구현할 수 있게 열어 두었습니다. 시퀀스에서 `Choose` 를 `Combine` (concat) 으로 잘못 구현하면 "첫 성공을 고른다" 가 아니라 "전부를 모은다" 가 되어, fallback 도 파서의 "또는" 도 망가집니다. 같은 시그니처라도 두 동사는 따로 살아야 합니다.


여기서 손으로 한 번 따라가 봅니다. 같은 두 시퀀스 `[1, 2]` 와 `[3, 4]` 입니다.

- `Combine` 은 둘을 모두 살립니다. 왼쪽 `[1, 2]` 뒤에 오른쪽 `[3, 4]` 를 이어 붙여 `[1, 2, 3, 4]` 입니다. 둘 다 들어 있습니다.
- `Choose` 는 하나만 고릅니다. 왼쪽 `[1, 2]` 가 비어 있지 않으니 왼쪽을 그대로 내고 오른쪽은 거들떠보지 않습니다. 결과는 `[1, 2]` 입니다.

같은 입력, 다른 결과. 이것이 이 장의 결정적 통찰입니다. **"모으기" 와 "고르기" 는 원래 별개의 추상입니다.** 시그니처가 같다고 같은 연산이 아닙니다.

3장에서 똑같은 교훈을 만났습니다. 거기서는 같은 `A → A → A` 시그니처 위에 덧셈과 평균이 둘 다 들어맞았지만, 평균은 묶는 순서를 바꾸면 결과가 달라져 (`avg(avg(a, b), c)` 와 `avg(a, avg(b, c))` 가 다름) Monoid 가 아니었습니다. 시그니처가 같아도 의미가 다른 둘이 공존했던 것입니다. 여기서도 같습니다. 같은 `E<a> → E<a> → E<a>` 시그니처 위에 의미가 다른 두 trait (`Combine` 과 `Choose`) 이 나란히 삽니다.

---

## 14.6 우연히 같아지는 자리 — MyMaybe

시퀀스에서 갈라졌던 `Combine` 과 `Choose` 가, `MyMaybe` 에서는 **우연히 일치합니다.** 둘 다 "첫 `Just`" 이기 때문입니다.

```csharp
// MaybeF — Combine 과 Choose 가 둘 다 "첫 Just"
public static K<MaybeF, A> Combine<A>(K<MaybeF, A> lhs, K<MaybeF, A> rhs) =>
    lhs.As() is MyMaybe<A>.Just ? lhs : rhs;
public static K<MaybeF, A> Choose<A>(K<MaybeF, A> fa, K<MaybeF, A> fb) =>
    fa.As() is MyMaybe<A>.Just ? fa : fb;
```

왜 같아질까요. 천천히 생각해 봅니다. 시퀀스에서 둘이 갈라진 까닭은 "모으기" 가 양쪽 값을 다 이어 붙일 수 있었기 때문입니다. `[1, 2]` 와 `[3, 4]` 를 모으면 네 개가 들어갑니다.

그런데 `Maybe` 는 값을 최대 하나만 담습니다. `Just` 하나거나 `Nothing` (빈 것) 입니다. 두 `Just` 를 "모은다" 해도 컨테이너에 둘이 들어갈 자리가 없으니, 결국 하나를 고를 수밖에 없습니다. 그래서 "모으기" 가 "고르기" 와 같은 자리에 떨어집니다. 둘 다 왼쪽이 `Just` 면 왼쪽, 아니면 오른쪽입니다.

손으로 확인해 봅니다. `Combine(Just 3, Just 9)` 도, `Choose(Just 3, Just 9)` 도 둘 다 `Just 3` 입니다. 왼쪽이 이미 성공이라 오른쪽 `Just 9` 는 버려집니다. 시퀀스였다면 `Combine` 은 `[3, 9]` 로 둘을 모았겠지만, `Maybe` 에는 둘을 담을 자리가 없어 같아집니다.

> **흔한 함정** — Maybe 에서 둘이 같으니 원래 같은 추상이라는 오해입니다.
>
> `Combine` 과 `Choose` 가 `Maybe` 에서 같은 것은 **우연** 입니다. `Maybe` 가 값을 하나만 담는 특수한 컨테이너라서 둘이 떨어지지 않는 것뿐입니다. 시퀀스처럼 여러 값을 담는 컨테이너에서는 곧장 갈라집니다. 두 자료 타입의 대비가 "둘은 별개" 라는 진실을 드러냅니다.

---

## 14.7 Alternative — Choice + Applicative + Empty

지금까지 "고르기" (`Choose`) 와 "빈 것" (`Empty`) 을 따로 봤습니다. `Alternative` 는 이 둘을 한 trait 으로 묶습니다. 한 문장으로 하면 "여러 시도 중 하나를 고르되, 전부 실패하면 `Empty`" 라는 패턴입니다.

이 trait 이 무엇을 물려받는지 먼저 봅니다. 세 조각의 합류입니다.

- `Choice` 에서 **고르기** (`Choose`) 를 물려받습니다.
- `Applicative` 에서 **끌어올림** (`Pure`, `Apply`) 을 물려받습니다. `Pure` 는 Normal 값을 Elevated 로 올리는 가장 단순한 끌어올림입니다 (5장).
- 거기에 자체 항등원 `Empty` 를 직접 선언합니다.

고르기 + 끌어올림 + 빈 것. 이 셋이 모이면 "후보들을 차례로 골라 보고, 다 비면 빈 것으로 떨어진다" 는 패턴이 완성됩니다.

```csharp
public interface Alternative<F> : Choice<F>, Applicative<F>
    where F : Alternative<F>
{
    static abstract K<F, A> Empty<A>();

    // virtual — 여러 후보 중 처음으로 성공하는 것 (전부 실패하면 Empty)
    static virtual K<F, A> OneOf<A>(params K<F, A>[] options)
    {
        var acc = F.Empty<A>();
        foreach (var opt in options)
            acc = F.Choose(acc, opt);
        return acc;
    }
}
```

`Choice` 와 `Applicative` 만 상속하고 `Empty` 는 `MonoidK` 와 별개로 다시 선언합니다. `OneOf` 는 `Empty` 에서 시작해 후보들을 `Choose` 로 차례로 이어, 처음 성공하는 것을 고릅니다. 명령형 if-else 연쇄가 한 호출로 줄어듭니다.


`OneOf` 가 어떻게 동작하는지 손으로 따라가 봅니다. 후보가 `[Nothing, Nothing, Just 3, Just 9]` 라고 합시다.

```
acc = Empty                       (Nothing 에서 시작)
acc = Choose(Nothing,  Nothing)  = Nothing   (첫 후보 — 여전히 실패)
acc = Choose(Nothing,  Nothing)  = Nothing   (둘째 후보 — 여전히 실패)
acc = Choose(Nothing,  Just 3)   = Just 3    (셋째 후보 — 첫 성공!)
acc = Choose(Just 3,   Just 9)   = Just 3    (넷째 후보 — 이미 성공이라 그대로)
결과: Just 3
```

`acc` 가 빈 것에서 출발해 후보를 하나씩 `Choose` 로 흡수합니다. 한 번 성공하면 그 뒤로는 왼쪽이 계속 `Just` 라 결과가 더 바뀌지 않습니다. 첫 성공이 끝까지 살아남는 것입니다. 명령형이라면 `if (a != null) ...; else if (b != null) ...` 가 후보 수만큼 늘어났을 텐데, 여기서는 `OneOf(a, b, c, d)` 한 호출입니다.


> **두 `Empty` 가 충돌하지 않는 까닭** — C# 에서 같은 시그니처의 멤버는 한 번 구현하면 두 인터페이스의 요구를 함께 채웁니다. 그래서 자료 타입이 `Empty` 를 한 번만 정의해도 그 한 구현이 `MonoidK` 와 `Alternative` 의 요구를 함께 채웁니다.

예제에서 부르는 `Alternatives.oneOf` 는 이 `OneOf` 를 어떤 `F` 든 받게 감싼 모듈 헬퍼입니다. 이름만 소문자일 뿐 동작은 본문 정의 그대로이고, 뒤에 나오는 `Alternatives.option` 도 같은 모듈의 함수입니다.

```csharp
// Nothing, Nothing, Just 3, Just 9 중 첫 성공 → Just 3
var firstHit = Alternatives.oneOf<MaybeF, int>(none, none, some3, some9);
```

> **OO 직감 다리** — `OneOf` 는 C# 의 null 병합 연쇄 `a ?? b ?? c ?? d` 와 같은 직감입니다. 처음으로 null 이 아닌 (성공하는) 것을 고릅니다. `Alternative` 는 그 발상을 어떤 Elevated World 에든 일반화한 것입니다.

> **미리보기입니다** — `Alternative` 가 `MonoidK` 를 상속하지 않고 `Empty` 를 직접 선언하는 것은 LanguageExt v5 의 설계와 정합합니다. 라이브러리에서도 자료 타입이 `MonoidK` 와 `Alternative` 를 각각 따로 구현합니다. 지금은 "선택과 결합이 별개의 trait 계층" 이라는 것만 가져가면 됩니다.

---

## 14.8 option — 실패하면 기본값 (fallback)

`Choose` 의 가장 흔한 쓰임은 **fallback** 입니다. 후보가 실패하면 미리 정한 기본값으로 떨어지는 것입니다. v5 는 이를 `option` 으로 두고, `Choose` 위에서 한 줄로 자랍니다.

```csharp
// fallback — fa 가 성공하면 그 값, 실패(Empty)면 기본값을 Pure 로 올려 돌려준다.
public static K<F, A> option<F, A>(A value, K<F, A> fa)
    where F : Alternative<F> =>
    F.Choose(fa, F.Pure(value));
```

`fa` 를 먼저 시도하고, 실패하면 `Pure(value)` 로 떨어집니다. `Pure` 는 항상 성공이므로 `option` 은 절대 실패하지 않습니다.

```csharp
var withVal = Alternatives.option<MaybeF, int>(99, some3);   // Just 3   — 성공이면 그 값
var withDef = Alternatives.option<MaybeF, int>(99, none);    // Just 99  — 실패면 기본값
```

`option` 은 C# 의 `x ?? value` (null 병합) 를 Elevated 로 올린 **일반형** 입니다. `x ?? value` 가 "x 가 null 이면 value" 였다면, `option(value, fa)` 는 "fa 가 실패면 value" 입니다. 어떤 Alternative 든 (시퀀스·Maybe·파서) 같은 한 줄로 fallback 을 얻습니다.

---

## 14.9 guard — 조건 필터의 토대


`guard` 를 처음 보면 "조건이 trait 과 무슨 상관인가" 싶습니다. 직감부터 깔아 둡니다.

명령형에서 조건은 보통 `if (조건) { 계속 } else { 멈춤 }` 으로 흐름을 가릅니다. 조건은 Normal World 의 `bool` 값이고, 흐름 제어는 `if` 문이 합니다.

`guard` 가 하는 일은 그 `bool` 조건을 Elevated World 의 값으로 **끌어올리는** 것입니다. 참이면 "성공" 이라는 Elevated 값을, 거짓이면 "실패" (`Empty`) 라는 Elevated 값을 냅니다. 조건이 더 이상 `if` 문이 아니라 `K<F, Unit>` 이라는 시민이 되어 Elevated World 안에서 다른 값들과 함께 흐릅니다.

조건을 값으로 올려 두면, 흐름 제어를 `if` 문이 아니라 `Bind` 가 맡습니다. 거짓이 만든 `Empty` 가 사슬에 끼면 `Bind` 가 그 뒤를 자동으로 쳐냅니다. 1부에서 본 "효과를 값으로" 라는 발상이 조건에까지 닿은 자리입니다.


`guard` 는 `Alternative` 위에서 자라는 또 하나의 고전 헬퍼입니다. 조건이 참이면 `Pure(unit)` (성공, 계속 진행), 거짓이면 `Empty` (실패, 가지를 쳐냄) 입니다.

```csharp
public static K<F, Unit> guard<F>(bool condition)
    where F : Alternative<F> =>
    condition ? F.Pure(Unit.Default) : F.Empty<Unit>();
```

정의를 보면 `guard` 가 두 능력을 동시에 빌려 씁니다. 참 자리에는 `Pure` (성공으로 올림, Applicative), 거짓 자리에는 `Empty` (실패, Alternative) 입니다. 두 능력이 한 함수 안에서 만납니다.

이 `guard` 를 Monad 의 `Bind` 와 결합하면 LINQ `where` 절 (조건 필터) 의 일반형이 됩니다. 어떻게 되는지 짝수만 남기는 예로 손계산해 봅니다.

```csharp
// 짝수만 남기기 — guard 가 where 절이 됨
from x in xs
from _ in guard(x % 2 == 0)
select x
```

시퀀스 `xs` 의 원소를 하나씩 따라가 봅니다.

- `x = 1` 이면 `guard(1 % 2 == 0)` 즉 `guard(false)` 가 `Empty` 를 냅니다. `Bind` 가 `Empty` 를 만나면 그 가지를 통째로 쳐냅니다. `1` 은 결과에서 사라집니다.
- `x = 2` 면 `guard(true)` 가 `Pure(unit)` 으로 통과합니다. 가지가 살아남아 `x` 즉 `2` 가 결과에 들어갑니다.

조건이 거짓인 가지는 `Empty` 가 되어 `Bind` 손에 쳐내지고, 참인 가지만 살아남습니다. `where x % 2 == 0` 이 정확히 이 일을 합니다. 예제의 `Guards.guard` 는 이 `guard` 를 담은 작은 헬퍼 모듈입니다.

```csharp
var pass = Guards.guard<MaybeF>(true);    // Just(())  — 통과
var fail = Guards.guard<MaybeF>(false);   // Nothing   — 가지 쳐냄
```

`Unit` 은 "의미 있는 값은 없지만 성공했다" 를 나타내는 빈 값입니다. `guard` 는 값을 만드는 게 아니라 "이 경로를 살릴지 쳐낼지" 만 결정하므로, 성공의 표지로 `Unit` 을 씁니다.

---

## 14.10 세 법칙 — "첫 성공" 의 약속을 정한다

`Choose` 가 "첫 성공을 고른다" 는 약속은 말로만 두면 자료 타입마다 제멋대로 구현될 수 있습니다. 3장 `Monoid` 가 결합·항등원 두 법칙으로 결합의 의미를 정해 두었듯, `Alternative` 는 세 등식으로 `Choose` 의 의미를 정해 둡니다. 세 등식을 외울 필요는 없습니다. 모두 "첫 성공이 이긴다" 는 한 문장을 형식으로 적은 것뿐입니다.

```text
좌 zero  : Choose(Empty,  Pure b) ≡ Pure b   (왼쪽이 실패면 오른쪽)
우 zero  : Choose(Pure a, Empty)  ≡ Pure a   (왼쪽이 성공이면 왼쪽)
좌 catch : Choose(Pure a, Pure b) ≡ Pure a   (둘 다 성공이면 첫째)
```

세 등식이 함께 "성공한 첫 후보가 이긴다, 전부 실패하면 `Empty` 로 떨어진다" 라는 한 약속을 이룹니다. `Empty` 는 `Choose` 의 항등원 역할 (좌·우 zero) 이고, 좌 catch 는 "먼저 성공한 쪽이 뒤를 가린다" 는 단락 (short-circuit, 먼저 성공하면 뒤를 더 보지 않고 멈춤) 의 표현입니다.

```csharp
// 세 법칙을 학습용 헬퍼로 검증 (MyMaybe 로, probe 로 비교)
var lz = AlternativeLaws.LeftZeroHolds<MaybeF, int>(7, aprobe);   // Choose(Nothing, Just 7) == Just 7
var rz = AlternativeLaws.RightZeroHolds<MaybeF, int>(7, aprobe);  // Choose(Just 7, Nothing) == Just 7
var lc = AlternativeLaws.LeftCatchHolds<MaybeF, int>(3, 9, aprobe); // Choose(Just 3, Just 9) == Just 3
```

`aprobe` 는 `MyMaybe` 두 값이 같은지 비교하도록 안을 꺼내는 학습용 헬퍼입니다. 앞 장들의 법칙 검증에서 쓴 `probe` 와 같은 방식으로, Elevated 값을 곧장 비교하지 못하는 자리에서 속을 열어 맞춰 봅니다.

> **법칙이 두 세계 그림의 무엇을 지키는가** — 세 법칙은 `Choose` 가 Elevated World 안에서 "후보를 고르는 화살표" 가 되게 합니다. `Empty` 가 양옆에서 사라지고 (좌·우 zero), 먼저 성공한 후보가 결과를 정한다는 (좌 catch) 약속이 깨지면, `Choose` 는 더 이상 "선택" 이 아니게 됩니다. 3장 항등원이 결합 사슬에서 사라졌듯, `Empty` 가 선택 사슬에서 사라집니다.

---

## 14.11 Choose 의 실무 얼굴 — 파서 결합자

`Choose` 와 `Empty` 가 가장 쓸모 있는 자리는 **파서 결합자** 입니다. v5 의 `Alternative` 는 `Choose` 위에 한 무리의 결합자를 쌓아 올립니다. 작은 파서들을 `Choose` 로 잇고 반복으로 늘려 큰 파서를 조립하는 도구입니다.

| 결합자 | 의미 | 토대 |
|---|---|---|
| `p1 │ p2` | p1 을 시도하고 실패하면 p2 (`Choose`) | `Choose` |
| `Many(p)` | p 를 0회 이상 반복해 모음 | `Choose` + `Pure([])` |
| `Some(p)` | p 를 1회 이상 반복 (최소 하나 성공) | `Choose` + `Many` |
| `SepBy(p, sep)` | p 를 `sep` 로 구분해 0회 이상 | `Choose` + `Many` |
| `Option(v, p)` | p 가 실패하면 기본값 `v` | `Choose` + `Pure` |
| `ManyUntil(p, end)` | `end` 가 성공할 때까지 p 반복 | `Choose` |

"이 규칙 아니면 저 규칙" (`p1 | p2`) 이 곧 `Choose` 이고, "0회 이상 반복" (`Many`) 이 `Choose` 와 `Pure([])` 의 재귀입니다. JSON·CSV·수식 파서가 모두 이 결합자들의 조합으로 짜입니다. 우리가 이 장에서 만든 `Choose` / `Empty` / `option` 이 그 토대의 최소 골격이고, 실무 파서 라이브러리는 그 위에 `Many` / `Some` / `SepBy` 를 얹은 것입니다.


파서가 무엇인지 한 줄로 깔아 둡니다. 파서는 "글자들을 받아 의미 있는 값으로 읽어내는 작은 함수" 입니다. 예를 들어 "숫자를 읽는 파서", "`+` 기호를 읽는 파서" 처럼 잘게 만들어 둡니다.

이때 "이 규칙 아니면 저 규칙" 을 자주 만납니다. "숫자 아니면 괄호 식" 같은 자리입니다. `Choose` 가 정확히 이 자리를 맡습니다. 왼쪽 파서를 시도하고, 실패하면 오른쪽 파서로 넘어갑니다. `p1.Choose(p2)` 한 줄이 "p1 으로 읽어 보고 안 되면 p2 로 읽어 본다" 가 됩니다. 앞에서 본 `|` 기호가 파서 세계에서 그대로 "또는" 으로 읽힙니다.

"0회 이상 반복" (`Many`) 도 `Choose` 위에서 자랍니다. "한 번 더 읽기 (`Choose` 의 왼쪽) 아니면 빈 것으로 멈추기 (`Pure([])`, 오른쪽)" 를 자기 자신으로 재귀하는 모양입니다. 숫자를 여러 자리 읽거나, 쉼표로 나뉜 항목을 죽 읽는 일이 모두 이 반복입니다.


> **여기는 디딤돌로 짚고 넘어가도 됩니다** — `Many` / `Some` 의 구현 (재귀 + 지연 평가) 은 후속 Part 의 파서 결합자에서 본격적으로 다룹니다. 지금 가져갈 것은 한 줄입니다. **`Choose` 한 동사가 파서 결합자 라이브러리 전체의 토대다.** "선택" 이 작은 파서를 큰 파서로 키우는 접착제입니다.

---

## 14.12 직접 해보기 — 챌린지

> **필수 ① — `MonoidK` 두 법칙 검증.** `MySeq` 의 `Combine` 과 `Empty` 가 항등 법칙 (좌 항등원 `Combine(Empty, x) == x`, 우 항등원 `Combine(x, Empty) == x`) 과 결합법칙을 지키는지 확인합니다. 3장 `Monoid` 의 두 법칙과 한 글자도 다르지 않다는 점을 짚어 봅니다.

> **필수 ② — `Combine` 과 `Choose` 가 다른 자료 타입 찾기.** `MyMaybe` 에서는 둘이 같았습니다. `MyLst` (12장 cons 구조) 에 `Combine` (concat) 과 `Choose` (첫 비어있지 않음) 를 부착하고 둘이 다름을 확인합니다.

> **심화 ③ — `option` 으로 설정값 fallback.** "환경 변수 → 설정 파일 → 기본값" 순으로 떨어지는 설정 조회를 `Choose` 와 `option` 으로 구현해 봅니다. 세 후보를 `oneOf` 로 잇고 마지막에 `option` 으로 기본값을 둡니다.

> **심화 ④ — `guard` 로 `where` 절 만들기.** `MySeq` 에 `Bind` 와 `guard` 를 결합해 `from x in xs where x % 2 == 0 select x` 와 같은 필터를 직접 구현해 봅니다. `guard(false)` 가 낸 `Empty` 가 `Bind` 에서 어떻게 가지를 쳐내는지 추적해 봅니다.

---

## 14.13 Elevated World 어휘로 다시 읽기

이 장의 두 새 동사를 두 평행 세계 어휘로 정리합니다. 둘 다 같은 Elevated World 안에서 `K<F, a>` 들을 모으는 자리입니다.

| 이 장의 코드 | 시그니처 자리 | 한 줄 의미 |
|---|---|---|
| `Combine` (SemigroupK / MonoidK) | `E<a> → E<a> → E<a>` | 두 Elevated 값을 모두 모음 (3장 Monoid 의 Elevated 판) |
| `Empty` (MonoidK / Alternative) | `() → E<a>` | 결합·선택의 항등원 (빈 시퀀스, `Nothing`) |
| `Choose` (Choice) | `E<a> → E<a> → E<a>` | 두 후보 중 성공하는 하나를 고름 |
| `OneOf` / `option` (Alternative) | `E<a> … → E<a>` | 여러 후보 중 첫 성공 / 실패하면 기본값 (`a ?? b`) |
| `guard` (Pure + Empty) | `bool → E<Unit>` | 참이면 성공, 거짓이면 가지 쳐냄 |

3장 `Monoid` 의 결합이 한 층 올라가 `Combine` 이 되었고, 거기에 "모으기 대신 고르기" 라는 형제 추상 `Choose` 가 같은 시그니처로 나란히 섰습니다. 같은 모양이라고 같은 의미가 아니라는 3장의 교훈이 Elevated World 에서 다시 확인됩니다.


네 trait 의 관계를 한 그림으로 정리하면 길을 잃지 않습니다. 이름이 넷이라 복잡해 보이지만 계단 두 개입니다.

```
모으기 계단 (+ 의 세계)            고르기 계단 (| 의 세계)
  SemigroupK  (Combine)              Choice  (Choose)
      │ + Empty                          │
  MonoidK     (Combine + Empty)          │
                                         ▼
             Alternative = Choice + Applicative + Empty
             (고르기 + 끌어올림 + 빈 것을 한자리에)
```

왼쪽은 "모으기" 계단입니다. `SemigroupK` (합치기) 위에 `MonoidK` (합치기 + 빈 것) 가 얹힙니다. 오른쪽은 "고르기" 계단입니다. `Choice` (고르기) 가 출발점입니다. 그리고 `Alternative` 가 고르기에 끌어올림 (`Applicative`) 과 빈 것 (`Empty`) 을 더해, 헬퍼 (`OneOf`, `option`, `guard`) 가 자랄 토대를 만듭니다. 네 이름이 사실은 "모으기 한 줄, 고르기 한 줄" 두 줄거리라는 것만 잡으면 충분합니다.


---

## 14.14 Q&A — 자기 점검

> **Q1. `SemigroupK` / `MonoidK` 는 3장 `Monoid` 와 어떻게 다릅니까?** (14.3절)
>
> 같은 결합 발상을 한 층 끌어올린 것입니다. 3장 `Monoid` 는 Normal World 의 완성 타입 (`int`, `string`) 을 합쳤고 (`A → A → A`), `MonoidK` 는 Elevated 값을 합칩니다 (`K<M, A> → K<M, A> → K<M, A>`). 끝의 `K` 가 한 층 위를 가리킵니다. 법칙 (좌·우 항등원 + 결합) 은 그대로입니다.

> **Q2. `Combine` 과 `Choose` 의 시그니처가 같은데 무엇이 다릅니까?** (14.5절)
>
> 의미가 다릅니다. `Combine` 은 둘을 모두 모으고 (시퀀스에서 concat), `Choose` 는 둘 중 하나만 고릅니다 (첫 비어있지 않은 쪽). 같은 `E<a> → E<a> → E<a>` 시그니처 위에 "모으기" 와 "고르기" 라는 별개의 추상이 공존합니다.

> **Q3. 왜 `Maybe` 에서는 `Combine` 과 `Choose` 가 같습니까?** (14.6절)
>
> `Maybe` 가 값을 최대 하나만 담기 때문입니다. 둘 다 "첫 `Just`" 로 떨어져 우연히 일치합니다. 시퀀스처럼 여러 값을 담는 컨테이너에서는 곧장 갈라집니다. 둘이 같은 것은 `Maybe` 의 특수성이지 두 추상이 같아서가 아닙니다.

> **Q4. `Alternative` 는 어떤 trait 들의 합류입니까?** (14.7절)
>
> `Choice` (고르기) 와 `Applicative` (Pure + Apply) 를 상속하고, 자체 항등원 `Empty` 를 직접 선언합니다. `MonoidK` 를 상속하지는 않습니다. 두 `Empty` 의 시그니처가 같아 자료 타입의 한 메서드가 둘을 모두 만족시킵니다.

> **Q5. `option` 은 명령형의 무엇에 해당합니까?** (14.8절)
>
> C# 의 `x ?? value` (null 병합) 입니다. `option(value, fa)` 는 `fa` 가 실패하면 `value` 로 떨어집니다. `Choose(fa, Pure(value))` 한 줄이고, `Pure` 가 항상 성공이라 절대 실패하지 않습니다.

> **Q6. Alternative 의 세 법칙은 무엇을 보장합니까?** (14.10절)
>
> 좌 zero (`Choose(Empty, Pure b) ≡ Pure b`), 우 zero (`Choose(Pure a, Empty) ≡ Pure a`), 좌 catch (`Choose(Pure a, Pure b) ≡ Pure a`) 입니다. 함께 "성공한 첫 후보가 이기고, 전부 실패하면 `Empty`" 라는 약속을 이룹니다. `Empty` 가 선택의 항등원이고, 좌 catch 가 단락의 표현입니다.

> **Q7. `guard` 와 `Choose` 는 실무에서 무엇의 토대입니까?** (14.9절, 14.11절)
>
> `guard` 는 LINQ `where` 절 (조건 필터) 의 토대입니다. 조건이 거짓이면 `Empty` 가 나와 `Bind` 가 뒤를 쳐냅니다. `Choose` 는 파서 결합자 (`p1 | p2`, `Many`, `Some`, `SepBy`) 의 토대입니다. 작은 파서를 큰 파서로 키우는 접착제입니다.

---

## 14.15 요약

- **이 장은 기초에 없던 두 동사를 더합니다.** 여러 후보 중 하나를 고르는 `Choose`, 두 Elevated 값을 합치는 `Combine` 입니다 (14.1절).
- **`MonoidK` 는 3장 `Monoid` 의 Elevated 판입니다.** Normal 의 `A → A → A` 결합이 한 층 올라가 `K<M, A> → K<M, A> → K<M, A>` 가 됩니다 (14.3절).
- **`Combine` 과 `Choose` 는 같은 시그니처, 다른 의미입니다.** 시퀀스에서 concat 과 첫 비어있지 않은 쪽으로 갈라집니다 (14.5절).
- **`Maybe` 에서 둘이 같은 것은 우연입니다.** 값을 하나만 담는 특수성 때문이고, 두 추상은 원래 별개입니다 (14.6절).
- **`Alternative` 는 `Choice` + `Applicative` + `Empty` 의 합류입니다.** `OneOf` 가 첫 성공, `option` 이 fallback (`x ?? value`) 입니다 (14.7절, 14.8절).
- **세 법칙이 "첫 성공" 을 정합니다.** 좌·우 zero 와 좌 catch 가 `Choose` 의 의미를 분명히 합니다 (14.10절).
- **`Choose` 는 파서 결합자의 토대입니다.** `p1 | p2`, `Many`, `Some` 이 모두 `Choose` 위에서 자랍니다 (14.11절).

이 장의 단일 목표는 하나였습니다. **"고르기" 와 "모으기" 가 같은 시그니처를 공유하는 별개의 추상임을, 그리고 기초 Monoid 가 Elevated World 로 올라간 형태가 MonoidK 임을 확인한다.**

---

## 14.16 4부를 마치며

4부는 한 가지를 코드로 확인하는 여정이었습니다. **기초에서 손으로 만든 추상이 toy 가 아니라 실무 자료 구조의 골격이었다는 것** 입니다. 시퀀스 (12장) 에서 다섯 trait 이 lazy 실무 시민에 그대로 붙었고, 맵·집합 (13장) 에서 trait 부착의 경계를 보았으며, 이 장 (14장) 에서 기초에 없던 선택·결합 추상을 더해 기초 Monoid 가 한 층 위에서 다시 나타나는 것을 보았습니다.

이제 어떤 실무 컬렉션을 만나든, 그 trait 인스턴스를 시그니처만 보고 그릴 수 있습니다. `Seq` 는 `Monad` + `Traversable` + `MonoidK` + `Alternative`, `Map` 은 값에 대한 `Functor` + `Foldable` + `Traversable`, `Set` 은 `Foldable` 입니다. 컨테이너의 모양을 보면 어떤 trait 이 붙고 어디서 막히는지 가늠됩니다. 이것이 4부의 도달점입니다.

> **실무 디딤돌** — `Choose` 와 `OneOf` 는 후속 Part 의 파서 결합자 (`p1 <|> p2`, `Many`, `Some`) 와 검증 폴백 (여러 소스에서 첫 성공) 의 토대입니다. `option` 은 설정값 fallback, `guard` 는 조건부 효과 실행으로 이어집니다.
>
> **테스트 디딤돌** — 이 장의 `MonoidK` 두 법칙 (항등·결합) 과 `Alternative` 세 법칙 (좌·우 zero, 좌 catch) 은 11부의 property-based 테스트에서 임의의 시퀀스·Maybe 로 자동 검증되어, 새 자료 타입에 `Combine` / `Choose` / `Empty` 를 부착할 때마다 법칙 성립을 한 줄로 확인하게 됩니다.
