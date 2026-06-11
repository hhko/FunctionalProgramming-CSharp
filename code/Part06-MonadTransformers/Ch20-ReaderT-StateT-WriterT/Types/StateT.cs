using Ch20.Traits;

namespace Ch20.Types;

// StateT<S, M, A> — 상태 효과를 *내부 모나드 M* 위에 얹는다. 내부는 `S → K<M, (A, S)>`.
// (LanguageExt v5 의 StateT 와 동일한 발상.)
//
// Bind 는 첫 계산의 새 상태 s1 을 다음 계산에 전달하되, 내부 효과는 M.Bind 로 잇는다.
public sealed class StateT<S, M, A>(Func<S, K<M, (A Value, S State)>> run) : K<StateTF<S, M>, A>
    where M : Monad<M>
{
    public K<M, (A Value, S State)> Run(S state) => run(state);
}

public sealed class StateTF<S, M> : MonadT<StateTF<S, M>, M>, Stateful<StateTF<S, M>, S>
    where M : Monad<M>
{
    public static K<StateTF<S, M>, A> Pure<A>(A value) =>
        new StateT<S, M, A>(s => M.Pure((value, s)));

    public static K<StateTF<S, M>, B> Map<A, B>(Func<A, B> f, K<StateTF<S, M>, A> fa) =>
        new StateT<S, M, B>(s => M.Map(t => (f(t.Value), t.State), fa.As().Run(s)));

    public static K<StateTF<S, M>, B> Apply<A, B>(K<StateTF<S, M>, Func<A, B>> mf, K<StateTF<S, M>, A> ma) =>
        Bind(mf, f => Map(f, ma));

    public static K<StateTF<S, M>, B> Bind<A, B>(K<StateTF<S, M>, A> ma, Func<A, K<StateTF<S, M>, B>> f) =>
        new StateT<S, M, B>(s => M.Bind(ma.As().Run(s), t => f(t.Value).As().Run(t.State)));

    public static K<StateTF<S, M>, A> Lift<A>(K<M, A> ma) =>
        new StateT<S, M, A>(s => M.Map(a => (a, s), ma));

    public static K<StateTF<S, M>, Unit> Put(S value) =>
        new StateT<S, M, Unit>(_ => M.Pure((Unit.Default, value)));

    public static K<StateTF<S, M>, Unit> Modify(Func<S, S> modify) =>
        new StateT<S, M, Unit>(s => M.Pure((Unit.Default, modify(s))));

    public static K<StateTF<S, M>, A> Gets<A>(Func<S, A> f) =>
        new StateT<S, M, A>(s => M.Pure((f(s), s)));
}

public static class StateTExtensions
{
    public static StateT<S, M, A> As<S, M, A>(this K<StateTF<S, M>, A> fa)
        where M : Monad<M> =>
        (StateT<S, M, A>)fa;
}
