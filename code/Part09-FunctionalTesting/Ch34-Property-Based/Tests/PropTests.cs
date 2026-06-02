using Ch34.Types;

namespace Ch34.Tests;

public static class PropTests
{
    static IEnumerable<int> ShrinkInt(int n) { if (n > 0) yield return n - 1; }   // 0 쪽으로 줄임

    // ① 참인 성질 — reverse(reverse(xs)) == xs (모든 리스트).
    public static bool ReverseInvolution()
    {
        var r = Prop.ForAll(
            Gen.ListOf(Gen.IntRange(0, 9), 12),
            xs => { var t = xs.ToList(); t.Reverse(); t.Reverse(); return t.SequenceEqual(xs); },
            _ => []);
        return r.Ok;
    }

    // ② 거짓인 성질 — "모든 n < 100" 은 거짓. 최소 반례는 100 이어야 한다.
    public static (bool failed, int counter) FindsMinimalCounterexample()
    {
        var r = Prop.ForAll(
            Gen.IntRange(0, 1000),
            n => n < 100,
            ShrinkInt);
        return (!r.Ok, r.Counterexample);
    }

    // ③ 참인 성질 — 덧셈 교환법칙.
    public static bool AdditionCommutes()
    {
        var r = Prop.ForAll(
            Gen.IntRange(-1000, 1000),
            n => n + 7 == 7 + n,
            ShrinkInt);
        return r.Ok;
    }
}
