using Ch16.Challenges;
using Ch16.Functions;
using Ch16.Tests;
using Ch16.Traits;
using Ch16.Types;

Console.WriteLine("================================================");
Console.WriteLine("16장 — 변환기 발상 & lift");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — lift: 내부 모나드를 변환기 한 층 위로 ──────────────────
Console.WriteLine("== 예제 1 — lift: Many 를 OptionT 위로 ==");

K<ManyF, int> choices = new Many<int>([-2, 0, 5]);
var lifted = Trans.lift<OptionTF<ManyF>, ManyF, int>(choices);
Console.WriteLine($"  Many [-2, 0, 5] 를 lift → {Show(lifted)}   (모두 Some)");
Console.WriteLine();

// ── 예제 2 — 두 효과를 한 스택에서 (비결정성 + 실패) ────────────────
Console.WriteLine("== 예제 2 — OptionT<ManyF>: 각 갈래가 독립적으로 실패 ==");

K<OptionTF<ManyF>, int> computed =
    from x in lifted
    from r in Safe.Recip(x)   // x==0 이면 그 갈래만 None
    select r;

Console.WriteLine($"  [-2,0,5] 각각 100/x = {Show(computed)}");
Console.WriteLine("  → 0 갈래만 None, 나머지는 Some. M(=Many) 효과와 Option 효과가 한 번에.");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (OptionT 도 진짜 모나드) ==");

Func<K<OptionTF<ManyF>, int>, string> probe = m => Show(m);
Func<int, K<OptionTF<ManyF>, int>> f = n => OptionTF<ManyF>.Pure(n + 1);
Func<int, K<OptionTF<ManyF>, int>> g = Safe.Recip;
var m0 = lifted;

var leftId  = MonadLaws.LeftIdentityHolds<OptionTF<ManyF>, int, int, string>(5, f, probe);
var rightId = MonadLaws.RightIdentityHolds<OptionTF<ManyF>, int, string>(m0, probe);
var assoc   = MonadLaws.AssociativityHolds<OptionTF<ManyF>, int, int, int, string>(m0, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";

// OptionT<ManyF, int> 의 내부 Many<Option<int>> 를 보기 좋게 출력.
static string Show(K<OptionTF<ManyF>, int> m) =>
    $"[{string.Join(", ", m.As().Run.As().Items.Select(o => o.ToString()))}]";
