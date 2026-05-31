using Ch10.Traits;

namespace Ch10.Types;

// 두 값을 나란히 담는 자료 타입. 2-인자 마커 K<PairF, L, A> 구현.
public sealed record Pair<L, A>(L First, A Second) : K<PairF, L, A>;

// 태그 타입 + Bifunctor trait 구현.
public sealed class PairF : Bifunctor<PairF>
{
    public static K<PairF, M, B> BiMap<L, A, M, B>(
        Func<L, M> first, Func<A, B> second, K<PairF, L, A> fab)
    {
        var p = (Pair<L, A>)fab;
        return new Pair<M, B>(first(p.First), second(p.Second));
    }

    // 명시적 재정의 — 구체 타입 PairF 에서 직접 호출 가능하게 함.
    // (interface 의 static virtual default 는 generic constraint context 안에서만 호출 가능.)
    public static K<PairF, M, A> MapFirst<L, A, M>(Func<L, M> first, K<PairF, L, A> fab) =>
        BiMap<L, A, M, A>(first, x => x, fab);

    public static K<PairF, L, B> MapSecond<L, A, B>(Func<A, B> second, K<PairF, L, A> fab) =>
        BiMap<L, A, L, B>(x => x, second, fab);
}
