using Ch12.Traits;

namespace Ch12.Functions;

// Foldable 모듈 + 확장 (6장과 동일). 실무 시퀀스의 Sum / Count / Any 등이 여기서 자란다.
public static class Foldable
{
    public static B foldLeft<F, A, B>(Func<B, A, B> f, B seed, K<F, A> fa)
        where F : Foldable<F> =>
        F.FoldLeft(f, seed, fa);

    public static B foldRight<F, A, B>(Func<A, B, B> f, B seed, K<F, A> fa)
        where F : Foldable<F> =>
        F.FoldRight(f, seed, fa);

    public static int count<F, A>(K<F, A> fa)
        where F : Foldable<F> =>
        F.Count(fa);

    public static bool any<F, A>(Func<A, bool> p, K<F, A> fa)
        where F : Foldable<F> =>
        F.Any(p, fa);

    public static bool all<F, A>(Func<A, bool> p, K<F, A> fa)
        where F : Foldable<F> =>
        F.All(p, fa);
}

public static class FoldableExtensions
{
    public static B FoldLeft<F, A, B>(this K<F, A> fa, Func<B, A, B> f, B seed)
        where F : Foldable<F> =>
        F.FoldLeft(f, seed, fa);

    public static B FoldRight<F, A, B>(this K<F, A> fa, Func<A, B, B> f, B seed)
        where F : Foldable<F> =>
        F.FoldRight(f, seed, fa);

    public static int Count<F, A>(this K<F, A> fa)
        where F : Foldable<F> =>
        F.Count(fa);

    public static bool Any<F, A>(this K<F, A> fa, Func<A, bool> p)
        where F : Foldable<F> =>
        F.Any(p, fa);

    public static bool All<F, A>(this K<F, A> fa, Func<A, bool> p)
        where F : Foldable<F> =>
        F.All(p, fa);
}
