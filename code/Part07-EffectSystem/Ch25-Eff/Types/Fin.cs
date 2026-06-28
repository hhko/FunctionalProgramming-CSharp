namespace Ch25.Types;

// Error / Fin — 24장에서 정의한 함수형 오류 모델 (자족적으로 재선언).
public sealed record Error(string Message, Exception? Exception = null)
{
    public static Error New(string message) => new(message);
    public static Error New(Exception ex) => new(ex.Message, ex);
    public override string ToString() => Message;
}

public abstract record Fin<A>
{
    public sealed record Succ(A Value) : Fin<A> { public override string ToString() => $"Succ({Value})"; }
    public sealed record Fail(Error Error) : Fin<A> { public override string ToString() => $"Fail({Error})"; }
}
