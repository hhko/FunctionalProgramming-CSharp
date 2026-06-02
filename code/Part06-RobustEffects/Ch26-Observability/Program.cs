using Ch26.Tests;
using Ch26.Types;

Console.WriteLine("================================================");
Console.WriteLine("26장 — Observability (Activity / 분산 추적)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — 중첩 span 으로 효과 구간 추적 ──────────────────────────
Console.WriteLine("== 예제 1 — 효과를 span 으로 감싸기 (중첩 트리) ==");

var tracer = new Tracer();

var result = tracer.Activity("handleRequest", () =>
{
    var user = tracer.Activity("loadUser", () =>
        tracer.Activity("queryDb", () => "철수"));
    var total = tracer.Activity("computeTotal", () => 1100);
    return $"{user}: {total}원";
});

Console.WriteLine($"  효과 결과 = {result}   (추적이 결과를 바꾸지 않음)");
Console.WriteLine("  추적 트리:");
foreach (var line in tracer.Render().Split('\n'))
    Console.WriteLine($"    {line}");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 보장 검증 ==");
var checks = new (string, bool)[]
{
    ("결과 불변", TracingLaws.ResultUnchangedHolds()),
    ("중첩 트리", TracingLaws.NestingHolds()),
    ("예외 안전 종료", TracingLaws.ClosesOnExceptionHolds()),
};
foreach (var (name, ok) in checks) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
