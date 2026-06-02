using Ch35.Types;

namespace Ch35.Functions;

public static class MonadExtensions
{
    public static K<M, B> Select<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> => M.MapDefault(f, ma);

    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> => M.SelectManyD(ma, bind, project);
}

// 능력 기반 효과 + 실행.
public static class Eff
{
    public static K<ReaderTF<RT>, Unit> WriteLine<RT>(string msg) where RT : Has<RT, IConsole> =>
        from con in ReaderTF<RT>.Asks(rt => RT.Get(rt))
        from _ in ReaderTF<RT>.LiftIO(new IO<Unit>(() => { con.WriteLine(msg); return Unit.Default; }))
        select _;

    public static K<ReaderTF<RT>, string> ReadLine<RT>() where RT : Has<RT, IConsole> =>
        from con in ReaderTF<RT>.Asks(rt => RT.Get(rt))
        from line in ReaderTF<RT>.LiftIO(new IO<string>(con.ReadLine))
        select line;

    public static A Run<RT, A>(K<ReaderTF<RT>, A> eff, RT rt) => ((ReaderT<RT, A>)eff).Run(rt).Run();
}
