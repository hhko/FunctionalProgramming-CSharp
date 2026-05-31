using Ch05.Traits;

namespace Ch05.Challenges;

// 심화 챌린지 B — MyEither 단락 Applicative.
//
// MyValidation 과 시그니처가 같지만 Apply 의 분기가 다르다.
// (Left fe, Left ae) 일 때 *첫 fe 만* 돌려준다 — 누적 안 함.
//
// 같은 시그니처가 자료 구조의 분기에 따라 *다른 결합 의미* 를 가짐을 보인다.
// 본문 §5.4 의 MyValidation 과 비교하면 차이가 한 줄로 드러난다.
public abstract record MyEither<E, A> : K<MyEitherF<E>, A>
{
    public sealed record Right(A Value) : MyEither<E, A>;
    public sealed record Left(E Error)  : MyEither<E, A>;
}

public sealed class MyEitherF<E> : Applicative<MyEitherF<E>>
{
    public static K<MyEitherF<E>, B> Map<A, B>(Func<A, B> f, K<MyEitherF<E>, A> fa) =>
        fa.As() switch
        {
            MyEither<E, A>.Right r => new MyEither<E, B>.Right(f(r.Value)),
            MyEither<E, A>.Left  l => new MyEither<E, B>.Left(l.Error),
            _ => throw new InvalidOperationException()
        };

    public static K<MyEitherF<E>, A> Pure<A>(A value) =>
        new MyEither<E, A>.Right(value);

    // 단락 동작 — 첫 Left 에서 종료. 누적 *없음*.
    public static K<MyEitherF<E>, B> Apply<A, B>(
        K<MyEitherF<E>, Func<A, B>> mf,
        K<MyEitherF<E>, A>          ma) =>
        (mf.As(), ma.As()) switch
        {
            (MyEither<E, Func<A, B>>.Right f, MyEither<E, A>.Right a) =>
                new MyEither<E, B>.Right(f.Value(a.Value)),

            (MyEither<E, Func<A, B>>.Left fe, _) =>
                new MyEither<E, B>.Left(fe.Error),        // 첫 에러만

            (_, MyEither<E, A>.Left ae) =>
                new MyEither<E, B>.Left(ae.Error),

            _ => throw new InvalidOperationException()
        };
}

public static class MyEitherExtensions
{
    public static MyEither<E, A> As<E, A>(this K<MyEitherF<E>, A> fa) =>
        (MyEither<E, A>)fa;
}
