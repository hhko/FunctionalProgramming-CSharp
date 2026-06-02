namespace Ch18.Types;

// Either<L, A> — 실패를 *값 L 과 함께* 담는다 (Option 의 None 과 달리 "왜 실패했는지" 를 남긴다).
public abstract record Either<L, A>
{
    public sealed record Left(L Error) : Either<L, A> { public override string ToString() => $"Left({Error})"; }
    public sealed record Right(A Value) : Either<L, A> { public override string ToString() => $"Right({Value})"; }

    public Either<L, B> MapRight<B>(Func<A, B> f) =>
        this is Right r ? new Either<L, B>.Right(f(r.Value)) : new Either<L, B>.Left(((Left)this).Error);
}
