using Ch23.Traits;

namespace Ch23.Types;

// ── 효과 실행 환경 ──────────────────────────────────────────────────
// EnvIO — IO 가 Run 될 때 운반되는 것. 여기선 취소 토큰만 (5부 본 DSL 은 자원·동기화도 운반).
public sealed record EnvIO(CancellationToken Token)
{
    public static EnvIO Default => new(CancellationToken.None);
}

// ── 내부 DSL 노드 (object 로 타입 소거) ─────────────────────────────
// 부수 작용을 *즉시 실행하지 않고* 노드 트리로 인코딩한다. Run 에서야 해석된다.
internal abstract record Node;
internal sealed record PureNode(object? Value) : Node;
internal sealed record EffectNode(Func<object?> Thunk) : Node;
internal sealed record BindNode(Node Source, Func<object?, Node> Cont) : Node;

// ── 트램폴린 인터프리터 ─────────────────────────────────────────────
// 핵심 — 깊은 Bind 체인을 *C# 재귀 없이* 힙 위의 연속 스택으로 해석한다 → 스택 안전.
internal static class Interpreter
{
    public static object? Run(Node node, EnvIO env)
    {
        var conts = new Stack<Func<object?, Node>>();
        var cur = node;
        while (true)
        {
            env.Token.ThrowIfCancellationRequested();
            switch (cur)
            {
                case BindNode b:
                    conts.Push(b.Cont);     // 연속을 힙 스택에 쌓고
                    cur = b.Source;         // 안쪽으로 내려간다 (C# 스택 사용 안 함)
                    break;

                case PureNode p:
                    if (conts.Count == 0) return p.Value;
                    cur = conts.Pop()(p.Value);
                    break;

                case EffectNode e:
                    var v = e.Thunk();      // ← 부수 작용은 여기서 *비로소* 실행
                    if (conts.Count == 0) return v;
                    cur = conts.Pop()(v);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

// ── IO<A> — 지연 효과 프로그램 ──────────────────────────────────────
public sealed class IO<A> : K<IOF, A>
{
    internal Node Node { get; }
    internal IO(Node node) => Node = node;

    public A Run(EnvIO env) => (A)Interpreter.Run(Node, env)!;
    public A Run() => Run(EnvIO.Default);

    // 부수 작용 thunk 로부터 IO 생성 (예: Console 출력).
    public static IO<A> Effect(Func<A> thunk) => new(new EffectNode(() => thunk()));
}

public sealed class IOF : Monad<IOF>
{
    public static K<IOF, A> Pure<A>(A value) => new IO<A>(new PureNode(value));

    public static K<IOF, B> Map<A, B>(Func<A, B> f, K<IOF, A> fa) =>
        new IO<B>(new BindNode(fa.As().Node, x => new PureNode(f((A)x!))));

    public static K<IOF, B> Apply<A, B>(K<IOF, Func<A, B>> mf, K<IOF, A> ma) =>
        Bind(mf, f => Map(f, ma));

    public static K<IOF, B> Bind<A, B>(K<IOF, A> ma, Func<A, K<IOF, B>> f) =>
        new IO<B>(new BindNode(ma.As().Node, x => f((A)x!).As().Node));
}

public static class IOExtensions
{
    public static IO<A> As<A>(this K<IOF, A> fa) => (IO<A>)fa;
}
