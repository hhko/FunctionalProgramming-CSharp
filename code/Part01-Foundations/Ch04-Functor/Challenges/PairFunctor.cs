using Ch04.Traits;

namespace Ch04.Challenges;

// 챌린지 ② — Pair<L, A> Functor.
//
// 두 값 (Left, Right) 을 들고 있는 자료. *두 번째 값 (Right) 만* Map 으로 변환된다.
// 첫 번째 값 (Left) 은 그대로 보존.
//
// 17 장 Bifunctor 의 도입 — 두 매개변수 중 한쪽만 변환하는 패턴이 일반화되면
// *양쪽 모두 변환 가능한 trait* (BiMap) 이 된다.
public sealed class Pair<L, A>(L left, A right) : K<PairF<L>, A>
{
    public L Left  { get; } = left;
    public A Right { get; } = right;

    public override string ToString() => $"({Left}, {Right})";
}

public sealed class PairF<L> : Functor<PairF<L>>
{
    public static K<PairF<L>, B> Map<A, B>(Func<A, B> f, K<PairF<L>, A> fa)
    {
        var p = fa.As();
        return new Pair<L, B>(p.Left, f(p.Right));
    }
}

public static class PairExtensions
{
    public static Pair<L, A> As<L, A>(this K<PairF<L>, A> fa) => (Pair<L, A>)fa;
}
