namespace Ch02.Traits;

// Functor — 컨테이너 *모양을 보존하며* 안의 값을 변환하는 능력.
//
// trait 정의. K<F, A> + self-bound + static abstract 의 세 도구가
// 모두 동원되는 첫 정통 trait. 시그니처가 *F 의 모양 보존을 강제* 한다 — `K<F, B>` 와
// `K<F, A>` 의 F 가 같다.
//
// 3장에서 본격 trait 활용. 2장에서는 *3-tuple 패턴의 한 예시* 로 사용된다.
public interface Functor<F> where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> fa);
}
