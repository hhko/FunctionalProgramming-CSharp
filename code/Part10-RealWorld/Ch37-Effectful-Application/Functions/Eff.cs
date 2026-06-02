using Ch37.Types;

namespace Ch37.Functions;

public static class MonadExtensions
{
    public static K<M, B> Select<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> => M.MapDefault(f, ma);

    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> => M.SelectManyD(ma, bind, project);
}

// 능력별 효과 — 각각 *하나의 Has 제약* 만 요구 (조합은 호출부에서).
public static class Eff
{
    public static K<ReaderTF<RT>, Unit> Print<RT>(string msg) where RT : Has<RT, IConsole> =>
        from con in ReaderTF<RT>.Asks(rt => RT.Get(rt))
        from _ in ReaderTF<RT>.LiftIO(new IO<Unit>(() => { con.WriteLine(msg); return Unit.Default; }))
        select _;

    public static K<ReaderTF<RT>, long> Now<RT>() where RT : Has<RT, IClock> =>
        from clk in ReaderTF<RT>.Asks(rt => RT.Get(rt))
        from t in ReaderTF<RT>.LiftIO(new IO<long>(clk.Now))
        select t;

    public static K<ReaderTF<RT>, Unit> Save<RT>(string key, string value) where RT : Has<RT, IStore> =>
        from st in ReaderTF<RT>.Asks(rt => RT.Get(rt))
        from _ in ReaderTF<RT>.LiftIO(new IO<Unit>(() => { st.Put(key, value); return Unit.Default; }))
        select _;

    public static K<ReaderTF<RT>, IReadOnlyList<string>> Keys<RT>() where RT : Has<RT, IStore> =>
        from st in ReaderTF<RT>.Asks(rt => RT.Get(rt))
        from ks in ReaderTF<RT>.LiftIO(new IO<IReadOnlyList<string>>(st.Keys))
        select ks;

    public static A Run<RT, A>(K<ReaderTF<RT>, A> eff, RT rt) => ((ReaderT<RT, A>)eff).Run(rt).Run();
}
