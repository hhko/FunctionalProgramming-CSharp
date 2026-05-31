namespace Ch01;

// 1장의 학습 코드를 위한 *placeholder* 타입.
//
// 본격 정의 (K<F, A> + Functor<F> + 3-tuple 패턴) 는 2장 / 4장에서 본다.
// 1장은 *비유 (Normal / Elevated World)* 만 정착시키는 자리라, 가장 단순한
// 형태의 Option / Result 만 제공한다.
public readonly record struct Option<A>
{
    public bool IsSome { get; }
    private readonly A? _value;

    private Option(bool isSome, A? value)
    {
        IsSome = isSome;
        _value = value;
    }

    public static Option<A> Some(A value) => new(true, value);
    public static Option<A> None => new(false, default);

    // E<a> → E<b> 유형 — 4장 Functor 의 미리보기.
    public Option<B> Select<B>(Func<A, B> f) =>
        IsSome ? Option<B>.Some(f(_value!)) : Option<B>.None;

    public override string ToString() =>
        IsSome ? $"Some({_value})" : "None";
}

// 정적 진입점 — `Option.Some(42)` 식으로 타입 추론을 돕는 helper.
public static class Option
{
    public static Option<A> Some<A>(A value) => Option<A>.Some(value);
    public static Option<A> None<A>() => Option<A>.None;
}

// 세 번째 Elevated 시민 — 실패할 수 있는 결과.
public readonly record struct Result<A>
{
    public bool IsOk { get; }
    private readonly A? _value;
    private readonly string? _error;

    private Result(bool isOk, A? value, string? error)
    {
        IsOk = isOk;
        _value = value;
        _error = error;
    }

    public static Result<A> Ok(A value) => new(true, value, null);
    public static Result<A> Fail(string error) => new(false, default, error);

    public override string ToString() =>
        IsOk ? $"Ok({_value})" : $"Fail(\"{_error}\")";
}

public static class Result
{
    public static Result<A> Ok<A>(A value) => Result<A>.Ok(value);
    public static Result<A> Fail<A>(string error) => Result<A>.Fail(error);
}
