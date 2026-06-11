using Ch08.Traits;

namespace Ch08.Functions;

// ApplicativeExtensions — K<F, A> 위 fluent 확장 (5장 ApplicativeExtensions.cs 미러).
//
// 본문 §8.5 의 lifted.Apply(emailV).Apply(passwordV) 사슬이 *명시 제네릭 없이*
// 컴파일되도록 한다. F 는 호출 인자 (this K<F, Func<A, B>>) 에서, A / B 는
// Func<A, B> 시그니처에서 추론된다. MyValidationF<DomainError> 도 같은 추론을 탄다.
public static class ApplicativeExtensions
{
    // mf.Apply(ma) — 함수 측 mf 에 값 측 ma 를 적용. 타입 추론으로 명시 제네릭 불필요.
    public static K<F, B> Apply<F, A, B>(this K<F, Func<A, B>> mf, K<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(mf, ma);

    // fa.Map(f) — Functor 도 같은 fluent 어법으로 노출.
    public static K<F, B> Map<F, A, B>(this K<F, A> fa, Func<A, B> f)
        where F : Applicative<F> =>
        F.Map(f, fa);
}
