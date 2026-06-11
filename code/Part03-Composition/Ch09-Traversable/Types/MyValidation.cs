using Ch09.Traits;

namespace Ch09.Types;

// 8장의 MyValidation 을 9장으로 이식 — Traverse 의 안쪽 효과 F 자리에 끼워
// "F 가 MyValidation 이면 누적" 을 실제로 보이기 위함 (본문 §9.7.1).
// Apply 가 두 Invalid 의 오류 목록을 이어붙여 누적한다. (단락하는 MyMaybe 와 대비.)
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

    // 누적의 심장 — 둘 다 Invalid 면 두 오류 목록을 이어붙인다 (8장 §8.6 과 같은 분기).
    public static K<MyValidationF<E>, B> Apply<A, B>(
        K<MyValidationF<E>, Func<A, B>> mf, K<MyValidationF<E>, A> ma) =>
        (mf.As(), ma.As()) switch
        {
            (MyValidation<E, Func<A, B>>.Valid f, MyValidation<E, A>.Valid a)
                => new MyValidation<E, B>.Valid(f.Value(a.Value)),
            (MyValidation<E, Func<A, B>>.Invalid fe, MyValidation<E, A>.Invalid ae)
                => new MyValidation<E, B>.Invalid([.. fe.Errors, .. ae.Errors]),   // 누적
            (MyValidation<E, Func<A, B>>.Invalid fe, _)
                => new MyValidation<E, B>.Invalid(fe.Errors),
            (_, MyValidation<E, A>.Invalid ae)
                => new MyValidation<E, B>.Invalid(ae.Errors),
            _ => throw new InvalidOperationException()
        };
}

public static class MyValidationExtensions
{
    public static MyValidation<E, A> As<E, A>(this K<MyValidationF<E>, A> fa) => (MyValidation<E, A>)fa;
}
