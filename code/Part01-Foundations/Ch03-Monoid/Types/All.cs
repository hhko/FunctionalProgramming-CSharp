using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Types;

// Boolean AND Monoid — Empty 는 true (vacuous truth), Combine 은 && (논리곱).
//
// 빈 컬렉션의 답이 true 인 이유: 합칠 게 없으니 거짓이 될 수가 없습니다.
// LINQ 의 xs.All(p) 가 빈 컬렉션에 대해 true 를 돌려주는 자리와 정합합니다.
public readonly record struct All(bool Value) : Monoid<All>
{
    [Pure]
    public static All Empty => new(true);

    [Pure]
    public All Combine(All rhs) => new(Value && rhs.Value);
}
