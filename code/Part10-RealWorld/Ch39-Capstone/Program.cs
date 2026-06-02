using Ch39.Challenges;
using Ch39.Functions;
using Ch39.Tests;
using Ch39.Types;

Console.WriteLine("================================================");
Console.WriteLine("39장 — 종합 capstone (주문 접수 서비스)");
Console.WriteLine("================================================");
Console.WriteLine();

// 도메인 검증 + 효과 DI + 테스트 더블을 한 흐름에.
(string, decimal)[] requests = [("A", 100m), ("", 50m), ("B", 200m), ("C", -5m)];

var con = new MemoryConsole();
var store = new MemoryStore();
var rt = new AppRT(con, store);

var accepted = Eff.Run(OrderService.ProcessAll<AppRT>(requests), rt);

Console.WriteLine("== 주문 배치 처리 ==");
foreach (var line in con.Output) Console.WriteLine($"    {line}");
Console.WriteLine();
Console.WriteLine($"  승인 = {accepted}건, 저장된 주문 = {store.Count()}건, 매출 합계 = {store.Total()}");
Console.WriteLine("  → 검증(1부) + 효과·DI(5부) + 테스트 더블(9부) 이 한 서비스로 합성.");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("유효만 승인(2)", CapstoneTests.AcceptsValidOnly()),
    ("매출 합계 300", CapstoneTests.TotalsRevenue()),
    ("승인2+거부2 로그", CapstoneTests.LogsAllOutcomes()),
};
foreach (var (n, ok) in checks) Console.WriteLine($"  {n} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
var allGood = checks.All(c => c.Item2);
Console.WriteLine(allGood ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
Console.WriteLine();
if (allGood) Console.WriteLine("★ 책 전체 (Part 1~10) 의 도구가 한 capstone 으로 합성되었습니다.");
