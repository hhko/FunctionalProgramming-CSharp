namespace Ch40.Types;

// 효과 스택 (Eff<RT,A> = ReaderT<RT, IO, A>) — 7부 Ch26 재현.
public interface K<in F, A>;

public interface Functor<F> where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> fa);
}

public interface Applicative<F> : Functor<F> where F : Applicative<F>
{
    static abstract K<F, A> Pure<A>(A value);
}

public interface Monad<M> : Applicative<M> where M : Monad<M>
{
    static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f);
    static virtual K<M, B> MapDefault<A, B>(Func<A, B> f, K<M, A> ma) => M.Bind(ma, a => M.Pure(f(a)));
    static virtual K<M, C> SelectManyD<A, B, C>(K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> proj) =>
        M.Bind(ma, a => M.Bind(bind(a), b => M.Pure(proj(a, b))));
}

public interface MonadIO<M> : Monad<M> where M : MonadIO<M>
{
    static abstract K<M, A> LiftIO<A>(IO<A> ma);
}

public interface Has<RT, TRAIT> where RT : Has<RT, TRAIT>
{
    static abstract TRAIT Get(RT runtime);
}

public readonly record struct Unit { public static readonly Unit Default = new(); }

public sealed class IO<A>(Func<A> thunk) : K<IOF, A> { public A Run() => thunk(); }

public sealed class IOF : MonadIO<IOF>
{
    public static K<IOF, B> Map<A, B>(Func<A, B> f, K<IOF, A> fa) => new IO<B>(() => f(((IO<A>)fa).Run()));
    public static K<IOF, A> Pure<A>(A value) => new IO<A>(() => value);
    public static K<IOF, B> Bind<A, B>(K<IOF, A> ma, Func<A, K<IOF, B>> f) =>
        new IO<B>(() => ((IO<B>)f(((IO<A>)ma).Run())).Run());
    public static K<IOF, A> LiftIO<A>(IO<A> ma) => ma;
}

public sealed class ReaderT<RT, A>(Func<RT, IO<A>> run) : K<ReaderTF<RT>, A>
{
    public IO<A> Run(RT rt) => run(rt);
}

public sealed class ReaderTF<RT> : MonadIO<ReaderTF<RT>>
{
    static IO<A> Inner<A>(K<ReaderTF<RT>, A> m, RT rt) => ((ReaderT<RT, A>)m).Run(rt);
    public static K<ReaderTF<RT>, A> Pure<A>(A value) => new ReaderT<RT, A>(_ => new IO<A>(() => value));
    public static K<ReaderTF<RT>, B> Map<A, B>(Func<A, B> f, K<ReaderTF<RT>, A> fa) =>
        new ReaderT<RT, B>(rt => new IO<B>(() => f(Inner(fa, rt).Run())));
    public static K<ReaderTF<RT>, B> Bind<A, B>(K<ReaderTF<RT>, A> ma, Func<A, K<ReaderTF<RT>, B>> f) =>
        new ReaderT<RT, B>(rt => new IO<B>(() => Inner(f(Inner(ma, rt).Run()), rt).Run()));
    public static K<ReaderTF<RT>, A> LiftIO<A>(IO<A> ma) => new ReaderT<RT, A>(_ => ma);
    public static K<ReaderTF<RT>, A> Asks<A>(Func<RT, A> f) => new ReaderT<RT, A>(rt => new IO<A>(() => f(rt)));
}
