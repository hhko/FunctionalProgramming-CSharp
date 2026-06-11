using Ch10.Traits;

namespace Ch10.Types;

// 두 갈래 중 하나만 담는 자료 타입 — Left(L) 또는 Right(R).
public abstract record Either<L, R> : K<EitherF, L, R>;
public sealed record Left<L, R>(L Value) : Either<L, R>;
public sealed record Right<L, R>(R Value) : Either<L, R>;

// 태그 타입 + Bifunctor trait 구현. 담긴 쪽의 함수만 적용된다.
public sealed class EitherF : Bifunctor<EitherF>
{
    public static K<EitherF, M, B> BiMap<L, A, M, B>(
        Func<L, M> first, Func<A, B> second, K<EitherF, L, A> fab) =>
        fab switch
        {
            Left<L, A> l => new Left<M, B>(first(l.Value)),
            Right<L, A> r => new Right<M, B>(second(r.Value)),
            _ => throw new InvalidOperationException()
        };

    // 명시적 재정의 — 구체 타입 EitherF 에서 직접 호출 가능하게 함.
    public static K<EitherF, M, A> MapFirst<L, A, M>(Func<L, M> first, K<EitherF, L, A> fab) =>
        BiMap<L, A, M, A>(first, x => x, fab);

    public static K<EitherF, L, B> MapSecond<L, A, B>(Func<A, B> second, K<EitherF, L, A> fab) =>
        BiMap<L, A, L, B>(x => x, second, fab);
}
