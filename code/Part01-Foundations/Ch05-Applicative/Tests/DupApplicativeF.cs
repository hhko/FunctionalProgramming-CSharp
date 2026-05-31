using Ch05.Traits;

namespace Ch05.Tests;

// 동형 사상 법칙 위반 시연 — Pure 가 *두 번* 둘러싼다.
//
// Apply 는 cartesian 처럼 동작 — 두 길이 2 → 결과 길이 4.
// 그러나 Pure(f(a)) 는 항상 길이 2 — 결과 크기가 다르다.
//
// 본문 §5.6.7.B — Pure 의 기준점이 깨지면 Lift2 결과 크기가 예상과 다르다.
public sealed class DupApp<A>(IEnumerable<A> items) : K<DupApplicativeF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();

    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

public sealed class DupApplicativeF : Applicative<DupApplicativeF>
{
    public static K<DupApplicativeF, B> Map<A, B>(Func<A, B> f, K<DupApplicativeF, A> fa) =>
        new DupApp<B>(((DupApp<A>)fa).Items.Select(f));

    // Pure 가 두 번 둘러쌈 — 동형 사상 법칙 위반의 근원.
    public static K<DupApplicativeF, A> Pure<A>(A value) =>
        new DupApp<A>([value, value]);

    public static K<DupApplicativeF, B> Apply<A, B>(
        K<DupApplicativeF, Func<A, B>> mf,
        K<DupApplicativeF, A>          ma) =>
        new DupApp<B>(
            ((DupApp<Func<A, B>>)mf).Items
                .SelectMany(f => ((DupApp<A>)ma).Items.Select(a => f(a))));
}

public static class DupAppExtensions
{
    public static DupApp<A> As<A>(this K<DupApplicativeF, A> fa) => (DupApp<A>)fa;
}
