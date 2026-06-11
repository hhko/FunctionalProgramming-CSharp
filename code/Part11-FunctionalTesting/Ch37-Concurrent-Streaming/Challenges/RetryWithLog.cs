using Ch37.Functions;

namespace Ch37.Challenges;

// 챌린지 ② 정답 — Schedule.Retry 에 *시도별 로그* 를 더해 횟수와 *순서* 를 함께 단언한다.
// 시간을 인자에서 뺀 발상이 순서 로그로도 확장된다 — 3 번째에 성공하면 로그는 정확히 [1, 2, 3].
public static class RetryWithLog
{
    // Retry 의 변형 — 매 시도마다 시도 번호를 log 에 기록한다. 반환값은 동일하게 시도 횟수.
    public static int RetryLogged(Func<bool> action, int maxAttempts, List<int> log)
    {
        if (maxAttempts < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), "최소 1 번은 시도해야 합니다.");

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            log.Add(attempt);              // 시도 순서를 기록 — 횟수뿐 아니라 순서도 결정적
            if (action())
                return attempt;
        }
        return maxAttempts;
    }

    // 3 번째에 성공하는 flaky → 로그가 정확히 [1, 2, 3] 인지 단언.
    public static bool ThirdAttemptLogsOneTwoThree()
    {
        var log = new List<int>();
        RetryLogged(Schedule.Flaky(succeedOn: 3), maxAttempts: 5, log);
        return log.SequenceEqual([1, 2, 3]);
    }

    // 즉시 성공 → 로그는 [1] 하나뿐. 로그 길이는 *항상* 시도 횟수와 같다.
    public static bool ImmediateSuccessLogsOne()
    {
        var log = new List<int>();
        RetryLogged(() => true, maxAttempts: 5, log);
        return log.SequenceEqual([1]);
    }
}
