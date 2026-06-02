using Ch25.Types;

namespace Ch25.Tests;

// bracket / Resources 의 보장을 검증 (콘솔 bool).
public static class ResourceLaws
{
    // ① 정상 경로 — acquire → use → release 순서.
    public static bool NormalOrderHolds()
    {
        var log = new List<string>();
        Resource.Bracket(
            () => { log.Add("acquire"); return 1; },
            _ => { log.Add("use"); return 0; },
            _ => log.Add("release"));
        return log.SequenceEqual(["acquire", "use", "release"]);
    }

    // ② 예외 경로 — use 가 던져도 release 는 실행된다.
    public static bool ReleaseOnExceptionHolds()
    {
        var released = false;
        try
        {
            Resource.Bracket<int, int>(
                () => 1,
                _ => throw new InvalidOperationException("boom"),
                _ => released = true);
        }
        catch (InvalidOperationException) { /* 예외는 전파됨 */ }
        return released;
    }

    // ③ 다중 자원 — LIFO 해제 (A,B 열면 B,A 닫힘).
    public static bool LifoReleaseHolds()
    {
        var r = new Resources();
        r.Acquire("A", () => 1, _ => { });
        r.Acquire("B", () => 2, _ => { });
        r.ReleaseAll();
        return r.Log.SequenceEqual(["acquire A", "acquire B", "release B", "release A"]);
    }
}
