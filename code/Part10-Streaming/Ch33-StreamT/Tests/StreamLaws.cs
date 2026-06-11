using Ch33.Types;

namespace Ch33.Tests;

public static class StreamLaws
{
    // ① 무한 스트림 + Take → 종료하며 올바른 값.
    public static bool InfiniteTakeHolds() =>
        Streams.From(1).Take(5).ToList().SequenceEqual([1, 2, 3, 4, 5]);

    // ② Map/Filter 합성 (제곱 중 짝수 처음 3개).
    public static bool MapFilterHolds() =>
        Streams.From(1).Map(x => x * x).Filter(x => x % 2 == 0).Take(3).ToList()
            .SequenceEqual([4, 16, 36]);

    // ③ 효과는 *당긴 만큼만* — Tapped 무한 스트림에서 Take(3) 하면 정확히 3개만 생산.
    public static bool LazyEffectsHolds()
    {
        var log = new List<int>();
        var _ = Streams.Tapped(1, log).Take(3).ToList();
        return log.Count == 3;        // 무한인데 3개만 (메모리/효과 안전)
    }
}
