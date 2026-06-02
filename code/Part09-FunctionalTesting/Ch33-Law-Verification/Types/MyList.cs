using Ch33.Traits;

namespace Ch33.Types;

// 검증 대상 인스턴스 (1부 toy). 법칙 모듈이 *이런 임의의 인스턴스* 를 받아 검사한다.
public sealed class MyList<A>(IEnumerable<A> items) : K<MyListF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();
}

public sealed class MyListF : Monad<MyListF>
{
    public static K<MyListF, B> Map<A, B>(Func<A, B> f, K<MyListF, A> fa) =>
        new MyList<B>(fa.As().Items.Select(f));
    public static K<MyListF, A> Pure<A>(A value) => new MyList<A>([value]);
    public static K<MyListF, B> Bind<A, B>(K<MyListF, A> ma, Func<A, K<MyListF, B>> f) =>
        new MyList<B>(ma.As().Items.SelectMany(a => f(a).As().Items));
}

// 일부러 법칙을 어기는 인스턴스 — 법칙 검증이 *실제로 위반을 잡아내는지* 확인용.
public sealed class BogusListF : Functor<BogusListF>
{
    public static K<BogusListF, B> Map<A, B>(Func<A, B> f, K<BogusListF, A> fa) =>
        new BogusList<B>([]);   // 입력 무시 → 항등 법칙 위반
}

public sealed class BogusList<A>(IEnumerable<A> items) : K<BogusListF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();
}

public static class ListExtensions
{
    public static MyList<A> As<A>(this K<MyListF, A> fa) => (MyList<A>)fa;
    public static BogusList<A> As<A>(this K<BogusListF, A> fa) => (BogusList<A>)fa;
}
