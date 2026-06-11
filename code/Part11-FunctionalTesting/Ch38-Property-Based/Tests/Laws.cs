using Ch38.Traits;

namespace Ch38.Tests;

// *재사용 가능한* 법칙 검증 모듈 — 1부의 챕터별 법칙 헬퍼를 일반화한 것.
// 임의의 Functor<F>/Monad<M> 인스턴스를 받아 법칙을 검사한다 (probe 로 비교).
// xUnit 으로 옮기면 각 메서드를 [Fact]/[Theory] 로 감싸고 ShouldBeTrue() 만 붙이면 된다.
public static class Laws
{
    // ── Functor 법칙 ────────────────────────────────────────────────
    public static bool FunctorIdentity<F, A>(K<F, A> fa, Func<K<F, A>, IEnumerable<A>> probe)
        where F : Functor<F> =>
        probe(F.Map<A, A>(x => x, fa)).SequenceEqual(probe(fa));

    public static bool FunctorComposition<F, A, B, C>(
        K<F, A> fa, Func<A, B> f, Func<B, C> g, Func<K<F, C>, IEnumerable<C>> probe)
        where F : Functor<F> =>
        probe(F.Map<B, C>(g, F.Map<A, B>(f, fa))).SequenceEqual(probe(F.Map<A, C>(x => g(f(x)), fa)));

    // ── Monad 법칙 ──────────────────────────────────────────────────
    public static bool MonadLeftIdentity<M, A, B>(
        A a, Func<A, K<M, B>> f, Func<K<M, B>, IEnumerable<B>> probe)
        where M : Monad<M> =>
        probe(M.Bind(M.Pure(a), f)).SequenceEqual(probe(f(a)));

    public static bool MonadRightIdentity<M, A>(
        K<M, A> m, Func<K<M, A>, IEnumerable<A>> probe)
        where M : Monad<M> =>
        probe(M.Bind(m, M.Pure)).SequenceEqual(probe(m));

    public static bool MonadAssociativity<M, A, B, C>(
        K<M, A> m, Func<A, K<M, B>> f, Func<B, K<M, C>> g, Func<K<M, C>, IEnumerable<C>> probe)
        where M : Monad<M> =>
        probe(M.Bind(M.Bind(m, f), g)).SequenceEqual(probe(M.Bind(m, x => M.Bind(f(x), g))));
}
