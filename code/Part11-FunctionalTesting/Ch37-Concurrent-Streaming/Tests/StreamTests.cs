using Ch37.Types;

namespace Ch37.Tests;

// Source 스트림의 *생성 시퀀스* 와 *effect 시퀀스* 를 기대 골든과 비교합니다 (골든 테스트).
public static class StreamTests
{
    // ① 생성 시퀀스 — From(5) 는 0,1,2,3,4 를 정확히 그 순서로 흘린다.
    public static bool ProducesExpectedSequence()
    {
        var trace = new List<int>();
        var result = Source.From(5, trace).ToList();
        return result.SequenceEqual([0, 1, 2, 3, 4]);
    }

    // ② map∘filter 골든 — 짝수만 통과시켜 ×10 하면 0,20,40 (모양·순서 보존).
    public static bool MapFilterMatchesGolden()
    {
        var trace = new List<int>();
        var result = Source.Map(
                Source.Filter(Source.From(5, trace), n => n % 2 == 0),
                n => n * 10)
            .ToList();
        return result.SequenceEqual([0, 20, 40]);
    }

    // ③ effect 게으름 — 스트림은 *소비* 될 때만 effect(trace 기록)를 낸다.
    //    Take(3) 이면 0,1,2 까지만 흐르고, trace 도 [0,1,2] 에서 멈춘다 (lazy 결정성).
    public static bool LazyEffectStopsEarly()
    {
        var trace = new List<int>();
        var taken = Source.From(100, trace).Take(3).ToList();
        return taken.SequenceEqual([0, 1, 2])
            && trace.SequenceEqual([0, 1, 2]);   // 4 번째 이후는 흐르지 않았다
    }
}
