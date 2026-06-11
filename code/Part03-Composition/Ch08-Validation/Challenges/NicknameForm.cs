using Ch08.Domain;
using Ch08.Functions;
using Ch08.Traits;
using Ch08.Types;

namespace Ch08.Challenges;

// 챌린지 2 — 검증자를 하나 더 추가 (닉네임, 2 자 이상).
//
// User 생성자 인자를 다섯으로 늘리고 Curry5 + Apply 사슬을 한 단계 더 잇는다.
// 다섯 칸이 모두 틀리면 오류가 *다섯 건* 누적된다 (Apply 의 Invalid + Invalid 분기가
// 한 단계 늘어도 그대로 이어붙는다).
public readonly record struct Nickname(string Value);

public sealed record UserPlus(Email Email, Password Password, Age Age, Tier Tier, Nickname Nickname);

public static class NicknameForm
{
    // 추가 검증자 — 닉네임 2 자 이상.
    public static K<MyValidationF<DomainError>, Nickname> Nickname(string raw) =>
        raw.Length >= 2
            ? MyValidationF<DomainError>.Pure(new Nickname(raw))
            : new MyValidation<DomainError, Nickname>.Invalid(
                [new DomainError("nickname", "2 자 이상")]);

    public static K<MyValidationF<DomainError>, UserPlus> Submit(
        string emailRaw, string passwordRaw, int ageRaw, string tierRaw, string nicknameRaw)
    {
        var emailV    = Validators.Email(emailRaw);
        var passwordV = Validators.Password(passwordRaw);
        var ageV      = Validators.Age(ageRaw);
        var tierV     = Validators.Tier(tierRaw);
        var nicknameV = Nickname(nicknameRaw);

        // 5 인자 생성자를 curry.
        Func<Email, Password, Age, Tier, Nickname, UserPlus> ctor =
            (e, p, a, t, n) => new UserPlus(e, p, a, t, n);
        var curried = Curry5(ctor);

        // Pure → 5 × Apply (fluent — 명시 제네릭 없이 추론).
        var lifted = MyValidationF<DomainError>.Pure(curried);
        var result = lifted
            .Apply(emailV)
            .Apply(passwordV)
            .Apply(ageV)
            .Apply(tierV)
            .Apply(nicknameV);

        return result;
    }

    // 다섯 칸이 모두 틀리면 오류 *5 건*. (Curry.Of 는 4 인자뿐이라 5 인자 curry 를 여기 둔다.)
    public static int AllFiveInvalidErrorCount()
    {
        var result = Submit("noatsign", "1234", -5, "Premium", "x");
        return result.As() switch
        {
            MyValidation<DomainError, UserPlus>.Invalid e => e.Errors.Count,
            _                                             => 0
        };
    }

    private static Func<A, Func<B, Func<C, Func<D, Func<E, R>>>>> Curry5<A, B, C, D, E, R>(
        Func<A, B, C, D, E, R> f) =>
        a => b => c => d => e => f(a, b, c, d, e);
}
