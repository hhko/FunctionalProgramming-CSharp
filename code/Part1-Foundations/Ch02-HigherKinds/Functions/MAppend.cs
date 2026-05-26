using Ch02.Traits;

namespace Ch02.Functions;

// *어떤 Monoid 든* 받는 일반 함수 — 3-tuple 패턴의 첫 ROI.
//
// 빈 시퀀스는 `Empty`, N 개 원소는 `Add` 누적. 컴파일러가 `where T : Monoid<T>` 만 보면
// `T.Empty`, `T.Add(...)` 호출이 보장된다 — 구체 타입 (`Sum`, `Product`) 을 *몰라도* 동작.
public static class MAppend
{
    public static T Run<T>(IEnumerable<T> items) where T : Monoid<T>
    {
        var acc = T.Empty;
        foreach (var item in items)
            acc = T.Add(acc, item);
        return acc;
    }
}
