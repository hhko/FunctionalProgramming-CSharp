namespace Ch42.Types;

// 도메인 검증 (3부/39장 Validation applicative 누적).
public abstract record Validation<A>
{
    public sealed record Valid(A Value) : Validation<A>;
    public sealed record Invalid(IReadOnlyList<string> Errors) : Validation<A>;
}

public static class Validation
{
    public static Validation<A> Ok<A>(A v) => new Validation<A>.Valid(v);
    public static Validation<A> Err<A>(string e) => new Validation<A>.Invalid([e]);

    static IReadOnlyList<string> Errs<A>(Validation<A> v) => v is Validation<A>.Invalid i ? i.Errors : [];

    public static Validation<C> Map2<A, B, C>(Validation<A> a, Validation<B> b, Func<A, B, C> f) =>
        (a, b) switch
        {
            (Validation<A>.Valid x, Validation<B>.Valid y) => new Validation<C>.Valid(f(x.Value, y.Value)),
            _ => new Validation<C>.Invalid([.. Errs(a), .. Errs(b)])
        };
}

// 검증된 주문 — 존재하면 id 비어있지 않고 amount >= 0 보장.
public sealed record Order(string Id, decimal Amount);

public static class OrderValidation
{
    static Validation<string> Id(string id) =>
        string.IsNullOrWhiteSpace(id) ? Validation.Err<string>("id 비어 있음") : Validation.Ok(id);

    static Validation<decimal> Amount(decimal a) =>
        a >= 0 ? Validation.Ok(a) : Validation.Err<decimal>($"amount {a} 음수");

    public static Validation<Order> Validate(string id, decimal amount) =>
        Validation.Map2(Id(id), Amount(amount), (i, a) => new Order(i, a));
}
