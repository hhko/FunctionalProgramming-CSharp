using Ch06.Traits;

namespace Ch06.Tests;

// 부수 효과 시연 — step 함수가 외부 카운터를 건드리면 호출 횟수가 관찰 가능.
//
// CountingFoldableF 의 FoldRight / FoldLeft 자체는 정상이지만, *step 함수 안에 부수
// 효과* 가 있으면 *Count* 같은 자유 함수의 두 정의 (FoldRight 기반 vs FoldLeft 기반) 가
// 카운터 값을 다르게 누적한다 — 관찰 가능한 차이.
//
// 본문 §4.6.5 의 핵심 — Foldable 의 step 은 *순수 함수* 여야 한다.
public sealed class CountingList<A>(IEnumerable<A> items) : K<CountingFoldableF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();
    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

public sealed class CountingFoldableF : Foldable<CountingFoldableF>
{
    public static int CallCount { get; private set; }
    public static void Reset() => CallCount = 0;

    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<CountingFoldableF, A> fa)
    {
        var list = ((CountingList<A>)fa).Items;
        var acc = seed;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            CallCount++;                       // ← 부수 효과 — step 호출 횟수 기록.
            acc = f(list[i], acc);
        }
        return acc;
    }

    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<CountingFoldableF, A> fa)
    {
        var list = ((CountingList<A>)fa).Items;
        var acc = seed;
        foreach (var item in list)
        {
            CallCount++;                       // ← 부수 효과 — step 호출 횟수 기록.
            acc = f(acc, item);
        }
        return acc;
    }
}
