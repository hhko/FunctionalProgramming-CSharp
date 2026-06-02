namespace Ch09.Traits;

// Traversable — 8장 정의 그대로 (Functor + Foldable 상속 + Traverse / Sequence).
//
//   Traverse : (A → K<F, B>) → K<T, A> → K<F, K<T, B>>
//
// 시퀀스에서 가장 쓸모 있는 자리 — List<Maybe<A>> → Maybe<List<A>> 같은 층 뒤집기.
// 예: 입력 문자열 시퀀스를 모두 파싱했을 때만 성공 시퀀스를 돌려준다.
public interface Traversable<T> : Functor<T>, Foldable<T> where T : Traversable<T>
{
    static abstract K<F, K<T, B>> Traverse<F, A, B>(Func<A, K<F, B>> f, K<T, A> ta)
        where F : Applicative<F>;

    // virtual — Sequence = Traverse id.
    static virtual K<F, K<T, A>> Sequence<F, A>(K<T, K<F, A>> tfa)
        where F : Applicative<F>
        => T.Traverse<F, K<F, A>, A>(x => x, tfa);
}
