using Ch17.Traits;

namespace Ch17.Functions;

// Writable 모듈 — tell / listen / pass 의 generic 어휘.
public static class Writable
{
    public static K<M, Unit> tell<M, W>(W item)
        where M : Writable<M, W> where W : Monoid<W> =>
        M.Tell(item);

    public static K<M, (A Value, W Output)> listen<M, W, A>(K<M, A> ma)
        where M : Writable<M, W> where W : Monoid<W> =>
        M.Listen(ma);

    public static K<M, A> pass<M, W, A>(K<M, (A Value, Func<W, W> Function)> action)
        where M : Writable<M, W> where W : Monoid<W> =>
        M.Pass(action);
}
