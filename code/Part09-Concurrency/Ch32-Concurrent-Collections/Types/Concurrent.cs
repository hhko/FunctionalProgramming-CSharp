using System.Collections.Immutable;

namespace Ch32.Types;

// AtomHashMap<K,V> — 27장 Atom 의 CAS 를 *불변 컬렉션* 에 적용. 락 없이 동시 갱신 안전.
// (2부 불변 Map 을 원자적 참조로 감싼 형태 = LanguageExt AtomHashMap 의 발상.)
public sealed class AtomHashMap<K, V> where K : notnull
{
    ImmutableDictionary<K, V> map = ImmutableDictionary<K, V>.Empty;

    public ImmutableDictionary<K, V> Snapshot => Volatile.Read(ref map);
    public int Count => Snapshot.Count;

    // CAS 루프 — 불변 맵을 새 버전으로 교체. 충돌 시 재시도 (잃어버린 삽입 없음).
    public void AddOrUpdate(K key, V value)
    {
        while (true)
        {
            var cur = Volatile.Read(ref map);
            var next = cur.SetItem(key, value);
            if (Interlocked.CompareExchange(ref map, next, cur) == cur) return;
        }
    }
}

// VectorClock — 분산 이벤트의 *인과성(부분 순서)* 추적. 노드별 논리 시계.
public sealed class VectorClock
{
    public ImmutableDictionary<string, long> Times { get; }

    public VectorClock(ImmutableDictionary<string, long>? times = null) =>
        Times = times ?? ImmutableDictionary<string, long>.Empty;

    long Get(string node) => Times.TryGetValue(node, out var v) ? v : 0;

    // 한 노드에서 이벤트 발생 → 그 노드의 시계 +1.
    public VectorClock Increment(string node) =>
        new(Times.SetItem(node, Get(node) + 1));

    // 메시지 수신 시 두 시계를 노드별 max 로 병합.
    public VectorClock Merge(VectorClock other)
    {
        var builder = ImmutableDictionary<string, long>.Empty.ToBuilder();
        foreach (var k in Times.Keys.Union(other.Times.Keys))
            builder[k] = Math.Max(Get(k), other.Get(k));
        return new VectorClock(builder.ToImmutable());
    }

    public enum Ordering { Equal, Before, After, Concurrent }

    // 부분 순서 비교 — 모든 성분 ≤ 면 Before, ≥ 면 After, 섞이면 Concurrent (인과 무관).
    public Ordering Compare(VectorClock other)
    {
        bool le = true, ge = true;
        foreach (var k in Times.Keys.Union(other.Times.Keys))
        {
            var a = Get(k);
            var b = other.Get(k);
            if (a < b) ge = false;
            if (a > b) le = false;
        }
        return (le, ge) switch
        {
            (true, true) => Ordering.Equal,
            (true, false) => Ordering.Before,
            (false, true) => Ordering.After,
            _ => Ordering.Concurrent
        };
    }

    public override string ToString() =>
        "{" + string.Join(", ", Times.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}:{kv.Value}")) + "}";
}
