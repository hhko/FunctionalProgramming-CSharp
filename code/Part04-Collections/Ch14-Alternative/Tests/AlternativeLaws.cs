using Ch14.Traits;

namespace Ch14.Tests;

// Alternative 세 법칙의 학습용 검증 헬퍼 (v5 Alternative.Laws 와 정합).
//
// Choose 가 "첫 성공을 고른다" 는 약속을 세 등식으로 고정한다:
//   leftZero  — Choose(Empty, Pure b) ≡ Pure b   (왼쪽 실패면 오른쪽)
//   rightZero — Choose(Pure a, Empty) ≡ Pure a   (왼쪽 성공이면 왼쪽)
//   leftCatch — Choose(Pure a, Pure b) ≡ Pure a  (둘 다 성공이면 첫째)
// probe — K<F, A> 를 비교 가능한 시퀀스로 추출 (MyMaybe 면 Just→[v], Nothing→[]).
public static class AlternativeLaws
{
    // ① 좌 zero — Choose(Empty, Pure(b)) ≡ Pure(b).
    public static bool LeftZeroHolds<F, A>(
        A b,
        Func<K<F, A>, IEnumerable<A>> probe)
        where F : Alternative<F>
    {
        var lhs = F.Choose(F.Empty<A>(), F.Pure(b));
        return probe(lhs).SequenceEqual(probe(F.Pure(b)));
    }

    // ② 우 zero — Choose(Pure(a), Empty) ≡ Pure(a).
    public static bool RightZeroHolds<F, A>(
        A a,
        Func<K<F, A>, IEnumerable<A>> probe)
        where F : Alternative<F>
    {
        var lhs = F.Choose(F.Pure(a), F.Empty<A>());
        return probe(lhs).SequenceEqual(probe(F.Pure(a)));
    }

    // ③ 좌 catch — Choose(Pure(a), Pure(b)) ≡ Pure(a).
    public static bool LeftCatchHolds<F, A>(
        A a,
        A b,
        Func<K<F, A>, IEnumerable<A>> probe)
        where F : Alternative<F>
    {
        var lhs = F.Choose(F.Pure(a), F.Pure(b));
        return probe(lhs).SequenceEqual(probe(F.Pure(a)));
    }
}
