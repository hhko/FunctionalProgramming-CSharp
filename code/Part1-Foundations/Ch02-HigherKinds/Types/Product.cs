using Ch02.Traits;

namespace Ch02.Types;

// 정수 곱셈 모노이드 — `Add = *`, `Empty = 1`.
//
// `Sum` 과 *같은 자료 타입* (int) 위에 *다른 모노이드 구조* 가 산다는 것을 보여 준다.
// 모노이드는 "값의 종류" 가 아니라 "결합 방법 + 항등원" 의 한 묶음이다.
public readonly record struct Product(int Value) : Monoid<Product>
{
    public static Product Add(Product a, Product b) => new(a.Value * b.Value);

    public static Product Empty => new(1);

    public override string ToString() => $"Product({Value})";
}
