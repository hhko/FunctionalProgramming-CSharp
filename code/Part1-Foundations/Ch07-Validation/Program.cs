using Ch07.Domain;
using Ch07.Functions;
using Ch07.Traits;
using Ch07.Types;

Console.WriteLine("================================================");
Console.WriteLine("7장 — Validation 실전");
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
