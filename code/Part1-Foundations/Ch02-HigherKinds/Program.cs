using Ch02.Functions;
using Ch02.Traits;
using Ch02.Types;

Console.WriteLine("================================================");
Console.WriteLine("2장 — Higher Kinds");
Console.WriteLine("================================================");
Console.WriteLine();

// ──────────────────────────────────────────────────────────────
// Part A — Addable / Monoid 의 3-tuple (가장 단순한 형태)
// ──────────────────────────────────────────────────────────────

Console.WriteLine("== A1 — 같은 일반 함수가 두 모노이드를 처리 ==");

var sumResult = MAppend.Run<Sum>([new Sum(1), new Sum(2), new Sum(3), new Sum(4)]);
Console.WriteLine($"  Sum(1..4)        = {sumResult}      (1+2+3+4)");

var productResult = MAppend.Run<Product>([new Product(1), new Product(2), new Product(3), new Product(4)]);
Console.WriteLine($"  Product(1..4)    = {productResult}  (1*2*3*4)");

Console.WriteLine();
Console.WriteLine("== A2 — 빈 시퀀스도 안전 (Empty 의 역할) ==");

var emptySum = MAppend.Run<Sum>([]);
Console.WriteLine($"  Sum(빈)          = {emptySum}      (Empty = 0)");

var emptyProduct = MAppend.Run<Product>([]);
Console.WriteLine($"  Product(빈)      = {emptyProduct}  (Empty = 1)");

Console.WriteLine();
Console.WriteLine("== A3 — 같은 자료 타입 (int) 위 두 모노이드 구조 ==");

int[] xs = [2, 3, 5];
var asSum = MAppend.Run<Sum>(xs.Select(n => new Sum(n)));
var asProduct = MAppend.Run<Product>(xs.Select(n => new Product(n)));
Console.WriteLine($"  [2, 3, 5] as Sum     = {asSum}     (2+3+5)");
Console.WriteLine($"  [2, 3, 5] as Product = {asProduct} (2*3*5)");

Console.WriteLine();

// ──────────────────────────────────────────────────────────────
// Part B — K<F, A> + Functor<F> 의 3-tuple (본문 §2.5)
// ──────────────────────────────────────────────────────────────

Console.WriteLine("== B1 — §2.5.3 호출 모양: MyListF.Map(f, xs) ==");

MyList<int>        listOfInt    = new MyList<int>([1, 2, 3]);
K<MyListF, string> mappedToText = MyListF.Map<int, string>(n => $"n={n}", listOfInt);
Console.WriteLine($"  xs                       = {listOfInt}");
Console.WriteLine($"  MyListF.Map(n=>$\"n={{n}}\")  = {mappedToText.As()}");

Console.WriteLine();
Console.WriteLine("== B2 — §2.5.5 체이닝 시 F 가 끝까지 살아 있다 ==");

K<MyListF, string> step1 = MyListF.Map<int, string>(n => n.ToString(), listOfInt);
K<MyListF, int>    step2 = MyListF.Map<string, int>(s => s.Length, step1);
Console.WriteLine($"  step1 (int → string)  = {step1.As()}");
Console.WriteLine($"  step2 (string → int)  = {step2.As()}     (\"1\".Length = 1, …)");
Console.WriteLine($"  체이닝 끝에도 F = MyListF 가 시그니처에 박혀 있다");

Console.WriteLine();
Console.WriteLine("== B3 — §2.8 어떤 Functor 든 받는 일반 함수: FunctorOps.Run ==");

K<MyListF, string> runResult = FunctorOps.Run<MyListF, int, string>(listOfInt, n => $"#{n}");
Console.WriteLine($"  FunctorOps.Run(xs, n=>$\"#{{n}}\") = {runResult.As()}");

Console.WriteLine();
Console.WriteLine("== B4 — §2.5.6 일반 함수의 두 번 적용: FunctorOps.ApplyTwice ==");

K<MyListF, string> applied = FunctorOps.ApplyTwice<MyListF, int, string>(
    listOfInt,
    n => n.ToString(),
    s => s + "!");
Console.WriteLine($"  ApplyTwice(xs, ToString, +\"!\") = {applied.As()}");
