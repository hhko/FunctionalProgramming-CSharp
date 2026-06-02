using Ch09.Traits;

namespace Ch09.Functions;

// Traversable 모듈 (8장과 동일). 시퀀스의 traverse / sequence.
public static class Traverse
{
    public static K<F, K<T, B>> traverse<T, F, A, B>(Func<A, K<F, B>> f, K<T, A> ta)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.Traverse<F, A, B>(f, ta);

    public static K<F, K<T, A>> sequence<T, F, A>(K<T, K<F, A>> tfa)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.Sequence<F, A>(tfa);
}
