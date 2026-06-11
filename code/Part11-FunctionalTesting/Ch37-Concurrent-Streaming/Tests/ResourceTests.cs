using Ch37.Types;

namespace Ch37.Tests;

// Resource.Use(bracket)와 Scope(LIFO 해제)의 *자원 수명 보장* 을 검증합니다.
public static class ResourceTests
{
    // ① 정상 경로 — acquire → use → release 순서로 실행된다.
    public static bool NormalOrderHolds()
    {
        var log = new List<string>();
        Resource.Use(
            () => { log.Add("acquire"); return 1; },
            _ => { log.Add("use"); return 0; },
            _ => log.Add("release"));
        return log.SequenceEqual(["acquire", "use", "release"]);
    }

    // ② 예외 경로 — use 가 예외를 던져도 release 는 *반드시* 실행된다 (flag 로 확인).
    public static bool ReleaseOnExceptionHolds()
    {
        var released = false;
        try
        {
            Resource.Use<int, int>(
                () => 1,
                _ => throw new InvalidOperationException("boom"),
                _ => released = true);
        }
        catch (InvalidOperationException) { /* 예외는 정상적으로 전파됩니다 */ }
        return released;
    }

    // ③ 중첩 자원 — A, B 순서로 열면 종료는 B, A 순서(LIFO).
    public static bool LifoReleaseOrderHolds()
    {
        var scope = new Scope();
        scope.Acquire("A", () => 1, _ => { });
        scope.Acquire("B", () => 2, _ => { });
        scope.ReleaseAll();
        return scope.Log.SequenceEqual(["acquire A", "acquire B", "release B", "release A"]);
    }
}
