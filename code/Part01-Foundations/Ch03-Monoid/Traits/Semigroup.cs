namespace Ch03.Traits;

// Semigroup — 결합 연산을 가진 값의 trait (Order 0, kind `*`).
//
// 값 자체가 결합 능력의 주인. lhs.Combine(rhs) 가 두 값을 합친다.
// + 연산자는 Combine 의 syntactic sugar 로 모든 구현체가 자동 제공받는다.
// 결합 법칙 (associativity) 을 약속:
//   a.Combine(b).Combine(c) == a.Combine(b.Combine(c))
//   (a + b) + c             == a + (b + c)
//
// 시그니처는 LanguageExt v5 의 `Semigroup<A>` 와 정합 — instance Combine + static virtual operator +.
public interface Semigroup<A>
    where A : Semigroup<A>
{
    A Combine(A rhs);

    static virtual A operator +(A lhs, A rhs) => lhs.Combine(rhs);
}
