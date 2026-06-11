using Ch38.Challenges;
using Ch38.Tests;
using Ch38.Traits;
using Ch38.Types;

Console.WriteLine("====================================================");
Console.WriteLine("38장 — property-based 심화 + 함수형 테스트 아키텍처");
Console.WriteLine("====================================================");
Console.WriteLine();

// ════════════════════════════════════════════════════════════════════
// Part A — property 엔진 심화 (생성기 + 축소)
//   무작위 입력으로 성질을 검사하고, 실패하면 최소 반례로 축소한다.
// ════════════════════════════════════════════════════════════════════
Console.WriteLine("── Part A — property 엔진 (생성기 + 축소) ──");
Console.WriteLine();

// 예제 1 — 참인 성질 (200 케이스 통과).
Console.WriteLine("== 예제 1 — reverse∘reverse = id (모든 리스트) ==");
var rev = Prop.ForAll(
    Gen.ListOf(Gen.IntRange(0, 9), 12),
    xs => { var t = xs.ToList(); t.Reverse(); t.Reverse(); return t.SequenceEqual(xs); },
    _ => []);
Console.WriteLine($"  {rev.Cases} 케이스 검사 → {(rev.Ok ? "통과" : "반례 발견")}");
Console.WriteLine();

// 예제 2 — 거짓 성질 + 최소 반례 축소.
Console.WriteLine("== 예제 2 — '모든 n < 100' (거짓) → 최소 반례 축소 ==");
var bad = Prop.ForAll(Gen.IntRange(0, 1000), n => n < 100, n => n > 0 ? [n - 1] : []);
Console.WriteLine($"  성질 성립? {bad.Ok}");
Console.WriteLine($"  최초 반례 발견까지 {bad.Cases} 케이스, 축소 {bad.Shrinks}회 → 최소 반례 = {bad.Counterexample}");
Console.WriteLine();

// 예제 3 — 정렬의 성질.
Console.WriteLine("== 예제 3 — 정렬 성질 (길이 보존 + 오름차순) ==");
Console.WriteLine($"  올바른 정렬 성질 통과? {SortProp.CorrectSortHolds()}");
Console.WriteLine($"  틀린 '정렬'(중복 제거) 반례 잡힘? {SortProp.BuggySortFails()}");
Console.WriteLine();

// ════════════════════════════════════════════════════════════════════
// Part B — 어떤 F 든 받는 법칙 모듈
//   Laws.* 한 모듈이 임의의 Functor<F>/Monad<M> 인스턴스를 받아 법칙을 검사한다.
// ════════════════════════════════════════════════════════════════════
Console.WriteLine("── Part B — 어떤 F 든 받는 법칙 모듈 ──");
Console.WriteLine();

K<MyListF, int> nums = new MyList<int>([1, 2, 3, 4]);
Func<K<MyListF, int>, IEnumerable<int>> probeI = x => x.As().Items;
Func<K<MyListF, string>, IEnumerable<string>> probeS = x => x.As().Items;
Func<int, K<MyListF, int>> f = n => new MyList<int>([n, n + 1]);
Func<int, K<MyListF, int>> g = n => new MyList<int>([n * 10]);

Console.WriteLine("== MyListF 에 대해 다섯 법칙 검사 (한 모듈로) ==");
var laws = new (string, bool)[]
{
    ("Functor 항등",   Laws.FunctorIdentity<MyListF, int>(nums, probeI)),
    ("Functor 합성",   Laws.FunctorComposition<MyListF, int, int, string>(nums, x => x + 1, x => $"#{x}", probeS)),
    ("Monad 좌 항등",  Laws.MonadLeftIdentity<MyListF, int, int>(5, f, probeI)),
    ("Monad 우 항등",  Laws.MonadRightIdentity<MyListF, int>(nums, probeI)),
    ("Monad 결합",     Laws.MonadAssociativity<MyListF, int, int, int>(nums, f, g, probeI)),
};
foreach (var (name, ok) in laws) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();

Console.WriteLine("== 위반 인스턴스(BogusListF)도 잡아내는가 ==");
var caught = BogusCheck.CatchesViolation();
Console.WriteLine($"  BogusListF 항등 법칙 위반을 잡음? {(caught ? "예 (검증이 틀린 구현을 거름)" : "아니오")}");
Console.WriteLine();

// ════════════════════════════════════════════════════════════════════
// Part C — 아키텍처 한 줄: 두 축의 결합
//   Part A 의 property 엔진(Gen + ForAll)과 Part B 의 법칙 모듈(Laws.*)이
//   맞물리면, "어떤 인스턴스든 *임의 입력* 으로 법칙을 자동 검증" 이 된다.
//   고정된 nums 하나가 아니라, Gen 으로 만든 무작위 MyList<int> 마다
//   Laws.FunctorIdentity 를 ForAll 로 때려 본다.
// ════════════════════════════════════════════════════════════════════
Console.WriteLine("── Part C — 아키텍처: property 엔진 × 법칙 모듈 ──");
Console.WriteLine();

// Gen<List<int>> 를 Gen<MyList<int>> 로 끌어올려 검증 대상 인스턴스를 무작위 생성.
Gen<MyList<int>> genMyList = Gen.ListOf(Gen.IntRange(0, 50), 15).Select(xs => new MyList<int>(xs));

// 무작위 MyList<int> 마다 Functor 항등 법칙을 검사 — Part A 엔진으로 Part B 법칙을 구동.
var autoFunctorId = Prop.ForAll(
    genMyList,
    xs => Laws.FunctorIdentity<MyListF, int>(xs, x => x.As().Items),
    _ => []);

// 무작위 MyList<int> 마다 Monad 우 항등 법칙도 같은 방식으로 검사.
var autoMonadRightId = Prop.ForAll(
    genMyList,
    xs => Laws.MonadRightIdentity<MyListF, int>(xs, x => x.As().Items),
    _ => []);

Console.WriteLine("== Gen 으로 만든 임의 MyList<int> 에 대해 법칙을 자동 검증 ==");
Console.WriteLine($"  Functor 항등 ({autoFunctorId.Cases} 케이스) → {(autoFunctorId.Ok ? "통과" : "반례 발견")}");
Console.WriteLine($"  Monad 우 항등 ({autoMonadRightId.Cases} 케이스) → {(autoMonadRightId.Ok ? "통과" : "반례 발견")}");
Console.WriteLine();

// ════════════════════════════════════════════════════════════════════
// 전체 검증
// ════════════════════════════════════════════════════════════════════
Console.WriteLine("── 전체 검증 ──");
var (failed, counter) = PropTests.FindsMinimalCounterexample();
var checks = new (string, bool)[]
{
    // Part A — property 엔진.
    ("A. reverse involution",      PropTests.ReverseInvolution()),
    ("A. 덧셈 교환",                PropTests.AdditionCommutes()),
    ("A. 최소 반례 = 100",          failed && counter == 100),
    ("A. 올바른 정렬 성질",          SortProp.CorrectSortHolds()),
    ("A. 틀린 정렬 반례 검출",        SortProp.BuggySortFails()),
    // Part B — 법칙 모듈.
    ("B. Functor 항등",             Laws.FunctorIdentity<MyListF, int>(nums, probeI)),
    ("B. Functor 합성",             Laws.FunctorComposition<MyListF, int, int, string>(nums, x => x + 1, x => $"#{x}", probeS)),
    ("B. Monad 좌 항등",            Laws.MonadLeftIdentity<MyListF, int, int>(5, f, probeI)),
    ("B. Monad 우 항등",            Laws.MonadRightIdentity<MyListF, int>(nums, probeI)),
    ("B. Monad 결합",               Laws.MonadAssociativity<MyListF, int, int, int>(nums, f, g, probeI)),
    ("B. 위반 인스턴스 검출",         caught),
    // Part C — 두 축의 결합.
    ("C. 임의 입력 Functor 항등 자동 검증",  autoFunctorId.Ok),
    ("C. 임의 입력 Monad 우 항등 자동 검증", autoMonadRightId.Ok),
};
foreach (var (name, ok) in checks) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
