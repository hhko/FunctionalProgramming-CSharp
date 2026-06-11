using Ch18.Traits;

namespace Ch18.Types;

// Option — "실패할 수 있음" 효과 (1부 MyMaybe 의 재등장). 그 자체로 완전한 모나드다.
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
        fa.As() switch { Option<A>.Some s => new Option<B>.Some(f(s.Value)), _ => Option<B>.None.Instance };

    public static K<OptionF, A> Pure<A>(A value) => new Option<A>.Some(value);

    public static K<OptionF, B> Apply<A, B>(K<OptionF, Func<A, B>> mf, K<OptionF, A> ma) =>
        (mf.As(), ma.As()) switch
        {
            (Option<Func<A, B>>.Some f, Option<A>.Some a) => new Option<B>.Some(f.Value(a.Value)),
            _ => Option<B>.None.Instance
        };

    public static K<OptionF, B> Bind<A, B>(K<OptionF, A> ma, Func<A, K<OptionF, B>> f) =>
        ma.As() switch { Option<A>.Some s => f(s.Value), _ => Option<B>.None.Instance };
}

public static class OptionExtensions
{
    public static Option<A> As<A>(this K<OptionF, A> fa) => (Option<A>)fa;
}
