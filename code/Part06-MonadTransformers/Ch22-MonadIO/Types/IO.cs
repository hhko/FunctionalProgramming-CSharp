using Ch22.Traits;

namespace Ch22.Types;

// IO<A> — *지연된 부수 작용*. 내부는 thunk `() → A`. Run() 하기 전엔 아무 일도 안 일어난다.
// (5부에서 본격 DSL/EnvIO 로 확장. 여기선 LiftIO 를 보이기 위한 최소 버전.)
public sealed class IO<A>(Func<A> thunk) : K<IOF, A>
{
    public A Run() => thunk();
}

// IOF 는 그 자체로 MonadIO — LiftIO 가 항등이다 (IO 를 IO 로 끌어올림 = 그대로).
public sealed class IOF : MonadIO<IOF>
{
    public static K<IOF, B> Map<A, B>(Func<A, B> f, K<IOF, A> fa) =>
        new IO<B>(() => f(fa.As().Run()));
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
