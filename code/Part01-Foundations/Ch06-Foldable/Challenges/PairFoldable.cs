using Ch06.Traits;

namespace Ch06.Challenges;

// 심화 챌린지 ② — Pair<L, A> Foldable.
//
// 두 값 (Left, Right) 을 들고 있는 자료. 두 번째 값 (Right) 만 fold 의 대상.
// 첫 번째 값 (Left) 은 fold 가 무시한다 — phantom 같은 자리.
//
// 17 장 Bifoldable 의 도입 — 두 매개변수 각각을 따로 fold 하는 trait 의 단순 사례.
public sealed class Pair<L, A>(L left, A right) : K<PairF<L>, A>
{
    public L Left  { get; } = left;
    public A Right { get; } = right;

    public override string ToString() => $"({Left}, {Right})";
}

public sealed class PairF<L> : Foldable<PairF<L>>
{
    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<PairF<L>, A> fa) =>
        f(fa.As().Right, seed);   // ← Right 한 번만 step

    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<PairF<L>, A> fa) =>
        f(seed, fa.As().Right);   // ← Right 한 번만 step (인자 순서만 다름)
}

public static class PairExtensions
{
    public static Pair<L, A> As<L, A>(this K<PairF<L>, A> fa) => (Pair<L, A>)fa;
}
