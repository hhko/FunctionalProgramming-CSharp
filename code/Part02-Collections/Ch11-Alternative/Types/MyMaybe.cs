using Ch11.Traits;

namespace Ch11.Types;

// MyMaybe — *부재* 가 실패인 Elevated World 에 선택·결합 부착.
//
// Maybe 에서는 Combine 과 Choose 가 *같다* — 둘 다 "첫 Just" 다:
//   Empty   = Nothing
//   Combine = Choose = 왼쪽이 Just 면 왼쪽, 아니면 오른쪽.
// 즉 "값이 없으면 대체값" (`x ?? fallback`) 의 일반형.
public abstract record MyMaybe<A> : K<MaybeF, A>
{
    public sealed record Just(A Value) : MyMaybe<A>
    {
        public override string ToString() => $"Just({Value})";
    }

    public sealed record Nothing : MyMaybe<A>
    {
        public static readonly Nothing Instance = new();
        public override string ToString() => "Nothing";
    }
}

// 라이브러리 패턴과 동일하게 MonoidK 와 Alternative 를 각각 따로 구현.
public sealed class MaybeF : MonoidK<MaybeF>, Alternative<MaybeF>
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

    // MonoidK.Empty — Nothing.
    public static K<MaybeF, A> Empty<A>() =>
        MyMaybe<A>.Nothing.Instance;

    // SemigroupK.Combine — 첫 Just (좌 편향).
    public static K<MaybeF, A> Combine<A>(K<MaybeF, A> lhs, K<MaybeF, A> rhs) =>
        lhs.As() is MyMaybe<A>.Just ? lhs : rhs;

    // Choice.Choose — Maybe 에서는 Combine 과 같다 (첫 Just).
    public static K<MaybeF, A> Choose<A>(K<MaybeF, A> fa, K<MaybeF, A> fb) =>
        fa.As() is MyMaybe<A>.Just ? fa : fb;
}

public static class MyMaybeExtensions
{
    public static MyMaybe<A> As<A>(this K<MaybeF, A> fa) => (MyMaybe<A>)fa;
}
