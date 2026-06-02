using Ch11.Traits;

namespace Ch11.Tests;

// MonoidK 세 법칙의 학습용 검증 헬퍼 (콘솔 bool 방식).
//
// 1부 Monoid 법칙 (좌·우 단위원 + 결합) 을 Elevated World 로 끌어올린 판.
public static class MonoidKLaws
{
    // ① 좌 단위원 — Combine(Empty, x) ≡ x.
    public static bool LeftIdentityHolds<F, A>(
        K<F, A> x,
        Func<K<F, A>, IEnumerable<A>> probe)
        where F : MonoidK<F>
    {
        var lhs = F.Combine(F.Empty<A>(), x);
        return probe(lhs).SequenceEqual(probe(x));
    }

    // ② 우 단위원 — Combine(x, Empty) ≡ x.
    public static bool RightIdentityHolds<F, A>(
        K<F, A> x,
        Func<K<F, A>, IEnumerable<A>> probe)
        where F : MonoidK<F>
    {
        var rhs = F.Combine(x, F.Empty<A>());
        return probe(rhs).SequenceEqual(probe(x));
    }

    // ③ 결합법칙 — Combine(Combine(x, y), z) ≡ Combine(x, Combine(y, z)).
    public static bool AssociativityHolds<F, A>(
        K<F, A> x,
        K<F, A> y,
        K<F, A> z,
        Func<K<F, A>, IEnumerable<A>> probe)
        where F : SemigroupK<F>
    {
        var lhs = F.Combine(F.Combine(x, y), z);
        var rhs = F.Combine(x, F.Combine(y, z));
        return probe(lhs).SequenceEqual(probe(rhs));
    }
}
