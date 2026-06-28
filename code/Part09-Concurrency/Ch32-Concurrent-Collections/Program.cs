using Ch32.Challenges;
using Ch32.Tests;
using Ch32.Types;

Console.WriteLine("================================================");
Console.WriteLine("32장 — 동시 컬렉션 & 인과성");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — AtomHashMap 동시 삽입 ─────────────────────────────────
Console.WriteLine("== 예제 1 — AtomHashMap: 동시 삽입 무손실 ==");
var map = new AtomHashMap<int, int>();
Parallel.For(0, 8000, i => map.AddOrUpdate(i, i * i));
Console.WriteLine($"  여러 스레드가 8000개 삽입 → Count = {map.Count}   {(map.Count == 8000 ? "✓ 무손실" : "✗")}");
Console.WriteLine();

// ── 예제 2 — VectorClock 인과성 ─────────────────────────────────────
Console.WriteLine("== 예제 2 — VectorClock: happens-before vs concurrent ==");
var c0 = new VectorClock().Increment("p1");
var c1 = c0.Increment("p1");
var d = new VectorClock().Increment("p2");
Console.WriteLine($"  {c0} vs {c1} → {c0.Compare(c1)}  (선행)");
Console.WriteLine($"  {c0} vs {d}  → {c0.Compare(d)}  (인과 무관)");
Console.WriteLine();

// ── 예제 3 — 메시지 송수신 시나리오 ─────────────────────────────────
Console.WriteLine("== 예제 3 — 송수신 인과성 ==");
var (send, after, order) = Causality.Scenario();
Console.WriteLine($"  p1 송신 {send} vs p2 수신후 {after} → {order}");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("동시 삽입 무손실", ConcurrentLaws.NoLostInsertsHolds()),
    ("happens-before", ConcurrentLaws.HappensBeforeHolds()),
    ("concurrent 감지", ConcurrentLaws.ConcurrentDetectedHolds()),
    ("clock 병합(max)", ConcurrentLaws.MergeHolds()),
};
foreach (var (name, ok) in checks) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
