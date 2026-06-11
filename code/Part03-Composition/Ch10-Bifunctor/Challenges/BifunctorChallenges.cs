using Ch10.Tests;
using Ch10.Traits;
using Ch10.Types;

namespace Ch10.Challenges;

// 본문 §10.8 직접 해보기 — 챌린지 정답 코드.
public static class BifunctorChallenges
{
    // 챌린지 1 — MapFirst 로 Either 의 오류 타입만 변환.
    //   Left("error") 의 오류 문자열 길이를 구해 Left(5) 로, Right(42) 는 그대로.
    public static (K<EitherF, int, int> fromLeft, K<EitherF, int, int> fromRight) MapErrorOnly()
    {
        Either<string, int> err = new Left<string, int>("error");
        Either<string, int> ok  = new Right<string, int>(42);
        var l = EitherF.MapFirst<string, int, int>(s => s.Length, err);   // Left(5)
        var r = EitherF.MapFirst<string, int, int>(s => s.Length, ok);    // Right(42) — 그대로
        return (l, r);
    }

    // 챌린지 2 — BiMap 으로 Pair 양쪽 동시 변환, MapSecond 는 4장 Functor 의 map.
    public static (Pair<int, string> both, Pair<int, int> secondOnly) PairTransforms()
    {
        var p = new Pair<int, string>(3, "hi");
        var both       = (Pair<int, string>)PairF.BiMap<int, string, int, string>(n => n + 1, s => s.ToUpper(), p); // Pair(4, "HI")
        var secondOnly = (Pair<int, int>)PairF.MapSecond<int, string, int>(s => s.Length, p);                       // Pair(3, 2) — 첫 인자 그대로
        return (both, secondOnly);
    }

    // 챌린지 3 — 두 법칙(항등)이 Pair / Either 양쪽 갈래에서 성립.
    public static bool AllIdentityLawsHold()
    {
        var p = new Pair<int, string>(3, "hi");
        Either<string, int> err = new Left<string, int>("e");
        Either<string, int> ok  = new Right<string, int>(7);
        return BifunctorLaws.IdentityHolds<PairF, int, string>(p)
            && BifunctorLaws.IdentityHolds<EitherF, string, int>(err)
            && BifunctorLaws.IdentityHolds<EitherF, string, int>(ok);
    }
}
