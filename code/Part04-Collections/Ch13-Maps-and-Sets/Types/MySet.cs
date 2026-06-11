using Ch13.Traits;

namespace Ch13.Types;

// MySet<A> — 중복 없는 원소 집합. K<SetF, A> 를 구현하고 *Foldable* 만 부착한다.
//
// 왜 Functor 가 *아닌가* — Set 에 Map 을 붙이려면 두 가지가 걸린다:
//   (1) 결과 원소들을 다시 *중복 제거* 하려면 EqualityComparer<B> 가 필요 → 무제약 Map<A,B>
//       시그니처를 어긴다 (K<F,A> 인코딩은 A/B 에 제약을 못 단다).
//   (2) Map 이 서로 다른 원소를 같은 값으로 보내면 *개수가 줄어* 모양 보존 (Functor 법칙) 위반.
// 그래서 Set 은 Foldable (읽기만) 로는 깔끔하지만 Functor 로는 경계 사례다. (Challenges 참고.)
public sealed class MySet<A>(IEnumerable<A> items) : K<SetF, A>
{
    public IReadOnlyList<A> Elements { get; } = Distinct(items);

    static IReadOnlyList<A> Distinct(IEnumerable<A> items)
    {
        var seen = new HashSet<A>();
        var ordered = new List<A>();
        foreach (var x in items)
            if (seen.Add(x))
                ordered.Add(x);
        return ordered;
    }

    public override string ToString() => $"{{{string.Join(", ", Elements)}}}";
}

public sealed class SetF : Foldable<SetF>
{
    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<SetF, A> fa)
    {
        var acc = seed;
        foreach (var a in fa.As().Elements)
            acc = f(acc, a);
        return acc;
    }

    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<SetF, A> fa)
    {
        var els = fa.As().Elements;
        var acc = seed;
        for (var i = els.Count - 1; i >= 0; i--)
            acc = f(els[i], acc);
        return acc;
    }
}

public static class MySetExtensions
{
    public static MySet<A> As<A>(this K<SetF, A> fa) => (MySet<A>)fa;
}
