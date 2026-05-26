using Ch04.Functions;
using Ch04.Traits;

namespace Ch04.Tests;

// Applicative 의 네 법칙을 학습용 헬퍼로 묶은 모듈.
//
// xUnit + Shouldly 로 옮기려면 각 호출을 [Fact] 로 감싸고 ShouldBe 단언만 추가하면 된다.
// 본 학습용 csproj 는 콘솔 데모이므로 bool 반환으로 통과 / 실패를 표시한다.
//
// 본문 §5.6 의 네 법칙 + §5.6.6 의 [Fact] 코드는 이 헬퍼와 의미가 같다.
public static class ApplicativeLaws
{
    // ① 항등 법칙 — Apply(Pure(id), fa) ≡ fa.
    public static bool IdentityHolds<F, A>(K<F, A> fa, Func<K<F, A>, IEnumerable<A>> probe)
        where F : Applicative<F>
    {
        var idF = F.Pure<Func<A, A>>(x => x);
        var lhs = F.Apply(idF, fa);
        var rhs = fa;
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ② 동형 사상 법칙 — Apply(Pure(f), Pure(a)) ≡ Pure(f(a)).
    public static bool HomomorphismHolds<F, A, B>(
        Func<A, B> f,
        A          a,
        Func<K<F, B>, IEnumerable<B>> probe)
        where F : Applicative<F>
    {
        var lhs = F.Apply(F.Pure(f), F.Pure(a));
        var rhs = F.Pure(f(a));
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ③ 교환 법칙 — Apply(mf, Pure(a)) ≡ Apply(Pure(g => g(a)), mf).
    public static bool InterchangeHolds<F, A, B>(
        K<F, Func<A, B>> mf,
        A                a,
        Func<K<F, B>, IEnumerable<B>> probe)
        where F : Applicative<F>
    {
        var lhs = F.Apply(mf, F.Pure(a));
        Func<Func<A, B>, B> applyToA = g => g(a);
        var rhs = F.Apply(F.Pure(applyToA), mf);
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ④ 합성 법칙 — Apply(Apply(Apply(Pure(compose), mg), mf), fa) ≡ Apply(mg, Apply(mf, fa)).
    public static bool CompositionHolds<F, A, B, C>(
        K<F, Func<A, B>> mf,
        K<F, Func<B, C>> mg,
        K<F, A>          fa,
        Func<K<F, C>, IEnumerable<C>> probe)
        where F : Applicative<F>
    {
        Func<Func<B, C>, Func<Func<A, B>, Func<A, C>>> compose =
            g => f => x => g(f(x));

        var lhs = F.Apply(
                    F.Apply(
                        F.Apply(F.Pure(compose), mg),
                        mf),
                    fa);
        var rhs = F.Apply(mg, F.Apply(mf, fa));
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ⑤ Functor 정합 법칙 — Map(f, fa) ≡ Apply(Pure(f), fa).
    //
    // Applicative 의 Map 이 Functor 의 Map 과 의미가 같음을 보장한다.
    // 본문 §5.2.4 의 `Map ≡ Apply ∘ Pure` 등식의 검증판.
    // LanguageExt v5 의 ApplicativeLaw<F>.functorLaw 와 동일.
    public static bool FunctorConsistencyHolds<F, A, B>(
        Func<A, B> f,
        K<F, A>    fa,
        Func<K<F, B>, IEnumerable<B>> probe)
        where F : Applicative<F>
    {
        var lhs = F.Map(f, fa);
        var rhs = F.Apply(F.Pure(f), fa);
        return probe(lhs).SequenceEqual(probe(rhs));
    }
}
