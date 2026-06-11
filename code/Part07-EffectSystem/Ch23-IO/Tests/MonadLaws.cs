using Ch23.Traits;

namespace Ch23.Tests;

// IO 도 진짜 모나드 — probe 가 Run 결과로 비교 (IO 는 외연 동등).
public static class MonadLaws
{
    public static bool LeftIdentityHolds<M, A, B, R>(
        A a, Func<A, K<M, B>> f, Func<K<M, B>, R> probe)
        where M : Monad<M> =>
        Equals(probe(M.Bind(M.Pure(a), f)), probe(f(a)));

    public static bool RightIdentityHolds<M, A, R>(
        K<M, A> m, Func<K<M, A>, R> probe)
        where M : Monad<M> =>
        Equals(probe(M.Bind(m, M.Pure)), probe(m));

    public static bool AssociativityHolds<M, A, B, C, R>(
        K<M, A> m, Func<A, K<M, B>> f, Func<B, K<M, C>> g, Func<K<M, C>, R> probe)
        where M : Monad<M> =>
        Equals(probe(M.Bind(M.Bind(m, f), g)), probe(M.Bind(m, x => M.Bind(f(x), g))));
}
