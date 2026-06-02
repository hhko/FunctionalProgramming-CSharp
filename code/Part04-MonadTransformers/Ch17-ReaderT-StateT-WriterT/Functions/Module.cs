using Ch17.Traits;

namespace Ch17.Functions;

public static class MonadExtensions
{
    public static K<M, B> Select<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> => M.MapDefault(f, ma);

    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> => M.SelectMany(ma, bind, project);
}

public static class Readable
{
    public static K<M, A> asks<M, Env, A>(Func<Env, A> f) where M : Readable<M, Env> => M.Asks(f);
    public static K<M, A> local<M, Env, A>(Func<Env, Env> f, K<M, A> ma) where M : Readable<M, Env> => M.Local(f, ma);
}

public static class Stateful
{
    public static K<M, S> get<M, S>() where M : Stateful<M, S> => M.Get;
    public static K<M, Unit> put<M, S>(S value) where M : Stateful<M, S> => M.Put(value);
    public static K<M, Unit> modify<M, S>(Func<S, S> f) where M : Stateful<M, S> => M.Modify(f);
    public static K<M, A> gets<M, S, A>(Func<S, A> f) where M : Stateful<M, S> => M.Gets(f);
}

public static class Trans
{
    public static K<T, A> lift<T, M, A>(K<M, A> ma)
        where T : MonadT<T, M> where M : Monad<M> => T.Lift(ma);
}
