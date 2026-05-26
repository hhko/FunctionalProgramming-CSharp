using Ch05.Traits;

namespace Ch05.Tests;

// 일관성 법칙 위반 시연 — FoldRight 와 FoldLeft 가 *다른 결과* 를 낸다.
//
// BogusFoldableF.FoldLeft 는 *자료를 무시* 하고 seed 만 반환한다. FoldRight 는 정상.
// 시그니처는 완벽 (반환 타입 일치) 한데 두 방향의 결과가 다름 — 결과 일관성 깨짐.
//
// 본문 §4.6.5 의 핵심 — 시그니처가 통과해도 *진짜 Foldable 이 아닐 수 있다*.
public sealed class BogusList<A>(IEnumerable<A> items) : K<BogusFoldableF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();
    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

public sealed class BogusFoldableF : Foldable<BogusFoldableF>
{
    // FoldRight — 정상 (오른쪽 끝부터 누적).
    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<BogusFoldableF, A> fa)
    {
        var list = ((BogusList<A>)fa).Items;
        var acc = seed;
        for (int i = list.Count - 1; i >= 0; i--)
            acc = f(list[i], acc);
        return acc;
    }

    // FoldLeft — *자료 무시*, 항상 seed 반환. ← 일관성 위반!
    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<BogusFoldableF, A> fa) =>
        seed;
}
