using Ch14.Challenges;
using Ch14.Functions;
using Ch14.Tests;
using Ch14.Traits;
using Ch14.Types;

Console.WriteLine("================================================");
Console.WriteLine("14장 — Alternative & MonoidK (선택과 결합)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — Combine 과 Choose 가 시퀀스에서 다르다 ─────────────────
Console.WriteLine("== 예제 1 — MySeq: Combine(concat) vs Choose(첫 비어있지않음) ==");

K<SeqF, int> a = new MySeq<int>([1, 2]);
K<SeqF, int> b = new MySeq<int>([3, 4]);
K<SeqF, int> empty = SeqF.Empty<int>();

Console.WriteLine($"  Combine([1,2],[3,4]) = {a.Combine(b).As()}   (둘 다 모음)");
Console.WriteLine($"  Choose ([1,2],[3,4]) = {a.Choose(b).As()}   (첫 비어있지 않은 쪽)");
Console.WriteLine($"  Choose ([],[3,4])    = {empty.Choose(b).As()}   (왼쪽 비어 → 오른쪽)");
Console.WriteLine();

// ── 예제 2 — Maybe 에서는 Combine 과 Choose 가 같다 ─────────────────
Console.WriteLine("== 예제 2 — MyMaybe: Combine = Choose = 첫 Just ==");

K<MaybeF, int> none  = MaybeF.Empty<int>();
K<MaybeF, int> some3 = new MyMaybe<int>.Just(3);
K<MaybeF, int> some9 = new MyMaybe<int>.Just(9);

Console.WriteLine($"  Combine(Nothing, Just 3) = {none.Combine(some3).As()}");
Console.WriteLine($"  Choose (Just 3, Just 9)  = {some3.Choose(some9).As()}");
Console.WriteLine($"  fallback 패턴 (x ?? 기본): Choose(Nothing, Just 9) = {none.Choose(some9).As()}");
Console.WriteLine();

// ── 예제 3 — OneOf: 여러 후보 중 첫 성공 ────────────────────────────
Console.WriteLine("== 예제 3 — OneOf: 여러 후보 중 첫 성공 ==");

var firstHit = Alternatives.oneOf<MaybeF, int>(none, none, some3, some9).As();
Console.WriteLine($"  OneOf(Nothing, Nothing, Just 3, Just 9) = {firstHit}");
Console.WriteLine();

// ── 예제 4 — 심화: guard (조건 필터의 토대) ─────────────────────────
Console.WriteLine("== 예제 4 — guard: 조건이 참이면 통과, 거짓이면 Empty ==");

var pass = Guards.guard<MaybeF>(true).As();
var fail = Guards.guard<MaybeF>(false).As();
Console.WriteLine($"  guard(true)  = {pass}");
Console.WriteLine($"  guard(false) = {fail}");
Console.WriteLine();

// ── 법칙 검증 (MonoidK) ─────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (MonoidK) ==");

Func<K<SeqF, int>, IEnumerable<int>> probe = fa => fa.As().Items;
var leftId  = MonoidKLaws.LeftIdentityHolds<SeqF, int>(a, probe);
var rightId = MonoidKLaws.RightIdentityHolds<SeqF, int>(a, probe);
var assoc   = MonoidKLaws.AssociativityHolds<SeqF, int>(a, b, new MySeq<int>([5, 6]), probe);

Console.WriteLine($"  좌 항등원 Combine(Empty, x) ≡ x : {Pass(leftId)}");
Console.WriteLine($"  우 항등원 Combine(x, Empty) ≡ x : {Pass(rightId)}");
Console.WriteLine($"  결합법칙                        : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "MonoidK 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");
Console.WriteLine();

// ── 예제 5 — option: 실패면 기본값 (x ?? value 의 Elevated 일반형) ───
Console.WriteLine("== 예제 5 — option: 실패면 기본값으로 떨어짐 ==");

var withVal = Alternatives.option<MaybeF, int>(99, some3).As();   // some3 성공 → Just 3
var withDef = Alternatives.option<MaybeF, int>(99, none).As();    // none 실패 → Just 99

Console.WriteLine($"  option(99, Just 3)  = {withVal}");
Console.WriteLine($"  option(99, Nothing) = {withDef}");
Console.WriteLine();

// ── 법칙 검증 (Alternative 세 법칙 — v5 Alternative.Laws 정합) ─────
Console.WriteLine("== 법칙 검증 (Alternative) ==");

Func<K<MaybeF, int>, IEnumerable<int>> aprobe =
    fa => fa.As() is MyMaybe<int>.Just j ? new[] { j.Value } : Array.Empty<int>();

var lz = AlternativeLaws.LeftZeroHolds<MaybeF, int>(7, aprobe);
var rz = AlternativeLaws.RightZeroHolds<MaybeF, int>(7, aprobe);
var lc = AlternativeLaws.LeftCatchHolds<MaybeF, int>(3, 9, aprobe);

Console.WriteLine($"  좌 zero  Choose(Empty, Pure b) ≡ Pure b : {Pass(lz)}");
Console.WriteLine($"  우 zero  Choose(Pure a, Empty) ≡ Pure a : {Pass(rz)}");
Console.WriteLine($"  좌 catch Choose(Pure a, Pure b) ≡ Pure a: {Pass(lc)}");
Console.WriteLine();
Console.WriteLine(lz && rz && lc ? "Alternative 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
