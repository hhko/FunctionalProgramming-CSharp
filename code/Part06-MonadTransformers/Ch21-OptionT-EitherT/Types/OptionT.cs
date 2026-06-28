using Ch21.Traits;

namespace Ch21.Types;

// OptionT<M, A> — *부재* 효과를 내부 모나드 M 위에 얹는다. 내부는 `K<M, Option<A>>`.
// 19장에서 정의한 그 변환기다. 이 장에서는 EitherT 와 나란히 두어, 실패가 "이유 없는 부재(None)"
// 인지 "이유 있는 오류(Left L)" 인지의 차이를 같은 코드로 대비한다.
//
// Bind 가 두 층 (M 의 효과 / Option 의 Some·None) 을 푸는 배관은 EitherT 와 똑같고,
// 다른 점은 Some·None 자리가 EitherT 에서는 Right·Left 라는 것뿐이다.
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
        Bind(mf, f => Map(f, ma));

    public static K<OptionTF<M>, B> Bind<A, B>(K<OptionTF<M>, A> ma, Func<A, K<OptionTF<M>, B>> f) =>
        new OptionT<M, B>(
            M.Bind(ma.As().Run, opt =>
                opt is Option<A>.Some s
                    ? f(s.Value).As().Run
                    : M.Pure<Option<B>>(Option<B>.None.Instance)));

    public static K<OptionTF<M>, A> Lift<A>(K<M, A> ma) =>
        new OptionT<M, A>(M.Map(a => (Option<A>)new Option<A>.Some(a), ma));

    // 실패 — 이유 없이 단락 (EitherT.Fail(L) 과 대비되는, 정보 없는 실패).
    public static K<OptionTF<M>, A> Fail<A>() =>
        new OptionT<M, A>(M.Pure<Option<A>>(Option<A>.None.Instance));
}

public static class OptionTExtensions
{
    public static OptionT<M, A> As<M, A>(this K<OptionTF<M>, A> fa)
        where M : Monad<M> =>
        (OptionT<M, A>)fa;
}
