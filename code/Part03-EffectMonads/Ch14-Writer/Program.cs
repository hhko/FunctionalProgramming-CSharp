using Ch14.Challenges;
using Ch14.Functions;
using Ch14.Tests;
using Ch14.Traits;
using Ch14.Types;

Console.WriteLine("================================================");
Console.WriteLine("14장 — Writer (누적 출력 효과)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — Tell + Bind 로 로그 누적 ───────────────────────────────
Console.WriteLine("== 예제 1 — 계산 과정을 로그로 누적 ((a+b)*c) ==");

var computed = TracedMath.Compute(2, 3, 4);
var (result, log) = computed.As().Run();
Console.WriteLine($"  결과 = {result}");
Console.WriteLine($"  로그 = {log}");
Console.WriteLine();

// ── 예제 2 — Listen: 하위 계산이 말한 출력을 값으로 ─────────────────
Console.WriteLine("== 예제 2 — Listen ==");

var listened = Writable.listen<WriterF<Log>, Log, int>(TracedMath.Compute(2, 3, 4));
var captured = listened.As();
Console.WriteLine($"  Listen 결과 값 = (result={captured.Value.Value}, 캡처된 로그 줄 수={captured.Value.Output.Lines.Count})");
Console.WriteLine();

// ── 예제 3 — Pass: 출력 후처리 (곱셈 줄만 남기기) ───────────────────
Console.WriteLine("== 예제 3 — Pass (출력 검열: '*' 포함 줄만) ==");

var passed = Writable.pass<WriterF<Log>, Log, int>(
    from r in TracedMath.Compute(2, 3, 4)
    select (r, (Func<Log, Log>)(l => new Log(l.Lines.Where(x => x.Contains('*')).ToList()))));

var (pr, plog) = passed.As().Run();
Console.WriteLine($"  결과 = {pr}");
Console.WriteLine($"  검열된 로그 = {plog}");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (Monad) ==");

Func<K<WriterF<Log>, int>, (int, string)> probe =
    m => (m.As().Value, string.Join("|", m.As().Output.Lines));
Func<int, K<WriterF<Log>, int>> f = n => new Writer<Log, int>(n + 1, Log.Of($"f({n})"));
Func<int, K<WriterF<Log>, int>> g = n => new Writer<Log, int>(n * 2, Log.Of($"g({n})"));
K<WriterF<Log>, int> m = new Writer<Log, int>(10, Log.Of("start"));

var leftId  = MonadLaws.LeftIdentityHolds<WriterF<Log>, int, int, (int, string)>(3, f, probe);
var rightId = MonadLaws.RightIdentityHolds<WriterF<Log>, int, (int, string)>(m, probe);
var assoc   = MonadLaws.AssociativityHolds<WriterF<Log>, int, int, int, (int, string)>(m, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
