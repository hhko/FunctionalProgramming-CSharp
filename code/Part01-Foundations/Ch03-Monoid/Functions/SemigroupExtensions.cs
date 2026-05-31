using Ch03.Traits;

namespace Ch03.Functions;

// v5 정통 extension static operator — 어떤 Semigroup 도 구체 타입에서 + 직접 호출 가능.
//
// trait 의 static virtual operator + default 는 generic 제약 context 안에서만 호출 가능.
// 이 extension 이 구체 타입 (Sum + Sum 등) 직접 호출의 자리를 만든다.
// v5 의 LanguageExt.Core/Traits/Semigroup/Semigroup.Operators.cs 의 어법을 그대로 따른다.
public static class SemigroupExtensions
{
    extension<A>(A _)
        where A : Semigroup<A>
    {
        public static A operator +(A lhs, A rhs) =>
            lhs.Combine(rhs);
    }
}
