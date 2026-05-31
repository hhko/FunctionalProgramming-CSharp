using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Types;

// 덧셈 Monoid — Empty 는 0, Combine 은 + (instance 의 결합 능력).
//
// v5 정합 — operator + 는 trait 의 default + SemigroupExtensions 의 extension 으로 작동.
// 자료 타입 자체에 operator + 명시 없음.
public readonly record struct Sum(int Value) : Monoid<Sum>
{
    [Pure]
    public static Sum Empty => new(0);

    [Pure]
    public Sum Combine(Sum rhs) => new(Value + rhs.Value);
}
