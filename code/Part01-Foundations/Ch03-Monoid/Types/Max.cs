using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Types;

// Max Monoid — Empty 는 int.MinValue, Combine 은 Math.Max.
//
// int.MinValue 가 어떤 int 와의 Max 에서 상대를 그대로 남기는 자리입니다
// (Max(int.MinValue, 5) == 5). 결합 법칙도 성립.
public readonly record struct Max(int Value) : Monoid<Max>
{
    [Pure]
    public static Max Empty => new(int.MinValue);

    [Pure]
    public Max Combine(Max rhs) => new(System.Math.Max(Value, rhs.Value));
}
