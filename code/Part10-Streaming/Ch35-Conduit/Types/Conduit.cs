namespace Ch35.Types;

// Conduit<I, O> — 또 다른 스트리밍 추상. I 스트림을 O 스트림으로 바꾸는 *변환관(transducer)*.
// Pipes 가 Producer/Consumer/Pipe 세 역할로 나뉜 데 비해, Conduit 은 변환 단계를 하나로 합쳐
// `IEnumerable<I> → IEnumerable<O>` 로 본다 (lazy LINQ 위). Then 으로 단계를 잇는다.
public sealed class Conduit<I, O>(Func<IEnumerable<I>, IEnumerable<O>> run)
{
    public IEnumerable<O> Apply(IEnumerable<I> input) => run(input);
    public Conduit<I, P> Then<P>(Conduit<O, P> next) => new(xs => next.Apply(Apply(xs)));
}

public static class Conduit
{
    public static Conduit<I, O> Map<I, O>(Func<I, O> f) => new(xs => xs.Select(f));
    public static Conduit<I, I> Filter<I>(Func<I, bool> p) => new(xs => xs.Where(p));
    public static Conduit<I, O> Choose<I, O>(Func<I, (bool ok, O value)> f) =>
        new(xs => xs.Select(f).Where(r => r.ok).Select(r => r.value));
}

// 실전 파이프라인 — *자원 안전한* 소스(열기/닫기 보장) 위에서 Conduit 변환 후 sink 로 접는다.
// (6부 bracket 의 finally 보장을 스트리밍에 결합.)
public static class Pipeline
{
    // events 는 호출자 소유 — 예외가 나도 (finally 실행 후) 닫힘 기록을 관찰할 수 있다.
    public static R RunOverFile<O, R>(
        IReadOnlyList<string> fileLines,
        Conduit<string, O> conduit,
        Func<IEnumerable<O>, R> sink,
        List<string> events)
    {
        events.Add("open file");
        try
        {
            return sink(conduit.Apply(fileLines));
        }
        finally
        {
            events.Add("close file");   // 예외에도 반드시 닫힘
        }
    }
}
