namespace Ch30.Types;

// Cell — 스트림의 한 조각: 머리(Head) + 나머지 스트림(Tail).
internal sealed record Cell<A>(A Head, StreamT<A> Tail);

// StreamT<A> — *효과적 lazy 스트림*. 한 조각을 *당길 때마다* (Pull) 계산이 일어난다.
// 여기서 효과는 "당길 때 실행되는 부수 작용" (= 내부 모나드 IO 의 한 스텝). Run 전엔 아무 일도 없다.
//
// 핵심 — Tail 은 *Pull 될 때만* 계산되므로, 무한/대용량 스트림도 끝까지 materialize 되지 않는다.
// (LanguageExt v5 StreamT<M,A> 의 발상 — 내부 M 위에서 한 조각씩 흘림.)
public sealed class StreamT<A>
{
    readonly Func<Cell<A>?> pull;
    internal StreamT(Func<Cell<A>?> pull) => this.pull = pull;

    // 한 조각 당기기 — null 이면 끝, 아니면 (머리, 나머지). 이때 부수 작용이 실행된다.
    internal Cell<A>? Pull() => pull();

    public static StreamT<A> Empty => new(() => null);

    // Cons — 머리와 나머지를 *지연* 으로 잇는다 (tail 은 Pull 시점에 계산).
    public static StreamT<A> Cons(Func<A> head, Func<StreamT<A>> tail) =>
        new(() => new Cell<A>(head(), tail()));

    public StreamT<B> Map<B>(Func<A, B> f) =>
        new(() => Pull() is { } c ? new Cell<B>(f(c.Head), c.Tail.Map(f)) : null);

    public StreamT<A> Filter(Func<A, bool> p) =>
        new(() =>
        {
            var s = this;
            while (true)
            {
                var c = s.Pull();
                if (c is null) return null;
                if (p(c.Head)) return new Cell<A>(c.Head, c.Tail.Filter(p));
                s = c.Tail;
            }
        });

    public StreamT<A> Take(int n) =>
        n <= 0 ? Empty : new(() => Pull() is { } c ? new Cell<A>(c.Head, c.Tail.Take(n - 1)) : null);

    // 소비 — 끝까지 당겨 리스트로 (여기서 비로소 모든 효과가 실행된다).
    public List<A> ToList()
    {
        var result = new List<A>();
        var s = this;
        while (s.Pull() is { } c) { result.Add(c.Head); s = c.Tail; }
        return result;
    }
}

// 생성기 모음.
public static class Streams
{
    // 무한 자연수 스트림 start, start+1, ...  (당길 때마다 한 칸씩).
    public static StreamT<int> From(int start) =>
        StreamT<int>.Cons(() => start, () => From(start + 1));

    // 당길 때마다 log 에 기록하는 무한 스트림 (효과가 *당길 때만* 일어남을 보이기 위함).
    public static StreamT<int> Tapped(int start, List<int> log) =>
        StreamT<int>.Cons(() => { log.Add(start); return start; }, () => Tapped(start + 1, log));
}
