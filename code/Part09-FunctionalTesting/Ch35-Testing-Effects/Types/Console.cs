namespace Ch35.Types;

// IConsole 능력 + 두 구현: 라이브 / 테스트 더블.
public interface IConsole
{
    void WriteLine(string line);
    string ReadLine();
}

public sealed class LiveConsole : IConsole
{
    public void WriteLine(string line) => Console.WriteLine(line);
    public string ReadLine() => Console.ReadLine() ?? "";
}

// MemoryConsole — *테스트 더블*. 입력은 큐, 출력은 리스트 (결정적·검증 가능).
public sealed class MemoryConsole(IEnumerable<string> inputs) : IConsole
{
    readonly Queue<string> queue = new(inputs);
    public List<string> Output { get; } = [];
    public void WriteLine(string line) => Output.Add(line);
    public string ReadLine() => queue.Count > 0 ? queue.Dequeue() : "";
}

public sealed record AppRT(IConsole Console) : Has<AppRT, IConsole>
{
    public static IConsole Get(AppRT rt) => rt.Console;
}
