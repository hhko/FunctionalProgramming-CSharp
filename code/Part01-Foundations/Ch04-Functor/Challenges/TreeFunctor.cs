using Ch04.Traits;

namespace Ch04.Challenges;

// 챌린지 ① 정답 — Tree<A> Functor.
//
// 이진 트리에 Functor 부착. List / Maybe 와 달리 *재귀 자료 구조* 인 점이 핵심.
// 본문 §3.3·§3.4 의 3-tuple 패턴 + §3.4.2 의 분기 처리 + §3.6 의 두 법칙이
// 재귀 자료에도 그대로 통한다는 것을 손으로 확인한다.
//
// 핵심 — Map 의 본문이 *재귀 호출* 로 구성된다. Leaf 에서 f 적용, Branch 에서 좌/우 자식 재귀.
public abstract record Tree<A> : K<TreeF, A>
{
    public sealed record Leaf(A Value) : Tree<A>;
    public sealed record Branch(Tree<A> Left, Tree<A> Right) : Tree<A>;

    public override string ToString() => this switch
    {
        Leaf l   => $"Leaf({l.Value})",
        Branch b => $"Branch({b.Left}, {b.Right})",
        _        => "?"
    };
}

public sealed class TreeF : Functor<TreeF>
{
    public static K<TreeF, B> Map<A, B>(Func<A, B> f, K<TreeF, A> fa) =>
        fa.As() switch
        {
            Tree<A>.Leaf l   => new Tree<B>.Leaf(f(l.Value)),                              // ← f 적용
            Tree<A>.Branch b => new Tree<B>.Branch(MapTree(f, b.Left), MapTree(f, b.Right)), // ← 좌/우 재귀
            _ => throw new InvalidOperationException()
        };

    // 재귀 helper — 정적 Map 을 그대로 호출해도 되지만, 가독성을 위해 분리.
    private static Tree<B> MapTree<A, B>(Func<A, B> f, Tree<A> t) =>
        Map<A, B>(f, t).As();
}

public static class TreeExtensions
{
    public static Tree<A> As<A>(this K<TreeF, A> fa) => (Tree<A>)fa;

    // 평탄화 — 법칙 검증 시 두 트리를 시퀀스로 비교하기 위한 probe.
    public static IEnumerable<A> Flatten<A>(this Tree<A> t)
    {
        switch (t)
        {
            case Tree<A>.Leaf l:
                yield return l.Value;
                break;
            case Tree<A>.Branch b:
                foreach (var x in b.Left.Flatten())  yield return x;
                foreach (var x in b.Right.Flatten()) yield return x;
                break;
        }
    }
}
