namespace Ch34.Types;

// Pipes — 데이터 파이프라인을 세 역할의 *합성* 으로 본다 (코루틴 / 당김 기반):
//   Producer<O>    : 값을 yield (생산)
//   Pipe<I, O>     : I 를 받아 O 로 변환
//   Consumer<I, R> : I 를 소비해 R 로 (소비/접기)
// 합성:  producer.Through(pipe).Run(consumer)   (= LanguageExt 의 producer | pipe | consumer)
//
// 당김 기반 — Consumer 가 당긴 만큼만 Producer 가 생산한다 (역압 backpressure 가 자연히 생김).

// Producer<O> — 한 조각을 당기면 (값, 다음 Producer) 또는 null(끝).
public sealed class Producer<O>
{
    readonly Func<(O Value, Producer<O> Next)?> pull;
    internal Producer(Func<(O, Producer<O>)?> pull) => this.pull = pull;
    internal (O Value, Producer<O> Next)? Pull() => pull();

    public Producer<O2> Through<O2>(Pipe<O, O2> pipe) => pipe.Apply(this);
    public R Run<R>(Consumer<O, R> consumer) => consumer.Apply(this);
}

public static class Producer
{
    public static Producer<int> From(int start) =>
        new(() => (start, From(start + 1)));                  // 무한 (당긴 만큼만)

    public static Producer<int> Tapped(int start, List<int> log) =>
        new(() => { log.Add(start); return (start, Tapped(start + 1, log)); });

    static Producer<O> FromIndex<O>(IReadOnlyList<O> xs, int i) =>
        new(() => i < xs.Count ? (xs[i], FromIndex(xs, i + 1)) : null);

    public static Producer<O> Of<O>(params O[] items) => FromIndex(items, 0);
}

// Pipe<I, O> — Producer<I> 를 Producer<O> 로 변환하는 합성 가능한 변환기.
public sealed class Pipe<I, O>
{
    readonly Func<Producer<I>, Producer<O>> run;
    internal Pipe(Func<Producer<I>, Producer<O>> run) => this.run = run;
    public Producer<O> Apply(Producer<I> p) => run(p);

    public Pipe<I, P> Then<P>(Pipe<O, P> next) => new(p => next.Apply(Apply(p)));
}

public static class Pipe
{
    public static Pipe<I, O> Map<I, O>(Func<I, O> f) =>
        new(p => MapP(p, f));
    static Producer<O> MapP<I, O>(Producer<I> p, Func<I, O> f) =>
        new(() => p.Pull() is { } c ? (f(c.Value), MapP(c.Next, f)) : null);

    public static Pipe<I, I> Filter<I>(Func<I, bool> pred) =>
        new(p => FilterP(p, pred));
    static Producer<I> FilterP<I>(Producer<I> p, Func<I, bool> pred) =>
        new(() =>
        {
            var s = p;
            while (s.Pull() is { } c)
            {
                if (pred(c.Value)) return (c.Value, FilterP(c.Next, pred));
                s = c.Next;
            }
            return null;
        });

    public static Pipe<I, I> Take<I>(int n) =>
        new(p => n <= 0 ? new Producer<I>(() => null)
                        : new Producer<I>(() => p.Pull() is { } c ? (c.Value, Take<I>(n - 1).Apply(c.Next)) : null));
}

// Consumer<I, R> — Producer<I> 를 당겨 R 로 접는다.
public sealed class Consumer<I, R>
{
    readonly Func<Producer<I>, R> run;
    internal Consumer(Func<Producer<I>, R> run) => this.run = run;
    public R Apply(Producer<I> p) => run(p);
}

public static class Consumer
{
    public static Consumer<int, int> Sum() =>
        new(p =>
        {
            var s = 0;
            var cur = p;
            while (cur.Pull() is { } c) { s += c.Value; cur = c.Next; }
            return s;
        });

    public static Consumer<I, List<I>> ToList<I>() =>
        new(p =>
        {
            var list = new List<I>();
            var cur = p;
            while (cur.Pull() is { } c) { list.Add(c.Value); cur = c.Next; }
            return list;
        });
}
