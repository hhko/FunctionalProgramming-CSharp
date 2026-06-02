using Ch30.Challenges;
using Ch30.Tests;
using Ch30.Types;

Console.WriteLine("================================================");
Console.WriteLine("30장 — StreamT (효과적 lazy 스트림)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — 무한 스트림 + Take ─────────────────────────────────────
Console.WriteLine("== 예제 1 — 무한 스트림을 유한하게 소비 ==");
var first5 = Streams.From(1).Take(5).ToList();
Console.WriteLine($"  From(1).Take(5) = [{string.Join(", ", first5)}]");
Console.WriteLine();

// ── 예제 2 — Map/Filter 합성 ────────────────────────────────────────
Console.WriteLine("== 예제 2 — 제곱 중 짝수 처음 3개 ==");
var sq = Streams.From(1).Map(x => x * x).Filter(x => x % 2 == 0).Take(3).ToList();
Console.WriteLine($"  From(1).Map(²).Filter(짝수).Take(3) = [{string.Join(", ", sq)}]");
Console.WriteLine();

// ── 예제 3 — 효과는 당긴 만큼만 ─────────────────────────────────────
Console.WriteLine("== 예제 3 — 무한 스트림인데 효과는 3번만 ==");
var log = new List<int>();
var pulled = Streams.Tapped(1, log).Take(3).ToList();
Console.WriteLine($"  결과 = [{string.Join(", ", pulled)}], 실제 생산된 원소 수 = {log.Count}");
Console.WriteLine("  → 무한 스트림이지만 당긴 3개만 계산 (메모리/효과 안전)");
Console.WriteLine();

// ── 예제 4 — 무한 소수 체 ───────────────────────────────────────────
Console.WriteLine("== 예제 4 — 게으른 소수 체 ==");
Console.WriteLine($"  소수 처음 10개 = [{string.Join(", ", Sieve.Primes(10))}]");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("무한+Take 종료", StreamLaws.InfiniteTakeHolds()),
    ("Map/Filter 합성", StreamLaws.MapFilterHolds()),
    ("당긴 만큼만 효과", StreamLaws.LazyEffectsHolds()),
};
foreach (var (name, ok) in checks) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
