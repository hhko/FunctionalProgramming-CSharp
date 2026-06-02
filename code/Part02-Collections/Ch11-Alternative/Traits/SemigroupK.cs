namespace Ch11.Traits;

// SemigroupK — *고차 Semigroup*. 1부 Monoid 의 결합 (Combine) 을 Elevated World 로 끌어올린 것.
//
//   Semigroup (Normal):   A      → A      → A        (예: int 덧셈)
//   SemigroupK (Elevated): K<M,A> → K<M,A> → K<M,A>   (예: 두 시퀀스 이어 붙이기)
//
// 결합법칙만 요구한다 (단위원은 MonoidK 가 추가).
public interface SemigroupK<M> where M : SemigroupK<M>
{
    static abstract K<M, A> Combine<A>(K<M, A> lhs, K<M, A> rhs);
}
