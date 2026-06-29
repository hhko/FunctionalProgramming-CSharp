namespace Ch39.Types;

// Validation<E, A> — applicative *누적* 검증 (3부 MyValidation 의 실전판).
// Apply 가 여러 필드를 동시에 검증하며 *모든 오류를 모은다* (첫 오류에서 멈추지 않음).
public abstract record Validation<E, A>
{
    public sealed record Valid(A Value) : Validation<E, A>;
    public sealed record Invalid(IReadOnlyList<E> Errors) : Validation<E, A>;

    public static Validation<E, A> Success(A value) => new Valid(value);   // 값을 Valid 로 끌어올림 (2부 Pure)
    public static Validation<E, A> Fail(E error) => new Invalid([error]);   // 오류 하나를 목록으로 감쌈

    public Validation<E, B> Map<B>(Func<A, B> f) =>
        this switch
        {
            Valid v => new Validation<E, B>.Valid(f(v.Value)),   // Valid 면 f 적용
            Invalid i => new Validation<E, B>.Invalid(i.Errors), // Invalid 면 오류 그대로 통과
            _ => throw new InvalidOperationException()
        };
}

public static class Validation
{
    // Pure — Normal 값을 Valid 한 칸으로 끌어올림 (2부 Applicative 의 Pure).
    public static Validation<E, A> Pure<E, A>(A value) => new Validation<E, A>.Valid(value);

    static IReadOnlyList<E> Errors<E, A>(Validation<E, A> v) =>
        v is Validation<E, A>.Invalid i ? i.Errors : [];     // Invalid 면 오류 목록, Valid 면 빈 목록

    // Apply — 2부 Applicative 의 그 Apply. 갇힌 함수 Validation<E, Func<A,B>> 에
    // 인자 Validation<E, A> 를 한 칸 채워 Validation<E, B> 를 낸다.
    // 둘 다 Valid 면 함수 적용, 아니면 *양쪽 오류 누적*.
    public static Validation<E, B> Apply<E, A, B>(
        this Validation<E, Func<A, B>> vf, Validation<E, A> va) =>
        (vf, va) switch
        {
            (Validation<E, Func<A, B>>.Valid f, Validation<E, A>.Valid a) =>
                new Validation<E, B>.Valid(f.Value(a.Value)),                // 둘 다 Valid → 함수에 인자 적용
            _ => new Validation<E, B>.Invalid([.. Errors(vf), .. Errors(va)]) // 아니면 양쪽 오류 합침
        };

    // Lift2 / Lift3 — 5장의 Lift 그대로. 평범한 다인자 함수 (A, B) → C 를 받아
    // Curry → Pure → Apply 를 한 번에 캡슐화한다. 호출부가 커리한 생성자를 손으로 적지 않아도 된다.
    public static Validation<E, C> Lift2<E, A, B, C>(
        Func<A, B, C> f, Validation<E, A> fa, Validation<E, B> fb) =>
        Pure<E, Func<A, Func<B, C>>>(a => b => f(a, b))     // 커리는 여기서 한 번만
            .Apply(fa)
            .Apply(fb);

    public static Validation<E, D> Lift3<E, A, B, C, D>(
        Func<A, B, C, D> f, Validation<E, A> fa, Validation<E, B> fb, Validation<E, C> fc) =>
        Pure<E, Func<A, Func<B, Func<C, D>>>>(a => b => c => f(a, b, c))
            .Apply(fa)
            .Apply(fb)
            .Apply(fc);
}
