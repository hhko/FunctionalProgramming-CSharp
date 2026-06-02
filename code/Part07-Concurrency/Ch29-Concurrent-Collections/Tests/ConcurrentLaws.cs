using Ch29.Types;

namespace Ch29.Tests;

public static class ConcurrentLaws
{
    // ① 동시 삽입 — 잃어버린 항목 없음.
    public static bool NoLostInsertsHolds()
    {
        var map = new AtomHashMap<int, int>();
        Parallel.For(0, 8000, i => map.AddOrUpdate(i, i * i));
        return map.Count == 8000;
    }

    // ② 인과성 — increment 한 시계는 이전 시계보다 Before.
    public static bool HappensBeforeHolds()
    {
        var c0 = new VectorClock().Increment("p1");
        var c1 = c0.Increment("p1");
        return c0.Compare(c1) == VectorClock.Ordering.Before;
    }

    // ③ 동시성 — 서로 다른 노드의 독립 이벤트는 Concurrent.
    public static bool ConcurrentDetectedHolds()
    {
        var a = new VectorClock().Increment("p1");   // {p1:1}
        var b = new VectorClock().Increment("p2");   // {p2:1}
        return a.Compare(b) == VectorClock.Ordering.Concurrent;
    }

    // ④ 병합 — 두 시계의 max.
    public static bool MergeHolds()
    {
        var a = new VectorClock().Increment("p1").Increment("p1");  // {p1:2}
        var b = new VectorClock().Increment("p2");                  // {p2:1}
        var m = a.Merge(b);
        return m.Times["p1"] == 2 && m.Times["p2"] == 1;
    }
}
