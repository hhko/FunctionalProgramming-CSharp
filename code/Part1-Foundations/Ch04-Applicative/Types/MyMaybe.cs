using Ch04.Traits;

namespace Ch04.Types;

public abstract record MyMaybe<A> : K<MyMaybeF, A>
{
    public sealed record Just(A Value) : MyMaybe<A>;
    public sealed record Nothing : MyMaybe<A>
    {
        public static readonly Nothing Instance = new();
    }}

// Functor + Applicative 부착.
public sealed class MyMaybeF : Applicative<MyMaybeF>
{
    public static K<MyMaybeF, B> Map<A, B>(Func<A, B> f, K<MyMaybeF, A> fa) =>
        fa.As() switch
        {
            MyMaybe<A>.Just j  => new MyMaybe<B>.Just(f(j.Value)),
            MyMaybe<A>.Nothing => MyMaybe<B>.Nothing.Instance,
            _ => throw new InvalidOperationException()
        };

    public static K<MyMaybeF, A> Pure<A>(A value) =>
        new MyMaybe<A>.Just(value);

    public static K<MyMaybeF, B> Apply<A, B>(K<MyMaybeF, Func<A, B>> mf, K<MyMaybeF, A> ma) =>
        (mf.As(), ma.As()) switch
        {
            (MyMaybe<Func<A, B>>.Just f, MyMaybe<A>.Just a) => new MyMaybe<B>.Just(f.Value(a.Value)),
            _                                                => MyMaybe<B>.Nothing.Instance
        };
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyMaybeExtensions
{
    public static MyMaybe<A> As<A>(this K<MyMaybeF, A> fa) => (MyMaybe<A>)fa;
}
