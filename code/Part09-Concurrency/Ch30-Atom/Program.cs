using Ch30.Tests;
using Ch30.Types;

Console.WriteLine("================================================");
Console.WriteLine("30장 — Atom (CAS 원자성)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — 기본 Swap ──────────────────────────────────────────────
Console.WriteLine("== 예제 1 — Swap 은 순수 함수로 갱신 ==");
var atom = new Atom<Counter>(new Counter(0));
atom.Swap(c => c with { N = c.N + 10 });
atom.Swap(c => c with { N = c.N * 2 });
Console.WriteLine($"  (+10) 후 (*2) → {atom.Value.N}");
Console.WriteLine();

// ── 예제 2 — 동시 증가: Atom vs 락 없는 int ────────────────────────
Console.WriteLine("== 예제 2 — 8 스레드 × 10,000 증가 ==");
const int tasks = 8, perTask = 10_000;
var expected = (long)tasks * perTask;

var safe = new Atom<Counter>(new Counter(0));
Parallel.For(0, tasks, _ =>
{
    for (var i = 0; i < perTask; i++) safe.Swap(c => c with { N = c.N + 1 });
});

int naive = 0;
Parallel.For(0, tasks, _ =>
{
    for (var i = 0; i < perTask; i++) naive++;   // 락 없는 공유 증가 — 경쟁
});

Console.WriteLine($"  기대값          = {expected}");
Console.WriteLine($"  Atom (CAS)      = {safe.Value.N}   {(safe.Value.N == expected ? "✓ 정확" : "✗")}");
Console.WriteLine($"  락 없는 int++   = {naive}   {(naive < expected ? "← 갱신 유실 (경쟁)" : "")}");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var c1 = AtomLaws.SequentialSwapHolds();
var c2 = AtomLaws.ConcurrentNoLostUpdatesHolds();
Console.WriteLine($"  순차 Swap        : {(c1 ? "통과" : "위반")}");
Console.WriteLine($"  동시 갱신 무손실 : {(c2 ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(c1 && c2 ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
