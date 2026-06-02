using Ch15.Functions;
using Ch15.Tests;
using Ch15.Traits;
using Ch15.Types;

Console.WriteLine("================================================");
Console.WriteLine("15장 — 왜 변환기가 필요한가 (합성의 벽)");
Console.WriteLine("================================================");
Console.WriteLine();

// 환경 = 설정 사전. 조회는 *환경 의존* 이면서 *실패 가능* — 두 효과가 동시에 필요하다.
var env = new Dictionary<string, int> { ["a"] = 10, ["b"] = 32 };

K<ROF<Dictionary<string, int>>, int> Lookup(string key) =>
    new ReaderOption<Dictionary<string, int>, int>(
        e => e.TryGetValue(key, out var v) ? new Option<int>.Some(v) : Option<int>.None.Instance);

// ── 예제 1 — 각각은 잘 합성된다 (복습) ──────────────────────────────
Console.WriteLine("== 예제 1 — Reader 와 Option 각각은 모나드 ==");
var r = ReaderF<int>.Bind(ReaderF<int>.Pure(3), x => ReaderF<int>.Pure(x + 1));
var o = OptionF.Bind(OptionF.Pure(3), x => OptionF.Pure(x + 1));
Console.WriteLine($"  Reader 합성 = {r.As().Run(0)},  Option 합성 = {o.As()}");
Console.WriteLine();

// ── 예제 2 — 벽: 두 효과를 동시에 ──────────────────────────────────
Console.WriteLine("== 예제 2 — env 의존 + 실패 가능 = Reader<Env, Option<A>> ==");
Console.WriteLine("  단순 Reader.Bind 로는 안쪽 Option 의 Some/None 을 풀 수 없어");
Console.WriteLine("  Reader<Env, Option<Option<…>>> 같은 중첩이 쌓인다 (수동 언랩 필요).");
Console.WriteLine();

// ── 예제 3 — 손으로 짠 ReaderOption 은 깔끔히 합성된다 ──────────────
Console.WriteLine("== 예제 3 — ReaderOption (배관을 손으로 짠 뒤) LINQ 합성 ==");

K<ROF<Dictionary<string, int>>, int> sum =
    from a in Lookup("a")
    from b in Lookup("b")
    select a + b;

var partialEnv = new Dictionary<string, int> { ["a"] = 10 };
Console.WriteLine($"  완전한 env 로 Run     = {sum.As().Run(env)}");          // Some(42)
Console.WriteLine($"  b 가 없는 env 로 Run  = {sum.As().Run(partialEnv)}");    // None
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (손으로 짠 ReaderOption 도 진짜 모나드) ==");

Func<K<ROF<Dictionary<string, int>>, int>, Option<int>> probe = m => m.As().Run(env);
Func<int, K<ROF<Dictionary<string, int>>, int>> f = n => ROF<Dictionary<string, int>>.Pure(n + 1);
Func<int, K<ROF<Dictionary<string, int>>, int>> g = n => ROF<Dictionary<string, int>>.Pure(n * 2);
var m0 = Lookup("a");

var leftId  = MonadLaws.LeftIdentityHolds<ROF<Dictionary<string, int>>, int, int, Option<int>>(7, f, probe);
var rightId = MonadLaws.RightIdentityHolds<ROF<Dictionary<string, int>>, int, Option<int>>(m0, probe);
var assoc   = MonadLaws.AssociativityHolds<ROF<Dictionary<string, int>>, int, int, int, Option<int>>(m0, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine("→ 결론: 이 Bind 배관을 효과 쌍마다 손으로 짜야 한다.");
Console.WriteLine("  4부의 변환기 ReaderT<Env, M, A> 는 *임의의 내부 모나드 M* 에 대해 이 배관을 자동 생성한다.");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
