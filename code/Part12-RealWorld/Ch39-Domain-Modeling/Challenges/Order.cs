using Ch39.Types;

namespace Ch39.Challenges;

// 챌린지 정답 — 주문 검증. 계층 없는 평평한 도메인에도 같은 어법이 그대로 간다.
// 다른 계층과 동일하게: private 생성자로 직접 생성을 막고, Create 만이 Pure→Apply 누적으로 값을 내준다.
public sealed record Order
{
    public int Quantity { get; }
    public decimal Price { get; }
    public decimal DiscountRate { get; }
    private Order(int quantity, decimal price, decimal discountRate) =>     // 옆문 잠금
        (Quantity, Price, DiscountRate) = (quantity, price, discountRate);

    static Validation<string, int> CheckQuantity(int q) =>
        q > 0 ? Validation<string, int>.Success(q) : Validation<string, int>.Fail("수량은 양수여야 함");

    static Validation<string, decimal> CheckPrice(decimal p) =>
        p >= 0 ? Validation<string, decimal>.Success(p) : Validation<string, decimal>.Fail("가격은 음수일 수 없음");

    static Validation<string, decimal> CheckDiscount(decimal d) =>
        d is >= 0 and <= 1 ? Validation<string, decimal>.Success(d) : Validation<string, decimal>.Fail("할인율은 0~1");

    // Customer.Create 와 같은 Lift3 — 5장의 Lift 가 Curry → Pure → Apply 를 캡슐화한다.
    public static Validation<string, Order> Create(int quantity, decimal price, decimal discountRate) =>
        Validation.Lift3((int q, decimal p, decimal d) => new Order(q, p, d),
            CheckQuantity(quantity),
            CheckPrice(price),
            CheckDiscount(discountRate));
}
