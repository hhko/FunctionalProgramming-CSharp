using Ch08.Domain;
using Ch08.Traits;
using Ch08.Types;

namespace Ch08.Functions;

// 작은 검증자 — *한 함수가 한 책임*. 각 함수가 *Validated<DomainError, X>* 반환.
public static class Validators
{
    public static K<MyValidationF<DomainError>, Email> Email(string raw) =>
        raw.Contains('@') && raw.Length <= 254
            ? MyValidationF<DomainError>.Pure(new Email(raw))
            : new MyValidation<DomainError, Email>.Invalid([new DomainError("email", "@ 필수 + 254 자 이하")]);

    public static K<MyValidationF<DomainError>, Password> Password(string raw) =>
        raw.Length >= 8
            ? MyValidationF<DomainError>.Pure(new Password(raw))
            : new MyValidation<DomainError, Password>.Invalid([new DomainError("password", "8 자 이상")]);

    public static K<MyValidationF<DomainError>, Age> Age(int raw) =>
        raw is >= 14 and <= 120
            ? MyValidationF<DomainError>.Pure(new Age(raw))
            : new MyValidation<DomainError, Age>.Invalid([new DomainError("age", "14-120 범위")]);

    public static K<MyValidationF<DomainError>, Tier> Tier(string raw) =>
        Enum.TryParse<Tier>(raw, ignoreCase: true, out var tier)
            ? MyValidationF<DomainError>.Pure(tier)
            : new MyValidation<DomainError, Tier>.Invalid([new DomainError("tier", "Free / Pro / Enterprise 중 하나")]);
}
