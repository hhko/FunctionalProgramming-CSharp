using Ch15.Traits;

namespace Ch15.Types;

// Reader — "환경 의존" 효과 (12장). 역시 그 자체로 완전한 모나드.
public sealed class Reader<Env, A>(Func<Env, A> run) : K<ReaderF<Env>, A>
{
    public A Run(Env env) => run(env);
}

public sealed class ReaderF<Env> : Monad<ReaderF<Env>>
{
    public static K<ReaderF<Env>, B> Map<A, B>(Func<A, B> f, K<ReaderF<Env>, A> fa) =>
        new Reader<Env, B>(e => f(fa.As().Run(e)));

    public static K<ReaderF<Env>, A> Pure<A>(A value) =>
        new Reader<Env, A>(_ => value);

    public static K<ReaderF<Env>, B> Apply<A, B>(K<ReaderF<Env>, Func<A, B>> mf, K<ReaderF<Env>, A> ma) =>
        new Reader<Env, B>(e => mf.As().Run(e)(ma.As().Run(e)));

    public static K<ReaderF<Env>, B> Bind<A, B>(K<ReaderF<Env>, A> ma, Func<A, K<ReaderF<Env>, B>> f) =>
        new Reader<Env, B>(e => f(ma.As().Run(e)).As().Run(e));
}

public static class ReaderExtensions
{
    public static Reader<Env, A> As<Env, A>(this K<ReaderF<Env>, A> fa) => (Reader<Env, A>)fa;
}
