using Ch16.Traits;

namespace Ch16.Types;

// OptionT<M, A> — *첫 변환기*. "실패 가능" 효과를 *내부 모나드 M* 위에 얹는다.
// 내부 표현은 `K<M, Option<A>>` — M 안에 Option 이 들어 있는 한 겹.
//
// 15장의 ReaderOption 은 내부가 Reader 로 *고정* 이었지만, OptionT 는 M 이 *무엇이든* 된다.
// Bind 가 두 층 (M 의 효과 / Option 의 Some·None) 을 푸는 배관은 15장과 똑같지만,
// 이제 M 에 대해 *한 번만* 작성하면 모든 내부 모나드에 통한다.
public sealed class OptionT<M, A>(K<M, Option<A>> run) : K<OptionTF<M>, A>
    where M : Monad<M>
{
    public K<M, Option<A>> Run { get; } = run;
}

public sealed class OptionTF<M> : MonadT<OptionTF<M>, M>
    where M : Monad<M>
{
    public static K<OptionTF<M>, A> Pure<A>(A value) =>
        new OptionT<M, A>(M.Pure<Option<A>>(new Option<A>.Some(value)));

    public static K<OptionTF<M>, B> Map<A, B>(Func<A, B> f, K<OptionTF<M>, A> fa) =>
        new OptionT<M, B>(M.Map(opt => opt.MapOption(f), fa.As().Run));

    public static K<OptionTF<M>, B> Apply<A, B>(K<OptionTF<M>, Func<A, B>> mf, K<OptionTF<M>, A> ma) =>
        Bind(mf, f => Bind(ma, a => Pure(f(a))));

    // 두 층을 푸는 배관 — M 효과는 M.Bind 로 흘리고, Option 은 Some/None 으로 분기.
    public static K<OptionTF<M>, B> Bind<A, B>(K<OptionTF<M>, A> ma, Func<A, K<OptionTF<M>, B>> f) =>
        new OptionT<M, B>(
            M.Bind(ma.As().Run, opt =>
                opt is Option<A>.Some s
                    ? f(s.Value).As().Run
                    : M.Pure<Option<B>>(Option<B>.None.Instance)));

    // Lift — 내부 모나드 계산을 OptionT 한 층 위로 (항상 Some 으로 감싼다).
    public static K<OptionTF<M>, A> Lift<A>(K<M, A> ma) =>
        new OptionT<M, A>(M.Map(a => (Option<A>)new Option<A>.Some(a), ma));
}

public static class OptionTExtensions
{
    public static OptionT<M, A> As<M, A>(this K<OptionTF<M>, A> fa)
        where M : Monad<M> =>
        (OptionT<M, A>)fa;
}
