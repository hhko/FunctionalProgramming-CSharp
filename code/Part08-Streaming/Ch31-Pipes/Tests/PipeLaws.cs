using Ch31.Types;

namespace Ch31.Tests;

public static class PipeLaws
{
    // ① producer → pipe → consumer 합성.
    public static bool PipelineHolds()
    {
        var result = Producer.Of(1, 2, 3, 4, 5)
            .Through(Pipe.Map<int, int>(x => x * 10))
            .Run(Consumer.Sum());
        return result == 150;
    }

    // ② Pipe.Then 합성 (Map 후 Filter).
    public static bool PipeCompositionHolds()
    {
        var list = Producer.Of(1, 2, 3, 4, 5, 6)
            .Through(Pipe.Map<int, int>(x => x * x).Then(Pipe.Filter<int>(x => x % 2 == 0)))
            .Run(Consumer.ToList<int>());
        return list.SequenceEqual([4, 16, 36]);
    }

    // ③ 역압 — Take 가 상류 생산을 멈춘다 (무한 Producer 라도 당긴 만큼만).
    public static bool BackpressureHolds()
    {
        var log = new List<int>();
        var _ = Producer.Tapped(1, log)
            .Through(Pipe.Take<int>(3))
            .Run(Consumer.ToList<int>());
        return log.Count == 3;
    }
}
