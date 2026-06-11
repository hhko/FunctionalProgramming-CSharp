namespace Ch09.Traits;

// Traversable — 두 kind 의 *swap* 능력.
//
//   Traverse :  (A → K<F, B>)  →  K<T, A>          →  K<F, K<T, B>>
//                                  ─────┬─────         ─────┬─────
//                                  T 안에 A 들              F 안에 T<B>
//
// "T 가 바깥, F 가 안" 에서 "F 가 바깥, T 가 안" 으로 카인드가 *맞바뀐다*.
//
// F 는 Applicative — 결합 + Pure 가능해야 swap 의 결과를 만들 수 있다.
public interface Traversable<T> : Functor<T>, Foldable<T> where T : Traversable<T>
{
    static abstract K<F, K<T, B>> Traverse<F, A, B>(Func<A, K<F, B>> f, K<T, A> ta)
        where F : Applicative<F>;

    // virtual — Traverse 의 단순한 변형. f 가 항등 함수일 때.
    static virtual K<F, K<T, A>> Sequence<F, A>(K<T, K<F, A>> tfa)
        where F : Applicative<F>
        => T.Traverse<F, K<F, A>, A>(x => x, tfa);
}

// Foldable trait 도 선언 (1부 4장 회고 — Traversable 이 상속).
public interface Foldable<F> where F : Foldable<F>
{
    static abstract S Fold<A, S>(Func<S, A, S> f, S initial, K<F, A> fa);
}
