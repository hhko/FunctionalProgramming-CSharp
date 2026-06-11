namespace Ch39.Types;

// Validation<E, A> — applicative *누적* 검증 (1부 MyValidation 의 실전판).
// Map2/Map3 가 여러 필드를 동시에 검증하며 *모든 오류를 모은다* (첫 오류에서 멈추지 않음).
public abstract record Validation<E, A>
{
    public sealed record Valid(A Value) : Validation<E, A>;
    public sealed record Invalid(IReadOnlyList<E> Errors) : Validation<E, A>;

    public static Validation<E, A> Success(A value) => new Valid(value);
    public static Validation<E, A> Fail(E error) => new Invalid([error]);

    public Validation<E, B> Map<B>(Func<A, B> f) =>
        this switch
        {
            Valid v => new Validation<E, B>.Valid(f(v.Value)),
            Invalid i => new Validation<E, B>.Invalid(i.Errors),
            _ => throw new InvalidOperationException()
        };
}

public static class Validation
{
    static IReadOnlyList<E> Errors<E, A>(Validation<E, A> v) =>
        v is Validation<E, A>.Invalid i ? i.Errors : [];

    // 두 검증을 결합 — 둘 다 성공해야 성공, 아니면 *양쪽 오류 누적*.
    public static Validation<E, C> Map2<E, A, B, C>(
        Validation<E, A> va, Validation<E, B> vb, Func<A, B, C> f) =>
        (va, vb) switch
        {
            (Validation<E, A>.Valid a, Validation<E, B>.Valid b) =>
                new Validation<E, C>.Valid(f(a.Value, b.Value)),
            _ => new Validation<E, C>.Invalid([.. Errors(va), .. Errors(vb)])
        };

    public static Validation<E, D> Map3<E, A, B, C, D>(
        Validation<E, A> va, Validation<E, B> vb, Validation<E, C> vc, Func<A, B, C, D> f) =>
        Map2(Map2(va, vb, (a, b) => (a, b)), vc, (ab, c) => f(ab.a, ab.b, c));
}
