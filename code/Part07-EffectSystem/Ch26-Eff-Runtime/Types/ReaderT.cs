using Ch26.Traits;

namespace Ch26.Types;

// Eff<RT, A> = ReaderT<RT, IO, A> — 7부의 결론.
// 런타임 RT 를 환경으로 주입(Reader)하고, IO 효과를 안쪽에 품는다(MonadIO).
// 6부에서 쌓은 변환기가 그대로 실무 효과 시스템의 골격이 된다.
public sealed class ReaderT<RT, M, A>(Func<RT, K<M, A>> run) : K<ReaderTF<RT, M>, A>
    where M : MonadIO<M>
{
    public K<M, A> Run(RT rt) => run(rt);
}

public sealed class ReaderTF<RT, M> :
    MonadIO<ReaderTF<RT, M>>,
    Readable<ReaderTF<RT, M>, RT>
    where M : MonadIO<M>
{
    public static K<ReaderTF<RT, M>, A> Pure<A>(A value) =>
        new ReaderT<RT, M, A>(_ => M.Pure(value));

    public static K<ReaderTF<RT, M>, B> Map<A, B>(Func<A, B> f, K<ReaderTF<RT, M>, A> fa) =>
        new ReaderT<RT, M, B>(rt => M.Map(f, fa.As().Run(rt)));

    public static K<ReaderTF<RT, M>, B> Apply<A, B>(K<ReaderTF<RT, M>, Func<A, B>> mf, K<ReaderTF<RT, M>, A> ma) =>
        Bind(mf, f => Map(f, ma));

    public static K<ReaderTF<RT, M>, B> Bind<A, B>(K<ReaderTF<RT, M>, A> ma, Func<A, K<ReaderTF<RT, M>, B>> f) =>
        new ReaderT<RT, M, B>(rt => M.Bind(ma.As().Run(rt), a => f(a).As().Run(rt)));

    public static K<ReaderTF<RT, M>, A> LiftIO<A>(IO<A> ma) =>
        new ReaderT<RT, M, A>(_ => M.LiftIO(ma));

    public static K<ReaderTF<RT, M>, A> Asks<A>(Func<RT, A> f) =>
        new ReaderT<RT, M, A>(rt => M.Pure(f(rt)));

    public static K<ReaderTF<RT, M>, A> Local<A>(Func<RT, RT> f, K<ReaderTF<RT, M>, A> ma) =>
        new ReaderT<RT, M, A>(rt => ma.As().Run(f(rt)));
}

public static class ReaderTExtensions
{
    public static ReaderT<RT, M, A> As<RT, M, A>(this K<ReaderTF<RT, M>, A> fa)
        where M : MonadIO<M> =>
        (ReaderT<RT, M, A>)fa;
}
