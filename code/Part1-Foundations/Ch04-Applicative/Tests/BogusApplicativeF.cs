using Ch04.Traits;

namespace Ch04.Tests;

// 항등 법칙 위반 시연 — Apply 가 입력과 무관하게 빈 결과를 돌려준다.
//
// 시그니처는 통과하지만 *Apply(Pure(id), fa) ≡ fa* 의 약속을 어긴다. 컴파일러는 막을 수 없지만
// Applicative 의 결합 의미가 무너진다.
//
// 본문 §5.6.7.A — 시그니처 외에 추가되는 *Applicative 의 네 법칙* 이 필요한 이유.
public sealed class BogusApp<A>(IEnumerable<A> items) : K<BogusApplicativeF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();

    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

public sealed class BogusApplicativeF : Applicative<BogusApplicativeF>
{
    public static K<BogusApplicativeF, B> Map<A, B>(Func<A, B> f, K<BogusApplicativeF, A> fa) =>
        new BogusApp<B>(((BogusApp<A>)fa).Items.Select(f));

    public static K<BogusApplicativeF, A> Pure<A>(A value) =>
        new BogusApp<A>([value]);

    // 입력 무시 — 항상 빈 결과. 항등 법칙 위반.
    public static K<BogusApplicativeF, B> Apply<A, B>(
        K<BogusApplicativeF, Func<A, B>> mf,
        K<BogusApplicativeF, A>          ma) =>
        new BogusApp<B>([]);
}

public static class BogusAppExtensions
{
    public static BogusApp<A> As<A>(this K<BogusApplicativeF, A> fa) => (BogusApp<A>)fa;
}
