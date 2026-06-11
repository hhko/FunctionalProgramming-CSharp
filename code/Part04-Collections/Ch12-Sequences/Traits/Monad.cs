namespace Ch12.Traits;

// Monad — 6장 정의 그대로 (Bind + LINQ 유도).
//
// 시퀀스의 Bind 가 곧 LINQ 의 SelectMany (flatMap) 다.
// from x in xs from y in f(x) select ... 가 이 Bind 로 변환된다.
public interface Monad<M> : Applicative<M> where M : Monad<M>
{
    static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f);

    // virtual — Map 을 Bind + Pure 로 유도.
    static virtual K<M, B> MapDefault<A, B>(Func<A, B> f, K<M, A> ma) =>
        M.Bind(ma, a => M.Pure(f(a)));

    // virtual — SelectMany 가 LINQ 의 비밀. C# 컴파일러가 from-from-select 를 *이 메서드 호출* 로 변환.
    static virtual K<M, C> SelectMany<A, B, C>(
        K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        M.Bind(ma, a => M.Bind(bind(a), b => M.Pure(project(a, b))));
}
