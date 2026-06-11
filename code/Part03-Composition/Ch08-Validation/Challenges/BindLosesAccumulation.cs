using Ch08.Domain;
using Ch08.Functions;
using Ch08.Traits;
using Ch08.Types;

namespace Ch08.Challenges;

// 챌린지 3 — MyValidation 에 Bind 를 직접 정의하면 *왜 누적이 사라지는가*.
//
// Bind 의 시그니처는 K<F, A> → (A → K<F, B>) → K<F, B> 이다. 둘째 인자가 *함수* 라서,
// 첫 인자가 Invalid 면 그 함수를 *호출할 값 (A) 자체가 없다*. 그래서 Bind 는 첫 Invalid 에서
// 무조건 단락할 수밖에 없다 — 둘째 검증을 *실행조차 못 한다*. 두 Invalid 를 동시에 손에 쥐는
// Apply 와 달리, Bind 는 *첫 오류 한 건* 만 남긴다.
//
// MyValidation 자체에는 Bind 가 없으므로 (8장의 약속) 여기서 직접 정의해 비교한다.
public static class BindLosesAccumulation
{
    // 직접 정의한 Bind — Valid 면 함수 적용, Invalid 면 단락 (둘째 검증 실행 안 함).
    public static K<MyValidationF<E>, B> Bind<E, A, B>(
        K<MyValidationF<E>, A> ma,
        Func<A, K<MyValidationF<E>, B>> f) =>
        ma.As() switch
        {
            MyValidation<E, A>.Valid v   => f(v.Value),                          // 다음 검증으로 진행
            MyValidation<E, A>.Invalid i => new MyValidation<E, B>.Invalid(i.Errors), // 첫 오류에서 단락
            _ => throw new InvalidOperationException()
        };

    // Bind 사슬로 폼 검증 — 첫 Invalid (email) 에서 단락. 나머지 세 검증은 실행조차 안 됨.
    public static K<MyValidationF<DomainError>, User> SubmitWithBind(
        string emailRaw, string passwordRaw, int ageRaw, string tierRaw) =>
        Bind(Validators.Email(emailRaw), email =>
        Bind(Validators.Password(passwordRaw), password =>
        Bind(Validators.Age(ageRaw), age =>
        Bind(Validators.Tier(tierRaw), tier =>
            MyValidationF<DomainError>.Pure(new User(email, password, age, tier))))));

    // 네 칸이 모두 틀린 같은 입력에 Bind 사슬을 적용하면 오류 *1 건* 만 나온다.
    // (같은 입력을 Apply 사슬 = SignUpForm.Submit 으로 풀면 4 건. 누적이 사라진 증거.)
    public static int BindErrorCount()
    {
        var result = SubmitWithBind("noatsign", "1234", -5, "Premium");
        return result.As() switch
        {
            MyValidation<DomainError, User>.Invalid e => e.Errors.Count,
            _                                         => 0
        };
    }

    // Apply (누적) 4 건 vs Bind (단락) 1 건 — 한 자리에서 대비.
    public static (int Apply, int Bind) Compare()
    {
        var apply = SignUpForm.Submit("noatsign", "1234", -5, "Premium");
        int applyCount = apply.As() switch
        {
            MyValidation<DomainError, User>.Invalid e => e.Errors.Count,
            _                                         => 0
        };
        return (applyCount, BindErrorCount());
    }
}
