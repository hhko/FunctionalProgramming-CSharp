using Ch37.Challenges;
using Ch37.Functions;
using Ch37.Tests;
using Ch37.Types;

Console.WriteLine("================================================");
Console.WriteLine("37장 — 동시·스트리밍·자원 효과 테스트");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — Atom: 1000 개 동시 증가 → 최종값은 항상 1000 ─────────────
Console.WriteLine("== 예제 1 — Atom (1000 개 동시 증가의 결정적 최종값) ==");
var atom = new Atom<Counter>(new Counter(0));
Parallel.For(0, 1000, _ => atom.Swap(c => c with { N = c.N + 1 }));
Console.WriteLine($"  동시 증가 1000 회 → 최종값 {atom.Value.N}   {(atom.Value.N == 1000 ? "✓ 원자성 성립" : "✗")}");
Console.WriteLine();

// ── 예제 2 — Schedule: 3 번째에 성공하는 flaky → 시도 3 회 ────────────
Console.WriteLine("== 예제 2 — Schedule (재시도 횟수) ==");
var attemptsFlaky = Schedule.Retry(Schedule.Flaky(succeedOn: 3), maxAttempts: 5);
var attemptsFail = Schedule.Retry(() => false, maxAttempts: 4);
Console.WriteLine($"  3 번째에 성공하는 flaky → 시도 {attemptsFlaky} 회");
Console.WriteLine($"  끝까지 실패 → maxAttempts(4)에서 포기 → 시도 {attemptsFail} 회");
Console.WriteLine();

// ── 예제 3 — Resource: 예외에도 해제, 중첩은 LIFO ────────────────────
Console.WriteLine("== 예제 3 — Resource (bracket / LIFO 해제) ==");
var released = false;
try
{
    Resource.Use<int, int>(() => 1, _ => throw new InvalidOperationException(), _ => released = true);
}
catch (InvalidOperationException) { }
var scope = new Scope();
scope.Acquire("A", () => 1, _ => { });
scope.Acquire("B", () => 2, _ => { });
scope.ReleaseAll();
Console.WriteLine($"  사용 중 예외 발생 → release 호출됨? {released}");
Console.WriteLine($"  중첩 종료 순서 = [{string.Join(", ", scope.Log)}]");
Console.WriteLine();

// ── 예제 4 — Source: effectful 스트림 골든 ──────────────────────────
Console.WriteLine("== 예제 4 — Source (effectful 스트림 골든) ==");
var trace = new List<int>();
var streamed = Source.Map(Source.Filter(Source.From(5, trace), n => n % 2 == 0), n => n * 10).ToList();
Console.WriteLine($"  From(5) → 짝수 filter → ×10 = [{string.Join(", ", streamed)}]");
Console.WriteLine();

// ── 검증 ────────────────────────────────────────────────────────────
Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("Atom 순차 Swap == 100",          AtomTests.SequentialSwapHolds()),
    ("Atom 동시(Parallel) == 1000",    AtomTests.ConcurrentParallelForHolds()),
    ("Atom 동시(Task) == 1000",        AtomTests.ConcurrentTasksHolds()),
    ("Schedule 3 번째 성공 → 3 회",     ScheduleTests.SucceedsOnThirdAttempt()),
    ("Schedule 즉시 성공 → 1 회",       ScheduleTests.SucceedsImmediately()),
    ("Schedule 실패 → maxAttempts 포기", ScheduleTests.GivesUpAtMaxAttempts()),
    ("Schedule 마지막 시도 성공",        ScheduleTests.SucceedsOnLastAttempt()),
    ("Resource 정상 순서",              ResourceTests.NormalOrderHolds()),
    ("Resource 예외에도 해제",          ResourceTests.ReleaseOnExceptionHolds()),
    ("Resource 중첩 LIFO 해제",         ResourceTests.LifoReleaseOrderHolds()),
    ("Stream 생성 시퀀스 골든",         StreamTests.ProducesExpectedSequence()),
    ("Stream map∘filter 골든",          StreamTests.MapFilterMatchesGolden()),
    ("Stream lazy effect 조기 정지",     StreamTests.LazyEffectStopsEarly()),
};
foreach (var (name, ok) in checks)
    Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");

// ── 챌린지 정답 (§37.8) — 본문 챌린지 세 개의 실행 가능한 정답 ──────────
Console.WriteLine();
Console.WriteLine("== 챌린지 정답 (§37.8) ==");
var challengeChecks = new (string, bool)[]
{
    ("① Atom 으로 바꾸면 항상 1000",     LostUpdate.AtomAlwaysReachesThousand()),
    ("② Retry 로그가 [1, 2, 3]",        RetryWithLog.ThirdAttemptLogsOneTwoThree()),
    ("② 즉시 성공 로그가 [1]",          RetryWithLog.ImmediateSuccessLogsOne()),
    ("③ 자원 셋 종료가 C, B, A (LIFO)",  NestedScope.ThreeResourcesReleaseLifo()),
    ("③ 예외에도 셋 다 해제",            NestedScope.ReleasesAllOnException()),
};
foreach (var (name, ok) in challengeChecks)
    Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
