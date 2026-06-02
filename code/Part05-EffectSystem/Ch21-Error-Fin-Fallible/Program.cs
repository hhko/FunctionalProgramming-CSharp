using Ch21.Functions;
using Ch21.Tests;
using Ch21.Traits;
using Ch21.Types;

Console.WriteLine("================================================");
Console.WriteLine("21장 — Error / Fin / Fallible (함수형 오류 모델)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — Fin 파이프라인: 첫 실패에서 단락 ──────────────────────
Console.WriteLine("== 예제 1 — Fin Bind 는 첫 실패에서 단락 ==");

static K<FinF, int> Positive(int x) =>
    x > 0 ? new Fin<int>.Succ(x) : new Fin<int>.Fail(Error.New($"{x} 은(는) 양수가 아님"));

var ok =
    from a in Positive(10)
    from b in Positive(20)
    select a + b;

var bad =
    from a in Positive(10)
    from b in Positive(-5)   // 여기서 단락
    from c in Positive(99)   // 실행 안 됨
    select a + b + c;

Console.WriteLine($"  10·20 모두 양수  → {ok.As()}");
Console.WriteLine($"  10·(-5)·99       → {bad.As()}   (-5 에서 단락, 99 는 미실행)");
Console.WriteLine();

// ── 예제 2 — Try: 예외를 Fin 으로 포획 ──────────────────────────────
Console.WriteLine("== 예제 2 — Try (예외 → Error 값) ==");

var parsed = Try.Run(() => int.Parse("42"));
var failed = Try.Run(() => int.Parse("not-a-number"));
Console.WriteLine($"  Try(int.Parse \"42\")           → {parsed.As()}");
Console.WriteLine($"  Try(int.Parse \"not-a-number\") → {failed.As()}");
Console.WriteLine();

// ── 예제 3 — Catch 복구 ─────────────────────────────────────────────
Console.WriteLine("== 예제 3 — Catch 로 실패 복구 ==");

var recovered = Fallibles.@catch<FinF, int>(failed, _ => new Fin<int>.Succ(-1));
Console.WriteLine($"  실패를 Catch → 기본값 -1 = {recovered.As()}");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (Monad) ==");

Func<K<FinF, int>, string> probe = m => m.As().ToString();
Func<int, K<FinF, int>> f = n => new Fin<int>.Succ(n + 1);
Func<int, K<FinF, int>> g = Positive;
K<FinF, int> m0 = new Fin<int>.Succ(5);

var leftId  = MonadLaws.LeftIdentityHolds<FinF, int, int, string>(7, f, probe);
var rightId = MonadLaws.RightIdentityHolds<FinF, int, string>(m0, probe);
var assoc   = MonadLaws.AssociativityHolds<FinF, int, int, int, string>(m0, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
