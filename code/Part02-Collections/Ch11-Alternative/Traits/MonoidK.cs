namespace Ch11.Traits;

// MonoidK — *고차 Monoid*. SemigroupK 에 *단위원* Empty 추가.
//
//   Monoid (Normal):   Empty: A       + Combine          (예: 0 + 덧셈)
//   MonoidK (Elevated): Empty: K<M,A>  + Combine          (예: 빈 시퀀스 + concat)
//
// 법칙 — Combine(Empty, x) ≡ x ≡ Combine(x, Empty)  (좌·우 단위원) + 결합법칙.
public interface MonoidK<M> : SemigroupK<M> where M : MonoidK<M>
{
    static abstract K<M, A> Empty<A>();
}
