using Ch19.Traits;

namespace Ch19.Types;

// Many — *비결정성* 효과 (여러 결과). OptionT 의 내부 모나드 M 으로 쓴다.
// OptionT<ManyF, A> = Many<Option<A>> — "여러 갈래 각각이 성공/실패할 수 있는" 계산.
public sealed class Many<A>(IReadOnlyList<A> items) : K<ManyF, A>
{
    public IReadOnlyList<A> Items { get; } = items;
    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

public sealed class ManyF : Monad<ManyF>
{
    public static K<ManyF, B> Map<A, B>(Func<A, B> f, K<ManyF, A> fa) =>
        new Many<B>(fa.As().Items.Select(f).ToList());
    public static K<ManyF, A> Pure<A>(A value) => new Many<A>([value]);
    public static K<ManyF, B> Apply<A, B>(K<ManyF, Func<A, B>> mf, K<ManyF, A> ma) =>
        new Many<B>(mf.As().Items.SelectMany(f => ma.As().Items.Select(f)).ToList());
    public static K<ManyF, B> Bind<A, B>(K<ManyF, A> ma, Func<A, K<ManyF, B>> f) =>
        new Many<B>(ma.As().Items.SelectMany(a => f(a).As().Items).ToList());
}

public static class ManyExtensions
{
    public static Many<A> As<A>(this K<ManyF, A> fa) => (Many<A>)fa;
}
