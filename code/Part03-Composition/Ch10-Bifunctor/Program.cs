using Ch10.Traits;
using Ch10.Types;
using Ch10.Tests;
using Ch10.Functions;
using Ch10.Challenges;

Console.WriteLine("=== Ch10 — Bifunctor ===\n");

// Pair — 두 값 모두 변환
var p = new Pair<int, string>(3, "hi");
var p2 = PairF.BiMap<int, string, int, string>(n => n + 1, s => s.ToUpper(), p);
Console.WriteLine($"BiMap on Pair(3, \"hi\")        = {p2}");

// fluent 확장 — 명시 제네릭 없이 점 호출. this 에서 F/L/A, 두 함수에서 M/B 추론.
var pf = p.BiMap(n => n + 1, s => s.ToUpper());
Console.WriteLine($"fab.BiMap(..) fluent on Pair  = {pf}");

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

Console.WriteLine("\n=== Biapplicative — 두 자리 동시 적용 ===\n");

// BiApply — 두 자리의 함수를 두 자리의 값에 동시에 적용. (Biapplicative = Applicative 의 2-인자 판)
var fns  = new Pair<Func<int, int>, Func<string, int>>(n => n + 1, s => s.Length);
var vals = PairF.BiPure(10, "hello");
var applied = PairF.BiApply<int, string, int, int>(fns, vals);
Console.WriteLine($"BiApply(<+1,len>, Pair(10,\"hello\")) = {applied}");

Console.WriteLine("\n=== 두 법칙 검증 ===\n");

Console.WriteLine($"Pair 항등 법칙        : {BifunctorLaws.IdentityHolds<PairF, int, string>(p)}");
Console.WriteLine($"Either 항등 법칙 (L)  : {BifunctorLaws.IdentityHolds<EitherF, string, int>(err)}");
Console.WriteLine($"Either 항등 법칙 (R)  : {BifunctorLaws.IdentityHolds<EitherF, string, int>(ok)}");

Console.WriteLine($"Pair 합성 법칙        : {BifunctorLaws.CompositionHolds<PairF, int, string, int, string, int, string>(n => n + 1, n => n * 2, s => s + "!", s => s.ToUpper(), p)}");

// ── 법칙을 임의 입력으로 (3장 §3.7.1 의 ForAll 재사용) ──
// 양쪽 인자 표본 함수는 고정 (n => n + 1, n => n * 2), 컨테이너 입력만 변주.
// 임의의 Pair · Either 100 건에서 항등·합성이 양쪽 인자 각각에 성립하는지 검사한다.
Console.WriteLine();
Console.WriteLine("=== 두 법칙을 임의 입력 100 건으로 (ForAll) ===\n");

// 생성기 — 컨테이너만 무작위로. (양쪽 자리 모두 int 표본.)
Func<Random, K<PairF, int, int>> genPair =
    r => new Pair<int, int>(r.Next(100), r.Next(100));
Func<Random, K<EitherF, int, int>> genEither =
    r => r.Next(2) == 0 ? new Left<int, int>(r.Next(100)) : new Right<int, int>(r.Next(100));

// 항등 법칙 — 임의 입력 100 건.
var pairIdent = Property.ForAll(genPair, BifunctorLaws.IdentityHolds<PairF, int, int>);
var eitherIdent = Property.ForAll(genEither, BifunctorLaws.IdentityHolds<EitherF, int, int>);

// 합성 법칙 — 표본 함수 고정, 컨테이너만 변주한 100 건.
var pairComp = Property.ForAll(genPair,
    fab => BifunctorLaws.CompositionHolds<PairF, int, int, int, int, int, int>(
        n => n + 1, n => n * 2, n => n + 1, n => n * 2, fab));
var eitherComp = Property.ForAll(genEither,
    fab => BifunctorLaws.CompositionHolds<EitherF, int, int, int, int, int, int>(
        n => n + 1, n => n * 2, n => n + 1, n => n * 2, fab));

Console.WriteLine($"Pair   항등 법칙 (임의 100 건) : {(pairIdent ? "통과" : "위반")}");
Console.WriteLine($"Pair   합성 법칙 (임의 100 건) : {(pairComp ? "통과" : "위반")}");
Console.WriteLine($"Either 항등 법칙 (임의 100 건) : {(eitherIdent ? "통과" : "위반")}");
Console.WriteLine($"Either 합성 법칙 (임의 100 건) : {(eitherComp ? "통과" : "위반")}");

Console.WriteLine("\n=== 챌린지 (§10.8) ===\n");

var (chLeft, chRight) = BifunctorChallenges.MapErrorOnly();
Console.WriteLine($"챌린지 1  MapFirst on Left(\"error\")   = {chLeft}    // 오류 문자열 길이 5");
Console.WriteLine($"챌린지 1  MapFirst on Right(42)        = {chRight}    // 그대로");
var (chBoth, chSecond) = BifunctorChallenges.PairTransforms();
Console.WriteLine($"챌린지 2  BiMap     on Pair(3,\"hi\")    = {chBoth}");
Console.WriteLine($"챌린지 2  MapSecond on Pair(3,\"hi\")    = {chSecond}    // 첫 인자 3 그대로");
Console.WriteLine($"챌린지 3  두 갈래 항등 법칙 모두 성립? : {BifunctorChallenges.AllIdentityLawsHold()}");

Console.WriteLine("\n=== 가짜 Bifunctor 반례 (§10.7.1) ===\n");

var bog = new Pair<int, int>(3, 5);
var (realB, bogusB) = BifunctorCounterexample.IdentityCompare(bog);
Console.WriteLine($"진짜 BiMap(id, id, Pair(3, 5))     = {realB}    // 항등 법칙 통과");
Console.WriteLine($"가짜 BogusBiMap(id, id, Pair(3, 5)) = {bogusB}    // 자리 뒤바뀜 → 위반");
Console.WriteLine($"진짜 통과 & 가짜 위반?             : {BifunctorCounterexample.RealHoldsBogusBreaks(bog)}");

Console.WriteLine("\n=== 또 다른 가짜 — 항등 통과 / 합성 깸 (§10.7.2) ===\n");

var (idR, once, split) = BifunctorCounterexample.CompositionBreaks();
Console.WriteLine($"항등 BogusBiMapTwice(id, id, Pair(3,5)) = {idR}    // 항등 법칙 통과");
Console.WriteLine($"합성 한 번에 (g∘f 두 번)  첫 자리       = {once}    // 18");
Console.WriteLine($"합성 나눠서 (f 두 번 → g 두 번) 첫 자리 = {split}    // 20");
Console.WriteLine($"항등 통과 & 합성 깸?                    : {BifunctorCounterexample.IdentityHoldsCompositionBreaks()}");

Console.WriteLine("\n=== 실전 — 계층 오류 매핑 (BiMap 한 줄, §10.4.1) ===\n");

var okRow = LayerMapping.ToDomain(new Right<DbError, Row>(new Row(1, "kim")));
var dbErr = LayerMapping.ToDomain(new Left<DbError, Row>(new DbError("E42")));
Console.WriteLine($"성공 행   Right(Row(1,\"kim\")) → {okRow}");
Console.WriteLine($"인프라 오류 Left(DbError(E42)) → {dbErr}");
