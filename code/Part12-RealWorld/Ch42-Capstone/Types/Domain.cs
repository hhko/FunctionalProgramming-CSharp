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

    // Pure: 함수를 Elevated World 로 끌어올린다 (오류 없는 Valid). Ok 와 같은 일.
    public static Validation<A> Pure<A>(A v) => new Validation<A>.Valid(v);

    // Apply: 끌어올린 함수에 끌어올린 인자를 적용하며, 양쪽 오류를 누적한다.
    public static Validation<B> Apply<A, B>(this Validation<Func<A, B>> vf, Validation<A> va) =>
        (vf, va) switch
        {
            (Validation<Func<A, B>>.Valid f, Validation<A>.Valid a) => new Validation<B>.Valid(f.Value(a.Value)),
            _ => new Validation<B>.Invalid([.. Errs(vf), .. Errs(va)])     // ← 두 오류를 모은다
        };

    // Lift2 — 5장의 Lift 그대로. 평범한 2인자 함수를 받아 Curry → Pure → Apply 를 캡슐화한다.
    public static Validation<C> Lift2<A, B, C>(Func<A, B, C> f, Validation<A> fa, Validation<B> fb) =>
        Pure<Func<A, Func<B, C>>>(a => b => f(a, b)).Apply(fa).Apply(fb);
}

// 검증된 주문 — 존재하면 id 비어있지 않고 amount >= 0 보장.
// 39장과 같은 어법: private 생성자로 직접 생성을 막고, Create 만이 검증을 통과한 값을 내준다.
public sealed record Order
{
    public string Id { get; }
    public decimal Amount { get; }
    private Order(string id, decimal amount) => (Id, Amount) = (id, amount);

    static Validation<string> CheckId(string id) =>
        string.IsNullOrWhiteSpace(id) ? Validation.Err<string>("id 비어 있음") : Validation.Ok(id);

    static Validation<decimal> CheckAmount(decimal a) =>
        a >= 0 ? Validation.Ok(a) : Validation.Err<decimal>($"amount {a} 음수");

    // Lift2 로 두 검증을 합성 (39장과 같은 Create 형). 5장 Lift 가 Curry → Pure → Apply 를 캡슐화한다.
    public static Validation<Order> Create(string id, decimal amount) =>
        Validation.Lift2((string i, decimal a) => new Order(i, a), CheckId(id), CheckAmount(amount));
}
