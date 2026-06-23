using Ch11.Challenges;
using Ch11.Functions;
using Ch11.Tests;
using Ch11.Types;

Console.WriteLine("=== Ch11 — NaturalTransformation ===\n");

// MyList → MyMaybe (headOrNone)
var xs = MyList<int>.Of(1, 2, 3);
var maybe = (MyMaybe<int>)ListToMaybe.Transform<int>(xs);
Console.WriteLine($"ListToMaybe([1, 2, 3])  = {maybe}");

var empty = MyList<int>.Empty;
var maybeEmpty = (MyMaybe<int>)ListToMaybe.Transform<int>(empty);
Console.WriteLine($"ListToMaybe([])         = {maybeEmpty}");

// MyMaybe → MyList
var just = MyMaybe<int>.Of(7);
var list = (MyList<int>)MaybeToList.Transform<int>(just);
Console.WriteLine($"MaybeToList(Just 7)     = [{string.Join(", ", list.Items)}]");

var nothing = MyMaybe<int>.Nothing;
var listEmpty = (MyList<int>)MaybeToList.Transform<int>(nothing);
Console.WriteLine($"MaybeToList(Nothing)    = [{string.Join(", ", listEmpty.Items)}]");

Console.WriteLine("\n=== 자연성 법칙 검증 (구체 헬퍼) ===\n");
Console.WriteLine($"ListToMaybe 자연성 ([1,2,3], x*2) : {NaturalityLaws.ListToMaybeNaturality<int, int>(xs, x => x * 2)}");
Console.WriteLine($"ListToMaybe 자연성 ([],      x*2) : {NaturalityLaws.ListToMaybeNaturality<int, int>(empty, x => x * 2)}");
Console.WriteLine($"MaybeToList 자연성 (Just 7,  x+1) : {NaturalityLaws.MaybeToListNaturality<int, int>(just, x => x + 1)}");
Console.WriteLine($"MaybeToList 자연성 (Nothing, x+1) : {NaturalityLaws.MaybeToListNaturality<int, int>(nothing, x => x + 1)}");

Console.WriteLine("\n=== 자연성 법칙 검증 (일반 trait 디스패치) ===\n");

// 일반 NaturalityHolds 로 같은 법칙을 trait 인자로 검증.
// mapF / mapG 는 각 컨테이너의 map 을 K<...> 어댑터로 감쌈.
var generalHolds = NaturalityLaws.NaturalityHolds<ListToMaybe, MyListF, MyMaybeF, int, int>(
    xs,
    x => x * 2,
    (fa, f) => NaturalityLaws.ListMap((MyList<int>)fa, f),    // F = MyList 의 map
    (ga, f) => NaturalityLaws.MaybeMap((MyMaybe<int>)ga, f),  // G = MyMaybe 의 map
    g => ((MyMaybe<int>)g) is Just<int> j ? [j.Val] : []);    // probe — Just 면 값, Nothing 이면 빈 시퀀스
Console.WriteLine($"NaturalityHolds<ListToMaybe> ([1,2,3], x*2) : {generalHolds}");

Console.WriteLine("\n=== 자연성 법칙 검증 (임의 입력 100 건, ForAll) ===\n");

// 3장 3.10.6절의 ForAll 을 그대로 재사용한다. 표본 함수 f = x => x + 1 은 고정하고
// 컨테이너 입력만 변주해, 자연성 법칙이 특정 값이 아니라 *임의의 컨테이너* 에서 성립함을 검사한다.
var randomList = Property.ForAll(
    RandomList,                                          // 임의 길이 0~5, 원소는 임의 int
    xs => NaturalityLaws.ListToMaybeNaturality(xs, x => x + 1));
var randomMaybe = Property.ForAll(
    RandomMaybe,                                         // 임의로 Just(int) 또는 Nothing
    m => NaturalityLaws.MaybeToListNaturality(m, x => x + 1));
Console.WriteLine($"ListToMaybe 자연성 (임의 List 100 건) : {(randomList ? "통과" : "위반")}");
Console.WriteLine($"MaybeToList 자연성 (임의 Maybe 100 건) : {(randomMaybe ? "통과" : "위반")}");

// 임의 입력 생성기 — 컨테이너 구조만 변주한다 (값은 단순 int).
static MyList<int> RandomList(Random r)
{
    var n = r.Next(0, 6);
    var items = new int[n];
    for (var i = 0; i < n; i++) items[i] = r.Next(-1000, 1000);
    return new MyList<int>(items);
}
static MyMaybe<int> RandomMaybe(Random r) =>
    r.Next(0, 2) == 0 ? MyMaybe<int>.Nothing : MyMaybe<int>.Of(r.Next(-1000, 1000));

Console.WriteLine("\n=== 챌린지 (§11.7) ===\n");

var (chHead, chSingleton) = NaturalChallenges.ContainerOnly();
Console.WriteLine($"챌린지 1  ListToMaybe([1,2,3]) = {chHead}    // 값 1 그대로, 컨테이너만 Maybe");
Console.WriteLine($"챌린지 1  MaybeToList(Just 7)  = [{string.Join(", ", chSingleton.Items)}]    // 값 7 그대로, 컨테이너만 List");
Console.WriteLine($"챌린지 2  진짜 ListToMaybe 자연성 성립? : {NaturalChallenges.RealKeepsNaturality()}");
Console.WriteLine($"챌린지 3  가짜 BogusEvenHead 자연성 위반? : {NaturalChallenges.BogusBreaksNaturality()}");

Console.WriteLine("\n=== 가짜 자연 변환 반례 (§11.5.1) ===\n");

var (bogusLhs, bogusRhs) = NaturalityCounterexample.NaturalityPaths(MyList<int>.Of(1, 2, 3));
Console.WriteLine($"먼저 map 그다음 가짜 Transform : {bogusLhs}    // [2,3,4] 의 첫 원소 2 → Just(2)");
Console.WriteLine($"먼저 가짜 Transform 그다음 map : {bogusRhs}    // [1,2,3] 의 첫 원소 1 → Nothing");
Console.WriteLine($"두 경로가 갈림 (자연성 위반)?  : {NaturalityCounterexample.BreaksNaturality(MyList<int>.Of(1, 2, 3))}");

Console.WriteLine("\n=== §11.6 — sequence 는 NaturalTransformation 이다 ===\n");

// sequence : MyList<MyMaybe<A>> → MyMaybe<MyList<A>> 를 Natural<F, G> 인스턴스로 실행.
// F = MyList∘MyMaybe, G = MyMaybe∘MyList — 값은 그대로, 두 층의 안팎만 뒤집힌다.
var lm1 = new ListOfMaybe<int>(MyList<MyMaybe<int>>.Of(MyMaybe<int>.Of(1), MyMaybe<int>.Of(2)));
Console.WriteLine($"sequence([Just 1, Just 2])   = {ShowMOL((MaybeOfList<int>)SequenceNat.Transform<int>(lm1))}    // 안팎이 뒤집힘");
var lm2 = new ListOfMaybe<int>(MyList<MyMaybe<int>>.Of(MyMaybe<int>.Of(1), MyMaybe<int>.Nothing));
Console.WriteLine($"sequence([Just 1, Nothing])  = {ShowMOL((MaybeOfList<int>)SequenceNat.Transform<int>(lm2))}    // 하나라도 Nothing 이면 전체 Nothing");

static string ShowMOL(MaybeOfList<int> v) =>
    v.Value is Just<MyList<int>> j ? $"Just([{string.Join(", ", j.Val.Items)}])" : "Nothing";
