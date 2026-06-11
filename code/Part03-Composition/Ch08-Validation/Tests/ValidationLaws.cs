using Ch08.Domain;
using Ch08.Functions;
using Ch08.Traits;
using Ch08.Types;

namespace Ch08.Tests;

// ValidationLaws — 누적 (applicative) vs 단락 (monadic) 차이를 학습용 bool 헬퍼로 묶은 모듈.
//
// 5장 ApplicativeLaws.cs 의 *bool 헬퍼 + where 제약 일반 함수* 패턴을 따른다.
// xUnit + Shouldly 로 옮기려면 각 호출을 [Fact] 로 감싸고 ShouldBeTrue 단언만 추가하면 된다.
// 본 학습용 csproj 는 콘솔 데모이므로 bool 반환으로 통과 / 실패를 표시한다.
//
// 본문 §8.6 (누적의 심장) + §8.7 (applicative vs monadic) 의 코드와 의미가 같다.
public static class ValidationLaws
{
    // ── 일반 Applicative 헬퍼 (where 제약) ───────────────────────────────

    // ① Functor 정합 — Map(f, fa) ≡ Apply(Pure(f), fa).
    // Applicative 의 Map 이 Functor 의 Map 과 의미가 같음을 보장한다.
    public static bool FunctorConsistencyHolds<F, A, B>(
        Func<A, B> f,
        K<F, A>    fa,
        Func<K<F, B>, B?> probe)
        where F : Applicative<F>
    {
        var lhs = F.Map(f, fa);
        var rhs = F.Apply(F.Pure(f), fa);
        return Equals(probe(lhs), probe(rhs));
    }

    // ② 항등 법칙 — Apply(Pure(id), fa) ≡ fa.
    public static bool IdentityHolds<F, A>(
        K<F, A> fa,
        Func<K<F, A>, A?> probe)
        where F : Applicative<F>
    {
        var idF = F.Pure<Func<A, A>>(x => x);
        var lhs = F.Apply(idF, fa);
        return Equals(probe(lhs), probe(fa));
    }

    // ── MyValidation 누적 vs 단락 (핵심 검증) ────────────────────────────

    // 누적 — Apply 의 Invalid + Invalid 분기가 두 오류 목록을 *이어붙인다*.
    // 두 칸 모두 Invalid 면 결과 오류 건수 = 두 칸 오류 건수의 합.
    public static bool ApplyAccumulatesErrors()
    {
        // 함수 측, 값 측 모두 Invalid — 각각 한 건.
        K<MyValidationF<DomainError>, Func<int, int>> mf =
            new MyValidation<DomainError, Func<int, int>>.Invalid(
                [new DomainError("f", "함수 측 오류")]);
        K<MyValidationF<DomainError>, int> ma =
            new MyValidation<DomainError, int>.Invalid(
                [new DomainError("a", "값 측 오류")]);

        var result = mf.Apply(ma);   // fluent 확장 — 명시 제네릭 없이 추론
        return ErrorCount(result) == 2;
    }

    // 누적 — 같은 입력 4 칸 오류를 applicative style 로 풀면 *4 건* 이 모인다.
    public static bool ApplicativeAccumulatesAllFourErrors()
    {
        var result = SignUpForm.Submit("noatsign", "1234", -5, "Premium");
        return ErrorCount(result) == 4;
    }

    // 단락 — 같은 입력 4 칸 오류를 monadic style 로 풀면 첫 오류에서 멈춰 *1 건* 만 나온다.
    public static bool MonadicShortCircuitsToOneError()
    {
        var result = StyleComparison.SubmitMonadic("noatsign", "1234", -5, "Premium");
        return ErrorCount(result) == 1;
    }

    // 누적 vs 단락 — 같은 입력에 두 어법을 적용하면 오류 건수가 갈린다 (4 vs 1).
    // 본문 §8.7 표 (applicative 4 건 / monadic 1 건) 의 검증판.
    public static bool AccumulateVsShortCircuitDiffer()
    {
        var (ap, mo) = StyleComparison.Compare("noatsign", "1234", -5, "Premium");
        return ErrorCount(ap.Value) == 4
            && ErrorCount(mo.Value) == 1;
    }

    // 모두 Valid 면 두 어법 결과가 같다 — 차이는 *오류가 있을 때만* 드러난다.
    public static bool BothStylesAgreeWhenAllValid()
    {
        var (ap, mo) = StyleComparison.Compare("a@b.com", "12345678", 30, "Pro");
        return IsValid(ap.Value) && IsValid(mo.Value);
    }

    // ── probe / 헬퍼 ────────────────────────────────────────────────────

    // Valid 면 값, Invalid 면 default — Functor 정합 probe 용.
    public static A? Peek<A>(K<MyValidationF<DomainError>, A> v) =>
        v.As() switch
        {
            MyValidation<DomainError, A>.Valid x => x.Value,
            _                                    => default
        };

    private static int ErrorCount<A>(K<MyValidationF<DomainError>, A> v) =>
        v.As() switch
        {
            MyValidation<DomainError, A>.Invalid e => e.Errors.Count,
            _                                      => 0
        };

    private static bool IsValid<A>(K<MyValidationF<DomainError>, A> v) =>
        v.As() is MyValidation<DomainError, A>.Valid;
}
