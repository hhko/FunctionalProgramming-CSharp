using Ch39.Types;

namespace Ch39.Challenges;

// 챌린지 정답 — 주문 검증. 수량/가격/할인율을 동시에 검증해 오류를 누적한다.
public sealed record Order(int Quantity, decimal Price, decimal DiscountRate);

public static class OrderValidation
{
    static Validation<string, int> Quantity(int q) =>
        q > 0 ? Validation<string, int>.Success(q) : Validation<string, int>.Fail("수량은 양수여야 함");

    static Validation<string, decimal> Price(decimal p) =>
        p >= 0 ? Validation<string, decimal>.Success(p) : Validation<string, decimal>.Fail("가격은 음수일 수 없음");

    static Validation<string, decimal> Discount(decimal d) =>
        d is >= 0 and <= 1 ? Validation<string, decimal>.Success(d) : Validation<string, decimal>.Fail("할인율은 0~1");

    public static Validation<string, Order> Validate(int q, decimal p, decimal d) =>
        Validation.Map3(Quantity(q), Price(p), Discount(d), (qq, pp, dd) => new Order(qq, pp, dd));
}
