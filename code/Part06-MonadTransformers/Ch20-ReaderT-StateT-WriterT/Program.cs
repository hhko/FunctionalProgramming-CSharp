using Ch20.Challenges;
using Ch20.Functions;
using Ch20.Tests;
using Ch20.Traits;
using Ch20.Types;

Console.WriteLine("================================================");
Console.WriteLine("20장 — ReaderT / StateT / WriterT");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — ReaderT<Env, Option>: 환경 의존 + 실패 (18장 ReaderOption 을 공짜로) ──
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

// ── 예제 3 — WriterT<Log, Option>: 로그 누적 (출력 효과 + 실패) ─────
Console.WriteLine("== 예제 3 — WriterT<Log, OptionF>: 로그를 내부 M 위에 누적 ==");

K<WriterTF<Log, OptionF>, int> logged =
    from _1 in Writable.tell<WriterTF<Log, OptionF>, Log>(Log.One("시작: 21"))
    from x in WriterTF<Log, OptionF>.Pure(21)
    from _2 in Writable.tell<WriterTF<Log, OptionF>, Log>(Log.One($"두 배 → {x * 2}"))
    select x * 2;

Console.WriteLine($"  tell·Pure·tell  → {ShowWriter(logged.As().Run.As())}");
Console.WriteLine("  → 값과 로그가 함께. 로그는 Monoid 의 Combine 으로 누적, 내부 M(Option) 위에 얹힌다.");
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

// ── 법칙 검증 (StateT · WriterT 도 같은 probe 틀로) ─────────────────
Console.WriteLine("== 법칙 검증 (StateT<List<int>, OptionF> · WriterT<Log, OptionF>) ==");

Func<K<StateTF<List<int>, OptionF>, int>, string> sprobe = m =>
    m.As().Run([]).As() is Option<(int V, List<int> S)>.Some s
        ? $"({s.Value.V},[{string.Join(",", s.Value.S)}])"
        : "None";
Func<int, K<StateTF<List<int>, OptionF>, int>> sf = n => StateTF<List<int>, OptionF>.Pure(n + 1);
Func<int, K<StateTF<List<int>, OptionF>, int>> sg = n => StateTF<List<int>, OptionF>.Pure(n * 2);
var sm0 = StateTF<List<int>, OptionF>.Pure(7);
var sLaws =
    MonadLaws.LeftIdentityHolds<StateTF<List<int>, OptionF>, int, int, string>(7, sf, sprobe) &&
    MonadLaws.RightIdentityHolds<StateTF<List<int>, OptionF>, int, string>(sm0, sprobe) &&
    MonadLaws.AssociativityHolds<StateTF<List<int>, OptionF>, int, int, int, string>(sm0, sf, sg, sprobe);

Func<K<WriterTF<Log, OptionF>, int>, string> wprobe = m =>
    m.As().Run.As() is Option<(int V, Log O)>.Some s ? $"({s.Value.V},{s.Value.O})" : "None";
Func<int, K<WriterTF<Log, OptionF>, int>> wf = n => WriterTF<Log, OptionF>.Pure(n + 1);
Func<int, K<WriterTF<Log, OptionF>, int>> wg = n => WriterTF<Log, OptionF>.Pure(n * 2);
var wm0 = WriterTF<Log, OptionF>.Pure(7);
var wLaws =
    MonadLaws.LeftIdentityHolds<WriterTF<Log, OptionF>, int, int, string>(7, wf, wprobe) &&
    MonadLaws.RightIdentityHolds<WriterTF<Log, OptionF>, int, string>(wm0, wprobe) &&
    MonadLaws.AssociativityHolds<WriterTF<Log, OptionF>, int, int, int, string>(wm0, wf, wg, wprobe);

Console.WriteLine($"  StateT  세 법칙 : {Pass(sLaws)}");
Console.WriteLine($"  WriterT 세 법칙 : {Pass(wLaws)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc && sLaws && wLaws ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";

static string ShowWriter(K<OptionF, (int Value, Log Output)> r) =>
    r.As() is Option<(int Value, Log Output)>.Some s
        ? $"Some(값={s.Value.Value}, 로그={s.Value.Output})"
        : "None";

static string ShowStack(K<OptionF, ((int A, int B) V, List<int> S)> r) =>
    r.As() is Option<((int A, int B) V, List<int> S)>.Some s
        ? $"Some(값=({s.Value.V.A}, {s.Value.V.B}), 남은 상태=[{string.Join(",", s.Value.S)}])"
        : "None";

static string ShowState(K<OptionF, (int V, List<int> S)> r) =>
    r.As() is Option<(int V, List<int> S)>.Some ? "Some(...)" : "None";
