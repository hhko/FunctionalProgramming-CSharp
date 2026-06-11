using Ch27.Types;

namespace Ch27.Challenges;

// 챌린지 ② 정답 — 실무 재시도 정책을 *조합* 으로 이름 붙여 정의.
public static class Policies
{
    // 최대 maxRetries 회 + 지수 백오프 (둘 다 만족하는 동안 = intersect).
    public static Schedule ExponentialCapped(int maxRetries, Duration first) =>
        Schedule.Recurs(maxRetries) & Schedule.Exponential(first);

    // 즉시 burst 회 시도 후, 그 다음부터는 고정 간격 (concat).
    public static Schedule BurstThenSpaced(int burst, Duration spacing) =>
        new(Schedule.Recurs(burst).Durations.Concat(Schedule.Spaced(spacing).Durations));
}
