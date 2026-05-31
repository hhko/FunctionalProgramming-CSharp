namespace Ch03.Traits;

// Monoid — Semigroup + 단위원 Empty (Order 0, kind `*`).
//
// Empty 는 타입에 하나뿐인 단위원이라 static abstract.
// Combine 은 값의 결합 능력이라 instance (Semigroup 에서 상속).
// 항등 법칙:
//   a.Combine(Empty) == a == Empty.Combine(a)
//   a + Empty        == a == Empty + a
//
// 시그니처는 LanguageExt v5 의 `Monoid<A> : Semigroup<A>` 와 정합.
public interface Monoid<A> : Semigroup<A>
    where A : Monoid<A>
{
    static abstract A Empty { get; }
}
