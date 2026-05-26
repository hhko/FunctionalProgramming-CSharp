using Ch08.Traits;

namespace Ch08.Functions;

// TraversableExtensions — K<T, A> 위 extension methods (라이브러리 Traversable.Extensions.cs 미러).
//
// 호출 어법: list.Traverse<...>(parseInt), tfa.Sequence<...>().
public static class TraversableExtensions
{
    public static K<F, K<T, B>> Traverse<T, F, A, B>(this K<T, A> ta, Func<A, K<F, B>> f)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.Traverse<F, A, B>(f, ta);

    public static K<F, K<T, A>> Sequence<T, F, A>(this K<T, K<F, A>> tfa)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.Sequence<F, A>(tfa);
}
