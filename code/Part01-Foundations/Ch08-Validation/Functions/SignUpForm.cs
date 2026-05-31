using Ch08.Domain;
using Ch08.Traits;
using Ch08.Types;

namespace Ch08.Functions;

// 회원가입 폼 — 4 개 입력 검증 후 User 생성.
//
// Curry → Pure → 4 × Apply 체인.
// 4 개 입력 *모두 잘못* 이어도 *4 개 에러를 모두 누적*.
public static class SignUpForm
{
    public static K<MyValidationF<DomainError>, User> Submit(
        string emailRaw, string passwordRaw, int ageRaw, string tierRaw)
    {
        var emailV    = Validators.Email(emailRaw);
        var passwordV = Validators.Password(passwordRaw);
        var ageV      = Validators.Age(ageRaw);
        var tierV     = Validators.Tier(tierRaw);

        // User 생성자를 *curry*.
        Func<Email, Password, Age, Tier, User> ctor =
            (e, p, a, t) => new User(e, p, a, t);
        var curried = Curry.Of(ctor);

        // Pure → 4 × Apply
        var lifted = MyValidationF<DomainError>.Pure(curried);
        var step1 = MyValidationF<DomainError>.Apply<Email, Func<Password, Func<Age, Func<Tier, User>>>>(lifted, emailV);
        var step2 = MyValidationF<DomainError>.Apply<Password, Func<Age, Func<Tier, User>>>(step1, passwordV);
        var step3 = MyValidationF<DomainError>.Apply<Age, Func<Tier, User>>(step2, ageV);
        var step4 = MyValidationF<DomainError>.Apply<Tier, User>(step3, tierV);

        return step4;
    }
}
