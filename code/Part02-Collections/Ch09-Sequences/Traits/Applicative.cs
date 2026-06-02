namespace Ch09.Traits;

// Applicative — 4장 정의 그대로 (Pure + Apply).
//
// 시퀀스의 Apply 는 *데카르트 곱* — 함수 시퀀스의 각 함수를 값 시퀀스의 각 값에 적용.
public interface Applicative<F> : Functor<F> where F : Applicative<F>
{
    static abstract K<F, A> Pure<A>(A value);
    static abstract K<F, B> Apply<A, B>(K<F, Func<A, B>> mf, K<F, A> ma);
}
