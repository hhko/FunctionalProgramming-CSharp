using Ch25.Types;

namespace Ch25.Traits;

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

// Fallible — 24장과 동일 (오류로 실패 + 복구).
public interface Fallible<F> where F : Fallible<F>
{
    static abstract K<F, A> Fail<A>(Error error);
    static abstract K<F, A> Catch<A>(K<F, A> fa, Func<Error, K<F, A>> handler);
}

// Alternative — 4부의 '고르기'. Empty (없음) + Choose (첫째 실패면 둘째). MonoidK 도 같은 모양.
public interface Alternative<F> where F : Alternative<F>
{
    static abstract K<F, A> Empty<A>();
    static abstract K<F, A> Choose<A>(K<F, A> fa, K<F, A> fb);
}

// Final — try/finally 의 함수형 대응. fa 의 성공·실패와 무관하게 @finally 를 실행한다.
public interface Final<F> where F : Final<F>
{
    static abstract K<F, A> Finally<A, X>(K<F, A> fa, K<F, X> @finally);
}
