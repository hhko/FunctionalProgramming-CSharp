namespace Ch09.Traits;

// Functor — 3장 정의 그대로. 9장에서는 *실무 시퀀스* 가 이 trait 의 인스턴스가 된다.
public interface Functor<F> where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> fa);
}
