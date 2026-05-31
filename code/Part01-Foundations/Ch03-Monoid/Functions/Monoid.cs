using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Functions;

// v5 정통 static helper — Monoid 의 자유 함수들 (empty / combine / 누적).
//
// 학습용 별칭으로 FoldAll.Of<M> 도 보존 (Foldable 의 디딤돌 어휘).
public static class Monoid
{
    // 단위원 — 일반 함수 어법.
    [Pure]
    public static A empty<A>() where A : Monoid<A> =>
        A.Empty;

    // 두 값을 결합 — 일반 함수 어법.
    [Pure]
    public static A combine<A>(A x, A y) where A : Monoid<A> =>
        x.Combine(y);

    // 어떤 Monoid 든 받는 누적 — v5 의 Monoid.combine<A>(IEnumerable<A>) 와 동일.
    [Pure]
    public static A combine<A>(IEnumerable<A> xs) where A : Monoid<A>
    {
        var acc = A.Empty;
        foreach (var x in xs)
            acc = acc.Combine(x);
        return acc;
    }

    // Instance record 를 trait 에서 가져오는 helper — v5 의 Monoid.instance<A>() 와 동일.
    [Pure]
    public static MonoidInstance<A> instance<A>() where A : Monoid<A> =>
        A.Instance;
}
