namespace Ch37.Functions;

// Schedule — *재시도 정책* 의 축소판. action 이 성공(true)할 때까지, 최대 maxAttempts 번 다시 시도합니다.
// (LanguageExt v5 Schedule 의 핵심 — "언제·몇 번 다시 할까" 를 값으로 다룹니다. 여기서는 횟수만.)
//
// 테스트 관점 — 재시도는 실제 시간(지연)을 끼면 비결정적이 됩니다. 그래서 toy 는 *지연을 빼고*
// "시도 횟수" 만 셉니다. "3 번째에 성공하는 flaky → 시도 3 회", "끝까지 실패 → maxAttempts 에서 포기"
// 처럼 횟수가 결정적이라, 시간에 의존하지 않고 단언할 수 있습니다.
public static class Schedule
{
    // Retry — action 이 true 를 낼 때까지 재시도. 실제 시도한 횟수를 반환합니다.
    // 성공하면 그 시점의 횟수, 끝까지 실패하면 maxAttempts 를 반환합니다.
    public static int Retry(Func<bool> action, int maxAttempts)
    {
        if (maxAttempts < 1)
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), "최소 1 번은 시도해야 합니다.");

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            if (action())
                return attempt;        // 성공 — 이번이 몇 번째 시도였는지 반환
        }
        return maxAttempts;            // 끝까지 실패 — maxAttempts 에서 포기
    }

    // Flaky — 테스트용 헬퍼. 호출될 때마다 카운터를 올리고, succeedOn 번째 호출에서만 true.
    // (예: succeedOn=3 이면 1·2 번째는 false, 3 번째에 true → "3 번째에 성공하는 flaky".)
    public static Func<bool> Flaky(int succeedOn)
    {
        var calls = 0;
        return () => ++calls == succeedOn;
    }
}
