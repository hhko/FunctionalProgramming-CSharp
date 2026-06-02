using Ch17.Traits;

namespace Ch17.Types;

// Option — 변환기의 *내부 모나드 M* 으로 쓴다. ReaderT<Env, OptionF, A> 는
// 15장의 ReaderOption 을 *내부 M=Option 으로 고정한 특수 사례* 가 된다 (이번엔 공짜로).
public abstract record Option<A> : K<OptionF, A>
{
    public sealed record Some(A Value) : Option<A> { public override string ToString() => $"Some({Value})"; }
    public sealed record None : Option<A>
    {
        public static readonly None Instance = new();
        public override string ToString() => "None";
    }
}

public sealed class OptionF : Monad<OptionF>
{
    public static K<OptionF, B> Map<A, B>(Func<A, B> f, K<OptionF, A> fa) =>
        fa.As() is Option<A>.Some s ? new Option<B>.Some(f(s.Value)) : Option<B>.None.Instance;
    public static K<OptionF, A> Pure<A>(A value) => new Option<A>.Some(value);
    public static K<OptionF, B> Apply<A, B>(K<OptionF, Func<A, B>> mf, K<OptionF, A> ma) =>
        (mf.As(), ma.As()) switch
        {
            (Option<Func<A, B>>.Some f, Option<A>.Some a) => new Option<B>.Some(f.Value(a.Value)),
            _ => Option<B>.None.Instance
        };
    public static K<OptionF, B> Bind<A, B>(K<OptionF, A> ma, Func<A, K<OptionF, B>> f) =>
        ma.As() is Option<A>.Some s ? f(s.Value) : Option<B>.None.Instance;
}

public static class OptionExtensions
{
    public static Option<A> As<A>(this K<OptionF, A> fa) => (Option<A>)fa;
}
