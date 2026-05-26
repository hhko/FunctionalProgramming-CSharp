using Ch05.Traits;

namespace Ch05.Challenges;

// 챌린지 ① 정답 — Tree<A> Foldable.
//
// 이진 트리에 Foldable 부착. List / Maybe 와 달리 재귀 자료 구조 인 점이 핵심.
// 본문 §4.3·§4.4 의 3-tuple 패턴 + §4.4.2 의 분기 처리 + §4.6 의 일관성 법칙이
// 재귀 자료에도 그대로 통한다는 것을 손으로 확인한다.
//
// 핵심 — FoldRight / FoldLeft 의 본문이 재귀 호출 로 구성된다.
//   Leaf 에서 step(value, ...) 적용, Branch 에서 좌/우 자식 재귀.
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

public sealed class TreeF : Foldable<TreeF>
{
    // FoldRight — 오른쪽 자식부터 누적해 왼쪽으로 진행.
    //   Branch(L, R) 의 FoldRight = L 의 FoldRight (seed = R 의 FoldRight 결과)
    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<TreeF, A> fa) =>
        fa.As() switch
        {
            Tree<A>.Leaf l   => f(l.Value, seed),
            Tree<A>.Branch b => FoldRightTree(f, FoldRightTree(f, seed, b.Right), b.Left),
            _ => throw new InvalidOperationException()
        };

    // FoldLeft — 왼쪽 자식부터 누적해 오른쪽으로 진행.
    //   Branch(L, R) 의 FoldLeft = R 의 FoldLeft (seed = L 의 FoldLeft 결과)
    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<TreeF, A> fa) =>
        fa.As() switch
        {
            Tree<A>.Leaf l   => f(seed, l.Value),
            Tree<A>.Branch b => FoldLeftTree(f, FoldLeftTree(f, seed, b.Left), b.Right),
            _ => throw new InvalidOperationException()
        };

    // 재귀 helper — 정적 메서드를 그대로 호출해도 되지만, 가독성을 위해 분리.
    private static B FoldRightTree<A, B>(Func<A, B, B> f, B seed, Tree<A> t) =>
        FoldRight<A, B>(f, seed, t);

    private static B FoldLeftTree<A, B>(Func<B, A, B> f, B seed, Tree<A> t) =>
        FoldLeft<A, B>(f, seed, t);
}

public static class TreeExtensions
{
    public static Tree<A> As<A>(this K<TreeF, A> fa) => (Tree<A>)fa;

    // 평탄화 — 일관성 법칙 검증 시 두 결과를 비교하기 위한 보조.
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
