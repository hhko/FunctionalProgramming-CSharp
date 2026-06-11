using Ch41.Types;

namespace Ch41.Tests;

public static class PipelineTests
{
    static readonly string[] Records = ["10", "x", "20", "", "30", "bad", "40"];

    // ① 파싱 성공만 합산 (10+20+30+40 = 100).
    public static bool SumsValidOnly()
    {
        var r = Pipeline.Run(Records, maxRetries: 5, flakyUntilAttempt: 1);
        return r is { Sum: 100, Valid: 4 };
    }

    // ② 불안정한 연결을 재시도로 복구 (3번째 시도에 성공).
    public static bool RetriesConnection()
    {
        var r = Pipeline.Run(Records, maxRetries: 5, flakyUntilAttempt: 3);
        return r.ConnectAttempts == 3 && r.Sum == 100;
    }

    // ③ 자원이 열리고 반드시 닫힌다.
    public static bool ResourceLifecycle()
    {
        var r = Pipeline.Run(Records, maxRetries: 5, flakyUntilAttempt: 1);
        return r.Events.SequenceEqual(["open source", "close source"]);
    }

    // ④ 재시도 한도를 넘으면 실패 (연결 안 됨 → 자원 안 열림).
    public static bool FailsWhenUnrecoverable()
    {
        try { Pipeline.Run(Records, maxRetries: 2, flakyUntilAttempt: 100); return false; }
        catch (InvalidOperationException) { return true; }
    }
}
