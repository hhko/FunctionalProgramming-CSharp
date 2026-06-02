using Ch12.Challenges;
using Ch12.Functions;
using Ch12.Tests;
using Ch12.Traits;
using Ch12.Types;

Console.WriteLine("================================================");
Console.WriteLine("12장 — Reader (환경 의존 효과)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — Asks / Ask: 환경에서 값을 읽는다 ───────────────────────
Console.WriteLine("== 예제 1 — Asks / Ask ==");

var cfg = new AppConfig(TaxRate: 0.1m, Currency: "원");

K<ReaderF<AppConfig>, decimal> rate = Readable.asks<ReaderF<AppConfig>, AppConfig, decimal>(c => c.TaxRate);
K<ReaderF<AppConfig>, AppConfig> whole = Readable.ask<ReaderF<AppConfig>, AppConfig>();

Console.WriteLine($"  Asks(c => c.TaxRate).Run(cfg) = {rate.As().Run(cfg)}");
Console.WriteLine($"  Ask.Run(cfg)                  = {whole.As().Run(cfg)}");
Console.WriteLine();

// ── 예제 2 — Bind/LINQ: 전역 없는 DI ────────────────────────────────
Console.WriteLine("== 예제 2 — 여러 Reader 합성 (config 를 안 넘긴다) ==");

var total = BudgetReader.TotalWithTax(1000m);
var altCfg = new AppConfig(0.2m, "$");
Console.WriteLine($"  TotalWithTax(1000).Run(cfg) = {total.As().Run(cfg)}");
Console.WriteLine($"  같은 계산, 다른 환경 Run     = {total.As().Run(altCfg)}");
Console.WriteLine();

// ── 예제 3 — Local: 환경 국소 변경 ──────────────────────────────────
Console.WriteLine("== 예제 3 — Local: 세율을 일시적으로 0 으로 ==");

var taxFree = Readable.local<ReaderF<AppConfig>, AppConfig, string>(
    c => c with { TaxRate = 0m },
    BudgetReader.TotalWithTax(1000m));

Console.WriteLine($"  일반   = {BudgetReader.TotalWithTax(1000m).As().Run(cfg)}");
Console.WriteLine($"  Local  = {taxFree.As().Run(cfg)}   (바깥 cfg 의 세율은 그대로)");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (Monad) ==");

var sample = new AppConfig(0.1m, "원");
Func<K<ReaderF<AppConfig>, int>, int> probe = r => r.As().Run(sample);
Func<int, K<ReaderF<AppConfig>, int>> f = n => new Reader<AppConfig, int>(c => n + (int)(c.TaxRate * 100));
Func<int, K<ReaderF<AppConfig>, int>> g = n => new Reader<AppConfig, int>(_ => n * 2);
K<ReaderF<AppConfig>, int> m = Readable.asks<ReaderF<AppConfig>, AppConfig, int>(c => c.Currency.Length);

var leftId  = MonadLaws.LeftIdentityHolds<ReaderF<AppConfig>, int, int>(5, f, probe);
var rightId = MonadLaws.RightIdentityHolds<ReaderF<AppConfig>, int>(m, probe);
var assoc   = MonadLaws.AssociativityHolds<ReaderF<AppConfig>, int, int, int>(m, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
