using Ch27.Functions;
using Ch27.Tests;
using Ch27.Types;

Console.WriteLine("================================================");
Console.WriteLine("24장 — Schedule (재시도 / 반복 정책)");
Console.WriteLine("================================================");
Console.WriteLine();

static string Show(Schedule s, int n) =>
    $"[{string.Join(", ", s.Durations.Take(n).Select(d => d.ToString()))}{(s.Durations.Skip(n).Any() ? ", ..." : "")}]";

// ── 예제 1 — Schedule 은 Duration 스트림 ────────────────────────────
Console.WriteLine("== 예제 1 — 정책을 값으로 ==");
Console.WriteLine($"  recurs(3)         = {Show(Schedule.Recurs(3), 5)}");
Console.WriteLine($"  spaced(50ms)      = {Show(Schedule.Spaced(Duration.Ms(50)), 3)}");
Console.WriteLine($"  exponential(10ms) = {Show(Schedule.Exponential(Duration.Ms(10)), 5)}");
Console.WriteLine();

// ── 예제 2 — 조합 (union | / intersect &) ───────────────────────────
Console.WriteLine("== 예제 2 — 정책 합성 ==");
var capped = Schedule.Recurs(5) & Schedule.Exponential(Duration.Ms(10));
Console.WriteLine($"  recurs(5) & exponential(10ms) = {Show(capped, 8)}   (최대 5회 + 지수 백오프)");
var either = Schedule.Spaced(Duration.Ms(100)) | Schedule.Spaced(Duration.Ms(10));
Console.WriteLine($"  spaced(100) | spaced(10)      = {Show(either, 3)}   (둘 중 짧은 간격)");
Console.WriteLine();

// ── 예제 3 — 효과에 retry 얹기 ──────────────────────────────────────
Console.WriteLine("== 예제 3 — 실패하는 효과에 retry ==");
var attempt = 0;
Fin<int> Flaky()
{
    attempt++;
    return attempt >= 3 ? new Fin<int>.Succ(42) : new Fin<int>.Fail(new Error($"{attempt}번째 실패"));
}
var (result, attempts, delays) = Retry.RetryFin(Flaky, capped);
Console.WriteLine($"  결과 = {result}, 시도 횟수 = {attempts}, 사용된 간격 = [{string.Join(", ", delays)}]");
Console.WriteLine();

// ── 예제 4 — repeat 로 반복 수집 ────────────────────────────────────
Console.WriteLine("== 예제 4 — repeat (성공을 N회 반복) ==");
var tick = 0;
var collected = Retry.RepeatCollect(() => ++tick, Schedule.Recurs(4));
Console.WriteLine($"  recurs(4) 반복 결과 = [{string.Join(", ", collected)}]   (최초 1 + 4회)");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 구조 법칙 검증 ==");
var checks = new (string, bool)[]
{
    ("recurs(n) 개수", ScheduleLaws.RecursCountHolds(7)),
    ("union 길이=max", ScheduleLaws.UnionLengthHolds()),
    ("intersect 길이=min", ScheduleLaws.IntersectLengthHolds()),
    ("union=짧은 간격", ScheduleLaws.UnionPicksMinHolds()),
    ("intersect=긴 간격", ScheduleLaws.IntersectPicksMaxHolds()),
};
foreach (var (name, ok) in checks) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
