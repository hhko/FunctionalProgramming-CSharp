using System.Diagnostics.Contracts;
using Ch03.Functions;

namespace Ch03.Traits;

// Semigroup — 결합 연산을 가진 값의 trait (Order 0, kind `*`).
//
// 값 자체가 결합 능력의 주인. lhs.Combine(rhs) 가 두 값을 합친다.
// static virtual operator + 가 모든 구현체에 default 로 제공된다.
// 결합 법칙을 약속:
//   a.Combine(b).Combine(c) == a.Combine(b.Combine(c))
//
// 시그니처는 LanguageExt v5 의 Semigroup<A> 와 정확히 정합.
public interface Semigroup<A>
    where A : Semigroup<A>
{
    [Pure]
    A Combine(A rhs);

    [Pure]
    static virtual A operator +(A lhs, A rhs) => lhs.Combine(rhs);

    // Instance — trait 을 값처럼 전달할 수 있게 record 형태로 노출.
    // C# 제약 시스템의 한계를 우회하는 자리 (v5 정합).
    [Pure]
    static virtual SemigroupInstance<A> Instance { get; } =
        new(Combine: Semigroup.combine);
}
