using Ch24.Traits;

namespace Ch24.Types;

// Fin<A> — 효과 계산의 *결과*: 성공 Succ(A) 또는 실패 Fail(Error). (= Either<Error, A>.)
// 1부 MyValidation 이 *효과 시스템 안의 오류* 로 자란 형태. Bind 는 첫 실패에서 단락한다.
public abstract record Fin<A> : K<FinF, A>
{
    public sealed record Succ(A Value) : Fin<A> { public override string ToString() => $"Succ({Value})"; }
    public sealed record Fail(Error Error) : Fin<A> { public override string ToString() => $"Fail({Error})"; }
}

public sealed class FinF : Monad<FinF>, Fallible<FinF>
{
    public static K<FinF, B> Map<A, B>(Func<A, B> f, K<FinF, A> fa) =>
        fa.As() switch
        {
            Fin<A>.Succ s => new Fin<B>.Succ(f(s.Value)),
            Fin<A>.Fail e => new Fin<B>.Fail(e.Error),
            _ => throw new InvalidOperationException()
        };

    public static K<FinF, A> Pure<A>(A value) => new Fin<A>.Succ(value);

    public static K<FinF, B> Apply<A, B>(K<FinF, Func<A, B>> mf, K<FinF, A> ma) =>
        (mf.As(), ma.As()) switch
        {
            (Fin<Func<A, B>>.Succ f, Fin<A>.Succ a) => new Fin<B>.Succ(f.Value(a.Value)),
            (Fin<Func<A, B>>.Fail e, _) => new Fin<B>.Fail(e.Error),
            (_, Fin<A>.Fail e) => new Fin<B>.Fail(e.Error),
            _ => throw new InvalidOperationException()
        };

    public static K<FinF, B> Bind<A, B>(K<FinF, A> ma, Func<A, K<FinF, B>> f) =>
        ma.As() switch
        {
            Fin<A>.Succ s => f(s.Value),
            Fin<A>.Fail e => new Fin<B>.Fail(e.Error),   // 첫 실패에서 단락
            _ => throw new InvalidOperationException()
        };

    public static K<FinF, A> Fail<A>(Error error) => new Fin<A>.Fail(error);

    public static K<FinF, A> Catch<A>(K<FinF, A> fa, Func<Error, K<FinF, A>> handler) =>
        fa.As() is Fin<A>.Fail e ? handler(e.Error) : fa;
}

public static class FinExtensions
{
    public static Fin<A> As<A>(this K<FinF, A> fa) => (Fin<A>)fa;
}
