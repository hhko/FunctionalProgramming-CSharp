using System;

namespace Ch03.Traits;

// SemigroupInstance — trait 을 record 형태로 노출 (v5 정합).
//
// trait 을 값처럼 전달할 수 있게 한다. C# 제약 시스템의 한계 (제약을 인자로 못 받음) 를
// 우회하는 자리. 학습용에는 단순화된 형태로 정의.
public record SemigroupInstance<A>(Func<A, A, A> Combine);
