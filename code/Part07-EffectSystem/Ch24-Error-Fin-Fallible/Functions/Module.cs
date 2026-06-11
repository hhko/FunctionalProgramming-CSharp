using Ch24.Traits;
using Ch24.Types;

namespace Ch24.Functions;

public static class MonadExtensions
{
    public static K<M, B> Select<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> => M.MapDefault(f, ma);

    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> => M.SelectMany(ma, bind, project);
}

public static class Fallibles
{
    public static K<F, A> fail<F, A>(Error error) where F : Fallible<F> => F.Fail<A>(error);
    public static K<F, A> @catch<F, A>(K<F, A> fa, Func<Error, K<F, A>> handler)
        where F : Fallible<F> => F.Catch(fa, handler);
}

public static class Try
{
    // 예외를 던지는 코드를 Fin 으로 포획 (예외 → Error 값).
    public static K<FinF, A> Run<A>(Func<A> f)
    {
        try { return new Fin<A>.Succ(f()); }
        catch (Exception e) { return new Fin<A>.Fail(Error.New(e)); }
    }
}
