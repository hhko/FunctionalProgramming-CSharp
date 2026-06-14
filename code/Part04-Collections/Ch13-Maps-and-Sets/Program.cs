using Ch13.Challenges;
using Ch13.Functions;
using Ch13.Tests;
using Ch13.Traits;
using Ch13.Types;

Console.WriteLine("================================================");
Console.WriteLine("13장 — Maps & Sets (키-값 / 집합 컨테이너)");
Console.WriteLine("================================================");
Console.WriteLine();

// ── 예제 1 — MyMap Functor: 값에만 작용 ─────────────────────────────
Console.WriteLine("== 예제 1 — MyMap Map 은 값에만 작용 (키 보존) ==");

K<MapF<string>, int> prices = new MyMap<string, int>(new Dictionary<string, int>
{
    ["apple"]  = 1000,
    ["banana"] = 500,
    ["cherry"] = 3000,
});

// 10% 인상 — 값만 바뀌고 키는 그대로.
K<MapF<string>, int> raised = prices.Map(p => (int)(p * 1.1));

Console.WriteLine($"  원본    = {prices.As()}");
Console.WriteLine($"  10% ↑   = {raised.As()}");
Console.WriteLine($"  키 보존? {prices.As().Pairs.Select(p => p.Key).SequenceEqual(raised.As().Pairs.Select(p => p.Key))}");
Console.WriteLine();

// ── 예제 2 — MyMap Foldable: 값들을 한 값으로 ───────────────────────
Console.WriteLine("== 예제 2 — MyMap Foldable: 값 합계 / 개수 ==");

var total = prices.FoldLeft((acc, v) => acc + v, 0);
var count = prices.Count();

Console.WriteLine($"  합계  = {total}");
Console.WriteLine($"  개수  = {count}");
Console.WriteLine();

// ── 예제 3 — MySet Foldable (Functor 는 부착 안 함) ─────────────────
Console.WriteLine("== 예제 3 — MySet 은 Foldable (Functor 는 경계 사례) ==");

K<SetF, int> set = new MySet<int>([1, 2, 2, 3, 3, 3]);   // 중복 제거됨
var setSum   = set.FoldLeft((acc, x) => acc + x, 0);
var setCount = set.Count();
var hasEven  = set.Any(x => x % 2 == 0);

Console.WriteLine($"  집합        = {set.As()}");
Console.WriteLine($"  합계        = {setSum}");
Console.WriteLine($"  원소 개수   = {setCount}");
Console.WriteLine($"  짝수 있음?  = {hasEven}");
Console.WriteLine("  (Set 에 Map 을 못 붙이는 이유는 Challenges/WhySetIsNotFunctor.md)");
Console.WriteLine();

// ── 예제 4 — 심화: 키 변환은 Functor 가 아니다 (충돌 시연) ───────────
Console.WriteLine("== 예제 4 — 키 변환은 Functor 가 아니다 (충돌) ==");

// 첫 글자로 키를 줄이면 apple/banana/cherry → a/b/c 지만,
// 만약 키가 충돌하면 (예: 길이로 키 변환) 원소가 합쳐진다.
var byLen = KeyMapping.MapKeys(prices.As(), k => k.Length);
Console.WriteLine($"  길이를 키로 = {byLen}   ← apple(5)/cherry(6)... 길이 충돌 시 덮어씀");
Console.WriteLine();

// ── 법칙 검증 ───────────────────────────────────────────────────────
Console.WriteLine("== 법칙 검증 (MyMap Functor) ==");

Func<K<MapF<string>, int>, IEnumerable<int>> probe = fa => fa.As().Values;
var idOk = FunctorLaws.IdentityHolds<MapF<string>, int>(prices, probe);

Func<K<MapF<string>, string>, IEnumerable<string>> probeC = fa => fa.As().Values;
var compOk = FunctorLaws.CompositionHolds<MapF<string>, int, int, string>(
    prices, v => v + 1, v => $"₩{v}", probeC);

Console.WriteLine($"  Functor 항등 : {Pass(idOk)}");
Console.WriteLine($"  Functor 합성 : {Pass(compOk)}");
Console.WriteLine();

Console.WriteLine(idOk && compOk ? "Functor 법칙 통과 [OK]" : "법칙 위반 발생 [FAIL]");
Console.WriteLine();

// ── 예제 5 — Map Traverse: 모든 값이 통과해야 전체 성공 ──────────────
Console.WriteLine("== 예제 5 — Map Traverse: 값 검증, 모두 통과해야 Just ==");

Func<int, K<MaybeF, int>> validate = v =>
    v > 0 ? new MyMaybe<int>.Just(v) : MyMaybe<int>.Nothing.Instance;

K<MapF<string>, int> badMap = new MyMap<string, int>(new Dictionary<string, int>
{
    ["apple"] = 1000, ["banana"] = -50, ["cherry"] = 3000,
});

var goodT = Traverse.traverse<MapF<string>, MaybeF, int, int>(validate, prices).As();
var badT  = Traverse.traverse<MapF<string>, MaybeF, int, int>(validate, badMap).As();

Console.WriteLine($"  모두 양수  traverse = {DescribeMap(goodT)}   (키 보존, 전체 Just)");
Console.WriteLine($"  음수 포함  traverse = {DescribeMap(badT)}              (하나 실패 → 전체 Nothing)");
Console.WriteLine();

return;

static string Pass(bool b) => b ? "통과" : "위반";

// traverse 결과는 MyMaybe<K<MapF<string>, int>> — 안쪽 Map 을 As() 로 풀어 출력.
static string DescribeMap(MyMaybe<K<MapF<string>, int>> m) => m switch
{
    MyMaybe<K<MapF<string>, int>>.Just j => $"Just({j.Value.As()})",
    _                                    => "Nothing"
};
