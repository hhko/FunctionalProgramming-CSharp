using Ch34.Types;

namespace Ch34.Challenges;

// 챌린지 정답 — 정렬의 성질을 property 로 검증.
// 올바른 정렬: 길이 보존 + 오름차순. 일부러 틀린 "정렬"(중복 제거)은 길이 보존 위반으로 잡힌다.
public static class SortProp
{
    public static bool CorrectSortHolds()
    {
        var r = Prop.ForAll(
            Gen.ListOf(Gen.IntRange(0, 50), 15),
            xs =>
            {
                var sorted = xs.OrderBy(x => x).ToList();
                var lengthOk = sorted.Count == xs.Count;
                var orderedOk = sorted.Zip(sorted.Skip(1), (a, b) => a <= b).All(x => x);
                return lengthOk && orderedOk;
            },
            _ => []);
        return r.Ok;
    }

    // 틀린 "정렬"(Distinct+OrderBy)은 길이 보존 성질을 어긴다 → 반례 발견.
    public static bool BuggySortFails()
    {
        var r = Prop.ForAll(
            Gen.ListOf(Gen.IntRange(0, 3), 10),       // 작은 범위 → 중복 잦음
            xs => xs.Distinct().OrderBy(x => x).Count() == xs.Count,
            _ => []);
        return !r.Ok;   // 위반을 잡았으면 true
    }
}
