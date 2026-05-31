using Ch06.Traits;

namespace Ch06.Functions;

// Foldable 모듈 — generic 헬퍼 함수 (라이브러리 Foldable.Module.cs 미러).
//
// 호출 어법: Foldable.foldLeft<MyListF, int, int>((s, n) => s + n, 0, nums).
// virtual default 자유 함수 (Count, All, Any, First, ToList) 도 모듈에 노출.
public static class Foldable
{
    public static B foldRight<F, A, B>(Func<A, B, B> f, B seed, K<F, A> fa)
        where F : Foldable<F> =>
        F.FoldRight(f, seed, fa);

    public static B foldLeft<F, A, B>(Func<B, A, B> f, B seed, K<F, A> fa)
        where F : Foldable<F> =>
        F.FoldLeft(f, seed, fa);

    public static int count<F, A>(K<F, A> fa)
        where F : Foldable<F> =>
        F.Count(fa);

    public static bool isEmpty<F, A>(K<F, A> fa)
        where F : Foldable<F> =>
        F.IsEmpty(fa);

    public static bool all<F, A>(Func<A, bool> p, K<F, A> fa)
        where F : Foldable<F> =>
        F.All(p, fa);

    public static bool any<F, A>(Func<A, bool> p, K<F, A> fa)
        where F : Foldable<F> =>
        F.Any(p, fa);

    public static A? first<F, A>(K<F, A> fa)
        where F : Foldable<F> =>
        F.First(fa);

    public static IEnumerable<A> toList<F, A>(K<F, A> fa)
        where F : Foldable<F> =>
        F.ToList(fa);
}
