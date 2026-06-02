namespace Ch37.Types;

// 세 가지 능력 — 콘솔 / 시계 / 저장소. 효과는 구현이 아니라 이 인터페이스들에 의존한다.
public interface IConsole { void WriteLine(string line); }
public interface IClock { long Now(); }
public interface IStore { void Put(string key, string value); IReadOnlyList<string> Keys(); string? Get(string key); }

// 테스트 더블 (인메모리).
public sealed class MemoryConsole : IConsole
{
    public List<string> Output { get; } = [];
    public void WriteLine(string line) => Output.Add(line);
}

public sealed class FixedClock(long now) : IClock { public long Now() => now; }

public sealed class MemoryStore : IStore
{
    readonly Dictionary<string, string> map = new();
    public void Put(string key, string value) => map[key] = value;
    public IReadOnlyList<string> Keys() => map.Keys.OrderBy(k => k).ToList();
    public string? Get(string key) => map.TryGetValue(key, out var v) ? v : null;
}

// AppRT — 세 능력을 모두 가진 런타임. Get 이름 충돌은 *명시적 인터페이스 구현* 으로 해소.
public sealed record AppRT(IConsole Console, IClock Clock, IStore Store)
    : Has<AppRT, IConsole>, Has<AppRT, IClock>, Has<AppRT, IStore>
{
    static IConsole Has<AppRT, IConsole>.Get(AppRT rt) => rt.Console;
    static IClock Has<AppRT, IClock>.Get(AppRT rt) => rt.Clock;
    static IStore Has<AppRT, IStore>.Get(AppRT rt) => rt.Store;
}
