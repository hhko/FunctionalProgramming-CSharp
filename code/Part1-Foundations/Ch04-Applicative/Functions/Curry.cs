namespace Ch04.Functions;

// 다인자 함수 → 한 인자씩 받는 함수 체인.
//
// (A, B) → C   ⟶   A → (B → C)
// (A, B, C) → D ⟶  A → (B → (C → D))
//
// Apply 가 *한 번에 한 인자씩만* 결합 가능하므로 Currying 이 *Applicative 의 전제 조건*.
public static class Curry
{
    public static Func<A, Func<B, C>> Of<A, B, C>(Func<A, B, C> f) =>
        a => b => f(a, b);

    public static Func<A, Func<B, Func<C, D>>> Of<A, B, C, D>(Func<A, B, C, D> f) =>
        a => b => c => f(a, b, c);
}
