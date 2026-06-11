using Ch15.Traits;

namespace Ch15.Functions;

// Readable 모듈 — asks / ask / local 의 generic 어휘 (LanguageExt Readable.Module 미러).
public static class Readable
{
    public static K<M, A> asks<M, Env, A>(Func<Env, A> f)
        where M : Readable<M, Env> =>
        M.Asks(f);

    public static K<M, Env> ask<M, Env>()
        where M : Readable<M, Env> =>
        M.Ask;

    public static K<M, A> local<M, Env, A>(Func<Env, Env> f, K<M, A> ma)
        where M : Readable<M, Env> =>
        M.Local(f, ma);
}
