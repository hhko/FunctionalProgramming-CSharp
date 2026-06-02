using Ch18.Traits;

namespace Ch18.Functions;

public static class MonadExtensions
{
    public static K<M, B> Select<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> => M.MapDefault(f, ma);

    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> => M.SelectMany(ma, bind, project);
}

public static class Trans
{
    public static K<T, A> lift<T, M, A>(K<M, A> ma)
        where T : MonadT<T, M> where M : Monad<M> => T.Lift(ma);
}
