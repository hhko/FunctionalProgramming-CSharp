using Ch08.Domain;
using Ch08.Functions;
using Ch08.Traits;
using Ch08.Types;

namespace Ch08.Challenges;

// 챌린지 1 — Apply 의 네 분기를 손으로 따라가기.
//
// 입력: Submit("noatsign", "12345678", 30, "Pro") — *email 만 Invalid*, 나머지 셋 Valid.
//
// 사슬은 Pure(curried) 로 시작해 Apply 를 네 번 호출한다. 각 단계에서 함수 측 / 값 측의
// 상태가 Apply 의 어느 분기를 타는지 추적한다.
//
//   step1 = lifted.Apply(emailV)     함수 측 Valid(curried), 값 측 Invalid(email)
//                                    → (_, Invalid ae) 분기 → Invalid([email 오류])   *여기서 한 건 담김*
//   step2 = step1.Apply(passwordV)   함수 측 Invalid([email]), 값 측 Valid(password)
//                                    → (Invalid fe, _) 분기 → Invalid([email 오류])   *값 측 무시, 그대로 통과*
//   step3 = step2.Apply(ageV)        함수 측 Invalid([email]), 값 측 Valid(age)
//                                    → (Invalid fe, _) 분기 → Invalid([email 오류])
//   step4 = step3.Apply(tierV)       함수 측 Invalid([email]), 값 측 Valid(tier)
//                                    → (Invalid fe, _) 분기 → Invalid([email 오류])
//
// 결과: Invalid 오류 1 건. email 만 Invalid 이므로 (Invalid + Invalid) 누적 분기를 한 번도
// 타지 않는다. (_, Invalid ae) 분기가 첫 한 건을 담고, 이후는 (Invalid fe, _) 로 그 한 건을
// 그대로 운반한다. *단락이 아니라* 누적 분기를 안 타서 한 건인 점에 주의.
public static class ApplyTrace
{
    // 본문 §8.5 사슬을 단계별로 노출 — Render 로 각 step 의 상태를 확인할 수 있다.
    public static IReadOnlyList<(string Label, string State)> Trace()
    {
        var emailV    = Validators.Email("noatsign");      // Invalid — @ 없음
        var passwordV = Validators.Password("12345678");   // Valid
        var ageV      = Validators.Age(30);                // Valid
        var tierV     = Validators.Tier("Pro");            // Valid

        Func<Email, Password, Age, Tier, User> ctor =
            (e, p, a, t) => new User(e, p, a, t);
        var curried = Curry.Of(ctor);

        var lifted = MyValidationF<DomainError>.Pure(curried);
        var step1  = lifted.Apply(emailV);
        var step2  = step1.Apply(passwordV);
        var step3  = step2.Apply(ageV);
        var step4  = step3.Apply(tierV);

        return
        [
            ("step1 = lifted.Apply(emailV)",  Describe(step1)),
            ("step2 = step1.Apply(passwordV)", Describe(step2)),
            ("step3 = step2.Apply(ageV)",      Describe(step3)),
            ("step4 = step3.Apply(tierV)",     Describe(step4)),
        ];
    }

    // 결과 오류 건수 — 챌린지 검증용. email 만 Invalid 이므로 1.
    public static int FinalErrorCount()
    {
        var result = SignUpForm.Submit("noatsign", "12345678", 30, "Pro");
        return result.As() switch
        {
            MyValidation<DomainError, User>.Invalid e => e.Errors.Count,
            _                                         => 0
        };
    }

    private static string Describe<A>(K<MyValidationF<DomainError>, A> v) =>
        v.As() switch
        {
            MyValidation<DomainError, A>.Valid    => "Valid(함수 일부 적용됨)",
            MyValidation<DomainError, A>.Invalid e => $"Invalid(오류 {e.Errors.Count} 건: {string.Join(", ", e.Errors)})",
            _                                      => "?"
        };
}
