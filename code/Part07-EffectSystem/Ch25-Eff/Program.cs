using Ch25.Functions;
using Ch25.Tests;
using Ch25.Traits;
using Ch25.Types;

Console.WriteLine("================================================");
Console.WriteLine("25장 — Eff (런타임 없는 효과)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — 지연 + 오류 포획 ──────────────────────────────────────
Console.WriteLine("== 예제 1 — Eff 는 Run 전까지 미실행, 예외는 Fail 로 포획 ==");

var log = new List<string>();
K<EffF, int> program =
    from a in Eff<int>.Effect(() => { log.Add("작용 a"); return 10; })
    from b in Eff<int>.Effect(() => { log.Add("작용 b"); return 4; })
    select a + b;

Console.WriteLine($"  조립 후 Run 전 — log 비었나? {log.Count == 0}");
Console.WriteLine($"  Run() → {program.As().Run()}");
Console.WriteLine($"  log = [{string.Join(", ", log)}]");

var boom = Eff<int>.Effect(() => int.Parse("nope"));
Console.WriteLine($"  예외 작용 Run() → {boom.As().Run()}   (예외가 Fail 로)");
Console.WriteLine();

// ── 예제 2 — Bind 단락 ──────────────────────────────────────────────
Console.WriteLine("== 예제 2 — 실패하면 이후 단계 미실행 ==");

var ranSecond = false;
var shortCircuit =
    from a in Fallibles.fail<EffF, int>(Error.New("초기 실패"))
    from b in Eff<int>.Effect(() => { ranSecond = true; return 1; })
    select a + b;

Console.WriteLine($"  Run() → {shortCircuit.As().Run()}");
Console.WriteLine($"  둘째 단계 실행됨? {ranSecond}   (false — 단락)");
Console.WriteLine();

// ── 예제 3 — Catch 복구 ─────────────────────────────────────────────
Console.WriteLine("== 예제 3 — Catch 로 폴백 ==");

var recovered = Fallibles.@catch<EffF, int>(boom, _ => EffF.Pure(-1));
Console.WriteLine($"  실패 → Catch → 폴백 = {recovered.As().Run()}");
Console.WriteLine();

// ── 예제 4 — Choose (고르기) 와 Finally (정리) ──────────────────────
Console.WriteLine("== 예제 4 — Choose 로 고르기, Finally 로 정리 ==");

var chosen = Alternatives.choose<EffF, int>(boom, EffF.Pure(99));
Console.WriteLine($"  실패 | 성공 → Choose = {chosen.As().Run()}   (첫째 실패 → 둘째 99)");

var cleanup = new List<string>();
var withCleanup = Finals.@finally<EffF, int, int>(
    boom,
    Eff<int>.Effect(() => { cleanup.Add("정리 실행"); return 0; }));
Console.WriteLine($"  실패 + Finally → {withCleanup.As().Run()}, 정리 = [{string.Join(", ", cleanup)}]   (실패여도 정리 실행)");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (Monad) ==");

Func<K<EffF, int>, string> probe = m => m.As().Run().ToString()!;
Func<int, K<EffF, int>> f = n => EffF.Pure(n + 1);
Func<int, K<EffF, int>> g = n => EffF.Pure(n * 2);
K<EffF, int> m0 = EffF.Pure(5);

var leftId  = MonadLaws.LeftIdentityHolds<EffF, int, int, string>(7, f, probe);
var rightId = MonadLaws.RightIdentityHolds<EffF, int, string>(m0, probe);
var assoc   = MonadLaws.AssociativityHolds<EffF, int, int, int, string>(m0, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
