using System.Diagnostics.Contracts;
using Ch03.Functions;

namespace Ch03.Traits;

// Monoid — Semigroup + 단위원 Empty (Order 0, kind `*`).
//
// Empty 는 타입에 하나뿐인 단위원이라 static abstract.
// Combine 은 값의 결합 능력이라 instance (Semigroup 에서 상속).
// 항등 법칙:
//   Empty.Combine(a) == a == a.Combine(Empty)
//
// 시그니처는 LanguageExt v5 의 Monoid<A> : Semigroup<A> 와 정확히 정합.
public interface Monoid<A> : Semigroup<A>
    where A : Monoid<A>
{
    [Pure]
    static abstract A Empty { get; }

    // Instance — Semigroup 의 Instance 를 가리고 단위원까지 가진 record 로 노출 (v5 정합).
    new static virtual MonoidInstance<A> Instance { get; } =
        new(Empty: A.Empty, Combine: Semigroup.combine);
}
