using Ch09.Traits;

namespace Ch09.Challenges;

// 챌린지 ① 정답 — MyLst (cons 연결 리스트) 에 *같은* Monad / Foldable 부착.
//
// MySeq 는 IEnumerable 백킹이었지만 MyLst 는 *재귀적 cons 구조* (Cons / Nil) 다.
// 표현은 완전히 다른데 *같은 trait* (Functor / Applicative / Monad / Foldable) 가 그대로 붙는다.
// → 추상이 자료 표현이 아니라 *시그니처가 약속하는 동작* 에 달려 있음을 보여 준다.
public abstract record MyLst<A> : K<LstF, A>
{
    public sealed record Nil : MyLst<A>
    {
        public static readonly Nil Instance = new();
    }

    public sealed record Cons(A Head, MyLst<A> Tail) : MyLst<A>;

    public static MyLst<A> FromEnumerable(IEnumerable<A> xs)
    {
        MyLst<A> acc = Nil.Instance;
        foreach (var x in xs.Reverse())
            acc = new Cons(x, acc);
        return acc;
    }

    public IEnumerable<A> ToEnumerable()
    {
        var cur = this;
        while (cur is Cons c)
        {
            yield return c.Head;
            cur = c.Tail;
        }
    }

    public override string ToString() => $"Lst[{string.Join(", ", ToEnumerable())}]";
}

public sealed class LstF : Monad<LstF>, Foldable<LstF>
{
    public static K<LstF, B> Map<A, B>(Func<A, B> f, K<LstF, A> fa) =>
        MyLst<B>.FromEnumerable(fa.As().ToEnumerable().Select(f));

    public static K<LstF, A> Pure<A>(A value) =>
        new MyLst<A>.Cons(value, MyLst<A>.Nil.Instance);

    public static K<LstF, B> Apply<A, B>(K<LstF, Func<A, B>> mf, K<LstF, A> ma)
    {
        var fs = mf.As().ToEnumerable();
        var xs = ma.As().ToEnumerable().ToList();
        return MyLst<B>.FromEnumerable(fs.SelectMany(f => xs.Select(f)));
    }

    public static K<LstF, B> Bind<A, B>(K<LstF, A> ma, Func<A, K<LstF, B>> f) =>
        MyLst<B>.FromEnumerable(
            ma.As().ToEnumerable().SelectMany(a => f(a).As().ToEnumerable()));

    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<LstF, A> fa)
    {
        var acc = seed;
        foreach (var a in fa.As().ToEnumerable())
            acc = f(acc, a);
        return acc;
    }

    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<LstF, A> fa)
    {
        var buffer = fa.As().ToEnumerable().ToList();
        var acc = seed;
        for (var i = buffer.Count - 1; i >= 0; i--)
            acc = f(buffer[i], acc);
        return acc;
    }
}

public static class MyLstExtensions
{
    public static MyLst<A> As<A>(this K<LstF, A> fa) => (MyLst<A>)fa;
}
