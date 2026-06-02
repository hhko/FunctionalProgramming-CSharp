using Ch17.Challenges;
using Ch17.Functions;
using Ch17.Tests;
using Ch17.Traits;
using Ch17.Types;

Console.WriteLine("================================================");
Console.WriteLine("17장 — ReaderT / StateT / WriterT");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — ReaderT<Env, Option>: 환경 의존 + 실패 (15장 ReaderOption 을 공짜로) ──
Console.WriteLine("== 예제 1 — ReaderT<int, OptionF>: env 의존 + 실패 ==");

K<ReaderTF<int, OptionF>, int> Divide() =>
    from d in Readable.asks<ReaderTF<int, OptionF>, int, int>(env => env)
    from r in d == 0
        ? Trans.lift<ReaderTF<int, OptionF>, OptionF, int>(Option<int>.None.Instance)
        : ReaderTF<int, OptionF>.Pure(100 / d)
    select r;

Console.WriteLine($"  100/5 (env=5) = {Divide().As().Run(5).As()}");
Console.WriteLine($"  100/0 (env=0) = {Divide().As().Run(0).As()}   (내부 Option 이 None)");
Console.WriteLine();

// ── 예제 2 — StateT<Stack, Option>: 상태 + 실패 ────────────────────
Console.WriteLine("== 예제 2 — StateT<List<int>, OptionF>: 스택 (상태 + 실패) ==");

K<StateTF<List<int>, OptionF>, (int A, int B)> twoPops =
    from _1 in Stack.Push(10)
    from _2 in Stack.Push(20)
    from a in Stack.Pop
    from b in Stack.Pop
    select (a, b);

Console.WriteLine($"  push10·push20·pop·pop  → {ShowStack(twoPops.As().Run([]))}");

K<StateTF<List<int>, OptionF>, int> tooManyPops =
    from a in Stack.Pop      // 빈 스택에서 pop → None
    select a;

Console.WriteLine($"  빈 스택에서 pop        → {ShowState(tooManyPops.As().Run([]))}");
Console.WriteLine();

// ── 법칙 검증 (ReaderT) ─────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (ReaderT<int, OptionF>) ==");

Func<K<ReaderTF<int, OptionF>, int>, Option<int>> probe = m => m.As().Run(3).As();
Func<int, K<ReaderTF<int, OptionF>, int>> f = n => ReaderTF<int, OptionF>.Pure(n + 1);
Func<int, K<ReaderTF<int, OptionF>, int>> g = n => ReaderTF<int, OptionF>.Pure(n * 2);
var m0 = Readable.asks<ReaderTF<int, OptionF>, int, int>(e => e);

var leftId  = MonadLaws.LeftIdentityHolds<ReaderTF<int, OptionF>, int, int, Option<int>>(7, f, probe);
var rightId = MonadLaws.RightIdentityHolds<ReaderTF<int, OptionF>, int, Option<int>>(m0, probe);
var assoc   = MonadLaws.AssociativityHolds<ReaderTF<int, OptionF>, int, int, int, Option<int>>(m0, f, g, probe);

Console.WriteLine($"  좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  우 항등 : {Pass(rightId)}");
Console.WriteLine($"  결합    : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";

static string ShowStack(K<OptionF, ((int A, int B) V, List<int> S)> r) =>
    r.As() is Option<((int A, int B) V, List<int> S)>.Some s
        ? $"Some(값=({s.Value.V.A}, {s.Value.V.B}), 남은 상태=[{string.Join(",", s.Value.S)}])"
        : "None";

static string ShowState(K<OptionF, (int V, List<int> S)> r) =>
    r.As() is Option<(int V, List<int> S)>.Some ? "Some(...)" : "None";
