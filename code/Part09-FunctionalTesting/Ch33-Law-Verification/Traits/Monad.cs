namespace Ch33.Traits;

public interface K<in F, A>;

public interface Functor<F> where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> fa);
}

public interface Monad<M> : Functor<M> where M : Monad<M>
{
    static abstract K<M, A> Pure<A>(A value);
    static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f);
}
