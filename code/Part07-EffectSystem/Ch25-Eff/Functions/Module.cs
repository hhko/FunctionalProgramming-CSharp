using Ch25.Traits;
using Ch25.Types;

namespace Ch25.Functions;

public static class MonadExtensions
{
    public static K<M, B> Select<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> => M.MapDefault(f, ma);

    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> => M.SelectMany(ma, bind, project);

    public static K<M, B> Bind<M, A, B>(this K<M, A> ma, Func<A, K<M, B>> f)
        where M : Monad<M> => M.Bind(ma, f);
}

public static class Fallibles
{
    public static K<F, A> fail<F, A>(Error error) where F : Fallible<F> => F.Fail<A>(error);
    public static K<F, A> @catch<F, A>(K<F, A> fa, Func<Error, K<F, A>> h)
        where F : Fallible<F> => F.Catch(fa, h);
}
