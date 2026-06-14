namespace Ch13.Traits;

// Applicative — 5장 정의 그대로 (Pure + Apply).
//
// Map 의 Traverse 에서 *목표 세계 F* 의 제약으로 쓰인다 (Map<K, F<V>> → F<Map<K, V>>).
public interface Applicative<F> : Functor<F> where F : Applicative<F>
{
    static abstract K<F, A> Pure<A>(A value);
    static abstract K<F, B> Apply<A, B>(K<F, Func<A, B>> mf, K<F, A> ma);
}
