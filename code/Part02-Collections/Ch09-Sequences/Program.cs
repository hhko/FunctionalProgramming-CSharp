using Ch09.Challenges;
using Ch09.Functions;
using Ch09.Tests;
using Ch09.Traits;
using Ch09.Types;

Console.WriteLine("================================================");
Console.WriteLine("9장 — Sequences (불변 시퀀스에 1부 추상 부착)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — MySeq Map (세 가지 호출 어법) ──────────────────────────
Console.WriteLine("== 예제 1 — MySeq Map (세 가지 호출 어법) ==");

K<SeqF, int> nums = new MySeq<int>([1, 2, 3, 4, 5]);

K<SeqF, int> r1 = SeqF.Map<int, int>(n => n * 2, nums);   // ① trait 정적
K<SeqF, int> r2 = Monad.map<SeqF, int, int>(n => n * 2, nums); // ② 모듈
K<SeqF, int> r3 = nums.Map(n => n * 2);                    // ③ 확장

Console.WriteLine($"  ① trait 정적  = {r1.As()}");
Console.WriteLine($"  ② 모듈 어법   = {r2.As()}");
Console.WriteLine($"  ③ 확장 어법   = {r3.As()}");
Console.WriteLine($"  세 결과 동일? {r1.As().Items.SequenceEqual(r2.As().Items) && r2.As().Items.SequenceEqual(r3.As().Items)}");
Console.WriteLine();

// ── 예제 2 — Bind 와 LINQ SelectMany 의 대응 ────────────────────────
Console.WriteLine("== 예제 2 — Bind ≡ LINQ SelectMany ==");

// Bind 직접 호출 — 각 n 을 [n, n*10] 으로 펼침.
K<SeqF, int> bound = nums.Bind(n => (K<SeqF, int>)new MySeq<int>([n, n * 10]));

// 같은 계산을 LINQ from-from-select 로.
K<SeqF, int> viaLinq =
    from n in nums
    from m in (K<SeqF, int>)new MySeq<int>([n, n * 10])
    select m;

Console.WriteLine($"  Bind 직접   = {bound.As()}");
Console.WriteLine($"  LINQ 쿼리   = {viaLinq.As()}");
Console.WriteLine($"  동일?       {bound.As().Items.SequenceEqual(viaLinq.As().Items)}");
Console.WriteLine();

// ── 예제 3 — Foldable (Sum / Count / Any) ───────────────────────────
Console.WriteLine("== 예제 3 — Foldable: 시퀀스를 한 값으로 끌어내림 ==");

var sum   = nums.FoldLeft((acc, n) => acc + n, 0);
var count = nums.Count();
var anyEven = nums.Any(n => n % 2 == 0);

Console.WriteLine($"  Sum   = {sum}");
Console.WriteLine($"  Count = {count}");
Console.WriteLine($"  Any(짝수) = {anyEven}");
Console.WriteLine();

// ── 예제 4 — Traverse: List<Maybe> → Maybe<List> ────────────────────
Console.WriteLine("== 예제 4 — Traverse: 모두 성공해야 성공 ==");

Func<string, K<MaybeF, int>> parse = s =>
    int.TryParse(s, out var v)
        ? new MyMaybe<int>.Just(v)
        : MyMaybe<int>.Nothing.Instance;

K<SeqF, string> good = new MySeq<string>(["1", "2", "3"]);
K<SeqF, string> bad  = new MySeq<string>(["1", "x", "3"]);

var goodR = Traverse.traverse<SeqF, MaybeF, string, int>(parse, good).As();
var badR  = Traverse.traverse<SeqF, MaybeF, string, int>(parse, bad).As();

Console.WriteLine($"  [\"1\",\"2\",\"3\"] traverse parse = {Describe(goodR)}");
Console.WriteLine($"  [\"1\",\"x\",\"3\"] traverse parse = {Describe(badR)}");
Console.WriteLine();

// ── 예제 5 — 같은 generic 함수가 MySeq / MyLst 모두에 ───────────────
Console.WriteLine("== 예제 5 — 표현이 달라도 같은 trait (MySeq vs MyLst) ==");

K<LstF, int> lst = MyLst<int>.FromEnumerable([1, 2, 3, 4, 5]);
var lstSum   = Foldable.foldLeft<LstF, int, int>((acc, n) => acc + n, 0, lst);
var lstBound = Monad.bind<LstF, int, int>(lst, n => LstF.Pure(n * 100));

Console.WriteLine($"  MyLst Sum (같은 Foldable.foldLeft) = {lstSum}");
Console.WriteLine($"  MyLst Bind = Lst[{string.Join(", ", lstBound.As().ToEnumerable())}]");
Console.WriteLine();

// ── 예제 6 — 심화 챌린지: 두 Applicative (곱 vs zip) ────────────────
Console.WriteLine("== 예제 6 — 시퀀스의 두 Applicative ==");

K<SeqF, Func<int, int>> fns = new MySeq<Func<int, int>>([n => n + 1, n => n * 10]);
K<SeqF, int> vals = new MySeq<int>([1, 2]);
var cart = SeqF.Apply(fns, vals).As();

K<ZipF, Func<int, int>> zfns = new ZipSeq<Func<int, int>>([n => n + 1, n => n * 10]);
K<ZipF, int> zargs = new ZipSeq<int>([1, 2]);
var zipped = ZipF.Apply(zfns, zargs).As();

Console.WriteLine($"  데카르트 곱 Apply = {cart}");
Console.WriteLine($"  zip Apply         = {zipped}");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 ==");

Func<K<SeqF, int>, IEnumerable<int>> probe = fa => fa.As().Items;
Func<int, K<SeqF, int>> f = n => new MySeq<int>([n, n + 1]);
Func<int, K<SeqF, int>> g = n => new MySeq<int>([n * 10]);

var leftId  = MonadLaws.LeftIdentityHolds<SeqF, int, int>(7, f, probe);
var rightId = MonadLaws.RightIdentityHolds<SeqF, int>(nums, probe);
var assoc   = MonadLaws.AssociativityHolds<SeqF, int, int, int>(nums, f, g, probe);
var foldOk  = FoldableLaws.ConsistencyHolds<SeqF, int>(nums, (a, b) => a + b, 0);
var countOk = FoldableLaws.CountConsistencyHolds<SeqF, int>(nums);

Console.WriteLine($"  Monad 좌 항등 : {Pass(leftId)}");
Console.WriteLine($"  Monad 우 항등 : {Pass(rightId)}");
Console.WriteLine($"  Monad 결합    : {Pass(assoc)}");
Console.WriteLine($"  Foldable 일관성: {Pass(foldOk)}");
Console.WriteLine($"  Count 일관성   : {Pass(countOk)}");
Console.WriteLine();

var allPass = leftId && rightId && assoc && foldOk && countOk;
Console.WriteLine(allPass ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";

// traverse 결과는 MyMaybe<K<SeqF, int>> — 안쪽 시퀀스를 As() 로 풀어 출력.
static string Describe(MyMaybe<K<SeqF, int>> m) => m switch
{
    MyMaybe<K<SeqF, int>>.Just j => $"Just({j.Value.As()})",
    _                            => "Nothing"
};
