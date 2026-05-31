using Ch05.Traits;

namespace Ch05.Challenges;

// 심화 챌린지 C — Pair<W, A> = Writer 의 단순형 Applicative.
//
// Pair<W, A> 는 *부수 로그 W* 와 *값 A* 의 쌍.
// Pure 가 (W 의 항등원, value), Apply 가 (w1 + w2, f(a)) — 로그를 *문자열 concat* 으로 누적.
//
// 본 챌린지는 W = string 으로 고정 (학습용). 일반화하면 W 가 임의 Monoid 면 된다.
public sealed record Pair<A>(string Log, A Value) : K<PairF, A>
{
    public override string ToString() => $"({Log}, {Value})";
}

public sealed class PairF : Applicative<PairF>
{
    public static K<PairF, B> Map<A, B>(Func<A, B> f, K<PairF, A> fa)
    {
        var p = (Pair<A>)fa;
        return new Pair<B>(p.Log, f(p.Value));
    }

    // Pure — 로그 항등원 (빈 문자열) + 값.
    public static K<PairF, A> Pure<A>(A value) =>
        new Pair<A>(string.Empty, value);

    // Apply — 로그를 monoid 로 누적, 함수와 값 결합.
    public static K<PairF, B> Apply<A, B>(K<PairF, Func<A, B>> mf, K<PairF, A> ma)
    {
        var pf = (Pair<Func<A, B>>)mf;
        var pa = (Pair<A>)ma;
        return new Pair<B>(pf.Log + pa.Log, pf.Value(pa.Value));
    }
}

public static class PairExtensions
{
    public static Pair<A> As<A>(this K<PairF, A> fa) => (Pair<A>)fa;
}
