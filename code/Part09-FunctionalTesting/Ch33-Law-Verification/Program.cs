using Ch33.Challenges;
using Ch33.Tests;
using Ch33.Traits;
using Ch33.Types;

Console.WriteLine("================================================");
Console.WriteLine("33장 — 합법칙 검증 (재사용 가능한 법칙 모듈)");
Console.WriteLine("================================================");
Console.WriteLine();

K<MyListF, int> nums = new MyList<int>([1, 2, 3, 4]);
Func<K<MyListF, int>, IEnumerable<int>> probeI = x => x.As().Items;
Func<K<MyListF, string>, IEnumerable<string>> probeS = x => x.As().Items;
Func<int, K<MyListF, int>> f = n => new MyList<int>([n, n + 1]);
Func<int, K<MyListF, int>> g = n => new MyList<int>([n * 10]);

Console.WriteLine("== MyListF 에 대해 다섯 법칙 검사 (한 모듈로) ==");
var results = new (string, bool)[]
{
    ("Functor 항등",   Laws.FunctorIdentity<MyListF, int>(nums, probeI)),
    ("Functor 합성",   Laws.FunctorComposition<MyListF, int, int, string>(nums, x => x + 1, x => $"#{x}", probeS)),
    ("Monad 좌 항등",  Laws.MonadLeftIdentity<MyListF, int, int>(5, f, probeI)),
    ("Monad 우 항등",  Laws.MonadRightIdentity<MyListF, int>(nums, probeI)),
    ("Monad 결합",     Laws.MonadAssociativity<MyListF, int, int, int>(nums, f, g, probeI)),
};
foreach (var (name, ok) in results) Console.WriteLine($"  {name} : {(ok ? "통과" : "위반")}");
Console.WriteLine();

Console.WriteLine("== 위반 인스턴스(BogusListF)도 잡아내는가 ==");
var caught = BogusCheck.CatchesViolation();
Console.WriteLine($"  BogusListF 항등 법칙 위반을 잡음? {(caught ? "예 (검증이 틀린 구현을 거름)" : "아니오")}");
Console.WriteLine();

var allGood = results.All(r => r.Item2) && caught;
Console.WriteLine(allGood ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
