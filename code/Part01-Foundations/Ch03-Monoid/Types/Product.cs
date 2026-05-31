using Ch03.Traits;

namespace Ch03.Types;

// 곱셈 Monoid — Empty 는 1, Combine 은 * (instance 의 결합 능력).
public readonly record struct Product(int Value) : Monoid<Product>
{
    public static Product Empty => new(1);
    public Product Combine(Product rhs) => new(Value * rhs.Value);

    // operator + 명시 — Semigroup 의 default operator 를 구체 타입에서 직접 호출 가능하게.
    // (+ 는 Semigroup 의 결합 sugar 이지 산술 + 가 아니다. Product 에서는 곱셈 결합.)
    public static Product operator +(Product lhs, Product rhs) => lhs.Combine(rhs);
}
