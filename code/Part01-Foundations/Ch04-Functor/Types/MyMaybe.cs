using Ch04.Traits;

namespace Ch04.Types;

// 두 번째 자료 타입 — *값이 있거나 없는* 옵셔널.
//
// Just(value) 또는 Nothing. 둘 다 K<MyMaybeF, A> 를 구현해 *같은 trait* 위에서
// MyList 와 동일하게 처리됨.
public abstract record MyMaybe<A> : K<MyMaybeF, A>
{
    public sealed record Just(A Value) : MyMaybe<A>;
    public sealed record Nothing : MyMaybe<A>
    {
        public static readonly Nothing Instance = new();
    }}

public sealed class MyMaybeF : Functor<MyMaybeF>
{
    public static K<MyMaybeF, B> Map<A, B>(Func<A, B> f, K<MyMaybeF, A> fa) =>
        fa.As() switch
        {
            MyMaybe<A>.Just j  => new MyMaybe<B>.Just(f(j.Value)),
            MyMaybe<A>.Nothing => MyMaybe<B>.Nothing.Instance,
            _ => throw new InvalidOperationException()
        };
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyMaybeExtensions
{
    public static MyMaybe<A> As<A>(this K<MyMaybeF, A> fa) => (MyMaybe<A>)fa;
}
