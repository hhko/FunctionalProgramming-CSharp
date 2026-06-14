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

// 태그 타입 — 키 Key 를 고정한 채 값에 대한 Functor / Foldable / Traversable 을 호스트.
public sealed class MapF<Key> : Functor<MapF<Key>>, Foldable<MapF<Key>>, Traversable<MapF<Key>>
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

    // Traversable.Traverse — 값들을 한꺼번에 effect 로 변환. Map<K, F<V>> → F<Map<K, V>>.
    // 키는 보존하고 안쪽 effect (Maybe / Validation) 만 바깥으로 모은다.
    // 12장 MySeq.Traverse 와 골격이 같되, 값 자리에 (key, b) 쌍을 다시 끼워 넣는 부분만 다르다.
    public static K<F, K<MapF<Key>, B>> Traverse<F, A, B>(Func<A, K<F, B>> f, K<MapF<Key>, A> ta)
        where F : Applicative<F>
    {
        var pairs = ta.As().Pairs;
        K<F, K<MapF<Key>, B>> acc = F.Pure<K<MapF<Key>, B>>(new MyMap<Key, B>([]));

        for (var i = pairs.Count - 1; i >= 0; i--)
        {
            var key = pairs[i].Key;
            var fb  = f(pairs[i].Value);

            // (key, b) 를 누적 Map 앞에 끼워 넣는 함수 — Apply 두 번으로 effect 안에서 수행.
            Func<B, Func<K<MapF<Key>, B>, K<MapF<Key>, B>>> insert =
                b => rest => new MyMap<Key, B>(
                    rest.As().Pairs.Prepend(new KeyValuePair<Key, B>(key, b)));

            var lifted = F.Pure(insert);
            var step1  = F.Apply<B, Func<K<MapF<Key>, B>, K<MapF<Key>, B>>>(lifted, fb);
            acc        = F.Apply<K<MapF<Key>, B>, K<MapF<Key>, B>>(step1, acc);
        }
        return acc;
    }
}

public static class MyMapExtensions
{
    public static MyMap<Key, V> As<Key, V>(this K<MapF<Key>, V> fa)
        where Key : notnull =>
        (MyMap<Key, V>)fa;
}
