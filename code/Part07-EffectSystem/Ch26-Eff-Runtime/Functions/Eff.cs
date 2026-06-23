using Ch26.Traits;
using Ch26.Types;

namespace Ch26.Functions;

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
    public static K<M, A> liftIO<M, A>(IO<A> io) where M : MonadIO<M> => M.LiftIO(io);
}

// Eff — 능력 기반 효과들. `Eff<RT,A>` 는 곧 `K<ReaderTF<RT, IOF>, A>` = ReaderT<RT, IO, A> 다.
public static class Eff
{
    // 런타임에서 콘솔 능력을 꺼내 한 줄 출력 (Has 제약이 능력 보유를 컴파일타임 보장).
    public static K<ReaderTF<RT, IOF>, Unit> WriteLine<RT>(string msg)
        where RT : Has<RT, IConsole> =>
        from con in Readable.asks<ReaderTF<RT, IOF>, RT, IConsole>(rt => RT.Get(rt))
        from _ in IOM.liftIO<ReaderTF<RT, IOF>, Unit>(
            new IO<Unit>(() => { con.WriteLine(msg); return Unit.Default; }))
        select _;

    public static K<ReaderTF<RT, IOF>, string> ReadLine<RT>()
        where RT : Has<RT, IConsole> =>
        from con in Readable.asks<ReaderTF<RT, IOF>, RT, IConsole>(rt => RT.Get(rt))
        from line in IOM.liftIO<ReaderTF<RT, IOF>, string>(new IO<string>(con.ReadLine))
        select line;

    // 효과를 주어진 런타임으로 실행 (Run(rt) 가 IO 를 만들고, IO.Run() 이 부수 효과 수행).
    public static A Run<RT, A>(K<ReaderTF<RT, IOF>, A> eff, RT rt) =>
        eff.As().Run(rt).As().Run();
}
