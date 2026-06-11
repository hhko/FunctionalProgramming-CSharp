using Ch37.Functions;

namespace Ch37.Tests;

// Schedule.Retry 의 *시도 횟수* 를 검증합니다. 지연이 없어 횟수가 결정적입니다.
public static class ScheduleTests
{
    // ① 3 번째에 성공하는 flaky → 시도 횟수는 정확히 3.
    public static bool SucceedsOnThirdAttempt()
    {
        var flaky = Schedule.Flaky(succeedOn: 3);
        var attempts = Schedule.Retry(flaky, maxAttempts: 5);
        return attempts == 3;
    }

    // ② 첫 시도에 성공 → 재시도 없이 1 회로 끝난다.
    public static bool SucceedsImmediately()
    {
        var attempts = Schedule.Retry(() => true, maxAttempts: 5);
        return attempts == 1;
    }

    // ③ 끝까지 실패 → maxAttempts 에서 포기 (시도 횟수 == maxAttempts).
    public static bool GivesUpAtMaxAttempts()
    {
        var attempts = Schedule.Retry(() => false, maxAttempts: 4);
        return attempts == 4;
    }

    // ④ 성공 자리가 maxAttempts 와 같으면 마지막 시도에서 성공 (== maxAttempts).
    public static bool SucceedsOnLastAttempt()
    {
        var flaky = Schedule.Flaky(succeedOn: 4);
        var attempts = Schedule.Retry(flaky, maxAttempts: 4);
        return attempts == 4;
    }
}
