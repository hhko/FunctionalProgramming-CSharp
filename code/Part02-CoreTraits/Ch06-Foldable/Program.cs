using Ch06.Challenges;
using Ch06.Functions;
using Ch06.Tests;
using Ch06.Traits;
using Ch06.Types;

Console.WriteLine("================================================");
Console.WriteLine("6장 — Foldable");
Console.WriteLine("================================================");
Console.WriteLine();

Console.WriteLine("== 예제 1 — MyList Fold (세 가지 호출 어법) ==");

K<MyListF, int> nums = new MyList<int>([1, 2, 3, 4, 5]);

// ① trait 정적 호출 — Foldable<MyListF>.FoldLeft 를 직접
int r1 = MyListF.FoldLeft<int, int>((acc, n) => acc + n, 0, nums);

// ② 모듈 어법 — Foldable.foldLeft<F, A, B> 의 generic 디스패치 (§6.2.6)
int r2 = Foldable.foldLeft<MyListF, int, int>((acc, n) => acc + n, 0, nums);

// ③ 확장 어법 — K<F, A> 위 점 호출 (§6.2.6, 가장 자연스러운 호출)
int r3 = nums.FoldLeft((acc, n) => acc + n, 0);

Console.WriteLine($"  ① trait 정적   MyListF.FoldLeft<>(f, 0, nums)  = {r1}");
Console.WriteLine($"  ② 모듈 어법    Foldable.foldLeft<>(f, 0, nums) = {r2}");
Console.WriteLine($"  ③ 확장 어법    nums.FoldLeft(f, 0)             = {r3}");
Console.WriteLine($"  세 결과 동일?  {r1 == r2 && r2 == r3}");

Console.WriteLine();
Console.WriteLine("== 예제 2 — MyMaybe Fold (세 가지 호출 어법) ==");

K<MyMaybeF, int> just = new MyMaybe<int>.Just(42);

int m1 = MyMaybeF.FoldLeft<int, int>((acc, n) => acc + n, 0, just);
int m2 = Foldable.foldLeft<MyMaybeF, int, int>((acc, n) => acc + n, 0, just);
int m3 = just.FoldLeft((acc, n) => acc + n, 0);

Console.WriteLine($"  ① trait 정적   MyMaybeF.FoldLeft<>(f, 0, just) = {m1}");
Console.WriteLine($"  ② 모듈 어법    Foldable.foldLeft<>(f, 0, just) = {m2}");
Console.WriteLine($"  ③ 확장 어법    just.FoldLeft(f, 0)             = {m3}");

K<MyMaybeF, int> nothing = MyMaybe<int>.Nothing.Instance;
int mN = nothing.FoldLeft((acc, n) => acc + n, 0);
Console.WriteLine($"  Nothing.FoldLeft(f, 0) = {mN}    // seed 그대로 (step 호출 0회)");

Console.WriteLine();
Console.WriteLine("== 예제 3 — virtual 자유 함수 (확장 어법) ==");
Console.WriteLine($"  nums.Count()   = {nums.Count()}");
Console.WriteLine($"  nums.IsEmpty() = {nums.IsEmpty()}");
Console.WriteLine($"  nums.All(n => n > 0) = {nums.All(n => n > 0)}");
Console.WriteLine($"  nums.Any(n => n > 4) = {nums.Any(n => n > 4)}");
Console.WriteLine($"  nums.First()   = {nums.First()}");

Console.WriteLine();
Console.WriteLine("== 예제 4 — 같은 일반 함수가 두 다른 Foldable 처리 ==");
Console.WriteLine($"  SumAnyFoldable.Run<MyListF>(nums)  = {SumAnyFoldable.Run<MyListF>(nums)}");
Console.WriteLine($"  SumAnyFoldable.Run<MyMaybeF>(just) = {SumAnyFoldable.Run<MyMaybeF>(just)}");
Console.WriteLine($"  SumAnyFoldable.Run<MyMaybeF>(nothing) = {SumAnyFoldable.Run<MyMaybeF>(nothing)}");

Console.WriteLine();
Console.WriteLine("== 예제 5 — 결과 일관성 법칙 (FoldRight ↔ FoldLeft, 덧셈은 가환·결합) ==");
bool listConsistency = FoldableLaws.ConsistencyHolds<MyListF, int>(
    nums, (a, b) => a + b, 0);
bool maybeConsistency = FoldableLaws.ConsistencyHolds<MyMaybeF, int>(
    just, (a, b) => a + b, 0);
Console.WriteLine($"  MyListF  결과 일관성: {(listConsistency ? "통과" : "위반")}");
Console.WriteLine($"  MyMaybeF 결과 일관성: {(maybeConsistency ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 6 — 자유 함수 일관성 (Count 가 두 방향으로 같은 결과) ==");
bool listFreeFn  = FoldableLaws.FreeFunctionConsistencyHolds<MyListF, int>(nums);
bool maybeFreeFn = FoldableLaws.FreeFunctionConsistencyHolds<MyMaybeF, int>(just);
Console.WriteLine($"  MyListF  자유 함수 일관성: {(listFreeFn ? "통과" : "위반")}");
Console.WriteLine($"  MyMaybeF 자유 함수 일관성: {(maybeFreeFn ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 6.5 — 법칙을 임의 입력으로: 최소 property 검증 (ForAll) ==");

// 특정 값이 아니라 임의 입력 100 건으로 결과 일관성을 검사한다 (3장 §3.7.1 의 ForAll 재사용).
// 생성기 — 길이 r.Next(5) 의 임의 int 리스트. 함수 인자 표본은 덧셈으로 고정 (가환·결합).
static K<MyListF, int> RandomMyList(Random r)
{
    var len = r.Next(5);
    var items = new int[len];
    for (var i = 0; i < len; i++) items[i] = r.Next(-1000, 1000);
    return new MyList<int>(items);
}

var consistencyRandom = Property.ForAll(
    RandomMyList,
    fa => FoldableLaws.ConsistencyHolds<MyListF, int>(fa, (a, b) => a + b, 0));
var freeFnRandom = Property.ForAll(
    RandomMyList,
    fa => FoldableLaws.FreeFunctionConsistencyHolds<MyListF, int>(fa));
Console.WriteLine($"  결과 일관성 (임의 100 건)   : {(consistencyRandom ? "통과" : "위반")}");
Console.WriteLine($"  자유 함수 일관성 (임의 100 건) : {(freeFnRandom ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 7 — 법칙 위반 시연 ① BogusFoldableF ==");
K<BogusFoldableF, int> bogus = new BogusList<int>([1, 2, 3]);
int bogusRight = BogusFoldableF.FoldRight<int, int>((a, acc) => a + acc, 0, bogus);
int bogusLeft  = BogusFoldableF.FoldLeft<int, int>((acc, a) => acc + a, 0, bogus);
Console.WriteLine($"  BogusList([1,2,3]).FoldRight(+, 0) = {bogusRight}     // 정상");
Console.WriteLine($"  BogusList([1,2,3]).FoldLeft (+, 0) = {bogusLeft}     // seed 그대로 (자료 무시)");
Console.WriteLine($"  → 결과 일관성 위반 — 두 방향이 다른 결과.");

Console.WriteLine();
Console.WriteLine("== 예제 8 — 법칙 위반 시연 ② CountingFoldableF ==");
CountingFoldableF.Reset();
K<CountingFoldableF, int> cnums = new CountingList<int>([1, 2, 3, 4, 5]);
int cSum = CountingFoldableF.FoldLeft<int, int>((acc, n) => acc + n, 0, cnums);
int onceCallCount = CountingFoldableF.CallCount;

int cSumAgain = CountingFoldableF.FoldRight<int, int>((n, acc) => n + acc, 0, cnums);
int twiceCallCount = CountingFoldableF.CallCount;

Console.WriteLine($"  Sum (FoldLeft 한 번) = {cSum}, CallCount = {onceCallCount}");
Console.WriteLine($"  Sum (+ FoldRight 한 번 더) = {cSumAgain}, CallCount = {twiceCallCount}");
Console.WriteLine($"  → 값은 같지만 부수 효과 (호출 횟수) 가 두 배 — 관찰 가능한 차이.");

Console.WriteLine();
Console.WriteLine("== 예제 9 — 챌린지 ① Tree<A> Foldable (필수) ==");
K<TreeF, int> tree = new Tree<int>.Branch(
    new Tree<int>.Leaf(1),
    new Tree<int>.Branch(new Tree<int>.Leaf(2), new Tree<int>.Leaf(3)));

Console.WriteLine($"  원본: {tree.As()}");
int treeSum   = TreeF.FoldLeft<int, int>((acc, n) => acc + n, 0, tree);
int treeCount = tree.Count();
bool treeConsistency = FoldableLaws.ConsistencyHolds<TreeF, int>(
    tree, (a, b) => a + b, 0);
Console.WriteLine($"  Tree Sum     = {treeSum}     // 1+2+3");
Console.WriteLine($"  Tree Count   = {treeCount}");
Console.WriteLine($"  결과 일관성: {(treeConsistency ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 10 — 심화 챌린지 Const<C, A> Foldable ==");
K<ConstF<string>, int> c1 = new Const<string, int>("hi");
int constSum   = ConstF<string>.FoldLeft<int, int>((acc, n) => acc + n, 0, c1);
int constCount = c1.Count();
Console.WriteLine($"  Const(\"hi\") : K<ConstF<string>, int>");
Console.WriteLine($"  FoldLeft(+, 0) = {constSum}     // step 0회 — seed 그대로");
Console.WriteLine($"  Count() = {constCount}              // 0 (phantom A, 원소 없음)");

Console.WriteLine();
Console.WriteLine("== 예제 11 — 심화 챌린지 Pair<L, _> Foldable ==");
K<PairF<string>, int> p1 = new Pair<string, int>("count", 7);
int pairSum = PairF<string>.FoldLeft<int, int>((acc, n) => acc + n, 0, p1);
int pairCount = p1.Count();
Console.WriteLine($"  Pair(\"count\", 7).FoldLeft(+, 0) = {pairSum}     // Right 한 번만 누적");
Console.WriteLine($"  Pair Count = {pairCount}                          // 1 (Right 한 원소)");
