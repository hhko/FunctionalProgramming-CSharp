using Ch06.Traits;

namespace Ch06.Types;

public abstract record MyMaybe<A> : K<MyMaybeF, A>
{
    public sealed record Just(A Value) : MyMaybe<A>;
    public sealed record Nothing : MyMaybe<A>
    {
        public static readonly Nothing Instance = new();
    }}

public sealed class MyMaybeF : Monad<MyMaybeF>
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

    // Bind 의 단락 회로 — Just 면 f 적용, Nothing 이면 *f 호출 자체 안 함*.
    public static K<MyMaybeF, B> Bind<A, B>(K<MyMaybeF, A> ma, Func<A, K<MyMaybeF, B>> f) =>
        ma.As() switch
        {
            MyMaybe<A>.Just j  => f(j.Value),
            MyMaybe<A>.Nothing => MyMaybe<B>.Nothing.Instance,
            _ => throw new InvalidOperationException()
        };
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyMaybeExtensions
{
    public static MyMaybe<A> As<A>(this K<MyMaybeF, A> fa) => (MyMaybe<A>)fa;
}
