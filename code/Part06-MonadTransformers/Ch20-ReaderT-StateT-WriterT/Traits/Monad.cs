namespace Ch20.Traits;

public interface K<in F, A>;

public interface Functor<F> where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> fa);
}

public interface Applicative<F> : Functor<F> where F : Applicative<F>
{
    static abstract K<F, A> Pure<A>(A value);
    static abstract K<F, B> Apply<A, B>(K<F, Func<A, B>> mf, K<F, A> ma);
}

public interface Monad<M> : Applicative<M> where M : Monad<M>
{
    static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f);

    static virtual K<M, B> MapDefault<A, B>(Func<A, B> f, K<M, A> ma) =>
        M.Bind(ma, a => M.Pure(f(a)));

    static virtual K<M, C> SelectMany<A, B, C>(
        K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        M.Bind(ma, a => M.Bind(bind(a), b => M.Pure(project(a, b))));
}

public interface MonadT<T, M> : Monad<T>
    where T : MonadT<T, M>
    where M : Monad<M>
{
    static abstract K<T, A> Lift<A>(K<M, A> ma);
}

// Monoid — WriterT 의 출력 W 가 갖춰야 하는 약속. 항등원 (Empty) 과 결합 연산 (Combine).
// (3부 Monoid 그대로. WriterT 의 Bind 가 두 단계의 출력을 Combine 으로 잇는다.)
public interface Monoid<W> where W : Monoid<W>
{
    static abstract W Empty { get; }
    static abstract W Combine(W a, W b);
}

public readonly record struct Unit
{
    public static readonly Unit Default = new();
    public override string ToString() => "()";
}
