using Ch06.Traits;

namespace Ch06.Functions;

// FoldableExtensions — K<F, A> 위 extension methods (라이브러리 Foldable.Extensions.cs 미러).
//
// 호출 어법: nums.FoldLeft((s, n) => s + n, 0), nums.Count(), nums.Any(p).
public static class FoldableExtensions
{
    public static B FoldRight<F, A, B>(this K<F, A> fa, Func<A, B, B> f, B seed)
        where F : Foldable<F> =>
        F.FoldRight(f, seed, fa);

    public static B FoldLeft<F, A, B>(this K<F, A> fa, Func<B, A, B> f, B seed)
        where F : Foldable<F> =>
        F.FoldLeft(f, seed, fa);

    public static int Count<F, A>(this K<F, A> fa)
        where F : Foldable<F> =>
        F.Count(fa);

    public static bool IsEmpty<F, A>(this K<F, A> fa)
        where F : Foldable<F> =>
        F.IsEmpty(fa);

    public static bool All<F, A>(this K<F, A> fa, Func<A, bool> p)
        where F : Foldable<F> =>
        F.All(p, fa);

    public static bool Any<F, A>(this K<F, A> fa, Func<A, bool> p)
        where F : Foldable<F> =>
        F.Any(p, fa);

    public static A? First<F, A>(this K<F, A> fa)
        where F : Foldable<F> =>
        F.First(fa);

    public static IEnumerable<A> ToList<F, A>(this K<F, A> fa)
        where F : Foldable<F> =>
        F.ToList(fa);
}
