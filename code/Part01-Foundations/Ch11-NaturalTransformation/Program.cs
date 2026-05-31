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

Console.WriteLine("\n=== 자연성 법칙 검증 ===\n");
Console.WriteLine($"ListToMaybe 자연성 ([1,2,3], x*2) : {NaturalityLaws.ListToMaybeNaturality<int, int>(xs, x => x * 2)}");
Console.WriteLine($"ListToMaybe 자연성 ([],      x*2) : {NaturalityLaws.ListToMaybeNaturality<int, int>(empty, x => x * 2)}");
Console.WriteLine($"MaybeToList 자연성 (Just 7,  x+1) : {NaturalityLaws.MaybeToListNaturality<int, int>(just, x => x + 1)}");
Console.WriteLine($"MaybeToList 자연성 (Nothing, x+1) : {NaturalityLaws.MaybeToListNaturality<int, int>(nothing, x => x + 1)}");
