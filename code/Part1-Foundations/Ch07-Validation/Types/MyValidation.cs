using Ch07.Traits;

namespace Ch07.Types;

// 5장 의 MyValidation 재사용 — Apply 만 정의 + Bind 안 함.
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

    public static K<MyValidationF<E>, B> Apply<A, B>(K<MyValidationF<E>, Func<A, B>> mf, K<MyValidationF<E>, A> ma) =>
        (mf.As(), ma.As()) switch
        {
            (MyValidation<E, Func<A, B>>.Valid f, MyValidation<E, A>.Valid a) =>
                new MyValidation<E, B>.Valid(f.Value(a.Value)),
            (MyValidation<E, Func<A, B>>.Invalid fe, MyValidation<E, A>.Invalid ae) =>
                new MyValidation<E, B>.Invalid([..fe.Errors, ..ae.Errors]),
            (MyValidation<E, Func<A, B>>.Invalid fe, _) =>
                new MyValidation<E, B>.Invalid(fe.Errors),
            (_, MyValidation<E, A>.Invalid ae) =>
                new MyValidation<E, B>.Invalid(ae.Errors),
            _ => throw new InvalidOperationException()
        };

    // MapFail — 에러에 컨텍스트 추가. *값* 은 그대로, *에러만* 변환.
    public static K<MyValidationF<E>, A> MapFail<A>(Func<E, E> f, K<MyValidationF<E>, A> fa) =>
        fa.As() switch
        {
            MyValidation<E, A>.Valid v   => v,
            MyValidation<E, A>.Invalid i => new MyValidation<E, A>.Invalid(i.Errors.Select(f).ToList()),
            _ => throw new InvalidOperationException()
        };
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyValidationExtensions
{
    public static MyValidation<E, A> As<E, A>(this K<MyValidationF<E>, A> fa) => (MyValidation<E, A>)fa;
}
