using Ch13.Functions;
using Ch13.Traits;

namespace Ch13.Tests;

// Functor 두 법칙의 학습용 검증 헬퍼 (4장과 동일, 콘솔 bool 방식).
//
// MyMap 에 대한 probe 는 값들의 시퀀스 (fa => fa.As().Values).
public static class FunctorLaws
{
    // ① 항등 — Map(id, fa) ≡ fa.
    public static bool IdentityHolds<F, A>(K<F, A> fa, Func<K<F, A>, IEnumerable<A>> probe)
        where F : Functor<F>
    {
        var lhs = F.Map<A, A>(x => x, fa);
        return probe(lhs).SequenceEqual(probe(fa));
    }

    // ② 합성 — Map(g, Map(f, fa)) ≡ Map(g ∘ f, fa).
    public static bool CompositionHolds<F, A, B, C>(
        K<F, A> fa,
        Func<A, B> f,
        Func<B, C> g,
        Func<K<F, C>, IEnumerable<C>> probe)
        where F : Functor<F>
    {
        var lhs = F.Map<B, C>(g, F.Map<A, B>(f, fa));
        var rhs = F.Map<A, C>(x => g(f(x)), fa);
        return probe(lhs).SequenceEqual(probe(rhs));
    }
}
