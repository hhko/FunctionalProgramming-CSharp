using Ch06.Traits;

namespace Ch06.Functions;

// Monad 모듈 — generic 헬퍼 함수 (라이브러리 Monad.Module.cs 미러).
//
// 호출 어법: Monad.bind<MyMaybeF, int, string>(ma, f), Monad.pure<MyMaybeF, int>(42).
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
