namespace Ch08.Functions;

public static class Curry
{
    public static Func<A, Func<B, Func<C, Func<D, E>>>> Of<A, B, C, D, E>(Func<A, B, C, D, E> f) =>
        a => b => c => d => f(a, b, c, d);
}
