namespace Ch37.Types;

// Resource — *획득 → 사용 → 해제* 를 한 함수로 묶는 bracket. 사용 중 예외가 나도 해제는 *반드시* 실행됩니다.
// (LanguageExt v5 의 use·release / bracket 의 축소판. C# 의 try/finally 를 함수형으로 캡처.)
//
// 테스트 관점 — "예외가 나도 해제됐는가" 는 release 가 flag 를 세우는지로, "중첩 자원의 종료 순서"
// 는 종료 로그가 LIFO 인지로 결정적으로 단언합니다. 자원 수명 자체가 검증 대상입니다.
public static class Resource
{
    public static R Use<T, R>(Func<T> acquire, Func<T, R> use, Action<T> release)
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

// Scope — 여러 자원을 추적하고 *역순(LIFO)* 으로 해제합니다. 예외에도 전부 해제 보장.
// (열린 순서 A→B 면 닫히는 순서 B→A — 중첩 자원의 올바른 정리.)
public sealed class Scope
{
    readonly Stack<Action> releases = new();

    // 종료 순서를 기록하는 로그 — LIFO 검증의 골든 대상.
    public List<string> Log { get; } = [];

    public T Acquire<T>(string name, Func<T> acquire, Action<T> release)
    {
        var value = acquire();
        Log.Add($"acquire {name}");
        releases.Push(() => { release(value); Log.Add($"release {name}"); });
        return value;
    }

    public void ReleaseAll()
    {
        while (releases.Count > 0)
            releases.Pop()();          // LIFO — 마지막에 연 것부터 닫는다
    }
}
