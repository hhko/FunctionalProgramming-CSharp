using Ch23.Functions;
using Ch23.Traits;
using Ch23.Types;

namespace Ch23.Challenges;

// 챌린지 ① 정답 — 리스트를 IO 로 순회해 하나의 IO 로 접는다 (스택 안전).
// foldl 처럼 Bind 를 쌓지만, 트램폴린 인터프리터 덕에 큰 리스트도 스택을 안 터뜨린다.
public static class IOForEach
{
    // 각 원소에 효과 step 을 적용하며 누적 (결과는 누적 합 같은 단일 값).
    public static IO<long> SumWith(IEnumerable<int> items, Func<int, long> step)
    {
        K<IOF, long> acc = IOF.Pure(0L);
        foreach (var x in items)
        {
            var captured = x;
            acc = acc.Bind(sum => IOF.Pure(sum + step(captured)));
        }
        return acc.As();
    }
}
