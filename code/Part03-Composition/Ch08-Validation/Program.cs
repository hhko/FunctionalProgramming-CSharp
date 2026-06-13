using Ch08.Challenges;
using Ch08.Domain;
using Ch08.Functions;
using Ch08.Tests;
using Ch08.Traits;
using Ch08.Types;

Console.WriteLine("================================================");
Console.WriteLine("8장 — Validation 실전");
Console.WriteLine("================================================");
Console.WriteLine();

// 사례 1 — 모든 입력이 올바름.
Console.WriteLine("== 사례 1 — 모두 정상 ==");
var ok = SignUpForm.Submit("a@b.com", "12345678", 30, "Pro");
Show(ok);

// 사례 2 — 한 입력만 잘못 (이메일 @ 없음).
Console.WriteLine();
Console.WriteLine("== 사례 2 — 이메일만 잘못 ==");
var oneErr = SignUpForm.Submit("noatsign", "12345678", 30, "Pro");
Show(oneErr);

// 사례 3 — *모든* 입력이 잘못 — 4 개 에러 누적.
Console.WriteLine();
Console.WriteLine("== 사례 3 — 4 개 모두 잘못 — 누적 ==");
var allErr = SignUpForm.Submit("noatsign", "1234", -5, "Premium");
Show(allErr);

// 사례 4 — MapFail 로 에러에 컨텍스트 추가.
Console.WriteLine();
Console.WriteLine("== 사례 4 — MapFail 로 에러 prefix 추가 ==");
var withPrefix = MyValidationF<DomainError>.MapFail<User>(
    e => new DomainError(e.Field, $"[가입 폼] {e.Message}"),
    allErr);
Show(withPrefix);

// 법칙 / 챌린지 자가 점검 — bool 헬퍼 결과를 한눈에.
Console.WriteLine();
Console.WriteLine("== 법칙 + 챌린지 자가 점검 ==");
Check("Apply 누적 (Invalid+Invalid → 2 건)", ValidationLaws.ApplyAccumulatesErrors());
Check("applicative 4 칸 오류 → 4 건",        ValidationLaws.ApplicativeAccumulatesAllFourErrors());
Check("monadic 4 칸 오류 → 1 건 (단락)",      ValidationLaws.MonadicShortCircuitsToOneError());
Check("누적 vs 단락 차이 (4 vs 1)",           ValidationLaws.AccumulateVsShortCircuitDiffer());
Check("모두 Valid 면 두 어법 일치",            ValidationLaws.BothStylesAgreeWhenAllValid());
Check("Functor 정합 (Map ≡ Apply∘Pure)",
    ValidationLaws.FunctorConsistencyHolds<MyValidationF<DomainError>, int, int>(
        n => n + 1, MyValidationF<DomainError>.Pure(41), ValidationLaws.Peek));
Check("챌린지 1 — email 만 Invalid → 1 건",   ApplyTrace.FinalErrorCount() == 1);
Check("챌린지 2 — 5 칸 모두 Invalid → 5 건",  NicknameForm.AllFiveInvalidErrorCount() == 5);
Check("챌린지 3 — Bind 단락 → 1 건 (누적 사라짐)", BindLosesAccumulation.BindErrorCount() == 1);

// 사례 5 — 칸 하나에 규칙 여럿 (§8.9.2). "abc" 는 세 규칙 모두 위반 → 3 건.
Console.WriteLine();
Console.WriteLine("== 사례 5 — 칸 하나에 규칙 여럿 (비밀번호 §8.9.2) ==");
ShowP("\"abc\"      ", PasswordRules.Strong("abc"));
ShowP("\"password1\"", PasswordRules.Strong("password1"));
ShowP("\"Passw0rd\" ", PasswordRules.Strong("Passw0rd"));

// 사례 6 — 또 다른 도메인: 서버 설정 (§8.9.1). 같은 누적 패턴.
Console.WriteLine();
Console.WriteLine("== 사례 6 — 서버 설정 검증 (다른 도메인 §8.9.1) ==");
ShowC("나쁜 설정", ConfigValidator.Parse("localhost", 99999, 0));
ShowC("정상 설정", ConfigValidator.Parse("api.example.com", 8080, 30));

static void Check(string label, bool ok) =>
    Console.WriteLine($"  {(ok ? "✓" : "✗")} {label}");

static void ShowP(string label, K<MyValidationF<DomainError>, Password> v)
{
    switch (v.As())
    {
        case MyValidation<DomainError, Password>.Valid x:
            Console.WriteLine($"  {label} → ✓ {x.Value}");
            break;
        case MyValidation<DomainError, Password>.Invalid e:
            Console.WriteLine($"  {label} → ✗ {e.Errors.Count} 건: {string.Join(" / ", e.Errors.Select(x => x.Message))}");
            break;
    }
}

static void ShowC(string label, K<MyValidationF<DomainError>, ServerConfig> v)
{
    switch (v.As())
    {
        case MyValidation<DomainError, ServerConfig>.Valid x:
            Console.WriteLine($"  {label} → ✓ {x.Value}");
            break;
        case MyValidation<DomainError, ServerConfig>.Invalid e:
            Console.WriteLine($"  {label} → ✗ {e.Errors.Count} 건: {string.Join(" / ", e.Errors)}");
            break;
    }
}

static void Show(K<MyValidationF<DomainError>, User> v)
{
    switch (v.As())
    {
        case MyValidation<DomainError, User>.Valid x:
            Console.WriteLine($"  ✓ User 생성: {x.Value}");
            break;
        case MyValidation<DomainError, User>.Invalid e:
            Console.WriteLine($"  ✗ 에러 {e.Errors.Count} 건:");
            foreach (var err in e.Errors)
                Console.WriteLine($"    - {err}");
            break;
    }
}
