namespace Ch06.Traits;

// Monad — Applicative 에 Bind 추가.
//
// Apply 와 Bind 의 결정적 차이:
//   Apply : K<M, A → B>      → K<M, A>  → K<M, B>     함수가 *컨테이너 안* (독립 결합)
//   Bind  : K<M, A>           → (A → K<M, B>) → K<M, B>   *값 → 효과* (의존 결합)
public interface Monad<M> : Applicative<M> where M : Monad<M>
{
    static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f);

    // virtual — Map 을 Bind + Pure 로 유도.
    static virtual K<M, B> MapDefault<A, B>(Func<A, B> f, K<M, A> ma) =>
        M.Bind(ma, a => M.Pure(f(a)));

    // virtual — SelectMany 가 LINQ 의 비밀. C# 컴파일러가 from-select 를 *이 메서드 호출* 로 변환.
    static virtual K<M, C> SelectMany<A, B, C>(
        K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        M.Bind(ma, a => M.Bind(bind(a), b => M.Pure(project(a, b))));
}
