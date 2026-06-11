using Ch37.Types;

namespace Ch37.Tests;

// Atom<T> 의 *결정적* 불변식을 검증합니다.
// 동시성 테스트의 핵심 — 스레드 *순서* 는 비결정적이지만, 순수 갱신의 *최종값* 은 항상 같습니다.
public static class AtomTests
{
    // ① 순차 Swap — 100 번 증가하면 최종값은 정확히 100.
    public static bool SequentialSwapHolds()
    {
        var atom = new Atom<Counter>(new Counter(0));
        for (var i = 0; i < 100; i++)
            atom.Swap(c => c with { N = c.N + 1 });
        return atom.Value.N == 100;
    }

    // ② 동시 Swap (Parallel.For) — 1000 개의 동시 증가 후 최종값은 항상 정확히 1000.
    //    잃어버린 갱신이 없다 = CAS 가 모든 증가를 반영했다 (원자성 성립).
    //
    //    [비교] 비원자 카운터(예: long n; n++)를 같은 방식으로 동시 증가하면 갱신이 유실되어
    //    1000 보다 작아질 수 *있습니다*. 그 결과는 비결정적이므로 단언 대상이 아니고, 여기서는
    //    atom 의 결정성만 단언합니다.
    public static bool ConcurrentParallelForHolds()
    {
        var atom = new Atom<Counter>(new Counter(0));
        Parallel.For(0, 1000, _ => atom.Swap(c => c with { N = c.N + 1 }));
        return atom.Value.N == 1000;
    }

    // ③ 동시 Swap (Task 1000 개) — 다른 동시성 경로에서도 최종값은 항상 정확히 1000.
    public static bool ConcurrentTasksHolds()
    {
        var atom = new Atom<Counter>(new Counter(0));
        var tasks = new Task[1000];
        for (var i = 0; i < tasks.Length; i++)
            tasks[i] = Task.Run(() => atom.Swap(c => c with { N = c.N + 1 }));
        Task.WaitAll(tasks);
        return atom.Value.N == 1000;
    }
}
