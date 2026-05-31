using Ch06.Traits;

namespace Ch06.Types;

// 두 번째 자료 타입 — 값이 있거나 없는 옵셔널.
//
// Just(value) 또는 Nothing. 둘 다 K<MyMaybeF, A> 를 구현해 같은 trait 위에서
// MyList 와 동일하게 처리됨.
public abstract record MyMaybe<A> : K<MyMaybeF, A>
{
    public sealed record Just(A Value) : MyMaybe<A>;
    public sealed record Nothing : MyMaybe<A>
    {
        public static readonly Nothing Instance = new();
    }
}

public sealed class MyMaybeF : Foldable<MyMaybeF>
{
    // FoldRight — Just(v) 면 step 한 번, Nothing 이면 seed 그대로.
    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<MyMaybeF, A> fa) =>
        fa.As() switch
        {
            MyMaybe<A>.Just j  => f(j.Value, seed),
            MyMaybe<A>.Nothing => seed,
            _ => throw new InvalidOperationException()
        };

    // FoldLeft — Just(v) 면 step 한 번 (인자 순서만 다름), Nothing 이면 seed 그대로.
    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<MyMaybeF, A> fa) =>
        fa.As() switch
        {
            MyMaybe<A>.Just j  => f(seed, j.Value),
            MyMaybe<A>.Nothing => seed,
            _ => throw new InvalidOperationException()
        };
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyMaybeExtensions
{
    public static MyMaybe<A> As<A>(this K<MyMaybeF, A> fa) => (MyMaybe<A>)fa;
}
