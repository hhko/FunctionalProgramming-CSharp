using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Tests;

// Monoid 두 법칙 검증 — 결합 + 항등 (instance + static 어법).
public static class MonoidLaws
{
    // 결합 법칙: a.Combine(b).Combine(c) == a.Combine(b.Combine(c))
    [Pure]
    public static bool AssociativityHolds<M>(M a, M b, M c)
        where M : Monoid<M> =>
        a.Combine(b).Combine(c)!.Equals(a.Combine(b.Combine(c)));

    // 좌 항등: Empty.Combine(a) == a
    [Pure]
    public static bool LeftIdentityHolds<M>(M a)
        where M : Monoid<M> =>
        M.Empty.Combine(a)!.Equals(a);

    // 우 항등: a.Combine(Empty) == a
    [Pure]
    public static bool RightIdentityHolds<M>(M a)
        where M : Monoid<M> =>
        a.Combine(M.Empty)!.Equals(a);
}
