using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Types;

// Min Monoid — Empty 는 int.MaxValue, Combine 은 Math.Min.
//
// int.MaxValue 가 어떤 int 와의 Min 에서 상대를 그대로 남깁니다
// (Min(int.MaxValue, 5) == 5). Max 와 대칭의 Monoid.
public readonly record struct Min(int Value) : Monoid<Min>
{
    [Pure]
    public static Min Empty => new(int.MaxValue);

    [Pure]
    public Min Combine(Min rhs) => new(System.Math.Min(Value, rhs.Value));
}
