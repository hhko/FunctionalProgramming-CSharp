using Ch30.Types;

namespace Ch30.Challenges;

// 챌린지 ② 정답 — *부수 효과가 있는* swap 함수가 CAS 재시도에서 왜 위험한지 시연.
//
// 최종 Value 는 정확(4000)하지만, swap 함수 안의 부수 효과(카운터)은 *재시도마다 다시 실행* 되어
// 4000 보다 커진다. → Swap 의 함수는 반드시 *순수* 해야 한다 (재적용이 안전하려면).
public static class ImpureSwapWarning
{
    public static (long Value, int SideEffectCalls) Demonstrate()
    {
        var atom = new Atom<Counter>(new Counter(0));
        var sideEffectCalls = 0;
        Parallel.For(0, 4, _ =>
        {
            for (var i = 0; i < 1000; i++)
                atom.Swap(c =>
                {
                    Interlocked.Increment(ref sideEffectCalls);   // ← 순수하지 않은 작용
                    return c with { N = c.N + 1 };
                });
        });
        return (atom.Value.N, sideEffectCalls);   // Value == 4000, SideEffectCalls >= 4000
    }
}
