using Ch11.Traits;

namespace Ch11.Types;

// 학습용 단순 MyList — 11장 NaturalTransformation 데모 한정.
public sealed record MyList<A>(IReadOnlyList<A> Items) : K<MyListF, A>
{
    public static MyList<A> Of(params A[] items) => new(items);
    public static MyList<A> Empty => new(Array.Empty<A>());
    public bool IsEmpty => Items.Count == 0;
    public A Head => Items[0];
}

// 태그 타입.
public sealed class MyListF { }
