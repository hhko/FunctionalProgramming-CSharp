using Ch09.Traits;

namespace Ch09.Tests;

// Foldable 일관성 법칙의 학습용 검증 헬퍼 (5장과 동일, 콘솔 bool 방식).
public static class FoldableLaws
{
    // ① 가환·결합 연산이면 FoldLeft 와 FoldRight 의 결과가 같다.
    public static bool ConsistencyHolds<F, A>(
        K<F, A> fa,
        Func<A, A, A> op,
        A identity)
        where F : Foldable<F>
    {
        var l = F.FoldLeft<A, A>((acc, a) => op(acc, a), identity, fa);
        var r = F.FoldRight<A, A>((a, acc) => op(a, acc), identity, fa);
        return Equals(l, r);
    }

    // ② Count 가 FoldLeft 정의든 FoldRight 정의든 같은 값.
    public static bool CountConsistencyHolds<F, A>(K<F, A> fa)
        where F : Foldable<F>
    {
        var viaLeft  = F.FoldLeft<A, int>((acc, _) => acc + 1, 0, fa);
        var viaRight = F.FoldRight<A, int>((_, acc) => acc + 1, 0, fa);
        return viaLeft == viaRight;
    }
}
