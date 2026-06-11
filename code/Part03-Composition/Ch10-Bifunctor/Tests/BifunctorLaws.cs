using Ch10.Traits;

namespace Ch10.Tests;

// Bifunctor 두 법칙 검증 — 항등 + 합성 (두 자리에서 각각).
public static class BifunctorLaws
{
    // 항등 법칙: BiMap(identity, identity, fab) == fab
    public static bool IdentityHolds<F, L, A>(K<F, L, A> fab)
        where F : Bifunctor<F> =>
        F.BiMap<L, A, L, A>(x => x, x => x, fab)!.Equals(fab);

    // 합성 법칙: BiMap(g1 ∘ f1, g2 ∘ f2, fab) == BiMap(g1, g2, BiMap(f1, f2, fab))
    public static bool CompositionHolds<F, L1, A1, L2, A2, L3, A3>(
        Func<L1, L2> f1, Func<L2, L3> g1,
        Func<A1, A2> f2, Func<A2, A3> g2,
        K<F, L1, A1> fab)
        where F : Bifunctor<F> =>
        F.BiMap(l => g1(f1(l)), a => g2(f2(a)), fab)!
         .Equals(F.BiMap(g1, g2, F.BiMap(f1, f2, fab)));
}
