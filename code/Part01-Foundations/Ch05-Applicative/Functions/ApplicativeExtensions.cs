using Ch05.Traits;

namespace Ch05.Functions;

// ApplicativeExtensions — K<F, A> 위 extension methods (라이브러리 Applicative.Extensions.cs 미러).
//
// 호출 어법: ma.Apply(mf), maybe.Map(n => n + 1) — Functor 도 같이 노출.
public static class ApplicativeExtensions
{
    public static K<F, B> Apply<F, A, B>(this K<F, Func<A, B>> mf, K<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(mf, ma);

    public static K<F, B> Map<F, A, B>(this K<F, A> fa, Func<A, B> f)
        where F : Applicative<F> =>
        F.Map(f, fa);
}
