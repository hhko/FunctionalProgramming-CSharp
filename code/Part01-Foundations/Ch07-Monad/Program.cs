using Ch07.Functions;
using Ch07.Traits;
using Ch07.Types;

Console.WriteLine("================================================");
Console.WriteLine("6장 — Monad + LINQ");
Console.WriteLine("================================================");
Console.WriteLine();

// 파싱 함수 — 정수면 Just, 아니면 Nothing.
static K<MyMaybeF, int> ParseInt(string s) =>
    int.TryParse(s, out var n)
        ? MyMaybeF.Pure(n)
        : MyMaybe<int>.Nothing.Instance;

Console.WriteLine("== 예제 1 — Bind 명시 호출 ==");

K<MyMaybeF, int> byBind = ParseInt("3").Bind(a =>
    ParseInt("4").Bind<MyMaybeF, int, int>(b =>
        MyMaybeF.Pure(a + b)));
Console.WriteLine($"  ParseInt(\"3\") + ParseInt(\"4\") = {Show(byBind)}");

Console.WriteLine();
Console.WriteLine("== 예제 2 — LINQ from-select (같은 결과) ==");

K<MyMaybeF, int> byLinq =
    from a in ParseInt("3")
    from b in ParseInt("4")
    select a + b;
Console.WriteLine($"  from a in 3 from b in 4 select a+b = {Show(byLinq)}");

Console.WriteLine();
Console.WriteLine("== 예제 3 — 단락 회로 (한 단계 실패) ==");

K<MyMaybeF, int> shortCircuit =
    from a in ParseInt("3")
    from b in ParseInt("xyz")             // ← Nothing 반환
    from c in ParseInt("5")               // ← 실행도 안 됨
    select a + b + c;
Console.WriteLine($"  중간에 \"xyz\" → {Show(shortCircuit)}    (b 이후는 평가 안 됨)");

Console.WriteLine();
Console.WriteLine("== 예제 4 — 다단계 LINQ ==");

K<MyMaybeF, int> threeStep =
    from a in ParseInt("10")
    from b in ParseInt("20")
    from c in ParseInt("30")
    select a + b + c;
Console.WriteLine($"  10 + 20 + 30 = {Show(threeStep)}");

static string Show(K<MyMaybeF, int> v) =>
    v.As() switch
    {
        MyMaybe<int>.Just j => $"Just({j.Value})",
        MyMaybe<int>.Nothing => "Nothing",
        _ => "?"
    };
