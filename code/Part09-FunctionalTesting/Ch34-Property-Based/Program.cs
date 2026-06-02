using Ch34.Challenges;
using Ch34.Tests;
using Ch34.Types;

Console.WriteLine("================================================");
Console.WriteLine("34장 — property-based testing (생성기 + 축소)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — 참인 성질 (200 케이스 통과) ───────────────────────────
Console.WriteLine("== 예제 1 — reverse∘reverse = id (모든 리스트) ==");
var rev = Prop.ForAll(
    Gen.ListOf(Gen.IntRange(0, 9), 12),
    xs => { var t = xs.ToList(); t.Reverse(); t.Reverse(); return t.SequenceEqual(xs); },
    _ => []);
Console.WriteLine($"  {rev.Cases} 케이스 검사 → {(rev.Ok ? "통과" : "반례 발견")}");
Console.WriteLine();

// ── 예제 2 — 거짓 성질 + 최소 반례 ─────────────────────────────────
Console.WriteLine("== 예제 2 — '모든 n < 100' (거짓) → 최소 반례 축소 ==");
var bad = Prop.ForAll(Gen.IntRange(0, 1000), n => n < 100, n => n > 0 ? [n - 1] : []);
Console.WriteLine($"  성질 성립? {bad.Ok}");
Console.WriteLine($"  최초 반례 발견까지 {bad.Cases} 케이스, 축소 {bad.Shrinks}회 → 최소 반례 = {bad.Counterexample}");
Console.WriteLine();

// ── 예제 3 — 정렬의 성질 ────────────────────────────────────────────
Console.WriteLine("== 예제 3 — 정렬 성질 (길이 보존 + 오름차순) ==");
Console.WriteLine($"  올바른 정렬 성질 통과? {SortProp.CorrectSortHolds()}");
Console.WriteLine($"  틀린 '정렬'(중복 제거) 반례 잡힘? {SortProp.BuggySortFails()}");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var (failed, counter) = PropTests.FindsMinimalCounterexample();
var checks = new (string, bool)[]
{
    ("reverse involution", PropTests.ReverseInvolution()),
    ("덧셈 교환", PropTests.AdditionCommutes()),
    ("최소 반례 = 100", failed && counter == 100),
    ("올바른 정렬 성질", SortProp.CorrectSortHolds()),
    ("틀린 정렬 반례 검출", SortProp.BuggySortFails()),
};
foreach (var (name, ok) in checks) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
