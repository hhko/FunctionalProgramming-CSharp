using Ch09.Traits;

namespace Ch09.Functions;

// LINQ from-from-select 사용을 위한 *확장 메서드* (6장과 동일).
//
// 핵심 — 시퀀스의 Select / SelectMany 가 *Monad 의 Map / Bind* 위에서 자동으로 자란다.
// 즉 LINQ 의 시퀀스 쿼리가 사실 Functor + Monad 였다.
public static class MonadExtensions
{
    public static K<M, B> Select<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> =>
        M.MapDefault(f, ma);

    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma,
        Func<A, K<M, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        M.SelectMany(ma, bind, project);

    public static K<M, B> Bind<M, A, B>(this K<M, A> ma, Func<A, K<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);

    public static K<M, B> Map<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> =>
        M.Map(f, ma);
}
