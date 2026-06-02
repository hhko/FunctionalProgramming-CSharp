using Ch23.Traits;

namespace Ch23.Types;

public readonly record struct Unit
{
    public static readonly Unit Default = new();
    public override string ToString() => "()";
}

// IConsole — 콘솔 *능력* (capability). 효과는 구체 구현이 아니라 이 인터페이스에 의존한다.
public interface IConsole
{
    void WriteLine(string line);
    string ReadLine();
}

// 라이브 구현 — 실제 콘솔.
public sealed class LiveConsole : IConsole
{
    public void WriteLine(string line) => Console.WriteLine(line);
    public string ReadLine() => Console.ReadLine() ?? "";
}

// 테스트 더블 — 입력은 큐에서, 출력은 리스트에 (결정적·검증 가능). 9부 테스트 런타임의 토대.
public sealed class TestConsole(IEnumerable<string> inputs) : IConsole
{
    readonly Queue<string> queue = new(inputs);
    public List<string> Output { get; } = [];
    public void WriteLine(string line) => Output.Add(line);
    public string ReadLine() => queue.Count > 0 ? queue.Dequeue() : "";
}

// AppRT — 애플리케이션 런타임. 콘솔 능력을 가진다 (Has<AppRT, IConsole>).
public sealed record AppRT(IConsole Console) : Has<AppRT, IConsole>
{
    public static IConsole Get(AppRT runtime) => runtime.Console;
}
