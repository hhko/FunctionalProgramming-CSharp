using Ch10.Traits;
using Ch10.Types;
using Ch10.Tests;

Console.WriteLine("=== Ch10 — Bifunctor ===\n");

// Pair — 두 값 모두 변환
var p = new Pair<int, string>(3, "hi");
var p2 = PairF.BiMap<int, string, int, string>(n => n + 1, s => s.ToUpper(), p);
Console.WriteLine($"BiMap on Pair(3, \"hi\")        = {p2}");

// Pair — 둘째 자리만 (Functor 의 map 에 해당)
var p3 = PairF.MapSecond<int, string, int>(s => s.Length, p);
Console.WriteLine($"MapSecond on Pair(3, \"hi\")    = {p3}");

// Either Left — 첫 함수만 적용
Either<string, int> err = new Left<string, int>("error");
var err2 = EitherF.BiMap<string, int, string, int>(s => s.ToUpper(), n => n * 2, err);
Console.WriteLine($"BiMap on Left(\"error\")        = {err2}");

// Either Right — 둘째 함수만 적용
Either<string, int> ok = new Right<string, int>(42);
var ok2 = EitherF.BiMap<string, int, string, int>(s => s.ToUpper(), n => n * 2, ok);
Console.WriteLine($"BiMap on Right(42)             = {ok2}");

Console.WriteLine("\n=== 두 법칙 검증 ===\n");

Console.WriteLine($"Pair 항등 법칙        : {BifunctorLaws.IdentityHolds<PairF, int, string>(p)}");
Console.WriteLine($"Either 항등 법칙 (L)  : {BifunctorLaws.IdentityHolds<EitherF, string, int>(err)}");
Console.WriteLine($"Either 항등 법칙 (R)  : {BifunctorLaws.IdentityHolds<EitherF, string, int>(ok)}");

Console.WriteLine($"Pair 합성 법칙        : {BifunctorLaws.CompositionHolds<PairF, int, string, int, string, int, string>(n => n + 1, n => n * 2, s => s + "!", s => s.ToUpper(), p)}");
