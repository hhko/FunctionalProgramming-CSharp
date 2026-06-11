using Ch13.Traits;

namespace Ch13.Types;

// MyMap<Key, V> — 키-값 컨테이너. K<MapF<Key>, V> 를 구현.
//
// 결정적 — trait 의 A 자리에 들어가는 것은 *값 V* 뿐이다. 키 Key 는 태그 MapF<Key> 안에 갇혀
// Map / Fold 가 절대 건드리지 않는다. "값만 끌어올린다" 가 키-값 Functor 의 핵심.
//
// 학습용이라 내부는 키로 정렬된 불변 배열 (외부 패키지 없이 .NET 내장만). 순회 순서가 결정적.
public sealed class MyMap<Key, V>(IEnumerable<KeyValuePair<Key, V>> pairs) : K<MapF<Key>, V>
    where Key : notnull
{
    public IReadOnlyList<KeyValuePair<Key, V>> Pairs { get; } =
        pairs.OrderBy(p => p.Key, Comparer<Key>.Default).ToList();

    public IEnumerable<V> Values => Pairs.Select(p => p.Value);

    public override string ToString() =>
        $"{{{string.Join(", ", Pairs.Select(p => $"{p.Key}: {p.Value}"))}}}";
}

// 태그 타입 — 키 Key 를 고정한 채 값에 대한 Functor / Foldable 을 호스트.
public sealed class MapF<Key> : Functor<MapF<Key>>, Foldable<MapF<Key>>
    where Key : notnull
{
    // Functor — 값에만 f 적용, 키는 보존.
    public static K<MapF<Key>, B> Map<A, B>(Func<A, B> f, K<MapF<Key>, A> fa) =>
        new MyMap<Key, B>(
            fa.As().Pairs.Select(p => new KeyValuePair<Key, B>(p.Key, f(p.Value))));

    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<MapF<Key>, A> fa)
    {
        var acc = seed;
        foreach (var p in fa.As().Pairs)
            acc = f(acc, p.Value);
        return acc;
    }

    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<MapF<Key>, A> fa)
    {
        var pairs = fa.As().Pairs;
        var acc = seed;
        for (var i = pairs.Count - 1; i >= 0; i--)
            acc = f(pairs[i].Value, acc);
        return acc;
    }
}

public static class MyMapExtensions
{
    public static MyMap<Key, V> As<Key, V>(this K<MapF<Key>, V> fa)
        where Key : notnull =>
        (MyMap<Key, V>)fa;
}
