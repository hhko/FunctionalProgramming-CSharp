using Ch10.Traits;

namespace Ch10.Functions;

// BifunctorExtensions — K<F, L, A> 위 extension methods (4장 FunctorExtensions 의 2-인자 판).
//
// 호출 어법: fab.BiMap(first, second) — 가장 자연스러운 점 호출.
// this 인자에서 F, L, A 가 추론되고, 두 함수에서 M, B 가 추론되어
// 명시 제네릭 (BiMap<int, string, int, string>) 없이 컴파일된다.
//
// MapFirst / MapSecond 도 같은 점 호출로 제공 — 한쪽 변환을 fluent 로 쓴다.
public static class BifunctorExtensions
{
    // 두 함수로 두 인자를 동시에 변환합니다.
    public static K<F, M, B> BiMap<F, L, A, M, B>(
        this K<F, L, A> fab, Func<L, M> first, Func<A, B> second)
        where F : Bifunctor<F> =>
        F.BiMap(first, second, fab);

    // 첫 인자만 변환 — 둘째 자리에 항등 함수.
    public static K<F, M, A> MapFirst<F, L, A, M>(
        this K<F, L, A> fab, Func<L, M> first)
        where F : Bifunctor<F> =>
        F.BiMap(first, x => x, fab);

    // 둘째 인자만 변환 — 4장 Functor 의 map 에 해당.
    public static K<F, L, B> MapSecond<F, L, A, B>(
        this K<F, L, A> fab, Func<A, B> second)
        where F : Bifunctor<F> =>
        F.BiMap(x => x, second, fab);
}
