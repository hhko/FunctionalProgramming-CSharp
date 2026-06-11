using Ch30.Types;

namespace Ch30.Tests;

public static class AtomLaws
{
    // ① 단일 스레드 Swap — 순차 적용 결과가 정확하다.
    public static bool SequentialSwapHolds()
    {
        var atom = new Atom<Counter>(new Counter(0));
        for (var i = 0; i < 100; i++) atom.Swap(c => c with { N = c.N + 1 });
        return atom.Value.N == 100;
    }

    // ② 동시 Swap — 잃어버린 갱신 없음 (CAS 가 모든 증가를 반영).
    public static bool ConcurrentNoLostUpdatesHolds()
    {
        var atom = new Atom<Counter>(new Counter(0));
        const int tasks = 8, perTask = 10_000;
        Parallel.For(0, tasks, _ =>
        {
            for (var i = 0; i < perTask; i++)
                atom.Swap(c => c with { N = c.N + 1 });
        });
        return atom.Value.N == (long)tasks * perTask;
    }
}
