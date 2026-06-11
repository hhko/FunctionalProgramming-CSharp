namespace Ch13.Traits;

// Functor — 3장 정의 그대로.
//
// Map<K, V> 에 부착하면 *값 V 에만* 작용한다. 키는 그대로 — "키-값 컨테이너의 끌어올림" 의 핵심.
public interface Functor<F> where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> fa);
}
