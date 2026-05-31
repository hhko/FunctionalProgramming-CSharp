namespace Ch01;

// 두 세계 사이를 오가는 4 가지 함수.
//
// 네 유형 (a→b / a→E<b> / E<a>→b / E<a>→E<b>) 의 자리를 코드로 보여 준다.
// 각 유형이 어느 장에서 본격 다뤄지는지 주석으로 표시.
public static class FourFunctionTypes
{
    public static void Run()
    {
        Console.WriteLine("== 두 세계 사이를 오가는 4 가지 함수 ==");

        // a → b — 평범한 합성. 어법 일치 → 직접 합성.
        Console.WriteLine();
        Console.WriteLine("  [a → b — 평범한 함수, 함수형 추상 *불필요*]");
        int Plus1(int n) => n + 1;
        string ToText(int n) => n.ToString();
        Console.WriteLine($"    ToText(Plus1(5)) = \"{ToText(Plus1(5))}\"   (Plus1 의 출력과 ToText 의 입력이 모두 int → 합성 OK)");

        // a → E<b> — 월드 교차 함수. 직접 합성 안 됨.
        Console.WriteLine();
        Console.WriteLine("  [a → E<b> — 월드 교차 함수 (7장 Monad / bind 의 자리)]");
        Console.WriteLine($"    Parse(\"42\") = {Parse("42")}");
        Console.WriteLine($"    Parse(\"xx\") = {Parse("xx")}");
        Console.WriteLine($"    FindUser(7) = {FindUser(7)}");
        Console.WriteLine($"    FindUser(0) = {FindUser(0)}");
        Console.WriteLine("    문제: Parse 의 출력 (Option<int>) 과 FindUser 의 입력 (int) 의 *어법이 다르다*.");
        Console.WriteLine("    → 직접 합성 불가능. 7장의 `bind` 가 a → E<b> 를 E<a> → E<b> 로 끌어올려 합성을 회복.");

        // E<a> → b — 끌어내림.
        Console.WriteLine();
        Console.WriteLine("  [E<a> → b — 끌어내림 (6장 Foldable / fold 의 자리)]");
        var xs = new List<int> { 1, 2, 3, 4, 5 };
        Console.WriteLine($"    Sum([1..5])  = {xs.Sum()}");
        Console.WriteLine($"    Count([..])  = {xs.Count}");
        Console.WriteLine($"    All(>0)      = {xs.All(x => x > 0)}");
        Console.WriteLine("    → 컨테이너 안의 구조를 *normal world 의 한 값* 으로 축소.");

        // E<a> → E<b> — 완전히 끌어올린 함수. 모양 보존.
        Console.WriteLine();
        Console.WriteLine("  [E<a> → E<b> — 완전히 끌어올린 함수 (4장 Functor / map 의 자리)]");
        Option<string> toText  = Parse("42").Select(n => n.ToString());
        List<int>      doubled = xs.Select(x => x * 2).ToList();
        Console.WriteLine($"    Parse(\"42\").Select(n=>n.ToString()) = {toText}");
        Console.WriteLine($"    [1..5].Select(x=>x*2)               = [{string.Join(", ", doubled)}]");
        Console.WriteLine("    → 컨테이너 모양은 보존, 안의 값만 변환. 두 E<a> → E<b> 는 평범한 합성으로 이어 붙는다.");
    }

    // "이 문자열을 int 로 파싱" — 실패할 수 있다 → 출력은 Elevated.
    private static Option<int> Parse(string s) =>
        int.TryParse(s, out var n) ? Option.Some(n) : Option.None<int>();

    // "이 id 로 사용자 조회" — 없을 수 있다 → 출력은 Elevated.
    private static Option<User> FindUser(int id) =>
        id > 0 ? Option.Some(new User(id, $"user-{id}")) : Option.None<User>();

    private sealed record User(int Id, string Name);
}
