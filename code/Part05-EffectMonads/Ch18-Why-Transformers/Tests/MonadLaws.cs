using Ch18.Traits;

namespace Ch18.Tests;

// 손으로 짠 ReaderOption 도 *진짜 모나드* 임을 법칙으로 확인한다.
// (배관이 번거로울 뿐, 모나드 법칙은 성립 — 그래서 4부 변환기로 자동화할 가치가 있다.)
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
