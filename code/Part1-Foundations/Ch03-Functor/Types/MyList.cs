using Ch03.Traits;

namespace Ch03.Types;

// 자료 타입 — 시퀀스를 들고 있고 K<MyListF, A> 를 구현.
public sealed class MyList<A>(IEnumerable<A> items) : K<MyListF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();

    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

// 태그 타입 — Functor 능력의 정적 호스트.
public sealed class MyListF : Functor<MyListF>
{
    public static K<MyListF, B> Map<A, B>(Func<A, B> f, K<MyListF, A> fa)
    {
        var list = fa.As();
        return new MyList<B>(list.Items.Select(f));
    }
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyListExtensions
{
    public static MyList<A> As<A>(this K<MyListF, A> fa) => (MyList<A>)fa;
}
