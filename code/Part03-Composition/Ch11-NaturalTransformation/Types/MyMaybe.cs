using Ch11.Traits;

namespace Ch11.Types;

// 학습용 단순 MyMaybe — 11장 NaturalTransformation 데모 한정.
public abstract record MyMaybe<A> : K<MyMaybeF, A>
{
    public bool IsJust => this is Just<A>;
    public static MyMaybe<A> Of(A value) => new Just<A>(value);
    public static MyMaybe<A> Nothing => new Nothing<A>();
}

public sealed record Just<A>(A Val) : MyMaybe<A>;
public sealed record Nothing<A>() : MyMaybe<A>;

// 태그 타입.
public sealed class MyMaybeF { }
