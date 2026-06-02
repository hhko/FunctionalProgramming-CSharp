using Ch09.Traits;

namespace Ch09.Types;

// MyMaybe — Traverse 의 *목표 Applicative F* 로 쓰기 위해 함께 둔다.
//
// 9장의 주인공은 MySeq 지만, traverse 는 두 번째 Elevated 세계 (여기선 Maybe) 를 필요로 한다.
// MySeq<int> 를 "모두 파싱 성공해야 성공" 으로 traverse 하면 MyMaybe<MySeq<int>> 가 나온다.
public abstract record MyMaybe<A> : K<MaybeF, A>
{
    public sealed record Just(A Value) : MyMaybe<A>;
    public sealed record Nothing : MyMaybe<A>
    {
        public static readonly Nothing Instance = new();
    }

    public override string ToString() => this switch
    {
        Just j  => $"Just({j.Value})",
        _       => "Nothing"
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
