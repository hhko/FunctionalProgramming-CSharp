using Ch22.Traits;
using Ch22.Types;

namespace Ch22.Functions;

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
}

public static class IOM
{
    // 어떤 MonadIO M 에든 IO 를 끌어올린다.
    public static K<M, A> liftIO<M, A>(IO<A> io) where M : MonadIO<M> => M.LiftIO(io);
}
