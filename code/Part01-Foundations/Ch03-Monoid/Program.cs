using Ch03.Types;
using Ch03.Functions;
using Ch03.Tests;

Console.WriteLine("=== Ch03 — Monoid / Semigroup ===\n");

// ─────────────────────────────────────────────────────────────────────
// Part A — 다섯 자료 타입의 Monoid 부착
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine("== A1 — Sum / Product / Concat ==");

var sums = new[] { new Sum(1), new Sum(2), new Sum(3) };
Console.WriteLine($"  Monoid.combine(Sum 1, 2, 3) = {Monoid.combine(sums)}      (1+2+3, Empty=0)");

var prods = new[] { new Product(2), new Product(3) };
Console.WriteLine($"  Monoid.combine(Product 2, 3) = {Monoid.combine(prods)}     (2*3, Empty=1)");

var concats = new[] { new Concat("a"), new Concat("b"), new Concat("c") };
Console.WriteLine($"  Monoid.combine(Concat a, b, c) = {Monoid.combine(concats)}");

Console.WriteLine();
Console.WriteLine("== A2 — Boolean: All / Any ==");

var alls = new[] { new All(true), new All(true), new All(false) };
Console.WriteLine($"  Monoid.combine(All T, T, F) = {Monoid.combine(alls)}    (모두 참? 거짓)");

var alls2 = new[] { new All(true), new All(true), new All(true) };
Console.WriteLine($"  Monoid.combine(All T, T, T) = {Monoid.combine(alls2)}     (모두 참)");

var anys = new[] { new Any(false), new Any(true), new Any(false) };
Console.WriteLine($"  Monoid.combine(Any F, T, F) = {Monoid.combine(anys)}     (하나라도 참? 참)");

var anys2 = new[] { new Any(false), new Any(false) };
Console.WriteLine($"  Monoid.combine(Any F, F)    = {Monoid.combine(anys2)}    (모두 거짓)");

Console.WriteLine();
Console.WriteLine("== A3 — Max / Min ==");

var maxes = new[] { new Max(3), new Max(7), new Max(2), new Max(5) };
Console.WriteLine($"  Monoid.combine(Max 3, 7, 2, 5) = {Monoid.combine(maxes)}");

var mins = new[] { new Min(3), new Min(7), new Min(2), new Min(5) };
Console.WriteLine($"  Monoid.combine(Min 3, 7, 2, 5) = {Monoid.combine(mins)}");

// 빈 컬렉션도 안전 — Empty 가 답을 들고 있음
Console.WriteLine($"  Monoid.combine(Max 빈)      = {Monoid.combine(Array.Empty<Max>())}   (Empty=int.MinValue)");
Console.WriteLine($"  Monoid.combine(Min 빈)      = {Monoid.combine(Array.Empty<Min>())}   (Empty=int.MaxValue)");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────
// Part B — 빈 컬렉션의 안전
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine("== B — 빈 컬렉션의 안전: Empty 가 답을 들고 있음 ==");

Console.WriteLine($"  Monoid.combine(Sum 빈)      = {Monoid.combine(Array.Empty<Sum>())}      (Empty=0)");
Console.WriteLine($"  Monoid.combine(Product 빈)  = {Monoid.combine(Array.Empty<Product>())}      (Empty=1)");
Console.WriteLine($"  Monoid.combine(Concat 빈)   = '{Monoid.combine(Array.Empty<Concat>()).Value}'        (Empty=\"\")");
Console.WriteLine($"  Monoid.combine(All 빈)      = {Monoid.combine(Array.Empty<All>())}   (Empty=true, vacuous truth)");
Console.WriteLine($"  Monoid.combine(Any 빈)      = {Monoid.combine(Array.Empty<Any>())}  (Empty=false, vacuous falsity)");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────
// Part C — + 연산자 데모 (SemigroupExtensions 의 extension operator)
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine("== C — + 연산자: SemigroupExtensions 의 extension static operator ==");

var sumOp = new Sum(1) + new Sum(2) + new Sum(3);
Console.WriteLine($"  Sum(1) + Sum(2) + Sum(3)            = {sumOp}");

var concatOp = new Concat("hello") + new Concat(" ") + new Concat("world");
Console.WriteLine($"  Concat 'hello' + ' ' + 'world'      = {concatOp}");

var allOp = new All(true) + new All(true) + new All(false);
Console.WriteLine($"  All T + T + F                       = {allOp}");

// + 와 Combine 은 같은 결과 — extension operator 가 Combine 으로 위임
var same = new Sum(2).Combine(new Sum(3)).Equals(new Sum(2) + new Sum(3));
Console.WriteLine($"  a.Combine(b) == a + b               = {same}");

// Semigroup.combine 자유 함수 어법 — instance + 와 같은 결과
var freeFun = Semigroup.combine(new Sum(2), new Sum(3));
Console.WriteLine($"  Semigroup.combine(Sum(2), Sum(3))   = {freeFun}");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────
// Part D — 평균을 Monoid 로 만드는 패턴
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine("== D — 평균을 Monoid 로: {Total, Count} 자료의 누적 ==");
Console.WriteLine("  (평균은 직접 Monoid 아님 — 자료의 모양을 바꿔 Monoid 어휘 안으로)");

int[] scores = { 80, 90, 70, 85, 75 };
var avgs = scores.Select(Avg.Of).ToArray();
var totalAvg = Monoid.combine(avgs);
Console.WriteLine($"  scores = [80, 90, 70, 85, 75]");
Console.WriteLine($"  Monoid.combine(Avg) = {totalAvg}  → 평균 = {totalAvg.Value}");

// 빈 컬렉션도 안전 — Empty (0, 0) 이 답을 들고 있음
var emptyAvg = Monoid.combine(Array.Empty<Avg>());
Console.WriteLine($"  Monoid.combine(Avg 빈) = {emptyAvg} → 평균 = {emptyAvg.Value} (Empty=(0,0))");

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

// 가짜 반례 — Mean (평균) 은 시그니처는 통과하지만 결합 법칙을 어긴다.
// (a·b)·c = 5.5, a·(b·c) = 4.0 → 묶는 순서가 결과를 바꿈. §3.7 의 평균 반례.
var (m1, m2, m3) = (new Mean(2), new Mean(4), new Mean(8));
Console.WriteLine($"  Mean 결합 법칙     : {(MonoidLaws.AssociativityHolds(m1, m2, m3) ? "통과" : "위반 (예상대로)")}");
Console.WriteLine($"    (a·b)·c = {m1.Combine(m2).Combine(m3).Value}, a·(b·c) = {m1.Combine(m2.Combine(m3)).Value}  → 시그니처는 통과, 결합 법칙은 위반");

// ─────────────────────────────────────────────────────────────────────
// Part E2 — 법칙을 임의 입력으로 (최소 property 검증)
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine();
Console.WriteLine("== E2 — 법칙을 임의 입력으로: 최소 property 검증 (ForAll) ==");

// 특정 값이 아니라 임의 입력 100 건으로 결합·항등을 검사한다.
var assocRandom = Property.ForAll(
    r => (new Sum(r.Next(-1000, 1000)), new Sum(r.Next(-1000, 1000)), new Sum(r.Next(-1000, 1000))),
    t => MonoidLaws.AssociativityHolds(t.Item1, t.Item2, t.Item3));
var identRandom = Property.ForAll(
    r => new Sum(r.Next(-1000, 1000)),
    a => MonoidLaws.LeftIdentityHolds(a) && MonoidLaws.RightIdentityHolds(a));
Console.WriteLine($"  Sum 결합 법칙 (임의 100 건) : {(assocRandom ? "통과" : "위반")}");
Console.WriteLine($"  Sum 항등 법칙 (임의 100 건) : {(identRandom ? "통과" : "위반")}");

// 가짜 Mean 은 임의 입력에서 곧장 반례가 잡힌다.
var meanRandom = Property.ForAll(
    r => (new Mean(r.Next(100)), new Mean(r.Next(100)), new Mean(r.Next(100))),
    t => MonoidLaws.AssociativityHolds(t.Item1, t.Item2, t.Item3));
Console.WriteLine($"  Mean 결합 법칙 (임의 100 건) : {(meanRandom ? "통과" : "위반 (예상대로)")}");

// ─────────────────────────────────────────────────────────────────────
// Part F — Instance record form (v5 정통 자리)
// ─────────────────────────────────────────────────────────────────────

Console.WriteLine();
Console.WriteLine("== F — Instance record form (trait 을 값처럼 전달) ==");

// trait 의 static virtual Instance — Monoid.instance<A>() helper 로 호출 (v5 정통).
var sumInstance = Monoid.instance<Sum>();
Console.WriteLine($"  Monoid.instance<Sum>().Empty           = {sumInstance.Empty}");
Console.WriteLine($"  Monoid.instance<Sum>().Combine(2, 3)   = {sumInstance.Combine(new Sum(2), new Sum(3))}");

var concatInstance = Monoid.instance<Concat>();
Console.WriteLine($"  Monoid.instance<Concat>().Empty        = '{concatInstance.Empty.Value}'");
Console.WriteLine($"  Monoid.instance<Concat>().Combine(a, b) = {concatInstance.Combine(new Concat("a"), new Concat("b"))}");

var allInstance = Monoid.instance<All>();
Console.WriteLine($"  Monoid.instance<All>().Empty           = {allInstance.Empty}");
