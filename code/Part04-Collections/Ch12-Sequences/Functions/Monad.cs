using Ch12.Traits;

namespace Ch12.Functions;

// Monad 모듈 — generic 헬퍼 (6장과 동일). 어떤 Monad M 든 받는다.
public static class Monad
{
    public static K<M, A> pure<M, A>(A value)
        where M : Monad<M> =>
        M.Pure(value);

    public static K<M, B> bind<M, A, B>(K<M, A> ma, Func<A, K<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);

    public static K<M, B> map<M, A, B>(Func<A, B> f, K<M, A> ma)
        where M : Monad<M> =>
        M.Map(f, ma);

    public static K<M, A> flatten<M, A>(K<M, K<M, A>> mma)
        where M : Monad<M> =>
        M.Bind(mma, x => x);
}
