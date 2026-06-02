using Ch11.Challenges;
using Ch11.Functions;
using Ch11.Tests;
using Ch11.Traits;
using Ch11.Types;

Console.WriteLine("================================================");
Console.WriteLine("11장 — Alternative & MonoidK (선택과 결합)");
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

Console.WriteLine($"  좌 단위원 Combine(Empty, x) ≡ x : {Pass(leftId)}");
Console.WriteLine($"  우 단위원 Combine(x, Empty) ≡ x : {Pass(rightId)}");
Console.WriteLine($"  결합법칙                        : {Pass(assoc)}");
Console.WriteLine();

Console.WriteLine(leftId && rightId && assoc ? "모든 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");

return;

static string Pass(bool b) => b ? "통과" : "위반";
