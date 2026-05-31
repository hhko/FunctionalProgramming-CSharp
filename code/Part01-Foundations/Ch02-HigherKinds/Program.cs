using Ch02.Functions;
using Ch02.Traits;
using Ch02.Types;

Console.WriteLine("================================================");
Console.WriteLine("2장 — Higher Kinds (K<F, A> + Functor<F>)");
Console.WriteLine("================================================");
Console.WriteLine();

// ──────────────────────────────────────────────────────────────────────
// Part A — MyList 진화: K<F, A> 부착으로 F 가 시그니처에 살아남음
// ──────────────────────────────────────────────────────────────────────

Console.WriteLine("== A1 — 호출 모양: MyListF.Map(f, xs) ==");

MyList<int>        xs           = new MyList<int>(new[] { 1, 2, 3 });
K<MyListF, string> mappedToText = MyListF.Map<int, string>(n => $"n={n}", xs);
Console.WriteLine($"  xs                          = {xs}");
Console.WriteLine($"  MyListF.Map(n => $\"n={{n}}\")  = {mappedToText.As()}");

Console.WriteLine();
Console.WriteLine("== A2 — 체이닝 시 F 가 끝까지 살아 있음 (모양 보존) ==");

K<MyListF, string> step1 = MyListF.Map<int, string>(n => n.ToString(), xs);
K<MyListF, int>    step2 = MyListF.Map<string, int>(s => s.Length, step1);
Console.WriteLine($"  step1 (int → string)        = {step1.As()}");
Console.WriteLine($"  step2 (string → int)        = {step2.As()}   (\"1\".Length = 1, ...)");
Console.WriteLine($"  → 두 단계 모두 F = MyListF 가 시그니처에 박혀 있음");

Console.WriteLine();

// ──────────────────────────────────────────────────────────────────────
// Part B — 어떤 Functor 든 받는 일반 함수: FunctorOps
// ──────────────────────────────────────────────────────────────────────

Console.WriteLine("== B1 — FunctorOps.Run: 어떤 F 든 받는 일반 함수 ==");

K<MyListF, string> runResult = FunctorOps.Run<MyListF, int, string>(xs, n => $"#{n}");
Console.WriteLine($"  FunctorOps.Run(xs, n => $\"#{{n}}\") = {runResult.As()}");

Console.WriteLine();
Console.WriteLine("== B2 — FunctorOps.ApplyTwice: 두 번 적용 ==");

K<MyListF, string> applied = FunctorOps.ApplyTwice<MyListF, int, string>(
    xs,
    n => n.ToString(),
    s => s + "!");
Console.WriteLine($"  ApplyTwice(xs, ToString, +\"!\") = {applied.As()}");

Console.WriteLine();
Console.WriteLine("→ K<F, A> 마커 + self-bound + static abstract 세 도구가 합쳐져,");
Console.WriteLine("  어떤 컨테이너 F 든 같은 함수 한 줄로 동작 — N×M 비용이 N+M 으로 줄어듦.");
