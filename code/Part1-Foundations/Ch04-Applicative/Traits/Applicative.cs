namespace Ch04.Traits;

// Applicative — Functor 에 두 능력 추가:
//   Pure  : 값을 컨테이너로 *그냥 감싸기*
//   Apply : *컨테이너 안의 함수* + *컨테이너 안의 값* → *컨테이너 안의 결과*
//
// Apply 시그니처가 결정적 — *함수가 컨테이너 안에 있다* 는 모양이 Functor 와 다른 점.
public interface Applicative<F> : Functor<F> where F : Applicative<F>
{
    static abstract K<F, A> Pure<A>(A value);
    static abstract K<F, B> Apply<A, B>(K<F, Func<A, B>> mf, K<F, A> ma);
}
