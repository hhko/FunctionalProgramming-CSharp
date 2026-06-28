using Ch20.Traits;

namespace Ch20.Types;

// Log — WriterT 의 출력 W 로 쓰는 *Monoid*. 빈 로그가 항등원, 이어 붙이기가 Combine.
// (3부 Monoid 의 항등원 · Combine 이 변환기에서도 그대로 재사용된다.)
public sealed class Log(IReadOnlyList<string> lines) : Monoid<Log>
{
    public IReadOnlyList<string> Lines { get; } = lines;

    public static Log Empty { get; } = new([]);
    public static Log Combine(Log a, Log b) => new([.. a.Lines, .. b.Lines]);

    // 한 줄짜리 로그를 만드는 편의 함수.
    public static Log One(string line) => new([line]);

    public override string ToString() => $"[{string.Join("; ", Lines)}]";
}

// WriterT<W, M, A> — 출력 누적 효과를 *내부 모나드 M* 위에 얹는다. 내부는 `K<M, (A, W)>`.
// 5부의 Writer<W, A> = 쌍 (A, W) 를, 내부 M 위로 올린 모양이다. 출력 W 는 Monoid 라야
// Bind 가 두 단계의 로그를 Combine 으로 이어 붙일 수 있다.
//
// (LanguageExt v5 의 WriterT 는 성능을 위해 `Func<W, K<M, (A, W)>>` 함수형 표현을 쓰지만,
//  여기서는 입문 눈높이로 쌍을 그대로 M 위에 올린 최소 표현을 쓴다. Bind·Tell 의 발상은 같다.)
public sealed class WriterT<W, M, A>(K<M, (A Value, W Output)> run) : K<WriterTF<W, M>, A>
    where M : Monad<M>
    where W : Monoid<W>
{
    public K<M, (A Value, W Output)> Run { get; } = run;
}

public sealed class WriterTF<W, M> : MonadT<WriterTF<W, M>, M>, Writable<WriterTF<W, M>, W>
    where M : Monad<M>
    where W : Monoid<W>
{
    public static K<WriterTF<W, M>, A> Pure<A>(A value) =>
        new WriterT<W, M, A>(M.Pure((value, W.Empty)));

    public static K<WriterTF<W, M>, B> Map<A, B>(Func<A, B> f, K<WriterTF<W, M>, A> fa) =>
        new WriterT<W, M, B>(M.Map(t => (f(t.Value), t.Output), fa.As().Run));

    public static K<WriterTF<W, M>, B> Apply<A, B>(K<WriterTF<W, M>, Func<A, B>> mf, K<WriterTF<W, M>, A> ma) =>
        Bind(mf, f => Map(f, ma));

    // 두 층을 푸는 배관 — M 효과는 M.Bind 로 흘리고, 출력 W 는 Monoid 의 Combine 으로 누적.
    public static K<WriterTF<W, M>, B> Bind<A, B>(K<WriterTF<W, M>, A> ma, Func<A, K<WriterTF<W, M>, B>> f) =>
        new WriterT<W, M, B>(
            M.Bind(ma.As().Run, t1 =>
                M.Map(t2 => (t2.Value, W.Combine(t1.Output, t2.Output)), f(t1.Value).As().Run)));

    // Lift — 내부 모나드 계산을 WriterT 한 층 위로 (로그는 빈 항등원으로 채운다).
    public static K<WriterTF<W, M>, A> Lift<A>(K<M, A> ma) =>
        new WriterT<W, M, A>(M.Map(a => (a, W.Empty), ma));

    // Tell — 값 없이 출력 한 조각만 누적 (Unit 을 값으로).
    public static K<WriterTF<W, M>, Unit> Tell(W output) =>
        new WriterT<W, M, Unit>(M.Pure((Unit.Default, output)));
}

public static class WriterTExtensions
{
    public static WriterT<W, M, A> As<W, M, A>(this K<WriterTF<W, M>, A> fa)
        where M : Monad<M>
        where W : Monoid<W> =>
        (WriterT<W, M, A>)fa;
}
