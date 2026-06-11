using Ch26.Functions;
using Ch26.Tests;
using Ch26.Traits;
using Ch26.Types;

Console.WriteLine("================================================");
Console.WriteLine("23장 — Eff<RT,A> = ReaderT<RT, IO, A> + Has DI");
Console.WriteLine("================================================");
Console.WriteLine();

// 콘솔 능력을 쓰는 *하나의 효과 코드* — 런타임 RT 가 Has<RT, IConsole> 이기만 하면 된다.
static K<ReaderTF<RT, IOF>, string> Greet<RT>() where RT : Has<RT, IConsole> =>
    from _1 in Eff.WriteLine<RT>("이름이 무엇인가요?")
    from name in Eff.ReadLine<RT>()
    from _2 in Eff.WriteLine<RT>($"반가워요, {name}!")
    select name;

// ── 예제 1 — 테스트 런타임으로 결정적 실행 ─────────────────────────
Console.WriteLine("== 예제 1 — 같은 효과 코드 + TestConsole (부수 작용 없이 결정적) ==");

var test = new TestConsole(["철수"]);
var returned = Eff.Run(Greet<AppRT>(), new AppRT(test));

Console.WriteLine($"  반환값      = {returned}");
Console.WriteLine($"  콘솔 출력   = [{string.Join(" / ", test.Output)}]");
Console.WriteLine("  → Greet 코드는 한 줄도 안 바뀌고, 런타임만 TestConsole 로 주입.");
Console.WriteLine();

// ── 예제 2 — 다른 입력의 런타임 ─────────────────────────────────────
Console.WriteLine("== 예제 2 — 다른 런타임 (다른 입력) ==");
var test2 = new TestConsole(["영희"]);
var returned2 = Eff.Run(Greet<AppRT>(), new AppRT(test2));
Console.WriteLine($"  반환값 = {returned2}, 출력 마지막 줄 = {test2.Output[^1]}");
Console.WriteLine("  (LiveConsole 런타임이면 실제 콘솔 입출력 — 코드는 동일)");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (Eff<AppRT> = ReaderT<AppRT, IO>) ==");

var dummy = new AppRT(new TestConsole([]));
Func<K<ReaderTF<AppRT, IOF>, int>, int> probe = m => Eff.Run(m, dummy);
Func<int, K<ReaderTF<AppRT, IOF>, int>> f = n => ReaderTF<AppRT, IOF>.Pure(n + 1);
Func<int, K<ReaderTF<AppRT, IOF>, int>> g = n => ReaderTF<AppRT, IOF>.Pure(n * 2);
K<ReaderTF<AppRT, IOF>, int> m0 = ReaderTF<AppRT, IOF>.Pure(5);

var leftId  = MonadLaws.LeftIdentityHolds<ReaderTF<AppRT, IOF>, int, int, int>(7, f, probe);
var rightId = MonadLaws.RightIdentityHolds<ReaderTF<AppRT, IOF>, int, int>(m0, probe);
var assoc   = MonadLaws.AssociativityHolds<ReaderTF<AppRT, IOF>, int, int, int, int>(m0, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
