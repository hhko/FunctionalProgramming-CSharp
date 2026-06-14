namespace Ch13.Traits;

// Traversable — 9장 정의 그대로 (Functor + Foldable 상속 + Traverse / Sequence).
//
// Map 에서는 값들을 한꺼번에 effect 로 변환한다: Map<K, F<V>> → F<Map<K, V>>.
// 키는 보존되고, 안쪽 effect (Maybe / Validation) 만 바깥으로 모인다.
public interface Traversable<T> : Functor<T>, Foldable<T> where T : Traversable<T>
{
    static abstract K<F, K<T, B>> Traverse<F, A, B>(Func<A, K<F, B>> f, K<T, A> ta)
        where F : Applicative<F>;

    // virtual — Sequence = Traverse id.
    static virtual K<F, K<T, A>> Sequence<F, A>(K<T, K<F, A>> tfa)
        where F : Applicative<F>
        => T.Traverse<F, K<F, A>, A>(x => x, tfa);
}
