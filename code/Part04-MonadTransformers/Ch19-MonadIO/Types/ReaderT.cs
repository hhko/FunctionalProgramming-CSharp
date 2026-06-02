using Ch19.Traits;

namespace Ch19.Types;

// ReaderT<Env, M, A> — 환경 효과를 *IO 를 품는 내부 모나드 M* 위에 얹는다.
// 내부 M 이 MonadIO 이므로, 이 스택 전체도 MonadIO 가 된다 (LiftIO 를 위로 전달).
//
// ReaderT<Env, IOF, A> 가 곧 5부 Eff<RT, A> = ReaderT<RT, IO, A> 의 축소판이다.
public sealed class ReaderT<Env, M, A>(Func<Env, K<M, A>> run) : K<ReaderTF<Env, M>, A>
    where M : MonadIO<M>
{
    public K<M, A> Run(Env env) => run(env);
}

public sealed class ReaderTF<Env, M> :
    MonadT<ReaderTF<Env, M>, M>,
    MonadIO<ReaderTF<Env, M>>,
    Readable<ReaderTF<Env, M>, Env>
    where M : MonadIO<M>
{
    public static K<ReaderTF<Env, M>, A> Pure<A>(A value) =>
        new ReaderT<Env, M, A>(_ => M.Pure(value));

    public static K<ReaderTF<Env, M>, B> Map<A, B>(Func<A, B> f, K<ReaderTF<Env, M>, A> fa) =>
        new ReaderT<Env, M, B>(env => M.Map(f, fa.As().Run(env)));

    public static K<ReaderTF<Env, M>, B> Apply<A, B>(K<ReaderTF<Env, M>, Func<A, B>> mf, K<ReaderTF<Env, M>, A> ma) =>
        Bind(mf, f => Map(f, ma));

    public static K<ReaderTF<Env, M>, B> Bind<A, B>(K<ReaderTF<Env, M>, A> ma, Func<A, K<ReaderTF<Env, M>, B>> f) =>
        new ReaderT<Env, M, B>(env => M.Bind(ma.As().Run(env), a => f(a).As().Run(env)));

    // 일반 lift — 내부 모나드 계산을 한 층 위로.
    public static K<ReaderTF<Env, M>, A> Lift<A>(K<M, A> ma) =>
        new ReaderT<Env, M, A>(_ => ma);

    // IO lift — 스택 맨 안쪽 IO 를 끌어올린다 (내부 M 의 LiftIO 에 위임).
    public static K<ReaderTF<Env, M>, A> LiftIO<A>(IO<A> ma) =>
        Lift(M.LiftIO(ma));

    public static K<ReaderTF<Env, M>, A> Asks<A>(Func<Env, A> f) =>
        new ReaderT<Env, M, A>(env => M.Pure(f(env)));

    public static K<ReaderTF<Env, M>, A> Local<A>(Func<Env, Env> f, K<ReaderTF<Env, M>, A> ma) =>
        new ReaderT<Env, M, A>(env => ma.As().Run(f(env)));
}

public static class ReaderTExtensions
{
    public static ReaderT<Env, M, A> As<Env, M, A>(this K<ReaderTF<Env, M>, A> fa)
        where M : MonadIO<M> =>
        (ReaderT<Env, M, A>)fa;
}
