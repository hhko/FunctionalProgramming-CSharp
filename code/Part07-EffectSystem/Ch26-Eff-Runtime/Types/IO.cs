using Ch26.Traits;

namespace Ch26.Types;

// IO<A> — 지연 부수 효과 (20장의 최소 버전). Eff<RT,A> 스택 맨 안쪽 모나드.
public sealed class IO<A>(Func<A> thunk) : K<IOF, A>
{
    public A Run() => thunk();
}

public sealed class IOF : MonadIO<IOF>
{
    public static K<IOF, B> Map<A, B>(Func<A, B> f, K<IOF, A> fa) => new IO<B>(() => f(fa.As().Run()));
    public static K<IOF, A> Pure<A>(A value) => new IO<A>(() => value);
    public static K<IOF, B> Apply<A, B>(K<IOF, Func<A, B>> mf, K<IOF, A> ma) =>
        new IO<B>(() => mf.As().Run()(ma.As().Run()));
    public static K<IOF, B> Bind<A, B>(K<IOF, A> ma, Func<A, K<IOF, B>> f) =>
        new IO<B>(() => f(ma.As().Run()).As().Run());
    public static K<IOF, A> LiftIO<A>(IO<A> ma) => ma;
}

public static class IOExtensions
{
    public static IO<A> As<A>(this K<IOF, A> fa) => (IO<A>)fa;
}
