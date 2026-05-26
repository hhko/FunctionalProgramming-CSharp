using Ch03.Traits;

namespace Ch03.Tests;

// 법칙 위반 시연 — 부수 효과를 가진 Functor 가 합성 법칙을 어긴다.
//
// CountingListF.Map 은 *부수 효과* (호출 카운터 증가) 를 갖는다. 시퀀스 변환 자체는
// 올바르지만, Map(g) ∘ Map(f) 는 카운터를 2 회 증가시키고 Map(g ∘ f) 는 1 회만
// 증가시킨다. 두 호출의 *관찰 가능한 차이* 가 합성 법칙 위반의 증거.
//
// 본문 §3.6.6 의 핵심 — Functor 의 Map 은 *순수 변환* 이어야 한다.
public sealed class CountingList<A>(IEnumerable<A> items) : K<CountingListF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();

    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

public sealed class CountingListF : Functor<CountingListF>
{
    public static int CallCount { get; private set; }

    public static void Reset() => CallCount = 0;

    public static K<CountingListF, B> Map<A, B>(Func<A, B> f, K<CountingListF, A> fa)
    {
        CallCount++;                       // ← 부수 효과 — 호출 횟수를 기록한다.
        var list = (CountingList<A>)fa;
        return new CountingList<B>(list.Items.Select(f));
    }
}

public static class CountingListExtensions
{
    public static CountingList<A> As<A>(this K<CountingListF, A> fa) => (CountingList<A>)fa;
}
