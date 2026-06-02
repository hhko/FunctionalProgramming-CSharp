namespace Ch24.Types;

// 효과 결과 (21장 Fin 축소판) — retry/repeat 시연용.
public sealed record Error(string Message)
{
    public override string ToString() => Message;
}

public abstract record Fin<A>
{
    public sealed record Succ(A Value) : Fin<A> { public override string ToString() => $"Succ({Value})"; }
    public sealed record Fail(Error Error) : Fin<A> { public override string ToString() => $"Fail({Error})"; }
}
