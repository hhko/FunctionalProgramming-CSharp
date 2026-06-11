using Ch13.Traits;

namespace Ch13.Functions;

// Functor 모듈 + 확장 (3장과 동일). 어떤 Functor F 든 받는다.
public static class Functor
{
    public static K<F, B> map<F, A, B>(Func<A, B> f, K<F, A> fa)
        where F : Functor<F> =>
        F.Map(f, fa);
}

public static class FunctorExtensions
{
    public static K<F, B> Map<F, A, B>(this K<F, A> fa, Func<A, B> f)
        where F : Functor<F> =>
        F.Map(f, fa);
}
