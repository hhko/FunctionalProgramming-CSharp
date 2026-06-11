using Ch04.Functions;
using Ch04.Traits;

namespace Ch04.Tests;

// Functor 두 법칙의 학습용 검증 헬퍼.
//
// xUnit 으로 옮기려면 각 호출을 [Fact] 로 감싸고 ShouldBe 같은 단언을 추가하면 된다.
// 본 학습용 csproj 는 단순 콘솔 데모이므로 bool 반환으로 통과 / 실패를 표시한다.
//
// 본문 §4.6.4 의 [Fact] 코드는 이 헬퍼와 의미가 같다.
public static class FunctorLaws
{
    // ① 항등 법칙 — Map(id, fa) == fa.
    // 반환 — 두 결과의 컬렉션 비교 결과.
    public static bool IdentityHolds<F, A>(K<F, A> fa, Func<K<F, A>, IEnumerable<A>> probe)
        where F : Functor<F>
    {
        var lhs = F.Map<A, A>(x => x, fa);
        var rhs = fa;
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ② 합성 법칙 — Map(g, Map(f, fa)) == Map(g ∘ f, fa).
    public static bool CompositionHolds<F, A, B, C>(
        K<F, A> fa,
        Func<A, B> f,
        Func<B, C> g,
        Func<K<F, C>, IEnumerable<C>> probe)
        where F : Functor<F>
    {
        var lhs = F.Map<B, C>(g, F.Map<A, B>(f, fa));
        var rhs = F.Map<A, C>(Compose.Of(g, f), fa);
        return probe(lhs).SequenceEqual(probe(rhs));
    }
}
