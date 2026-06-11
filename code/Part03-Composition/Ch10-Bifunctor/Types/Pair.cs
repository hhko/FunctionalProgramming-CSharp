using Ch10.Traits;

namespace Ch10.Types;

// 두 값을 나란히 담는 자료 타입. 2-인자 마커 K<PairF, L, A> 구현.
public sealed record Pair<L, A>(L First, A Second) : K<PairF, L, A>;

// 태그 타입 + Bifunctor / Biapplicative trait 구현.
// Pair 는 두 값을 모두 담으므로 두 자리의 함수가 두 자리의 값에 동시에 적용된다.
public sealed class PairF : Biapplicative<PairF>
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

    // BiPure — 두 값을 각 자리에 그냥 감싸기.
    public static K<PairF, L, A> BiPure<L, A>(L first, A second) =>
        new Pair<L, A>(first, second);

    // BiApply — 두 자리의 함수를 두 자리의 값에 동시에 적용.
    public static K<PairF, C, D> BiApply<A, B, C, D>(
        K<PairF, Func<A, C>, Func<B, D>> fab, K<PairF, A, B> fcd)
    {
        var fs = (Pair<Func<A, C>, Func<B, D>>)fab;
        var vs = (Pair<A, B>)fcd;
        return new Pair<C, D>(fs.First(vs.First), fs.Second(vs.Second));
    }
}
