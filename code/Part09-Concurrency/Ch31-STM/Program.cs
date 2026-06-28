using Ch31.Challenges;
using Ch31.Functions;
using Ch31.Tests;
using Ch31.Types;

Console.WriteLine("================================================");
Console.WriteLine("31장 — STM & Ref (트랜잭션 메모리)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — 단일 트랜잭션 이체 ─────────────────────────────────────
Console.WriteLine("== 예제 1 — 두 Ref 를 한 트랜잭션으로 (all-or-nothing) ==");
var alice = new Ref<long>(100);
var bob = new Ref<long>(0);
STMOps.Atomically(Bank.Transfer(alice, bob, 30));
Console.WriteLine($"  이체 30 후: alice={alice.Current}, bob={bob.Current}, 합={alice.Current + bob.Current}");
Console.WriteLine();

// ── 예제 2 — 동시 이체에도 총액 보존 ───────────────────────────────
Console.WriteLine("== 예제 2 — 8 스레드가 마구 이체해도 총액 불변 ==");
var a = new Ref<long>(1000);
var b = new Ref<long>(1000);
var before = a.Current + b.Current;
Parallel.For(0, 8000, i =>
{
    long amt = (i % 13) + 1;
    STMOps.Atomically(i % 2 == 0 ? Bank.Transfer(a, b, amt) : Bank.Transfer(b, a, amt));
});
var after = a.Current + b.Current;
Console.WriteLine($"  이체 전 총액 = {before}, 이체 후 총액 = {after}   {(before == after ? "✓ 보존" : "✗ 깨짐")}");
Console.WriteLine($"  (alice={a.Current}, bob={b.Current})");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var c1 = STMLaws.SingleTransferHolds();
var c2 = STMLaws.ConcurrentConservationHolds();
Console.WriteLine($"  단일 이체       : {(c1 ? "통과" : "위반")}");
Console.WriteLine($"  동시 총액 보존  : {(c2 ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(c1 && c2 ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
