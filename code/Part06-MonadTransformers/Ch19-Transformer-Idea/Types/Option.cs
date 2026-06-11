using Ch19.Traits;

namespace Ch19.Types;

// Option — "실패 가능" 효과. OptionT 가 *내부 모나드 위에 얹을* 효과 층.
public abstract record Option<A> : K<OptionF, A>
{
    public sealed record Some(A Value) : Option<A> { public override string ToString() => $"Some({Value})"; }
    public sealed record None : Option<A>
    {
        public static readonly None Instance = new();
        public override string ToString() => "None";
    }

    // 내부 helper — 트레잇 디스패치 없이 직접 매핑 (OptionT 본문에서 사용).
    public Option<B> MapOption<B>(Func<A, B> f) =>
        this is Some s ? new Option<B>.Some(f(s.Value)) : Option<B>.None.Instance;
}

public sealed class OptionF : Monad<OptionF>
{
    public static K<OptionF, B> Map<A, B>(Func<A, B> f, K<OptionF, A> fa) => fa.As().MapOption(f);
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
