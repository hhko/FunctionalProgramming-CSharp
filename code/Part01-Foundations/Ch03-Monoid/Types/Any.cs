using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Types;

// Boolean OR Monoid — Empty 는 false (vacuous falsity), Combine 은 || (논리합).
//
// 빈 컬렉션의 답이 false 인 이유: 합칠 게 없으니 참인 게 없습니다.
// LINQ 의 xs.Any(p) 가 빈 컬렉션에 대해 false 를 돌려주는 자리와 정합합니다.
public readonly record struct Any(bool Value) : Monoid<Any>
{
    [Pure]
    public static Any Empty => new(false);

    [Pure]
    public Any Combine(Any rhs) => new(Value || rhs.Value);
}
