using Ch40.Challenges;
using Ch40.Functions;
using Ch40.Tests;
using Ch40.Types;

Console.WriteLine("================================================");
Console.WriteLine("40장 — 효과 기반 애플리케이션 (Eff<RT> + Has DI)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — 테스트 런타임으로 앱 워크플로 실행 ────────────────────
Console.WriteLine("== 예제 1 — 노트 앱 (Console+Clock+Store, 테스트 런타임) ==");
var con = new MemoryConsole();
var store = new MemoryStore();
var rt = new AppRT(con, new FixedClock(1000), store);

var count = Eff.Run(NoteApp.Run<AppRT>(), rt);

Console.WriteLine($"  반환 노트 수 = {count}");
Console.WriteLine($"  콘솔 출력:");
foreach (var line in con.Output) Console.WriteLine($"    {line}");
Console.WriteLine($"  저장소 내용:");
foreach (var k in store.Keys()) Console.WriteLine($"    {k} = {store.Get(k)}");
Console.WriteLine("  → 같은 앱 코드가 LiveConsole/SystemClock/DB 런타임이면 실제 동작.");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("노트 2개 저장", AppTests.SavesTwoNotes()),
    ("시계 능력 반영", AppTests.UsesClock()),
    ("콘솔 출력 정확", AppTests.ConsoleOutput()),
};
foreach (var (n, ok) in checks) Console.WriteLine($"  {n} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
