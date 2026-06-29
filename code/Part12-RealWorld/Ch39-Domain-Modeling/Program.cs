using Ch39.Challenges;
using Ch39.Tests;
using Ch39.Types;

Console.WriteLine("================================================");
Console.WriteLine("39장 — 도메인 모델링 & 검증 파이프라인 (계층 값 객체)");
Console.WriteLine("================================================");
Console.WriteLine();

static string Show(Validation<string, Customer> v) => v switch
{
    Validation<string, Customer>.Valid ok =>
        $"Valid({ok.Value.Name.Value}, {ok.Value.Contact.Email.Value}, {ok.Value.Address.City.Value} {ok.Value.Address.Postal.Value})",
    Validation<string, Customer>.Invalid bad => $"Invalid([{string.Join(" / ", bad.Errors)}])",
    _ => "?"
};

Console.WriteLine("== 계층 도메인 등록 (계층-교차 누적) ==");
Console.WriteLine($"  유효 입력        → {Show(Customer.Create("alice", "alice@example.com", "010-1234-5678", "1 Main St", "Seoul", "04524"))}");
Console.WriteLine($"  계층-교차 3 오류 → {Show(Customer.Create("ab", "no-at", "010-1234-5678", "1 Main St", "Seoul", "bad"))}");
Console.WriteLine($"  email 만 오류    → {Show(Customer.Create("alice", "bad", "010-1234-5678", "1 Main St", "Seoul", "04524"))}");
Console.WriteLine("  → 잘못된 username + email + postal 이 서로 다른 계층에 있어도 한 번에 모인다.");
Console.WriteLine();

Console.WriteLine("== 한 복합 값 객체 안에서의 누적 ==");
var ci = ContactInfo.Create("no-at", "abc");
Console.WriteLine($"  email·phone 둘 다 오류 → {(ci is Validation<string, ContactInfo>.Invalid i ? $"Invalid([{string.Join(" / ", i.Errors)}])" : "Valid")}");
Console.WriteLine();

Console.WriteLine("== 주문 검증 (심화) ==");
var order = Order.Create(0, -5, 2);
Console.WriteLine($"  잘못된 주문 → {(order is Validation<string, Order>.Invalid o ? $"Invalid([{string.Join(" / ", o.Errors)}])" : "Valid")}");
Console.WriteLine();

Console.WriteLine("== 검증 ==");
var checks = new (string, bool)[]
{
    ("유효 등록", DomainTests.ValidRegistration()),
    ("계층-교차 누적(3)", DomainTests.AccumulatesAcrossLevels()),
    ("부분 오류(1)", DomainTests.PartialErrors()),
    ("복합 안 누적(2)", DomainTests.LeafErrorsAccumulateWithinComposite()),
};
foreach (var (n, ok) in checks) Console.WriteLine($"  {n} : {(ok ? "통과" : "위반")}");
Console.WriteLine();
Console.WriteLine(checks.All(c => c.Item2) ? "모든 검증 통과 [OK]" : "검증 실패 [FAIL]");
