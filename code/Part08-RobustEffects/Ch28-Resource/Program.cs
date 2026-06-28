using Ch28.Tests;
using Ch28.Types;

Console.WriteLine("================================================");
Console.WriteLine("28장 — Resource & bracket (자원 수명)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — bracket 정상 경로 ──────────────────────────────────────
Console.WriteLine("== 예제 1 — bracket: 획득 → 사용 → 해제 ==");
var log1 = new List<string>();
var len = Resource.Bracket(
    () => { log1.Add("파일 열기"); return "data.txt"; },
    file => { log1.Add($"읽기({file})"); return file.Length; },
    file => log1.Add($"닫기({file})"));
Console.WriteLine($"  결과(파일명 길이) = {len}");
Console.WriteLine($"  이벤트 = [{string.Join(" → ", log1)}]");
Console.WriteLine();

// ── 예제 2 — 예외에도 해제 보장 ─────────────────────────────────────
Console.WriteLine("== 예제 2 — 사용 중 예외가 나도 해제는 실행 ==");
var log2 = new List<string>();
try
{
    Resource.Bracket<int, int>(
        () => { log2.Add("열기"); return 1; },
        _ => { log2.Add("사용 중 예외!"); throw new InvalidOperationException("boom"); },
        _ => log2.Add("닫기 (finally)"));
}
catch (InvalidOperationException e)
{
    log2.Add($"예외 전파: {e.Message}");
}
Console.WriteLine($"  이벤트 = [{string.Join(" → ", log2)}]");
Console.WriteLine();

// ── 예제 3 — 다중 자원 LIFO 해제 ────────────────────────────────────
Console.WriteLine("== 예제 3 — 중첩 자원의 LIFO 해제 ==");
var res = new Resources();
res.Acquire("DB연결", () => "db", _ => { });
res.Acquire("트랜잭션", () => "tx", _ => { });
res.Acquire("커서", () => "cur", _ => { });
res.ReleaseAll();
Console.WriteLine($"  {string.Join(" → ", res.Log)}");
Console.WriteLine("  (연 순서 DB→Tx→커서, 닫힌 순서 커서→Tx→DB)");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 보장 검증 ==");
var checks = new (string, bool)[]
{
    ("정상 순서", ResourceLaws.NormalOrderHolds()),
    ("예외에도 해제", ResourceLaws.ReleaseOnExceptionHolds()),
    ("LIFO 해제", ResourceLaws.LifoReleaseHolds()),
};
foreach (var (name, ok) in checks) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
