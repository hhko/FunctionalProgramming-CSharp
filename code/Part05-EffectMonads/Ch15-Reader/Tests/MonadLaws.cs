using Ch15.Traits;

namespace Ch15.Tests;

// Monad 세 법칙의 학습용 검증 헬퍼 (콘솔 bool 방식).
//
// Reader 는 함수라 *외연 동등* — 같은 샘플 환경으로 Run 한 결과로 비교한다 (probe 가 그 역할).
public static class MonadLaws
{
    public static bool LeftIdentityHolds<M, A, B>(
        A a, Func<A, K<M, B>> f, Func<K<M, B>, B> probe)
        where M : Monad<M> =>
        probe(M.Bind(M.Pure(a), f))!.Equals(probe(f(a)));

    public static bool RightIdentityHolds<M, A>(
        K<M, A> m, Func<K<M, A>, A> probe)
        where M : Monad<M> =>
        probe(M.Bind(m, M.Pure))!.Equals(probe(m));

    public static bool AssociativityHolds<M, A, B, C>(
        K<M, A> m, Func<A, K<M, B>> f, Func<B, K<M, C>> g, Func<K<M, C>, C> probe)
        where M : Monad<M>
    {
        var lhs = M.Bind(M.Bind(m, f), g);
        var rhs = M.Bind(m, x => M.Bind(f(x), g));
        return probe(lhs)!.Equals(probe(rhs));
    }
}
