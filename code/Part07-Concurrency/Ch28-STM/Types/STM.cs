using Ch28.Traits;

namespace Ch28.Types;

// 트랜잭션 안에서 다루는 참조의 비제네릭 면 (커밋 시 타입 무관하게 적용).
public interface IRef
{
    long Version { get; set; }
    void Apply(object? value);
}

// Ref<A> — *트랜잭션 참조*. STM 안에서만 읽고 쓴다.
public sealed class Ref<A>(A initial) : IRef
{
    A current = initial;
    public long Version { get; set; }
    public A Current => current;
    public void Apply(object? value) => current = (A)value!;
}

// 트랜잭션 로그 — 읽은 참조의 버전(검증용)과 쓸 값(커밋용)을 모은다.
public sealed class Txn
{
    public Dictionary<IRef, long> Reads { get; } = new();
    public Dictionary<IRef, object?> Writes { get; } = new();
}

// STM<A> — *트랜잭션 모나드*. Run(txn) 은 로그를 쌓을 뿐, 실제 적용은 commit 에서.
public sealed class STM<A>(Func<Txn, A> run) : K<STMF, A>
{
    public A Run(Txn txn) => run(txn);
}

public sealed class STMF : Monad<STMF>
{
    public static K<STMF, A> Pure<A>(A value) => new STM<A>(_ => value);

    public static K<STMF, B> Map<A, B>(Func<A, B> f, K<STMF, A> fa) =>
        new STM<B>(t => f(fa.As().Run(t)));

    public static K<STMF, B> Apply<A, B>(K<STMF, Func<A, B>> mf, K<STMF, A> ma) =>
        new STM<B>(t => mf.As().Run(t)(ma.As().Run(t)));

    public static K<STMF, B> Bind<A, B>(K<STMF, A> ma, Func<A, K<STMF, B>> f) =>
        new STM<B>(t => f(ma.As().Run(t)).As().Run(t));
}

public static class STMExtensions
{
    public static STM<A> As<A>(this K<STMF, A> fa) => (STM<A>)fa;
}
