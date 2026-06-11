using Ch12.Traits;

namespace Ch12.Tests;

// Monad 세 법칙의 학습용 검증 헬퍼 (콘솔 bool 방식).
//
// xUnit 으로 옮기려면 각 호출을 [Fact] 로 감싸고 ShouldBe 단언을 추가하면 된다 (9부 Ch33).
// probe — K<M, A> 를 비교 가능한 시퀀스로 추출 (MySeq 면 fa => fa.As().Items).
public static class MonadLaws
{
    // ① 좌 항등 — Bind(Pure(a), f) ≡ f(a).
    public static bool LeftIdentityHolds<M, A, B>(
        A a,
        Func<A, K<M, B>> f,
        Func<K<M, B>, IEnumerable<B>> probe)
        where M : Monad<M>
    {
        var lhs = M.Bind(M.Pure(a), f);
        var rhs = f(a);
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ② 우 항등 — Bind(m, Pure) ≡ m.
    public static bool RightIdentityHolds<M, A>(
        K<M, A> m,
        Func<K<M, A>, IEnumerable<A>> probe)
        where M : Monad<M>
    {
        var lhs = M.Bind(m, M.Pure);
        var rhs = m;
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ③ 결합 — Bind(Bind(m, f), g) ≡ Bind(m, x => Bind(f(x), g)).
    public static bool AssociativityHolds<M, A, B, C>(
        K<M, A> m,
        Func<A, K<M, B>> f,
        Func<B, K<M, C>> g,
        Func<K<M, C>, IEnumerable<C>> probe)
        where M : Monad<M>
    {
        var lhs = M.Bind(M.Bind(m, f), g);
        var rhs = M.Bind(m, x => M.Bind(f(x), g));
        return probe(lhs).SequenceEqual(probe(rhs));
    }
}
