# Part 2 — Collections (불변 컬렉션)

## 4부의 배경

기초는 학습용 toy 자료 타입 (`MyList`, `MyMaybe`, `MyValidation`) 위에 5개 trait 을 직접 부착하며 추상의 내부 구조를 손에 잡았습니다. 그런데 toy 타입은 "추상이 어떻게 작동하는지" 를 보여 주기 위한 최소 골격 일뿐, 실무에서 매일 쓰는 자료 구조가 아닙니다.

4부의 출발점은 **기초에서 손으로 만든 추상이 실제 라이브러리 컬렉션에서 그대로 작동함을 확인** 하는 것입니다. LanguageExt v5 의 `Seq` / `Lst` / `Map` / `HashMap` / `Set` 은 기초에서 정의한 것과 동일한 `Functor` / `Foldable` / `Traversable` / `Monad` trait 의 인스턴스입니다. `MyList` 에 부착했던 것과 똑같은 3-tuple 패턴 (자료 / 태그 / trait) 이 라이브러리 컬렉션에도 그대로 들어 있습니다.

동시에 4부는 기초에 없던 새 추상 두 개를 추가합니다. 컬렉션이 여러 후보 중 하나를 고르거나 (`Alternative` / `Choice`), 둘을 하나로 합치는 (`SemigroupK` / `MonoidK`) 능력입니다. 이 둘은 기초의 `Monoid` (Ch03) 를 Elevated World 로 끌어올린 형태입니다.

**기초의 추상이 toy 가 아니라 실무 자료 구조의 골격이었다는 것** 을 코드로 확인하는 것이 4부의 결정적 설계입니다.

---

## 4부의 목표

4부를 마치면 독자는 다음을 갖게 됩니다.

- 기초에서 정의한 `Functor` / `Foldable` / `Traversable` / `Monad` trait 이 `Seq` / `Lst` / `Map` / `HashMap` 같은 실무 컬렉션의 인스턴스로 어떻게 부착되는지 직접 읽고 구현.
- 순서가 있는 컬렉션 (`Seq` / `Lst` / `Arr`) 과 키-값 / 집합 컬렉션 (`Map` / `HashMap` / `Set` / `TrieMap`) 의 내부 자료 표현과 trait 부착의 차이.
- `Alternative<F>` / `Choice<F>` — 여러 Elevated 값 중 하나를 선택하는 `Choose` / `Empty` 의 직접 구현.
- `SemigroupK<M>` / `MonoidK<M>` — 두 Elevated 값을 결합하는 `Combine` / `Empty` 의 직접 구현. 기초 `Monoid` 와의 정합.
- LINQ / 일반 `IEnumerable` 어법이 사실 `Foldable` + `Functor` + `Monad` 위에 서 있었음을 시그니처 단계에서 재확인.

어떤 실무 컬렉션을 만나든 그 trait 인스턴스를 시그니처만 보고 그릴 수 있는 능력이 4부의 도달점입니다.

---

## 4부의 무대 — 컬렉션이라는 Elevated World

| 세계 | 시민 | 의미 |
|---|---|---|
| **Elevated World** (위) | `Seq<a>`, `Lst<a>`, `Map<k, v>`, `HashMap<k, v>`, `Set<a>` | "여러 개일 수 있음" 효과를 인코딩한 컨테이너. 기초의 `MyList` 가 가리키던 자리의 실무 시민들입니다. |
| **Normal World** (아래) | `a`, `b`, `(k, v)` | 컬렉션 안에 담기는 일상의 값들. |

4부의 새 동사 두 개는 같은 Elevated World 안에서 작동합니다.

| 동사 | 시그니처 자리 | 처리하는 trait |
|---|---|---|
| `Choose` | `E<a> → E<a> → E<a>` (둘 중 하나 선택) | **Alternative / Choice** |
| `Combine` / `Empty` | `E<a> → E<a> → E<a>` , `() → E<a>` (둘을 합침 / 항등원) | **SemigroupK / MonoidK** |

`Map` 은 값을 바꾸고, `Bind` 는 효과를 잇고, `Choose` / `Combine` 은 Elevated 값들을 모읍니다. 같은 무대 위 다른 동사입니다.

---

## 4부의 학습 흐름

| 장 | 두 축에서의 자리 | 한 줄 |
|---|---|---|
| 12장 | 기초 trait 의 실무 인스턴스 | lazy `MySeq` (+ cons `MyLst`) 를 `Functor` / `Applicative` / `Monad` / `Foldable` / `Traversable` 인스턴스로 |
| 13장 | 키-값 / 집합 컬렉션 | `MyMap` 은 값에만 `Functor`, `MySet` 은 `Foldable` 만 (Functor 는 경계 사례) |
| 14장 | 선택과 결합 | `Choice` / `SemigroupK` / `MonoidK` / `Alternative` — 같은 시그니처, 다른 의미 |

각 장은 기초와 동일하게 **목적 (왜) → 기능 (무엇) → 예제 (어떻게)** 5부 narrative arc 로 구성됩니다.

---

## 3개 장의 구성

### 12장 — Sequences / Seq · Lst

순서가 있는 컬렉션입니다. 기초의 `MyList` 가 가리키던 자리를 실무 시민 lazy 시퀀스가 채웁니다. 학습용 `MySeq<a>` (lazy `IEnumerable` 백킹) 에 같은 `Functor` / `Applicative` / `Monad` / `Foldable` / `Traversable` trait 을 직접 부착하고, toy `MyList` 가 즉시 materialize 했던 것과 달리 계산이 끝까지 미뤄지는 차이를 봅니다. LINQ `Select` / `SelectMany` 가이 trait 들의 별명이었음을 재확인합니다.

두 가지가 더 드러납니다. ① 챌린지의 `MyLst` (재귀적 cons 구조) 는 표현이 완전히 달라도 같은 trait 이 그대로 붙음을 보여 줍니다. 추상은 표현이 아니라 시그니처의 약속에 달려 있습니다. ② 시퀀스에는 적법한 Applicative 가 둘 있습니다. 데카르트 곱 (`MySeq.Apply`) 과 짝 맞춤 (`ZipSeq`) 입니다. 같은 자료 타입에 서로 다른 추상이 공존할 수 있다는 첫 사례입니다.

### 13장 — Maps & Sets / Map · Set

키-값과 집합 컬렉션입니다. 학습용 `MyMap<Key, V>` 에서 `Functor` 의 `Map` 이 값에만 작용하고 키는 보존되는 자리 (키-값 컨테이너의 끌어올림) 를 직접 구현으로 봅니다. 키 자체를 바꾸는 연산은 충돌·재정렬 때문에 Functor 가 아니라는 것도 반례로 확인합니다.

`Set` 은 경계 사례로 다룹니다. `MySet<a>` 에는 `Foldable` (원소를 한 값으로 끌어내림) 만 깔끔히 붙고, `Functor` 는 붙지 않습니다 — (1) 결과를 다시 중복 제거하려면 `B` 에 동등성 제약이 필요해 무제약 `Map` 시그니처를 어기고, (2) 서로 다른 원소가 같은 값으로 가면 개수가 줄어 모양 보존 (Functor 법칙) 을 깨기 때문입니다. trait 부착의 경계를 보는 자리입니다. (정렬 `Map` (AVL) / `HashMap` (HAMT) / `TrieMap` 의 내부 표현 차이는 개념으로 짚습니다.)

### 14장 — Alternative & MonoidK / 선택과 결합

기초에 없던 새 추상입니다. `Choice<F>.Choose` 로 여러 Elevated 후보 중 하나를 고르고, `SemigroupK<M>.Combine` / `MonoidK<M>.Empty` 로 두 Elevated 값을 합칩니다. 학습용 trait 을 직접 정의해 `MySeq` / `MyMaybe` 에 부착하고, 기초 `Monoid` (Normal World 의 결합) 가 어떻게 Elevated World 로 올라가는지를 시그니처로 봅니다.

이 장의 핵심입니다. `Combine` 과 `Choose` 는 시그니처가 똑같은데 의미가 다릅니다. `MySeq` 에서 `Combine` 은 두 시퀀스를 concat (모두 모음) 하지만 `Choose` 는 첫 비어있지 않은 쪽 (하나만 고름) 입니다. `MyMaybe` 에서는 둘이 우연히 일치 (첫 `Just`) 합니다. "합치기" 와 "고르기" 가 원래 별개의 추상임을 두 자료 타입의 대비로 봅니다. 끝으로 `guard` (`Pure` + `Empty` 의 결합) 가 조건 분기·필터의 토대가 됨을 다룹니다. 학습용 `Alternative<F>` 의 trait 계층은 LanguageExt v5 와 정합합니다. `Alternative` 가 `Choice` + `Applicative` 를 상속하며 `Empty` 를 직접 선언하고, 자료 타입이 `MonoidK` 와 `Alternative` 를 각각 따로 구현합니다.

---

## 4부의 코드

본문 예제는 모두 `code/Part04-Collections/` 에 실행 가능한 형태로 들어 있습니다 (외부 패키지 의존 0 — LanguageExt 소스는 참고 원전으로만 사용).

```bash
dotnet run --project code/Part04-Collections/Ch12-Sequences/Ch12.csproj
dotnet run --project code/Part04-Collections/Ch13-Maps-and-Sets/Ch13.csproj
dotnet run --project code/Part04-Collections/Ch14-Alternative/Ch14.csproj
```

각 프로젝트는 기초와 동일한 구성 (`Traits/` · `Types/` · `Functions/` · `Tests/` · `Challenges/` · `Program.cs`) 이며, `Program.cs` 의 콘솔 데모가 모든 법칙 검증 결과를 출력합니다.

---

## 4부의 진입점

기초를 마쳤다면 Ch12 — Sequences 부터 시작합니다. `MyList` 에 부착했던 trait 이 실무 `MySeq` 에 그대로 들어 있음을 확인하는 것이 4부 전체의 출발점입니다.
