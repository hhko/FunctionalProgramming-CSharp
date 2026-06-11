using Ch06.Traits;

namespace Ch06.Tests;

// Foldable 일관성 법칙의 학습용 검증 헬퍼.
//
// xUnit 으로 옮기려면 각 호출을 [Fact] 로 감싸고 ShouldBe 같은 단언을 추가하면 된다.
// 본 학습용 csproj 는 단순 콘솔 데모이므로 bool 반환으로 통과 / 실패를 표시한다.
//
// 본문 §6.6.4 의 [Fact] 코드는 이 헬퍼와 의미가 같다.
public static class FoldableLaws
{
    // ① 결과 일관성 — 가환·결합 step 일 때 FoldRight 와 FoldLeft 의 결과가 같다.
    //
    // FoldRight 의 step: (a, acc) => f(a, acc)
    // FoldLeft  의 step: (acc, a) => f(acc, a)
    //   같은 가환·결합 연산이면 두 방향의 누적이 같은 결과에 도착.
    public static bool ConsistencyHolds<F, A>(
        K<F, A> fa,
        Func<A, A, A> op,           // 가환·결합 이항 연산 (+, *, max, ...)
        A identity)                  // op 의 항등원 (0, 1, MinValue, ...)
        where F : Foldable<F>
    {
        var rhs = F.FoldRight<A, A>((a, acc) => op(a, acc), identity, fa);
        var lhs = F.FoldLeft<A, A>((acc, a) => op(acc, a), identity, fa);
        return Equals(lhs, rhs);
    }

    // ② 자유 함수 일관성 — Count 가 FoldLeft 정의든 FoldRight 정의든 같은 결과.
    //
    // Foldable trait 의 virtual default 는 FoldLeft 로 Count 를 정의한다.
    // 사용자가 FoldRight 로 직접 정의해도 같은 값이 나와야 한다 (자료 순회의 부수 효과 없음).
    public static bool FreeFunctionConsistencyHolds<F, A>(K<F, A> fa)
        where F : Foldable<F>
    {
        var viaFoldLeft  = F.FoldLeft<A, int>((acc, _) => acc + 1, 0, fa);
        var viaFoldRight = F.FoldRight<A, int>((_, acc) => acc + 1, 0, fa);
        return viaFoldLeft == viaFoldRight;
    }
}
