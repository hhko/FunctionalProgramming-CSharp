using Ch39.Types;

namespace Ch39.Tests;

public static class DomainTests
{
    // ① 모두 유효 → Valid(Customer).
    public static bool ValidRegistration() =>
        Customer.Create("alice", "alice@example.com", "010-1234-5678", "1 Main St", "Seoul", "04524")
            is Validation<string, Customer>.Valid;

    // ② 계층을 가로지른 오류가 *모두* 누적 (잘못된 username + email + postal → 3개).
    //    하위 복합 값 객체 안에서 누적된 오류가 상위 Apply 체인으로 그대로 합류한다.
    public static bool AccumulatesAcrossLevels() =>
        Customer.Create("ab", "no-at", "010-1234-5678", "1 Main St", "Seoul", "bad")
            is Validation<string, Customer>.Invalid { Errors.Count: 3 };

    // ③ 일부만 오류 → 그 오류만 (email 한 칸만 틀림).
    public static bool PartialErrors() =>
        Customer.Create("alice", "bad", "010-1234-5678", "1 Main St", "Seoul", "04524")
            is Validation<string, Customer>.Invalid { Errors.Count: 1 };

    // ④ 한 복합 값 객체 안에서 두 잎이 모두 틀리면 그 계층에서 2개가 먼저 모인다.
    public static bool LeafErrorsAccumulateWithinComposite() =>
        ContactInfo.Create("no-at", "abc")
            is Validation<string, ContactInfo>.Invalid { Errors.Count: 2 };
}
