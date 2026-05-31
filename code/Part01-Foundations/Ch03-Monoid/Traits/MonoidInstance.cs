using System;

namespace Ch03.Traits;

// MonoidInstance — SemigroupInstance + 단위원 Empty (v5 정합).
//
// SemigroupInstance 를 상속해 Combine 을 물려받고 Empty 한 자리만 더한다.
public record MonoidInstance<A>(A Empty, Func<A, A, A> Combine)
    : SemigroupInstance<A>(Combine);
