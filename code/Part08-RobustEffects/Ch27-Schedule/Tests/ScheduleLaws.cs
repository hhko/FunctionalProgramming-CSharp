using Ch27.Types;

namespace Ch27.Tests;

// Schedule 의 구조 법칙 검증 (콘솔 bool).
public static class ScheduleLaws
{
    static List<double> Take(Schedule s, int n) => s.Durations.Take(n).Select(d => d.TotalMs).ToList();

    // ① recurs(n) 은 정확히 n 개의 간격을 낸다.
    public static bool RecursCountHolds(int n) =>
        Schedule.Recurs(n).Durations.Count() == n;

    // ② union 의 길이는 둘 중 *긴* 쪽 (recurs(2) | recurs(5) → 5).
    public static bool UnionLengthHolds() =>
        (Schedule.Recurs(2) | Schedule.Recurs(5)).Durations.Count() == 5;

    // ③ intersect 의 길이는 둘 중 *짧은* 쪽 (recurs(2) & recurs(5) → 2).
    public static bool IntersectLengthHolds() =>
        (Schedule.Recurs(2) & Schedule.Recurs(5)).Durations.Count() == 2;

    // ④ union 은 더 짧은 간격을 택한다 (spaced(100) | spaced(10) 의 처음 = 10).
    public static bool UnionPicksMinHolds() =>
        Take(Schedule.Spaced(Duration.Ms(100)) | Schedule.Spaced(Duration.Ms(10)), 1)[0] == 10;

    // ⑤ intersect 은 더 긴 간격을 택한다 (spaced(100) & spaced(10) 의 처음 = 100).
    public static bool IntersectPicksMaxHolds() =>
        Take((Schedule.Spaced(Duration.Ms(100)) & Schedule.Spaced(Duration.Ms(10))), 1)[0] == 100;
}
