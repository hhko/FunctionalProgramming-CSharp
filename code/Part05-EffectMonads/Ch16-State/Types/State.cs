using Ch16.Traits;

namespace Ch16.Types;

// State<S, A> — 내부는 함수 `S → (A, S)`. 상태 s 를 받아 *값 a* 와 *새 상태 s'* 을 함께 돌려준다.
// (LanguageExt v5 의 `State<S, A>(Func<S, (A, S)> runState)` 와 동일한 발상.)
//
// 효과 = "상태를 읽고 쓰기". 가변 필드 없이, Bind 가 새 상태를 다음 단계로 자동으로 실어 나른다.
public sealed class State<S, A>(Func<S, (A Value, S State)> run) : K<StateF<S>, A>
{
    public (A Value, S State) Run(S state) => run(state);
}

public sealed class StateF<S> : Monad<StateF<S>>, Stateful<StateF<S>, S>
{
    public static K<StateF<S>, B> Map<A, B>(Func<A, B> f, K<StateF<S>, A> fa) =>
        new State<S, B>(s =>
        {
            var (a, s1) = fa.As().Run(s);
            return (f(a), s1);
        });

    public static K<StateF<S>, A> Pure<A>(A value) =>
        new State<S, A>(s => (value, s));

    // Apply — 함수 계산이 상태를 흘리고, 이어서 값 계산이 *그 상태부터* 이어 흘린다.
    public static K<StateF<S>, B> Apply<A, B>(K<StateF<S>, Func<A, B>> mf, K<StateF<S>, A> ma) =>
        new State<S, B>(s =>
        {
            var (f, s1) = mf.As().Run(s);
            var (a, s2) = ma.As().Run(s1);
            return (f(a), s2);
        });

    // Bind — 첫 계산의 새 상태 s1 을 다음 계산에 자동 전달.
    public static K<StateF<S>, B> Bind<A, B>(K<StateF<S>, A> ma, Func<A, K<StateF<S>, B>> f) =>
        new State<S, B>(s =>
        {
            var (a, s1) = ma.As().Run(s);
            return f(a).As().Run(s1);
        });

    // Stateful 멤버.
    public static K<StateF<S>, Unit> Put(S value) =>
        new State<S, Unit>(_ => (Unit.Default, value));

    public static K<StateF<S>, Unit> Modify(Func<S, S> modify) =>
        new State<S, Unit>(s => (Unit.Default, modify(s)));

    public static K<StateF<S>, A> Gets<A>(Func<S, A> f) =>
        new State<S, A>(s => (f(s), s));
}

public static class StateExtensions
{
    public static State<S, A> As<S, A>(this K<StateF<S>, A> fa) => (State<S, A>)fa;
}
