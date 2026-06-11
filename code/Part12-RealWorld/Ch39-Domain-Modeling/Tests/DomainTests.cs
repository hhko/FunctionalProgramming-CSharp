using Ch39.Types;

namespace Ch39.Tests;

public static class DomainTests
{
    // ① 모두 유효 → Valid(User).
    public static bool ValidRegistration() =>
        Registration.Register("alice", "alice@example.com", 30) is Validation<string, User>.Valid;

    // ② 여러 오류 → *모두* 누적 (applicative, 첫 오류에서 멈추지 않음).
    public static bool AccumulatesAllErrors() =>
        Registration.Register("ab", "no-at-sign", 200) is Validation<string, User>.Invalid { Errors.Count: 3 };

    // ③ 일부만 오류 → 그 오류들만.
    public static bool PartialErrors() =>
        Registration.Register("alice", "bad", 30) is Validation<string, User>.Invalid { Errors.Count: 1 };
}
