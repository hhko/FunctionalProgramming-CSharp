using Ch26.Types;

namespace Ch26.Traits;

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

public interface MonadIO<M> : Monad<M> where M : MonadIO<M>
{
    static abstract K<M, A> LiftIO<A>(IO<A> ma);
}

public interface Readable<M, Env> where M : Readable<M, Env>
{
    static abstract K<M, A> Asks<A>(Func<Env, A> f);
    static virtual K<M, Env> Ask => M.Asks(e => e);
    static abstract K<M, A> Local<A>(Func<Env, Env> f, K<M, A> ma);
}

// Has — *능력 기반 DI*. 런타임 RT 가 특정 능력 TRAIT 를 가짐을 타입으로 보장한다.
// LanguageExt v5 의 Has<RT, TRAIT> 와 정합. 효과가 `where RT : Has<RT, IConsole>` 를 요구하면
// 컴파일러가 "이 런타임은 콘솔 능력이 있다" 를 검증한다.
public interface Has<RT, TRAIT> where RT : Has<RT, TRAIT>
{
    static abstract TRAIT Get(RT runtime);
}
