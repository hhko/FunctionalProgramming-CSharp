using Ch32.Types;

namespace Ch32.Tests;

public static class ConduitLaws
{
    // ① Conduit.Then 합성.
    public static bool CompositionHolds()
    {
        var c = Conduit.Map<int, int>(x => x + 1).Then(Conduit.Filter<int>(x => x % 2 == 0));
        return c.Apply([1, 2, 3, 4]).SequenceEqual([2, 4]);
    }

    // ② 파이프라인이 자원을 연 뒤 반드시 닫는다.
    public static bool ResourceClosedHolds()
    {
        var events = new List<string>();
        var count = Pipeline.RunOverFile(
            ["10", "x", "30"],
            Conduit.Choose<string, int>(s => (int.TryParse(s, out var v), v)),
            xs => xs.Count(),
            events);
        return count == 2 && events.SequenceEqual(["open file", "close file"]);
    }

    // ③ sink 가 예외를 던져도 자원은 닫힌다.
    public static bool ClosesOnExceptionHolds()
    {
        var events = new List<string>();
        try
        {
            Pipeline.RunOverFile<int, int>(
                ["1"], Conduit.Map<string, int>(int.Parse),
                _ => throw new InvalidOperationException(),
                events);
        }
        catch (InvalidOperationException) { }
        return events.Contains("close file");
    }
}
