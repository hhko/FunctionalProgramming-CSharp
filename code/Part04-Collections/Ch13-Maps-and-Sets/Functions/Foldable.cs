using Ch13.Traits;

namespace Ch13.Functions;

// Foldable 모듈 + 확장 (6장과 동일). Map 의 값들, Set 의 원소들을 한 값으로.
public static class Foldable
{
    public static B foldLeft<F, A, B>(Func<B, A, B> f, B seed, K<F, A> fa)
        where F : Foldable<F> =>
        F.FoldLeft(f, seed, fa);

    public static int count<F, A>(K<F, A> fa)
        where F : Foldable<F> =>
        F.Count(fa);
}

public static class FoldableExtensions
{
    public static B FoldLeft<F, A, B>(this K<F, A> fa, Func<B, A, B> f, B seed)
        where F : Foldable<F> =>
        F.FoldLeft(f, seed, fa);

    public static int Count<F, A>(this K<F, A> fa)
        where F : Foldable<F> =>
        F.Count(fa);

    public static bool Any<F, A>(this K<F, A> fa, Func<A, bool> p)
        where F : Foldable<F> =>
        F.Any(p, fa);
}
