using Ch07.Domain;
using Ch07.Traits;
using Ch07.Types;

namespace Ch07.Functions;

// StyleComparison — 같은 입력에 *applicative style* 과 *monadic style* 을 모두 적용해
// 결과를 나란히 보여 준다.
//
//   * applicative style — Apply 사슬. 모든 검증을 *독립* 으로 수행해 *오류 누적*.
//                          이미 SignUpForm.Submit 이 이 방식이다.
//   * monadic style    — 첫 검증의 결과로 *다음 검증을 결정* 하는 결합. 첫 오류에서
//                          *단락 (short-circuit)* — 나머지 검증은 *실행조차 안 한다*.
//
// MyValidation 은 의도적으로 Bind 를 정의하지 않는다 (5장 의 약속). 그래서 monadic
// 시뮬레이션은 *switch expression 으로 직접 단락* 시킨다. 의미는 정확히 같다.
public static class StyleComparison
{
    public sealed record Result(string Style, K<MyValidationF<DomainError>, User> Value);

    public static (Result Applicative, Result Monadic) Compare(
        string emailRaw, string passwordRaw, int ageRaw, string tierRaw)
    {
        var ap = SubmitApplicative(emailRaw, passwordRaw, ageRaw, tierRaw);
        var mo = SubmitMonadic(emailRaw, passwordRaw, ageRaw, tierRaw);
        return (new Result("applicative (누적)", ap), new Result("monadic (단락)", mo));
    }

    // applicative style — SignUpForm.Submit 그대로. 4 개 모두 평가, 오류 누적.
    public static K<MyValidationF<DomainError>, User> SubmitApplicative(
        string emailRaw, string passwordRaw, int ageRaw, string tierRaw) =>
        SignUpForm.Submit(emailRaw, passwordRaw, ageRaw, tierRaw);

    // monadic style — 차례로 평가하다 첫 Invalid 에서 단락. 이후 검증은 실행조차 안 함.
    //
    // 이 함수가 보여주는 것: *둘 다 elevated world 안에서 의미를 정의할 수 있다*. 그런데
    // *어느 의미가 도메인에 맞는가* 는 별개. 회원가입 폼은 *모든 오류를 한 번에 보여주는*
    // applicative 가 더 친절하다. *DB 조회 사슬* (사용자 조회 → 권한 조회 → 데이터 조회)
    // 같은 의존 연속은 monadic 이 자연스럽다.
    public static K<MyValidationF<DomainError>, User> SubmitMonadic(
        string emailRaw, string passwordRaw, int ageRaw, string tierRaw)
    {
        var emailV = Validators.Email(emailRaw);
        if (emailV.As() is MyValidation<DomainError, Email>.Invalid ei)
            return new MyValidation<DomainError, User>.Invalid(ei.Errors);

        var passwordV = Validators.Password(passwordRaw);
        if (passwordV.As() is MyValidation<DomainError, Password>.Invalid pi)
            return new MyValidation<DomainError, User>.Invalid(pi.Errors);

        var ageV = Validators.Age(ageRaw);
        if (ageV.As() is MyValidation<DomainError, Age>.Invalid ai)
            return new MyValidation<DomainError, User>.Invalid(ai.Errors);

        var tierV = Validators.Tier(tierRaw);
        if (tierV.As() is MyValidation<DomainError, Tier>.Invalid ti)
            return new MyValidation<DomainError, User>.Invalid(ti.Errors);

        // 모두 Valid — User 조립
        var email    = ((MyValidation<DomainError, Email>.Valid)emailV.As()).Value;
        var password = ((MyValidation<DomainError, Password>.Valid)passwordV.As()).Value;
        var age      = ((MyValidation<DomainError, Age>.Valid)ageV.As()).Value;
        var tier     = ((MyValidation<DomainError, Tier>.Valid)tierV.As()).Value;
        return MyValidationF<DomainError>.Pure(new User(email, password, age, tier));
    }

    public static string Render(K<MyValidationF<DomainError>, User> v) =>
        v.As() switch
        {
            MyValidation<DomainError, User>.Valid valid     => $"Valid({valid.Value})",
            MyValidation<DomainError, User>.Invalid invalid => $"Invalid([{string.Join(" | ", invalid.Errors)}])",
            _ => "?"
        };
}
