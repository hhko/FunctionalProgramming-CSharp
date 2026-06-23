using Ch04.Challenges;
using Ch04.Functions;
using Ch04.Tests;
using Ch04.Traits;
using Ch04.Types;

Console.WriteLine("================================================");
Console.WriteLine("4장 — Functor");
Console.WriteLine("================================================");
Console.WriteLine();

Console.WriteLine("== 예제 1 — MyList Map (세 가지 호출 어법) ==");

K<MyListF, int> nums = new MyList<int>([1, 2, 3, 4, 5]);

// ① trait 정적 호출 — Functor<MyListF>.Map 을 직접
K<MyListF, int> r1 = MyListF.Map<int, int>(n => n * 2, nums);

// ② 모듈 어법 — Functor.map<F, A, B> 의 generic 디스패치 (§4.2.6)
K<MyListF, int> r2 = Functor.map<MyListF, int, int>(n => n * 2, nums);

// ③ 확장 어법 — K<F, A> 위 점 호출 (§4.2.6, 가장 자연스러운 호출)
K<MyListF, int> r3 = nums.Map(n => n * 2);

K<MyListF, int> doubled = r3;  // 이후 예제에서 재사용
Console.WriteLine($"  ① trait 정적   MyListF.Map<>(f, nums) = {r1.As()}");
Console.WriteLine($"  ② 모듈 어법    Functor.map<>(f, nums) = {r2.As()}");
Console.WriteLine($"  ③ 확장 어법    nums.Map(f)            = {r3.As()}");
Console.WriteLine($"  세 결과 동일?  {r1.As().Items.SequenceEqual(r2.As().Items) && r2.As().Items.SequenceEqual(r3.As().Items)}");

Console.WriteLine();
Console.WriteLine("== 예제 2 — MyMaybe Map (세 가지 호출 어법) ==");

K<MyMaybeF, int> just = new MyMaybe<int>.Just(42);

// ① trait 정적 호출
K<MyMaybeF, string> m1 = MyMaybeF.Map<int, string>(n => $"#{n}", just);

// ② 모듈 어법
K<MyMaybeF, string> m2 = Functor.map<MyMaybeF, int, string>(n => $"#{n}", just);

// ③ 확장 어법
K<MyMaybeF, string> m3 = just.Map(n => $"#{n}");

K<MyMaybeF, string> mapped = m3;  // 이후 예제에서 재사용
Console.WriteLine($"  ① trait 정적   MyMaybeF.Map<>(f, just) = {m1.As()}");
Console.WriteLine($"  ② 모듈 어법    Functor.map<>(f, just)  = {m2.As()}");
Console.WriteLine($"  ③ 확장 어법    just.Map(f)             = {m3.As()}");

// Nothing 도 같은 세 어법으로 동작 — ③ 확장 어법만 시연.
K<MyMaybeF, int> nothing = MyMaybe<int>.Nothing.Instance;
K<MyMaybeF, string> mappedN = nothing.Map(n => $"#{n}");
Console.WriteLine($"  Nothing.Map(n => #n) = {mappedN.As()}    // 확장 어법");

Console.WriteLine();
Console.WriteLine("== 예제 3 — 같은 일반 함수가 두 다른 Functor 처리 ==");

var listResult = MapAnyFunctor.Run<MyListF, int, int>(nums, n => n + 100);
Console.WriteLine($"  MyList: {listResult.As()}");

var maybeResult = MapAnyFunctor.Run<MyMaybeF, int, int>(just, n => n + 100);
Console.WriteLine($"  MyMaybe: {maybeResult.As()}");

Console.WriteLine();
Console.WriteLine("== 예제 4 — 합성 법칙 (Map(g) ∘ Map(f) ≡ Map(g ∘ f)) ==");

Func<int, int> f = n => n + 1;
Func<int, int> g = n => n * 10;

var lhs = MyListF.Map<int, int>(g, MyListF.Map<int, int>(f, nums));
var rhs = MyListF.Map<int, int>(Compose.Of(g, f), nums);

Console.WriteLine($"  Map(g).Map(f) = {lhs.As()}");
Console.WriteLine($"  Map(g ∘ f)    = {rhs.As()}");
Console.WriteLine($"  같은가? {lhs.As().Items.SequenceEqual(rhs.As().Items)}");

Console.WriteLine();
Console.WriteLine("== 예제 5 — 항등 법칙 자동 검증 (Map(id, fa) ≡ fa) ==");

bool identityOnList = FunctorLaws.IdentityHolds<MyListF, int>(
    nums,
    fa => fa.As().Items);
Console.WriteLine($"  MyList 항등 법칙: {(identityOnList ? "통과" : "위반")}");

bool identityOnMaybe = FunctorLaws.IdentityHolds<MyMaybeF, int>(
    just,
    fa => fa.As() is MyMaybe<int>.Just j ? [j.Value] : Array.Empty<int>());
Console.WriteLine($"  MyMaybe 항등 법칙: {(identityOnMaybe ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 6 — 합성 법칙 자동 검증 ==");

bool compositionOnList = FunctorLaws.CompositionHolds<MyListF, int, int, int>(
    nums, f, g,
    fa => fa.As().Items);
Console.WriteLine($"  MyList 합성 법칙: {(compositionOnList ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 6b — 두 법칙을 임의 입력으로 (ForAll, 3장 3.10.6절) ==");

// 특정 값이 아니라 임의 입력 100 건으로 두 법칙을 검사한다.
// 표본 함수는 f / g 로 고정하고, 컨테이너 입력만 무작위로 변주한다.
Func<Random, K<MyListF, int>> genList =
    r => new MyList<int>(Enumerable.Range(0, r.Next(5)).Select(_ => r.Next(100)));
Func<Random, K<MyMaybeF, int>> genMaybe =
    r => r.Next(2) == 0 ? MyMaybe<int>.Nothing.Instance : new MyMaybe<int>.Just(r.Next(100));

bool listIdentityRandom = Property.ForAll(
    genList,
    fa => FunctorLaws.IdentityHolds<MyListF, int>(fa, x => x.As().Items));
bool listCompositionRandom = Property.ForAll(
    genList,
    fa => FunctorLaws.CompositionHolds<MyListF, int, int, int>(fa, f, g, x => x.As().Items));
Console.WriteLine($"  MyList 항등 법칙 (임의 100 건) : {(listIdentityRandom ? "통과" : "위반")}");
Console.WriteLine($"  MyList 합성 법칙 (임의 100 건) : {(listCompositionRandom ? "통과" : "위반")}");

Func<K<MyMaybeF, int>, IEnumerable<int>> probeMaybe =
    x => x.As() is MyMaybe<int>.Just j ? [j.Value] : Array.Empty<int>();
bool maybeIdentityRandom = Property.ForAll(
    genMaybe,
    fa => FunctorLaws.IdentityHolds<MyMaybeF, int>(fa, probeMaybe));
bool maybeCompositionRandom = Property.ForAll(
    genMaybe,
    fa => FunctorLaws.CompositionHolds<MyMaybeF, int, int, int>(fa, f, g, probeMaybe));
Console.WriteLine($"  MyMaybe 항등 법칙 (임의 100 건) : {(maybeIdentityRandom ? "통과" : "위반")}");
Console.WriteLine($"  MyMaybe 합성 법칙 (임의 100 건) : {(maybeCompositionRandom ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 7 — 법칙 위반 시연 ① BogusListF (항등 법칙) ==");

K<BogusListF, int> bogus = new BogusList<int>([1, 2, 3]);
var bogusIdentity = BogusListF.Map<int, int>(x => x, bogus);
Console.WriteLine($"  BogusList([1, 2, 3]).Map(x => x) = {bogusIdentity.As()}");
Console.WriteLine($"  → 항등 함수를 매핑했는데 결과가 빈 리스트 — 컨테이너 종류는 같지만 원소 개수가 변함 (3 → 0). 항등 법칙 위반.");

Console.WriteLine();
Console.WriteLine("== 예제 8 — 법칙 위반 시연 ② CountingListF (합성 법칙) ==");

CountingListF.Reset();
K<CountingListF, int> cnums = new CountingList<int>([1, 2, 3]);
var clhs = CountingListF.Map<int, int>(g, CountingListF.Map<int, int>(f, cnums));
int callsTwoMaps = CountingListF.CallCount;

CountingListF.Reset();
var crhs = CountingListF.Map<int, int>(Compose.Of(g, f), cnums);
int callsOneMap = CountingListF.CallCount;

Console.WriteLine($"  Map(g) ∘ Map(f) 호출 수: {callsTwoMaps}");
Console.WriteLine($"  Map(g ∘ f)      호출 수: {callsOneMap}");
Console.WriteLine($"  값은 같은가? {clhs.As().Items.SequenceEqual(crhs.As().Items)}");
Console.WriteLine($"  → 부수 효과 (호출 수) 가 달라 *관찰 가능한 차이* — 합성 법칙 위반.");

Console.WriteLine();
Console.WriteLine("== 예제 9 — 챌린지 ① Tree<A> Functor (필수) ==");

K<TreeF, int> tree = new Tree<int>.Branch(
    new Tree<int>.Leaf(1),
    new Tree<int>.Branch(new Tree<int>.Leaf(2), new Tree<int>.Leaf(3)));

Console.WriteLine($"  원본: {tree.As()}");
var treeMapped = TreeF.Map<int, string>(n => $"#{n}", tree);
Console.WriteLine($"  Map(n => #n) = {treeMapped.As()}");

// 두 법칙 검증 — probe 는 트리를 평탄화한 시퀀스.
bool treeIdentity = FunctorLaws.IdentityHolds<TreeF, int>(
    tree,
    fa => fa.As().Flatten());
bool treeComposition = FunctorLaws.CompositionHolds<TreeF, int, int, int>(
    tree, f, g,
    fa => fa.As().Flatten());
Console.WriteLine($"  Tree 항등 법칙: {(treeIdentity ? "통과" : "위반")}");
Console.WriteLine($"  Tree 합성 법칙: {(treeComposition ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 10 — 심화 챌린지 Const<C, A> Functor ==");

K<ConstF<string>, int> c1 = new Const<string, int>("hi");
var c2 = ConstF<string>.Map<int, double>(n => Math.Sqrt(n), c1);
Console.WriteLine($"  Const(\"hi\") : K<ConstF<string>, int>");
Console.WriteLine($"  Map(Sqrt) = {c2.As()}    // f 는 *호출되지 않는다*. Value 는 그대로.");

Console.WriteLine();
Console.WriteLine("== 예제 11 — 심화 챌린지 Pair<L, _> Functor ==");

K<PairF<string>, int> p1 = new Pair<string, int>("count", 7);
var p2 = PairF<string>.Map<int, int>(n => n * n, p1);
Console.WriteLine($"  Pair(\"count\", 7).Map(n => n*n) = {p2.As()}");
Console.WriteLine($"  → Right (값) 만 변환, Left 는 그대로.");
