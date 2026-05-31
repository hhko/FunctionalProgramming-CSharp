namespace Ch10.Traits;

// Bifunctor — 두 타입 인자 모두에 작용하는 trait.
//
// BiMap : (L → M) → (A → B) → F<L, A> → F<M, B>
//   두 함수로 두 인자를 동시에 변환. 모양 F 는 보존, 안의 두 값만 변환.
//
// MapFirst / MapSecond 는 BiMap 의 한 자리에 항등 함수를 넣은 기본 구현.
public interface Bifunctor<F>
    where F : Bifunctor<F>
{
    static abstract K<F, M, B> BiMap<L, A, M, B>(
        Func<L, M> first, Func<A, B> second, K<F, L, A> fab);

    // 첫 인자만 변환 — 둘째 자리에 항등 함수
    static virtual K<F, M, A> MapFirst<L, A, M>(Func<L, M> first, K<F, L, A> fab) =>
        F.BiMap(first, x => x, fab);

    // 둘째 인자만 변환 — 4장 Functor 의 map 에 해당
    static virtual K<F, L, B> MapSecond<L, A, B>(Func<A, B> second, K<F, L, A> fab) =>
        F.BiMap(x => x, second, fab);
}
