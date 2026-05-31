using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Functions;

// v5 정통 static helper — Semigroup.combine<A>(x, y) 자유 함수.
//
// 일반 함수 어법으로 결합을 호출. instance 어법 (a.Combine(b)) 의 평행 자리이고,
// trait 의 Instance record form 의 Combine 필드에 참조된다.
public static partial class Semigroup
{
    [Pure]
    public static A combine<A>(A x, A y) where A : Semigroup<A> =>
        x + y;
}
