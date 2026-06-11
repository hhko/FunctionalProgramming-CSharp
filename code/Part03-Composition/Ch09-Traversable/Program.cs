using Ch09.Challenges;
using Ch09.Functions;
using Ch09.Tests;
using Ch09.Traits;
using Ch09.Types;

Console.WriteLine("================================================");
Console.WriteLine("9장 — Traversable");
Console.WriteLine("================================================");
Console.WriteLine();

Console.WriteLine("== 예제 1 — Traverse 로 *각 원소가 검증을 통과하는지* ==");

K<MyListF, int> nums = new MyList<int>([2, 4, 6]);

// 짝수면 Just, 홀수면 Nothing 반환하는 함수.
Func<int, K<MyMaybeF, int>> evenCheck = n =>
    n % 2 == 0
        ? MyMaybeF.Pure(n)
        : MyMaybe<int>.Nothing.Instance;

K<MyMaybeF, K<MyListF, int>> result = MyListF.Traverse<MyMaybeF, int, int>(evenCheck, nums);
ShowMaybeOfList(result);

Console.WriteLine();
Console.WriteLine("== 예제 2 — 한 원소가 홀수 — 결과는 Nothing ==");

K<MyListF, int> mixed = new MyList<int>([2, 3, 6]);
K<MyMaybeF, K<MyListF, int>> result2 = MyListF.Traverse<MyMaybeF, int, int>(evenCheck, mixed);
ShowMaybeOfList(result2);

Console.WriteLine();
Console.WriteLine("== 예제 3 — ParseInt 로 문자열 목록 전체 파싱 (본문 §9.1 / §9.3) ==");

// ParseInt : string → MyMaybe<int>. 파싱 성공이면 Just, 실패면 Nothing.
// 본문 §9.1 / §9.3 의 동기 예제 — 효과가 원소마다 흩어진 List<E<a>> 를
// traverse 로 E<List<a>> 로 모은다.
Func<string, K<MyMaybeF, int>> parseInt = s =>
    int.TryParse(s, out var n)
        ? MyMaybeF.Pure(n)
        : MyMaybe<int>.Nothing.Instance;

// 모두 성공 — Just([2, 4, 6]).
K<MyListF, string> rawOk = new MyList<string>(["2", "4", "6"]);
K<MyMaybeF, K<MyListF, int>> parsedOk =
    Traversable.traverse<MyListF, MyMaybeF, string, int>(parseInt, rawOk);
ShowMaybeOfList(parsedOk);

// 한 원소가 파싱 실패 — 전체 Nothing (단락).
K<MyListF, string> rawBad = new MyList<string>(["2", "x", "6"]);
K<MyMaybeF, K<MyListF, int>> parsedBad =
    Traversable.traverse<MyListF, MyMaybeF, string, int>(parseInt, rawBad);
ShowMaybeOfList(parsedBad);

Console.WriteLine();
Console.WriteLine("== 예제 4 — 법칙 검증 (TraversableLaws) ==");

K<MyListF, int> lawList = new MyList<int>([1, 2, 3]);

// 항등 법칙 — Traverse(Pure, t) 가 t 를 보존.
bool identityHolds = TraversableLaws.IdentityHolds<MyListF, MyMaybeF, int>(
    lawList, TraversableLaws.ProbeList);
Console.WriteLine($"  항등 법칙 Traverse(Pure, t) ≡ Pure(t) : {(identityHolds ? "통과" : "위반")}");

// sequence = traverse(id) 등식.
K<MyListF, K<MyMaybeF, int>> tfa = new MyList<K<MyMaybeF, int>>(
    [new MyMaybe<int>.Just(1), new MyMaybe<int>.Just(2), new MyMaybe<int>.Just(3)]);
bool seqEqId = TraversableLaws.SequenceEqualsTraverseId<MyListF, MyMaybeF, int>(
    tfa, TraversableLaws.ProbeList);
Console.WriteLine($"  sequence ≡ traverse(id)              : {(seqEqId ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 4b — 항등 법칙을 임의 입력으로: 최소 property 검증 (ForAll, §9.10) ==");

// 특정 값 [2, 4] 가 아니라 임의 입력 100 건으로 항등 법칙을 검사한다.
// 생성기 — 길이 r.Next(5) 의 임의 int 리스트. 항등 효과 n => Pure(n) 는
// TraversableLaws.IdentityHolds 안에서 a => F.Pure(a) 로 걸린다 (F = MyMaybeF).
var identityRandom = Property.ForAll(
    r => new MyList<int>(Enumerable.Range(0, r.Next(5)).Select(_ => r.Next(-1000, 1000))),
    ta => TraversableLaws.IdentityHolds<MyListF, MyMaybeF, int>(ta, TraversableLaws.ProbeList));
Console.WriteLine($"  항등 법칙 Traverse(Pure, t) ≡ Pure(t) (임의 100 건) : {(identityRandom ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== 예제 5 — 챌린지 (§9.11) ==");

// 챌린지 1 — 단락 추적. [2, 3, 6] 은 홀수 3 때문에 전체 Nothing.
var ch1 = TraversableChallenges.ShortCircuit([2, 3, 6]);
Console.WriteLine($"  챌린지 1  Traverse([2,3,6]) 통과? : {TraversableChallenges.AllPass(ch1)}    // false — 3 에서 단락");

// 챌린지 2 — MyMaybe 는 단락. 셋이 모두 홀수여도 오류 개수 정보 없이 단일 Nothing.
bool ch2 = TraversableChallenges.ShortCircuitLosesAllButFirst([3, 5, 7]);
Console.WriteLine($"  챌린지 2  MyMaybe 단락 (누적 못함) : {ch2}    // MyValidation 이면 오류 3 건 누적 (개념 주석 참고)");

// 챌린지 3 — sequence = traverse(id) 동일 결과.
var (viaSeq, viaTid) = TraversableChallenges.SequenceVsTraverseId([1, 2, 3]);
Console.WriteLine($"  챌린지 3  sequence ≡ traverse(id)  : {TraversableChallenges.SameResult(viaSeq, viaTid)}");

Console.WriteLine();
Console.WriteLine("== 예제 6 — 가짜 Traversable 반례 (§9.10.1) ==");

// 진짜 Traverse 는 입력 순서 보존, 가짜는 Reverse 누락으로 뒤집힘 → 항등 법칙 위반.
var (realOrder, bogusOrder) = TraversableCounterexample.IdentityOrder([1, 2, 3]);
Console.WriteLine($"  진짜 Traverse([1,2,3])      = {new MyList<int>(realOrder)}    // 항등 법칙 통과");
Console.WriteLine($"  가짜 BogusTraverse([1,2,3]) = {new MyList<int>(bogusOrder)}    // 순서 뒤집힘 → 위반");
Console.WriteLine($"  진짜 순서 보존 & 가짜 위반?  : {TraversableCounterexample.RealKeepsOrderBogusBreaks([1, 2, 3])}");

Console.WriteLine();
Console.WriteLine("== 예제 7 — 안쪽 효과를 MyValidation 으로: 누적 (§9.7.1) ==");

// 같은 [3,5,7] — MyMaybe 면 단락(1건), MyValidation 이면 누적(3건). Traverse 코드는 동일.
var vresult = ValidationTraverse.TraverseAllOdd();
switch (vresult)
{
    case MyValidation<string, K<MyListF, int>>.Invalid e:
        Console.WriteLine($"  Traverse([3,5,7], evenV) = Invalid {e.Errors.Count} 건: {string.Join(" / ", e.Errors)}");
        break;
    case MyValidation<string, K<MyListF, int>>.Valid v:
        Console.WriteLine($"  Valid({v.Value.As()})");
        break;
}
Console.WriteLine($"  누적 오류 건수: {ValidationTraverse.AccumulatedErrorCount()}    // 3 — F 만 바꿔 단락→누적");

static void ShowMaybeOfList(K<MyMaybeF, K<MyListF, int>> r)
{
    switch (r.As())
    {
        case MyMaybe<K<MyListF, int>>.Just j:
            Console.WriteLine($"  Just({j.Value.As()})");
            break;
        case MyMaybe<K<MyListF, int>>.Nothing:
            Console.WriteLine($"  Nothing");
            break;
    }
}
