using Ch07.Traits;

namespace Ch07.Tests;

// Monad 의 세 법칙을 학습용 헬퍼로 묶은 모듈.
//
// xUnit + Shouldly 로 옮기려면 각 호출을 [Fact] 로 감싸고 ShouldBe 단언만 추가하면 된다.
// 본 학습용 csproj 는 콘솔 데모이므로 bool 반환으로 통과 / 실패를 표시한다.
//
// 본문 §7.9 의 세 법칙 + §7.15 테스트 디딤돌의 property-based 검증과 의미가 같다.
public static class MonadLaws
{
    // ① 좌항등 (left identity) — Bind(Pure(a), f) ≡ f(a).
    //
    // Pure 로 끌어올린 값을 곧장 Bind 하면 f 를 직접 적용한 것과 같다.
    // Pure 가 합성에 아무 효과도 더하지 않음을 보장한다.
    public static bool LeftIdentityHolds<M, A, B>(
        A                a,
        Func<A, K<M, B>> f,
        Func<K<M, B>, IEnumerable<B>> probe)
        where M : Monad<M>
    {
        var lhs = M.Bind(M.Pure(a), f);
        var rhs = f(a);
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ② 우항등 (right identity) — Bind(m, Pure) ≡ m.
    //
    // 꺼낸 값을 곧장 Pure 로 다시 올리면 원래 컨테이너와 같다.
    public static bool RightIdentityHolds<M, A>(
        K<M, A> m,
        Func<K<M, A>, IEnumerable<A>> probe)
        where M : Monad<M>
    {
        var lhs = M.Bind(m, M.Pure);
        var rhs = m;
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ③ 결합 (associativity) — Bind(Bind(m, f), g) ≡ Bind(m, a => Bind(f(a), g)).
    //
    // 사슬을 어디서 끊어 묶어도 같은 결과. bind 사슬을 마음 놓고 길게 이을 수 있는 근거.
    public static bool AssociativityHolds<M, A, B, C>(
        K<M, A>          m,
        Func<A, K<M, B>> f,
        Func<B, K<M, C>> g,
        Func<K<M, C>, IEnumerable<C>> probe)
        where M : Monad<M>
    {
        var lhs = M.Bind(M.Bind(m, f), g);
        var rhs = M.Bind(m, a => M.Bind(f(a), g));
        return probe(lhs).SequenceEqual(probe(rhs));
    }
}
