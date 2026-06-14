using Ch13.Traits;

namespace Ch13.Types;

// MyMaybe — Map 의 Traverse 목표 Applicative F 로 쓰기 위해 둔다.
//
// Map<K, V> 의 각 값을 검증하면 MyMaybe<MyMap<K, V>> 가 나온다 — 모두 통과해야 Just,
// 하나라도 실패하면 Nothing (키는 보존). 13.8절 Map Traversable 의 두 번째 세계.
public abstract record MyMaybe<A> : K<MaybeF, A>
{
    public sealed record Just(A Value) : MyMaybe<A>;

    public sealed record Nothing : MyMaybe<A>
    {
        public static readonly Nothing Instance = new();
    }

    public override string ToString() => this switch
    {
        Just j => $"Just({j.Value})",
        _      => "Nothing"
    };
}

public sealed class MaybeF : Applicative<MaybeF>
{
    public static K<MaybeF, B> Map<A, B>(Func<A, B> f, K<MaybeF, A> fa) =>
        fa.As() switch
        {
            MyMaybe<A>.Just j => new MyMaybe<B>.Just(f(j.Value)),
            _                 => MyMaybe<B>.Nothing.Instance
        };

    public static K<MaybeF, A> Pure<A>(A value) =>
        new MyMaybe<A>.Just(value);

    public static K<MaybeF, B> Apply<A, B>(K<MaybeF, Func<A, B>> mf, K<MaybeF, A> ma) =>
        (mf.As(), ma.As()) switch
        {
            (MyMaybe<Func<A, B>>.Just f, MyMaybe<A>.Just a) => new MyMaybe<B>.Just(f.Value(a.Value)),
            _                                               => MyMaybe<B>.Nothing.Instance
        };
}

public static class MyMaybeExtensions
{
    public static MyMaybe<A> As<A>(this K<MaybeF, A> fa) => (MyMaybe<A>)fa;
}
