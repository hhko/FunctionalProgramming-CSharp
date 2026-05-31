using Ch05.Traits;

namespace Ch05.Types;

// Validation — Applicative 만 정의 + Monad 의 Bind *없음*.
//
// 핵심 — Apply 시 양쪽 에러를 *누적*. Bind 가 있으면 첫 에러에서 단락 — 누적 안 됨.
// 이게 6장 (Validation 실전) 의 결정적 발상.
public abstract record MyValidation<E, A> : K<MyValidationF<E>, A>
{
    public sealed record Valid(A Value) : MyValidation<E, A>;
    public sealed record Invalid(IReadOnlyList<E> Errors) : MyValidation<E, A>;
}

public sealed class MyValidationF<E> : Applicative<MyValidationF<E>>
{
    public static K<MyValidationF<E>, B> Map<A, B>(Func<A, B> f, K<MyValidationF<E>, A> fa) =>
        fa.As() switch
        {
            MyValidation<E, A>.Valid v   => new MyValidation<E, B>.Valid(f(v.Value)),
            MyValidation<E, A>.Invalid i => new MyValidation<E, B>.Invalid(i.Errors),
            _ => throw new InvalidOperationException()
        };

    public static K<MyValidationF<E>, A> Pure<A>(A value) =>
        new MyValidation<E, A>.Valid(value);

    // Apply 의 핵심 — 양쪽 에러 *누적*.
    public static K<MyValidationF<E>, B> Apply<A, B>(K<MyValidationF<E>, Func<A, B>> mf, K<MyValidationF<E>, A> ma) =>
        (mf.As(), ma.As()) switch
        {
            (MyValidation<E, Func<A, B>>.Valid f, MyValidation<E, A>.Valid a) =>
                new MyValidation<E, B>.Valid(f.Value(a.Value)),

            (MyValidation<E, Func<A, B>>.Invalid fe, MyValidation<E, A>.Invalid ae) =>
                new MyValidation<E, B>.Invalid([..fe.Errors, ..ae.Errors]),    // 누적

            (MyValidation<E, Func<A, B>>.Invalid fe, _) =>
                new MyValidation<E, B>.Invalid(fe.Errors),

            (_, MyValidation<E, A>.Invalid ae) =>
                new MyValidation<E, B>.Invalid(ae.Errors),

            _ => throw new InvalidOperationException()
        };
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyValidationExtensions
{
    public static MyValidation<E, A> As<E, A>(this K<MyValidationF<E>, A> fa) => (MyValidation<E, A>)fa;
}
