using Ch16.Traits;

namespace Ch16.Functions;

// Stateful 모듈 — get / put / modify / gets 의 generic 어휘.
public static class Stateful
{
    public static K<M, S> get<M, S>() where M : Stateful<M, S> => M.Get;
    public static K<M, Unit> put<M, S>(S value) where M : Stateful<M, S> => M.Put(value);
    public static K<M, Unit> modify<M, S>(Func<S, S> f) where M : Stateful<M, S> => M.Modify(f);
    public static K<M, A> gets<M, S, A>(Func<S, A> f) where M : Stateful<M, S> => M.Gets(f);
}
