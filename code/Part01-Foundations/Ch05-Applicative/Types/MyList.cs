using Ch05.Traits;

namespace Ch05.Types;

// 셋째 인스턴스 — MyList Applicative.
//
// 결합 의미 — cartesian product. (모든 함수 × 모든 값) 의 모든 짝.
// Pure 는 단일 원소 리스트. Apply 가 두 리스트의 곱집합.
//
// 챌린지 ① 의 정답에 해당. 본문 §5.9.1 참조.
public sealed class MyList<A>(IEnumerable<A> items) : K<MyListF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();

    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

public sealed class MyListF : Applicative<MyListF>
{
    public static K<MyListF, B> Map<A, B>(Func<A, B> f, K<MyListF, A> fa) =>
        new MyList<B>(fa.As().Items.Select(f));

    public static K<MyListF, A> Pure<A>(A value) =>
        new MyList<A>([value]);

    public static K<MyListF, B> Apply<A, B>(K<MyListF, Func<A, B>> mf, K<MyListF, A> ma)
    {
        var fs = mf.As().Items;
        var xs = ma.As().Items;
        return new MyList<B>(fs.SelectMany(f => xs.Select(a => f(a))));
    }
}

public static class MyListExtensions
{
    public static MyList<A> As<A>(this K<MyListF, A> fa) => (MyList<A>)fa;
}
