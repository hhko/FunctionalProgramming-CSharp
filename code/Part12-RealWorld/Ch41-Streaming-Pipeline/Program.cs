using Ch41.Challenges;
using Ch41.Tests;
using Ch41.Types;

Console.WriteLine("================================================");
Console.WriteLine("38장 — 동시성·스트리밍 실전 파이프라인");
Console.WriteLine("================================================");
Console.WriteLine();

string[] records = ["10", "x", "20", "", "30", "bad", "40"];

// ── 예제 1 — 결합된 파이프라인 실행 ─────────────────────────────────
Console.WriteLine("== 예제 1 — retry + bracket + 스트리밍 + 동시 집계 ==");
var r = Pipeline.Run(records, maxRetries: 5, flakyUntilAttempt: 3);
Console.WriteLine($"  연결 시도 횟수 = {r.ConnectAttempts} (3번째 성공 — 재시도 복구)");
Console.WriteLine($"  유효 레코드 = {r.Valid}개, 합 = {r.Sum} (동시 집계)");
Console.WriteLine($"  자원 이벤트 = [{string.Join(" → ", r.Events)}]");
Console.WriteLine();

// ── 예제 2 — 평균 (집계 후처리) ─────────────────────────────────────
Console.WriteLine("== 예제 2 — 평균 ==");
Console.WriteLine($"  평균 = {Average.Of(records)}");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("유효만 합산", PipelineTests.SumsValidOnly()),
    ("연결 재시도 복구", PipelineTests.RetriesConnection()),
    ("자원 개폐 보장", PipelineTests.ResourceLifecycle()),
    ("복구 불가 시 실패", PipelineTests.FailsWhenUnrecoverable()),
};
foreach (var (n, ok) in checks) Console.WriteLine($"  {n} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
