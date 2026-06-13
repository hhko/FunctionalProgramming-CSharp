namespace Ch08.Functions;

public static class Curry
{
    public static Func<A, Func<B, Func<C, Func<D, E>>>> Of<A, B, C, D, E>(Func<A, B, C, D, E> f) =>
        a => b => c => d => f(a, b, c, d);

    // 3 인자 curry — §8.9.2 칸별 복수 규칙 / §8.9.1 서버 설정 (3 칸) 에서 사용.
    public static Func<A, Func<B, Func<C, D>>> Of<A, B, C, D>(Func<A, B, C, D> f) =>
        a => b => c => f(a, b, c);
}
