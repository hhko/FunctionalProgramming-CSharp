using Ch16.Challenges;
using Ch16.Functions;
using Ch16.Tests;
using Ch16.Traits;
using Ch16.Types;

Console.WriteLine("================================================");
Console.WriteLine("13장 — State (상태 스레딩 효과)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — Get / Put / Modify / Gets ──────────────────────────────
Console.WriteLine("== 예제 1 — 상태를 읽고 쓰는 네 동사 ==");

var program =
    from x  in Stateful.get<StateF<int>, int>()       // 현재 상태 읽기
    from _1 in Stateful.put<StateF<int>, int>(x + 10) // 통째로 교체
    from _2 in Stateful.modify<StateF<int>, int>(s => s * 2) // 함수로 변형
    from y  in Stateful.gets<StateF<int>, int, string>(s => $"상태={s}") // 일부 추출
    select y;

var (value, finalState) = program.As().Run(5);
Console.WriteLine($"  초기 상태 5 → Run 결과 값 = \"{value}\", 최종 상태 = {finalState}");
Console.WriteLine($"  같은 계산, 초기 상태 100 → {program.As().Run(100)}");
Console.WriteLine();

// ── 예제 2 — Fresh ID 생성기 (상태 자동 스레딩) ─────────────────────
Console.WriteLine("== 예제 2 — 고유 ID 생성기 ==");

var labeled = FreshId.Label(["alice", "bob", "carol"]);
var (rows, next) = labeled.As().Run(0);
Console.WriteLine($"  Label([alice, bob, carol]).Run(0):");
foreach (var (id, name) in rows)
    Console.WriteLine($"    {id} → {name}");
Console.WriteLine($"  다음 사용 가능 ID = {next}");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (Monad) ==");

Func<K<StateF<int>, int>, (int, int)> probe = m => m.As().Run(7);
Func<int, K<StateF<int>, int>> f = n => new State<int, int>(s => (n + s, s + 1));
Func<int, K<StateF<int>, int>> g = n => new State<int, int>(s => (n * 2, s + 10));
K<StateF<int>, int> m = Stateful.get<StateF<int>, int>();

var leftId  = MonadLaws.LeftIdentityHolds<StateF<int>, int, int, (int, int)>(3, f, probe);
var rightId = MonadLaws.RightIdentityHolds<StateF<int>, int, (int, int)>(m, probe);
var assoc   = MonadLaws.AssociativityHolds<StateF<int>, int, int, int, (int, int)>(m, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
