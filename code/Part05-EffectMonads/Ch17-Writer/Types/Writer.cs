using Ch17.Traits;

namespace Ch17.Types;

// Writer<W, A> — 결과 값 A 와 누적 출력 W 의 *쌍*. (W 는 Monoid.)
//
// LanguageExt v5 는 효율을 위해 `Func<W,(A,W)>` 형태를 쓰지만, 누적의 본질을 가장 또렷이
// 보여 주는 *고전적 쌍 표현* `(A, W)` 으로 학습한다. Bind 가 두 출력을 Monoid 로 합친다.
public sealed class Writer<W, A>(A value, W output) : K<WriterF<W>, A>
    where W : Monoid<W>
{
    public A Value { get; } = value;
    public W Output { get; } = output;

    public (A Value, W Output) Run() => (Value, Output);
    public override string ToString() => $"({Value}, {Output})";
}

public sealed class WriterF<W> : Monad<WriterF<W>>, Writable<WriterF<W>, W>
    where W : Monoid<W>
{
    public static K<WriterF<W>, B> Map<A, B>(Func<A, B> f, K<WriterF<W>, A> fa)
    {
        var w = fa.As();
        return new Writer<W, B>(f(w.Value), w.Output);
    }

    // Pure — 값만, 출력은 항등원 Empty.
    public static K<WriterF<W>, A> Pure<A>(A value) =>
        new Writer<W, A>(value, W.Empty);

    // Apply — 두 출력을 Combine 으로 누적.
    public static K<WriterF<W>, B> Apply<A, B>(K<WriterF<W>, Func<A, B>> mf, K<WriterF<W>, A> ma)
    {
        var wf = mf.As();
        var wa = ma.As();
        return new Writer<W, B>(wf.Value(wa.Value), wf.Output.Combine(wa.Output));
    }

    // Bind — 첫 출력 w1 과 다음 출력 w2 를 Monoid 로 합친다 (누적의 핵심).
    public static K<WriterF<W>, B> Bind<A, B>(K<WriterF<W>, A> ma, Func<A, K<WriterF<W>, B>> f)
    {
        var w1 = ma.As();
        var w2 = f(w1.Value).As();
        return new Writer<W, B>(w2.Value, w1.Output.Combine(w2.Output));
    }

    // Writable.Tell — 값은 없고(Unit) 출력만 남긴다.
    public static K<WriterF<W>, Unit> Tell(W item) =>
        new Writer<W, Unit>(Unit.Default, item);

    // Writable.Listen — 계산이 "말한" 출력을 값에 함께 담는다.
    public static K<WriterF<W>, (A Value, W Output)> Listen<A>(K<WriterF<W>, A> ma)
    {
        var w = ma.As();
        return new Writer<W, (A, W)>((w.Value, w.Output), w.Output);
    }

    // Writable.Pass — 출력을 후처리하는 함수를 적용 (검열/필터).
    public static K<WriterF<W>, A> Pass<A>(K<WriterF<W>, (A Value, Func<W, W> Function)> action)
    {
        var w = action.As();
        var (value, fn) = w.Value;
        return new Writer<W, A>(value, fn(w.Output));
    }
}

public static class WriterExtensions
{
    public static Writer<W, A> As<W, A>(this K<WriterF<W>, A> fa)
        where W : Monoid<W> =>
        (Writer<W, A>)fa;
}
