using Ch17.Traits;

namespace Ch17.Types;

// ReaderT<Env, M, A> — 환경 효과를 *내부 모나드 M* 위에 얹는다. 내부는 `Env → K<M, A>`.
// (LanguageExt v5 의 `ReaderT<Env, M, A>(Func<Env, K<M, A>> runReader)` 와 동일.)
//
// Bind 의 핵심 — env 를 바깥/안쪽 계산 *양쪽에* 흘리면서, 내부 효과는 M.Bind 로 잇는다.
public sealed class ReaderT<Env, M, A>(Func<Env, K<M, A>> run) : K<ReaderTF<Env, M>, A>
    where M : Monad<M>
{
    public K<M, A> Run(Env env) => run(env);
}

public sealed class ReaderTF<Env, M> : MonadT<ReaderTF<Env, M>, M>, Readable<ReaderTF<Env, M>, Env>
    where M : Monad<M>
{
    public static K<ReaderTF<Env, M>, A> Pure<A>(A value) =>
        new ReaderT<Env, M, A>(_ => M.Pure(value));

    public static K<ReaderTF<Env, M>, B> Map<A, B>(Func<A, B> f, K<ReaderTF<Env, M>, A> fa) =>
        new ReaderT<Env, M, B>(env => M.Map(f, fa.As().Run(env)));

    public static K<ReaderTF<Env, M>, B> Apply<A, B>(K<ReaderTF<Env, M>, Func<A, B>> mf, K<ReaderTF<Env, M>, A> ma) =>
        Bind(mf, f => Map(f, ma));

    public static K<ReaderTF<Env, M>, B> Bind<A, B>(K<ReaderTF<Env, M>, A> ma, Func<A, K<ReaderTF<Env, M>, B>> f) =>
        new ReaderT<Env, M, B>(env => M.Bind(ma.As().Run(env), a => f(a).As().Run(env)));

    public static K<ReaderTF<Env, M>, A> Lift<A>(K<M, A> ma) =>
        new ReaderT<Env, M, A>(_ => ma);

    public static K<ReaderTF<Env, M>, A> Asks<A>(Func<Env, A> f) =>
        new ReaderT<Env, M, A>(env => M.Pure(f(env)));

    public static K<ReaderTF<Env, M>, A> Local<A>(Func<Env, Env> f, K<ReaderTF<Env, M>, A> ma) =>
        new ReaderT<Env, M, A>(env => ma.As().Run(f(env)));
}

public static class ReaderTExtensions
{
    public static ReaderT<Env, M, A> As<Env, M, A>(this K<ReaderTF<Env, M>, A> fa)
        where M : Monad<M> =>
        (ReaderT<Env, M, A>)fa;
}
