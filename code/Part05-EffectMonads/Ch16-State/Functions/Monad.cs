using Ch16.Traits;

namespace Ch16.Functions;

public static class Monad
{
    public static K<M, A> pure<M, A>(A value) where M : Monad<M> => M.Pure(value);
    public static K<M, B> bind<M, A, B>(K<M, A> ma, Func<A, K<M, B>> f) where M : Monad<M> => M.Bind(ma, f);
    public static K<M, B> map<M, A, B>(Func<A, B> f, K<M, A> ma) where M : Monad<M> => M.Map(f, ma);
}

public static class MonadExtensions
{
    public static K<M, B> Select<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> => M.MapDefault(f, ma);

    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> => M.SelectMany(ma, bind, project);

    public static K<M, B> Bind<M, A, B>(this K<M, A> ma, Func<A, K<M, B>> f)
        where M : Monad<M> => M.Bind(ma, f);
}
