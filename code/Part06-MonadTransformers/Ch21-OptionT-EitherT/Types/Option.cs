using Ch21.Traits;

namespace Ch21.Types;

// Option<A> — 실패를 *이유 없이* 담는다 (Either 의 Left(L) 와 달리 "왜" 를 남기지 않는다).
// EitherT 와의 대비를 위해 Either.MapRight 와 짝이 되는 MapOption 을 둔다.
public abstract record Option<A> : K<OptionF, A>
{
    public sealed record Some(A Value) : Option<A> { public override string ToString() => $"Some({Value})"; }
    public sealed record None : Option<A>
    {
        public static readonly None Instance = new();
        public override string ToString() => "None";
    }

    public Option<B> MapOption<B>(Func<A, B> f) =>
        this is Some s ? new Option<B>.Some(f(s.Value)) : Option<B>.None.Instance;
}

public sealed class OptionF : Monad<OptionF>
{
    public static K<OptionF, A> Pure<A>(A value) => new Option<A>.Some(value);
    public static K<OptionF, B> Map<A, B>(Func<A, B> f, K<OptionF, A> fa) =>
        fa.As() is Option<A>.Some s ? new Option<B>.Some(f(s.Value)) : Option<B>.None.Instance;
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
