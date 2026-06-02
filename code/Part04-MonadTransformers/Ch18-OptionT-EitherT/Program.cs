using Ch18.Challenges;
using Ch18.Functions;
using Ch18.Tests;
using Ch18.Traits;
using Ch18.Types;

Console.WriteLine("================================================");
Console.WriteLine("18장 — OptionT / EitherT (오류·부재 + 내부 효과)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — EitherT<string, ManyF>: 갈래마다 오류 보존 ─────────────
Console.WriteLine("== 예제 1 — EitherT<string, ManyF>: 실패 이유를 남긴다 ==");

K<ManyF, int> inputs = new Many<int>([3, -1, 5, 0]);

K<EitherTF<string, ManyF>, int> checked2 =
    from x in Trans.lift<EitherTF<string, ManyF>, ManyF, int>(inputs)
    from p in Parse.Positive(x)
    select p * 10;

Console.WriteLine($"  입력 [3, -1, 5, 0] 검사 → {Show(checked2)}");
Console.WriteLine("  → -1, 0 갈래는 Left(이유), 3·5 갈래는 Right. 비결정 구조는 살아남는다.");
Console.WriteLine();

// ── 예제 2 — Lift + Bind 합성 ───────────────────────────────────────
Console.WriteLine("== 예제 2 — lift 로 내부 Many 효과를 끌어올린다 ==");
var lifted = Trans.lift<EitherTF<string, ManyF>, ManyF, int>(new Many<int>([1, 2]));
Console.WriteLine($"  lift([1,2]) = {Show(lifted)}");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (EitherT) ==");

Func<K<EitherTF<string, ManyF>, int>, string> probe = Show;
Func<int, K<EitherTF<string, ManyF>, int>> f = n => EitherTF<string, ManyF>.Pure(n + 1);
Func<int, K<EitherTF<string, ManyF>, int>> g = Parse.Positive;
var m0 = lifted;

var leftId  = MonadLaws.LeftIdentityHolds<EitherTF<string, ManyF>, int, int, string>(5, f, probe);
var rightId = MonadLaws.RightIdentityHolds<EitherTF<string, ManyF>, int, string>(m0, probe);
var assoc   = MonadLaws.AssociativityHolds<EitherTF<string, ManyF>, int, int, int, string>(m0, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";

static string Show(K<EitherTF<string, ManyF>, int> m) =>
    $"[{string.Join(", ", m.As().Run.As().Items.Select(e => e.ToString()))}]";
