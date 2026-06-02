namespace Ch24.Types;

// Duration — 한 번의 대기 간격.
public readonly record struct Duration(TimeSpan Span)
{
    public static Duration Ms(double ms) => new(TimeSpan.FromMilliseconds(ms));
    public static Duration Zero => new(TimeSpan.Zero);
    public double TotalMs => Span.TotalMilliseconds;
    public override string ToString() => $"{TotalMs}ms";
}

// Schedule — *(잠재적으로 무한한) Duration 스트림*. "다음에 언제 다시 시도/반복할지" 를 기술한다.
// (LanguageExt v5 의 Schedule : Semigroup<Schedule> 발상 — 여기선 IEnumerable<Duration> 로 모델링.)
//
// 핵심 조합:
//   union     | : 둘 중 *짧은* 간격, 길이는 *긴* 쪽까지 (둘 중 하나라도 계속하면 계속)
//   intersect & : 둘 중 *긴* 간격, 길이는 *짧은* 쪽까지 (둘 다 계속해야 계속)
public sealed class Schedule(IEnumerable<Duration> durations)
{
    public IEnumerable<Duration> Durations => durations;

    // ── 생성자 ──────────────────────────────────────────────────────
    public static Schedule Recurs(int times) =>
        new(Enumerable.Repeat(Duration.Zero, Math.Max(0, times)));

    public static Schedule Spaced(Duration d) =>
        new(Repeat(d));

    public static Schedule Exponential(Duration first) =>
        new(Exp(first));

    public static Schedule Linear(Duration step) =>
        new(Lin(step));

    public static Schedule Once => Recurs(1);

    // ── 조합 ────────────────────────────────────────────────────────
    public Schedule Union(Schedule other) => new(Merge(Durations, other.Durations, min: true));
    public Schedule Intersect(Schedule other) => new(Merge(Durations, other.Durations, min: false));

    public static Schedule operator |(Schedule a, Schedule b) => a.Union(b);
    public static Schedule operator &(Schedule a, Schedule b) => a.Intersect(b);

    // ── 내부 생성기 ─────────────────────────────────────────────────
    static IEnumerable<Duration> Repeat(Duration d) { while (true) yield return d; }

    static IEnumerable<Duration> Exp(Duration first)
    {
        var cur = first.TotalMs;
        while (true) { yield return Duration.Ms(cur); cur *= 2; }
    }

    static IEnumerable<Duration> Lin(Duration step)
    {
        var cur = step.TotalMs;
        while (true) { yield return Duration.Ms(cur); cur += step.TotalMs; }
    }

    // union(min=true): 둘 중 하나라도 남아 있으면 계속, 간격은 둘 다 있을 때 min.
    // intersect(min=false): 둘 다 있어야 계속, 간격은 max.
    static IEnumerable<Duration> Merge(IEnumerable<Duration> xs, IEnumerable<Duration> ys, bool min)
    {
        using var ex = xs.GetEnumerator();
        using var ey = ys.GetEnumerator();
        var hx = ex.MoveNext();
        var hy = ey.MoveNext();
        while (true)
        {
            if (hx && hy)
            {
                var d = min
                    ? (ex.Current.TotalMs <= ey.Current.TotalMs ? ex.Current : ey.Current)
                    : (ex.Current.TotalMs >= ey.Current.TotalMs ? ex.Current : ey.Current);
                yield return d;
                hx = ex.MoveNext();
                hy = ey.MoveNext();
            }
            else if (min && hx) { yield return ex.Current; hx = ex.MoveNext(); }
            else if (min && hy) { yield return ey.Current; hy = ey.MoveNext(); }
            else yield break;
        }
    }
}
