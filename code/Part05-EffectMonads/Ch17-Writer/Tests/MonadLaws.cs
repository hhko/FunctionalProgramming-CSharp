using Ch17.Traits;

namespace Ch17.Tests;

// Monad 세 법칙 검증 헬퍼 (콘솔 bool 방식).
//
// Writer 는 (값, 출력) 쌍 — probe 가 비교 가능한 결과 R 로 추출한다
// (출력 Log 는 리스트라 Program 에서 문자열로 평탄화해 비교).
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
        where M : Monad<M>
    {
        var lhs = M.Bind(M.Bind(m, f), g);
        var rhs = M.Bind(m, x => M.Bind(f(x), g));
        return Equals(probe(lhs), probe(rhs));
    }
}
