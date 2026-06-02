using Ch33.Tests;
using Ch33.Traits;
using Ch33.Types;

namespace Ch33.Challenges;

// 챌린지 ② 정답 — 법칙 위반 인스턴스(BogusListF)를 같은 모듈로 검사하면 위반으로 잡힌다.
public static class BogusCheck
{
    // BogusListF 는 항등 법칙을 어긴다 → FunctorIdentity 가 false 를 반환해야 정상.
    public static bool CatchesViolation()
    {
        K<BogusListF, int> fa = new BogusList<int>([1, 2, 3]);
        var identityHolds = Laws.FunctorIdentity<BogusListF, int>(fa, x => x.As().Items);
        return !identityHolds;   // 위반을 "잡았다" = true
    }
}
