using Ch34.Challenges;
using Ch34.Tests;
using Ch34.Types;

Console.WriteLine("================================================");
Console.WriteLine("31장 — Pipes (Producer / Consumer / Pipe)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — producer → pipe → consumer ────────────────────────────
Console.WriteLine("== 예제 1 — 파이프라인 합성 ==");
var sum = Producer.Of(1, 2, 3, 4, 5)
    .Through(Pipe.Map<int, int>(x => x * 10))
    .Run(Consumer.Sum());
Console.WriteLine($"  Of(1..5) | Map(*10) | Sum = {sum}");
Console.WriteLine();

// ── 예제 2 — Pipe.Then 으로 변환 합성 ───────────────────────────────
Console.WriteLine("== 예제 2 — Map then Filter ==");
var evens = Producer.Of(1, 2, 3, 4, 5, 6)
    .Through(Pipe.Map<int, int>(x => x * x).Then(Pipe.Filter<int>(x => x % 2 == 0)))
    .Run(Consumer.ToList<int>());
Console.WriteLine($"  Of(1..6) | Map(²) | Filter(짝수) | ToList = [{string.Join(", ", evens)}]");
Console.WriteLine();

// ── 예제 3 — 역압: 무한 Producer + Take ────────────────────────────
Console.WriteLine("== 예제 3 — 역압 (무한 생산자, Take 3) ==");
var log = new List<int>();
var taken = Producer.Tapped(1, log)
    .Through(Pipe.Take<int>(3))
    .Run(Consumer.ToList<int>());
Console.WriteLine($"  결과 = [{string.Join(", ", taken)}], 실제 생산 수 = {log.Count}");
Console.WriteLine("  → Consumer 가 당긴 3개만 Producer 가 생산 (push 가 아니라 pull)");
Console.WriteLine();

// ── 예제 4 — 단어 수 세기 파이프라인 ────────────────────────────────
Console.WriteLine("== 예제 4 — 단어 수 세기 ==");
var n = WordCount.Count(" Hello ", "", "FUNCTIONAL", "  ", "csharp");
Console.WriteLine($"  유효 단어 수 = {n}");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("파이프라인 합성", PipeLaws.PipelineHolds()),
    ("Pipe.Then 합성", PipeLaws.PipeCompositionHolds()),
    ("역압", PipeLaws.BackpressureHolds()),
};
foreach (var (name, ok) in checks) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
