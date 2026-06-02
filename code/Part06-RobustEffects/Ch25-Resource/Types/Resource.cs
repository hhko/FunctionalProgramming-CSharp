namespace Ch25.Types;

// bracket — *획득 → 사용 → 해제* 를 한 값으로 묶는다. 사용 중 예외가 나도 해제는 *반드시* 실행된다.
// (LanguageExt v5 의 bracket / use·release 의 핵심 의미.) C# 의 try/finally 를 함수형으로 캡처.
public static class Resource
{
    public static B Bracket<A, B>(Func<A> acquire, Func<A, B> use, Action<A> release)
    {
        var resource = acquire();      // 획득
        try
        {
            return use(resource);      // 사용
        }
        finally
        {
            release(resource);         // 해제 — 예외가 나도 반드시
        }
    }
}

// Resources — 여러 자원을 추적하고 *역순(LIFO)* 으로 해제한다. 예외에도 전부 해제 보장.
// (열린 순서 A→B 면 닫히는 순서 B→A — 중첩 자원의 올바른 정리.)
public sealed class Resources
{
    readonly Stack<Action> releases = new();
    public List<string> Log { get; } = [];

    public A Acquire<A>(string name, Func<A> acquire, Action<A> release)
    {
        var value = acquire();
        Log.Add($"acquire {name}");
        releases.Push(() => { release(value); Log.Add($"release {name}"); });
        return value;
    }

    public void ReleaseAll()
    {
        while (releases.Count > 0)
            releases.Pop()();      // LIFO
    }
}
