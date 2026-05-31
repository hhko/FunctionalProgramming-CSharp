using Ch03.Types;
using Ch03.Functions;
using Ch03.Tests;

Console.WriteLine("=== Ch03 — Monoid / Semigroup ===\n");

// ─────────────────────────────────────────────────────────────────────
// Part A — 다섯 자료 타입의 Monoid 부착
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine("== A1 — Sum / Product / Concat ==");

var sums = new[] { new Sum(1), new Sum(2), new Sum(3) };
Console.WriteLine($"  FoldAll(Sum 1, 2, 3)        = {FoldAll.Of(sums)}      (1+2+3, Empty=0)");

var prods = new[] { new Product(2), new Product(3) };
Console.WriteLine($"  FoldAll(Product 2, 3)       = {FoldAll.Of(prods)}     (2*3, Empty=1)");

var concats = new[] { new Concat("a"), new Concat("b"), new Concat("c") };
Console.WriteLine($"  FoldAll(Concat a, b, c)     = {FoldAll.Of(concats)}");

Console.WriteLine();
Console.WriteLine("== A2 — Boolean: All / Any ==");

var alls = new[] { new All(true), new All(true), new All(false) };
Console.WriteLine($"  FoldAll(All T, T, F)        = {FoldAll.Of(alls)}    (모두 참? 거짓)");

var alls2 = new[] { new All(true), new All(true), new All(true) };
Console.WriteLine($"  FoldAll(All T, T, T)        = {FoldAll.Of(alls2)}     (모두 참)");

var anys = new[] { new Any(false), new Any(true), new Any(false) };
Console.WriteLine($"  FoldAll(Any F, T, F)        = {FoldAll.Of(anys)}     (하나라도 참? 참)");

var anys2 = new[] { new Any(false), new Any(false) };
Console.WriteLine($"  FoldAll(Any F, F)           = {FoldAll.Of(anys2)}    (모두 거짓)");

Console.WriteLine();
Console.WriteLine("== A3 — Max / Min ==");

var maxes = new[] { new Max(3), new Max(7), new Max(2), new Max(5) };
Console.WriteLine($"  FoldAll(Max 3, 7, 2, 5)     = {maxes[0].Combine(maxes[1]).Combine(maxes[2]).Combine(maxes[3])}");

var mins = new[] { new Min(3), new Min(7), new Min(2), new Min(5) };
Console.WriteLine($"  FoldAll(Min 3, 7, 2, 5)     = {mins[0].Combine(mins[1]).Combine(mins[2]).Combine(mins[3])}");

// 빈 컬렉션도 안전 — Empty 가 답을 들고 있음
Console.WriteLine($"  FoldAll(Max 빈)              = {FoldAll.Of(Array.Empty<Max>())}   (Empty=int.MinValue)");
Console.WriteLine($"  FoldAll(Min 빈)              = {FoldAll.Of(Array.Empty<Min>())}   (Empty=int.MaxValue)");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────
// Part B — 빈 컬렉션의 안전
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine("== B — 빈 컬렉션의 안전: Empty 가 답을 들고 있음 ==");

Console.WriteLine($"  FoldAll(Sum 빈)              = {FoldAll.Of(Array.Empty<Sum>())}      (Empty=0)");
Console.WriteLine($"  FoldAll(Product 빈)          = {FoldAll.Of(Array.Empty<Product>())}      (Empty=1)");
Console.WriteLine($"  FoldAll(Concat 빈)           = '{FoldAll.Of(Array.Empty<Concat>()).Value}'        (Empty=\"\")");
Console.WriteLine($"  FoldAll(All 빈)              = {FoldAll.Of(Array.Empty<All>())}   (Empty=true, vacuous truth)");
Console.WriteLine($"  FoldAll(Any 빈)              = {FoldAll.Of(Array.Empty<Any>())}  (Empty=false, vacuous falsity)");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────
// Part C — + 연산자 데모 (Semigroup 의 default operator)
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine("== C — + 연산자: Combine 의 syntactic sugar ==");

var sumOp = new Sum(1) + new Sum(2) + new Sum(3);
Console.WriteLine($"  Sum(1) + Sum(2) + Sum(3)            = {sumOp}");

var concatOp = new Concat("hello") + new Concat(" ") + new Concat("world");
Console.WriteLine($"  Concat 'hello' + ' ' + 'world'      = {concatOp}");

var allOp = new All(true) + new All(true) + new All(false);
Console.WriteLine($"  All T + T + F                       = {allOp}");

// + 와 Combine 은 같은 결과 — operator 는 syntactic sugar
var same = new Sum(2).Combine(new Sum(3)).Equals(new Sum(2) + new Sum(3));
Console.WriteLine($"  a.Combine(b) == a + b               = {same}");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────
// Part D — 평균을 Monoid 로 만드는 패턴
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine("== D — 평균을 Monoid 로: {Total, Count} 자료의 누적 ==");
Console.WriteLine("  (평균은 직접 Monoid 아님 — 자료의 모양을 바꿔 Monoid 어휘 안으로)");

int[] scores = { 80, 90, 70, 85, 75 };
var avgs = scores.Select(Avg.Of).ToArray();
var totalAvg = FoldAll.Of(avgs);
Console.WriteLine($"  scores = [80, 90, 70, 85, 75]");
Console.WriteLine($"  FoldAll(Avg) = {totalAvg}  → 평균 = {totalAvg.Value}");

// 빈 컬렉션도 안전 — Empty (0, 0) 이 답을 들고 있음
var emptyAvg = FoldAll.Of(Array.Empty<Avg>());
Console.WriteLine($"  FoldAll(Avg 빈) = {emptyAvg} → 평균 = {emptyAvg.Value} (Empty=(0,0))");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────
// Part E — 두 법칙 검증
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine("== E — 두 법칙 검증: 결합 + 항등 ==");

var (s1, s2, s3) = (new Sum(2), new Sum(3), new Sum(5));
Console.WriteLine($"  Sum 결합 법칙      : {MonoidLaws.AssociativityHolds(s1, s2, s3)}");
Console.WriteLine($"  Sum 좌 항등        : {MonoidLaws.LeftIdentityHolds(s1)}");
Console.WriteLine($"  Sum 우 항등        : {MonoidLaws.RightIdentityHolds(s1)}");

var (p1, p2, p3) = (new Product(2), new Product(3), new Product(4));
Console.WriteLine($"  Product 결합 법칙  : {MonoidLaws.AssociativityHolds(p1, p2, p3)}");

var (c1, c2, c3) = (new Concat("x"), new Concat("y"), new Concat("z"));
Console.WriteLine($"  Concat 결합 법칙   : {MonoidLaws.AssociativityHolds(c1, c2, c3)}");

var (a1, a2, a3) = (new All(true), new All(false), new All(true));
Console.WriteLine($"  All 결합 법칙      : {MonoidLaws.AssociativityHolds(a1, a2, a3)}");
Console.WriteLine($"  All 좌 항등        : {MonoidLaws.LeftIdentityHolds(a1)}");

var (mx1, mx2, mx3) = (new Max(3), new Max(7), new Max(5));
Console.WriteLine($"  Max 결합 법칙      : {MonoidLaws.AssociativityHolds(mx1, mx2, mx3)}");

var (av1, av2, av3) = (Avg.Of(80), Avg.Of(90), Avg.Of(70));
Console.WriteLine($"  Avg 결합 법칙      : {MonoidLaws.AssociativityHolds(av1, av2, av3)}");
Console.WriteLine($"  Avg 좌 항등        : {MonoidLaws.LeftIdentityHolds(av1)}");
