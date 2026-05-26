using Ch05.Traits;

namespace Ch05.Types;

public sealed class MyList<A>(IEnumerable<A> items) : K<MyListF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();
    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

public sealed class MyListF : Foldable<MyListF>
{
    // FoldRight — 오른쪽 (마지막 원소) 부터 seed 와 합쳐 왼쪽으로 진행.
    //   [a, b, c]  →  f(a, f(b, f(c, seed)))
    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<MyListF, A> fa)
    {
        var list = fa.As();
        var acc = seed;
        for (int i = list.Items.Count - 1; i >= 0; i--)
            acc = f(list.Items[i], acc);
        return acc;
    }

    // FoldLeft — 왼쪽 (첫 원소) 부터 seed 와 합쳐 오른쪽으로 진행.
    //   [a, b, c]  →  f(f(f(seed, a), b), c)
    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<MyListF, A> fa)
    {
        var list = fa.As();
        var acc = seed;
        foreach (var item in list.Items)
            acc = f(acc, item);
        return acc;
    }
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyListExtensions
{
    public static MyList<A> As<A>(this K<MyListF, A> fa) => (MyList<A>)fa;
}
