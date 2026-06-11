using Ch22.Functions;
using Ch22.Tests;
using Ch22.Traits;
using Ch22.Types;

Console.WriteLine("================================================");
Console.WriteLine("22장 — MonadIO & LiftIO (IO 를 스택 안으로)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — LiftIO + 지연 ──────────────────────────────────────────
Console.WriteLine("== 예제 1 — ReaderT<int, IOF>: env 주입 + 지연 IO ==");

var sideLog = new List<string>();
IO<int> effect = new IO<int>(() => { sideLog.Add("[IO] 부수작용 실행 → 7"); return 7; });

// env(배수) 를 읽고, IO 로 값을 얻어, 곱한다.
K<ReaderTF<int, IOF>, int> stack =
    from m in Readable.asks<ReaderTF<int, IOF>, int, int>(env => env)
    from n in IOM.liftIO<ReaderTF<int, IOF>, int>(effect)
    select m * n;

// Run(env) 는 IO 를 *조립만* 한다 — 아직 부수 작용 없음.
var io = stack.As().Run(6);
Console.WriteLine($"  Run(env=6) 직후 — IO 미실행? sideLog 비었나 = {sideLog.Count == 0}");

// io.Run() 에서야 부수 작용이 일어난다.
var result = io.As().Run();
Console.WriteLine($"  io.Run() → {result}   (6 × 7)");
Console.WriteLine($"  sideLog = [{string.Join(", ", sideLog)}]");
Console.WriteLine();

Console.WriteLine("  → ReaderT<Env, IOF, A> 는 5부 Eff<RT,A> = ReaderT<RT, IO, A> 의 축소판.");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (ReaderT<int, IOF>) ==");

Func<K<ReaderTF<int, IOF>, int>, int> probe = m => m.As().Run(3).As().Run();
Func<int, K<ReaderTF<int, IOF>, int>> f = n => ReaderTF<int, IOF>.Pure(n + 1);
Func<int, K<ReaderTF<int, IOF>, int>> g = n => ReaderTF<int, IOF>.Pure(n * 2);
var m0 = Readable.asks<ReaderTF<int, IOF>, int, int>(e => e);

var leftId  = MonadLaws.LeftIdentityHolds<ReaderTF<int, IOF>, int, int, int>(7, f, probe);
var rightId = MonadLaws.RightIdentityHolds<ReaderTF<int, IOF>, int, int>(m0, probe);
var assoc   = MonadLaws.AssociativityHolds<ReaderTF<int, IOF>, int, int, int, int>(m0, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
