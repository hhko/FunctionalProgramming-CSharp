using Ch39.Challenges;
using Ch39.Tests;
using Ch39.Types;

Console.WriteLine("================================================");
Console.WriteLine("36장 — 도메인 모델링 & 검증 파이프라인");
Console.WriteLine("================================================");
Console.WriteLine();

static string Show(Validation<string, User> v) => v switch
{
    Validation<string, User>.Valid ok => $"Valid({ok.Value.Name.Value}, {ok.Value.Email.Value}, {ok.Value.Age.Value})",
    Validation<string, User>.Invalid bad => $"Invalid([{string.Join(" / ", bad.Errors)}])",
    _ => "?"
};

Console.WriteLine("== 회원가입 검증 (applicative 누적) ==");
Console.WriteLine($"  유효 입력      → {Show(Registration.Register("alice", "alice@example.com", 30))}");
Console.WriteLine($"  3개 모두 오류  → {Show(Registration.Register("ab", "no-at", 200))}");
Console.WriteLine($"  email 만 오류  → {Show(Registration.Register("alice", "bad", 30))}");
Console.WriteLine("  → 첫 오류에서 멈추지 않고 *모든* 오류를 모은다 (monadic 단락과 대비).");
Console.WriteLine();

Console.WriteLine("== 주문 검증 (심화) ==");
var order = OrderValidation.Validate(0, -5, 2);
Console.WriteLine($"  잘못된 주문 → {(order is Validation<string, Order>.Invalid i ? $"Invalid([{string.Join(" / ", i.Errors)}])" : "Valid")}");
Console.WriteLine();

Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("유효 등록", DomainTests.ValidRegistration()),
    ("모든 오류 누적(3)", DomainTests.AccumulatesAllErrors()),
    ("부분 오류(1)", DomainTests.PartialErrors()),
};
foreach (var (n, ok) in checks) Console.WriteLine($"  {n} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
