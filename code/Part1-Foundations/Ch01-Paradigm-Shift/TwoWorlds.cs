namespace Ch01;

// 본문 §1.6 — 비유: 두 평행 세계 (Normal / Elevated World).
//
// 값과 함수가 *두 세계 모두에* 산다. 같은 정보 `int` 가 Normal 의 `int` 로도,
// Elevated 의 `Option<int>` / `List<int>` / `Result<int>` 로도 살 수 있다.
public static class TwoWorlds
{
    public static void Run()
    {
        Console.WriteLine("== §1.6 두 평행 세계 — Normal World vs Elevated World ==");

        // §1.6.1 — Normal World 시민 + 평범한 함수 합성.
        Console.WriteLine("  [§1.6.1 Normal World — 평범한 값과 함수]");
        int n = 42;
        string s = "hello";
        Console.WriteLine($"    Normal 시민:  int n = {n}, string s = \"{s}\"");

        int Plus1(int x) => x + 1;
        string ToText(int x) => x.ToString();
        Console.WriteLine($"    Normal 합성:  ToText(Plus1({n})) = \"{ToText(Plus1(n))}\"   (어법 일치 → 직접 합성 OK)");

        // §1.6.2 — Elevated World 시민 (컨테이너로 끌어올린 값).
        Console.WriteLine();
        Console.WriteLine("  [§1.6.2 Elevated World — 끌어올린 값]");
        Option<int>  maybeN = Option.Some(42);
        List<string> manyS  = ["hello", "world"];
        Result<int>  okN    = Result.Ok(42);
        Console.WriteLine($"    Elevated 시민: Option<int>  maybeN = {maybeN}");
        Console.WriteLine($"                  List<string> manyS  = [{string.Join(", ", manyS.Select(x => $"\"{x}\""))}]");
        Console.WriteLine($"                  Result<int>  okN    = {okN}");
        Console.WriteLine($"                  Result<int>  failN  = {Result.Fail<int>("음수 입력")}");

        // §1.6.3 — 함수도 두 세계에 산다.
        Console.WriteLine();
        Console.WriteLine("  [§1.6.3 함수도 두 세계에 산다]");
        Func<int, int> plus1 = x => x + 1;
        Option<Func<int, int>> maybePlus1 = Option.Some<Func<int, int>>(x => x + 1);
        List<Func<int, int>>   manyOps    = [x => x + 1, x => x * 2];
        Console.WriteLine($"    Normal 함수:   Func<int,int> plus1 — 적용 plus1(10) = {plus1(10)}");
        Console.WriteLine($"    Elevated 함수: Option<Func<int,int>> maybePlus1 — 함수가 컨테이너 *안*");
        Console.WriteLine($"                  List<Func<int,int>> manyOps  — 여러 변환이 컨테이너 안");
        Console.WriteLine("    → 4장 Applicative 의 `apply : E<a→b> → E<a> → E<b>` 가 이 자리의 함수를 쓴다.");
    }
}
