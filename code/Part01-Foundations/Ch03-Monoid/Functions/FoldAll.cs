using Ch03.Traits;

namespace Ch03.Functions;

// 어떤 Monoid M 이든 목록을 하나로 접는 자유 함수.
//   - M.Empty (타입의 단위원, static) 에서 시작 → 빈 목록도 안전.
//   - acc.Combine(x) (값의 결합 능력, instance) 으로 누적.
//   - 결합 법칙이 묶는 순서 무관을 보장.
public static class FoldAll
{
    public static M Of<M>(IEnumerable<M> items)
        where M : Monoid<M>
    {
        var acc = M.Empty;
        foreach (var x in items)
            acc = acc.Combine(x);
        return acc;
    }
}
