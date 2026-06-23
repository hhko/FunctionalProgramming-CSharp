using Ch23.Functions;
using Ch23.Tests;
using Ch23.Traits;
using Ch23.Types;

Console.WriteLine("================================================");
Console.WriteLine("20장 — IO (지연 효과 DSL + 스택 안전)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — 지연: Run 전에는 부수 효과가 없다 ─────────────────────
Console.WriteLine("== 예제 1 — IO 는 Run 전까지 아무 일도 안 한다 ==");

var log = new List<string>();
K<IOF, int> program =
    from a in IO<int>.Effect(() => { log.Add("[IO] 첫 작용 → 10"); return 10; })
    from b in IO<int>.Effect(() => { log.Add("[IO] 둘째 작용 → 32"); return 32; })
    select a + b;

Console.WriteLine($"  IO 조립 후, Run 전 — log 비었나? {log.Count == 0}");
var result = program.As().Run();
Console.WriteLine($"  Run() → {result}");
Console.WriteLine($"  log = [{string.Join(", ", log)}]");
Console.WriteLine();

// ── 예제 2 — 깊은 Bind 체인의 스택 안전성 ───────────────────────────
Console.WriteLine("== 예제 2 — 100,000 단계 Bind (스택 안전) ==");

K<IOF, long> deep = IOF.Pure(0L);
for (var i = 1; i <= 100_000; i++)
{
    var j = i;
    deep = deep.Bind(acc => IOF.Pure(acc + j));
}
Console.WriteLine($"  sum(1..100000) via 10만 Bind = {deep.As().Run()}   (StackOverflow 없음)");
Console.WriteLine();

// ── 예제 3 — EnvIO 취소 ─────────────────────────────────────────────
Console.WriteLine("== 예제 3 — EnvIO 의 취소 ==");
using var cts = new CancellationTokenSource();
cts.Cancel();
try
{
    program.As().Run(new EnvIO(cts.Token));
    Console.WriteLine("  (취소 안 됨 — 예상 밖)");
}
catch (OperationCanceledException)
{
    Console.WriteLine("  이미 취소된 토큰으로 Run → OperationCanceledException (인터프리터가 단계마다 확인)");
}
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (Monad) ==");

Func<K<IOF, int>, int> probe = m => m.As().Run();
Func<int, K<IOF, int>> f = n => IOF.Pure(n + 1);
Func<int, K<IOF, int>> g = n => IOF.Pure(n * 2);
K<IOF, int> m0 = IOF.Pure(5);

var leftId  = MonadLaws.LeftIdentityHolds<IOF, int, int, int>(7, f, probe);
var rightId = MonadLaws.RightIdentityHolds<IOF, int, int>(m0, probe);
var assoc   = MonadLaws.AssociativityHolds<IOF, int, int, int, int>(m0, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
