using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Types;

// 곱셈 Monoid — Empty 는 1, Combine 은 * (instance 의 결합 능력).
//
// v5 정합 — operator + 는 trait 의 default + SemigroupExtensions 의 extension 으로 작동.
// (+ 는 Semigroup 의 결합 sugar 이지 산술 + 가 아니다. Product 에서는 곱셈 결합.)
public readonly record struct Product(int Value) : Monoid<Product>
{
    [Pure]
    public static Product Empty => new(1);

    [Pure]
    public Product Combine(Product rhs) => new(Value * rhs.Value);
}
