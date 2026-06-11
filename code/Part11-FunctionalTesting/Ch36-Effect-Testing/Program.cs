using Ch36.Challenges;
using Ch36.Functions;
using Ch36.Tests;
using Ch36.Types;

Console.WriteLine("================================================");
Console.WriteLine("36장 — 효과 코드의 결정적 테스트 (테스트 런타임 / MemoryConsole 더블)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — 테스트 더블로 효과 실행 ───────────────────────────────
Console.WriteLine("== 예제 1 — MemoryConsole 주입 (실제 콘솔 없이) ==");
var con = new MemoryConsole(["철수"]);
var name = Eff.Run(Greeter.Ask<AppRT>(), new AppRT(con));
Console.WriteLine($"  반환값 = {name}");
Console.WriteLine($"  캡처된 출력 = [{string.Join(" / ", con.Output)}]");
Console.WriteLine("  → 같은 Greeter 코드가 LiveConsole 이면 실제 입출력, MemoryConsole 이면 테스트.");
Console.WriteLine();

// ── 예제 2 — 효과 코드를 단언으로 검증 ──────────────────────────────
Console.WriteLine("== 예제 2 — 효과 테스트 (결정적) ==");
var checks = new (string, bool)[]
{
    ("출력 정확", EffectTests.GreetingOutputsCorrectly()),
    ("입력별 출력 변화", EffectTests.DifferentInputDifferentOutput()),
    ("결정성(같은 입력→같은 출력)", EffectTests.DeterministicHolds()),
};
foreach (var (n, ok) in checks) Console.WriteLine($"  {n} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
