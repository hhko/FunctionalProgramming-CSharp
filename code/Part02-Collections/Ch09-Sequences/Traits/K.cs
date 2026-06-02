namespace Ch09.Traits;

// 2장 회고 — F 안에 A 가 있다는 type-level 신호.
//
// 9장에서 F 자리에 들어오는 것은 toy 타입이 아니라 *실무 시퀀스* 의 태그 (SeqF) 다.
// 같은 K<F, A> 어휘 위에서 1부의 모든 동사 (Map / Apply / Bind / Fold / Traverse) 가
// 그대로 작동한다는 것이 9장의 핵심.
public interface K<in F, A>;
