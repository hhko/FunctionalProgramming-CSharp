using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Types;

// 문자열 이어붙이기 Monoid — Empty 는 "", Combine 은 string + (instance 의 결합 능력).
//
// v5 정합 — operator + 는 trait 의 default + SemigroupExtensions 의 extension 으로 작동.
public readonly record struct Concat(string Value) : Monoid<Concat>
{
    [Pure]
    public static Concat Empty => new("");

    [Pure]
    public Concat Combine(Concat rhs) => new(Value + rhs.Value);
}
