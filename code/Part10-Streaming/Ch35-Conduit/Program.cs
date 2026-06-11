using Ch35.Challenges;
using Ch35.Tests;
using Ch35.Types;

Console.WriteLine("================================================");
Console.WriteLine("32장 — Conduit & 실전 파이프라인");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — Conduit 변환 합성 ──────────────────────────────────────
Console.WriteLine("== 예제 1 — Conduit.Then 으로 변환 단계 합성 ==");
var conduit = Conduit.Map<int, int>(x => x + 1).Then(Conduit.Filter<int>(x => x % 2 == 0));
Console.WriteLine($"  [1,2,3,4] | Map(+1) | Filter(짝수) = [{string.Join(", ", conduit.Apply([1, 2, 3, 4]))}]");
Console.WriteLine();

// ── 예제 2 — 자원 안전 ETL ──────────────────────────────────────────
Console.WriteLine("== 예제 2 — 로그 파일 ETL (파싱→임계값→합산, 자원 개폐 보장) ==");
var (total, events) = Etl.SumAboveThreshold(["10", "bad", "30", "5", "100"], threshold: 20);
Console.WriteLine($"  임계값 20 이상 합 = {total}   (30 + 100)");
Console.WriteLine($"  자원 이벤트 = [{string.Join(" → ", events)}]");
Console.WriteLine();

// ── 예제 3 — Pipes vs Conduit ───────────────────────────────────────
Console.WriteLine("== 예제 3 — Pipes vs Conduit ==");
Console.WriteLine("  Pipes(31장)   : Producer/Consumer/Pipe 세 역할 분리 + 당김 역압");
Console.WriteLine("  Conduit(32장) : 변환을 IEnumerable<I>→IEnumerable<O> 하나로 (lazy LINQ 위)");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("Conduit 합성", ConduitLaws.CompositionHolds()),
    ("자원 개폐 보장", ConduitLaws.ResourceClosedHolds()),
    ("예외에도 닫힘", ConduitLaws.ClosesOnExceptionHolds()),
};
foreach (var (name, ok) in checks) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
