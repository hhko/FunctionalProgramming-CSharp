namespace Ch03.Traits;

// Functor — 컨테이너 안의 값을 *모양을 보존하며* 변환하는 능력.
//
// self-bound + static abstract + K<F, A> 의 3 도구가 모두 동원되는 첫 정통 trait.
// 시그니처가 *결정적* — F 가 입력 / 출력에 모두 등장 (모양 보존), A → B 만 바뀐다.
public interface Functor<F> where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> fa);
}
