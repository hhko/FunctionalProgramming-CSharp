using Ch21.Traits;

namespace Ch21.Types;

// EitherT<L, M, A> — *오류 값을 가진 실패* 효과를 내부 모나드 M 위에 얹는다.
// 내부는 `K<M, Either<L, A>>`. OptionT 가 None 으로 *왜* 를 잃는 데 비해, EitherT 는 Left(L) 로 보존한다.
//
// 스택 순서의 의미 — EitherT<L, ManyF, A> = Many<Either<L, A>> 는
// "여러 갈래 각각이 자기 오류로 실패" (갈래 구조는 살아남음). 반대 순서였다면
// 한 번의 실패가 전체 비결정성을 죽였을 것이다. *어느 효과가 바깥인가* 가 의미를 바꾼다.
public sealed class EitherT<L, M, A>(K<M, Either<L, A>> run) : K<EitherTF<L, M>, A>
    where M : Monad<M>
{
    public K<M, Either<L, A>> Run { get; } = run;
}

public sealed class EitherTF<L, M> : MonadT<EitherTF<L, M>, M>
    where M : Monad<M>
{
    public static K<EitherTF<L, M>, A> Pure<A>(A value) =>
        new EitherT<L, M, A>(M.Pure<Either<L, A>>(new Either<L, A>.Right(value)));

    public static K<EitherTF<L, M>, B> Map<A, B>(Func<A, B> f, K<EitherTF<L, M>, A> fa) =>
        new EitherT<L, M, B>(M.Map(e => e.MapRight(f), fa.As().Run));

    public static K<EitherTF<L, M>, B> Apply<A, B>(K<EitherTF<L, M>, Func<A, B>> mf, K<EitherTF<L, M>, A> ma) =>
        Bind(mf, f => Map(f, ma));

    public static K<EitherTF<L, M>, B> Bind<A, B>(K<EitherTF<L, M>, A> ma, Func<A, K<EitherTF<L, M>, B>> f) =>
        new EitherT<L, M, B>(
            M.Bind(ma.As().Run, e =>
                e is Either<L, A>.Right r
                    ? f(r.Value).As().Run
                    : M.Pure<Either<L, B>>(new Either<L, B>.Left(((Either<L, A>.Left)e).Error))));

    public static K<EitherTF<L, M>, A> Lift<A>(K<M, A> ma) =>
        new EitherT<L, M, A>(M.Map(a => (Either<L, A>)new Either<L, A>.Right(a), ma));

    // 실패 — 오류 값과 함께 단락.
    public static K<EitherTF<L, M>, A> Fail<A>(L error) =>
        new EitherT<L, M, A>(M.Pure<Either<L, A>>(new Either<L, A>.Left(error)));
}

public static class EitherTExtensions
{
    public static EitherT<L, M, A> As<L, M, A>(this K<EitherTF<L, M>, A> fa)
        where M : Monad<M> =>
        (EitherT<L, M, A>)fa;
}
