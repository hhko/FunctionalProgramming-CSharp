using Ch18.Traits;

namespace Ch18.Types;

// ReaderOption<Env, A> — *두 효과를 동시에* 담는 모나드: 환경 의존(Reader) + 실패 가능(Option).
// 내부는 `Reader<Env, Option<A>>` 즉 함수 `Env → Option<A>`.
//
// 이것이 18장의 핵심 — Reader 와 Option 각각은 모나드지만, *둘을 겹친* 것은
// 공짜로 모나드가 되지 *않는다*. Bind 를 직접 짜야 하고, 그 본문은 두 층 (env / Some·None) 을
// *손으로* 풀어야 한다. 아래 Bind 가 바로 그 "손으로 짠 배관" 이다.
//
// 6부의 변환기 ReaderT<Env, M, A> 는 이 배관을 *임의의 내부 모나드 M 에 대해 자동 생성* 한다.
public sealed class ReaderOption<Env, A>(Func<Env, Option<A>> run) : K<ROF<Env>, A>
{
    public Option<A> Run(Env env) => run(env);

    // 내부 효과를 끌어올리는 lift 들 (변환기의 lift 가 일반화할 대상).
    public static ReaderOption<Env, A> LiftReader(Reader<Env, A> r) =>
        new(e => new Option<A>.Some(r.Run(e)));

    public static ReaderOption<Env, A> LiftOption(Option<A> o) =>
        new(_ => o);
}

public sealed class ROF<Env> : Monad<ROF<Env>>
{
    public static K<ROF<Env>, A> Pure<A>(A value) =>
        new ReaderOption<Env, A>(_ => new Option<A>.Some(value));

    public static K<ROF<Env>, B> Map<A, B>(Func<A, B> f, K<ROF<Env>, A> fa) =>
        Bind(fa, a => Pure(f(a)));

    public static K<ROF<Env>, B> Apply<A, B>(K<ROF<Env>, Func<A, B>> mf, K<ROF<Env>, A> ma) =>
        Bind(mf, f => Bind(ma, a => Pure(f(a))));

    // ★ 손으로 짠 배관 — 두 층을 직접 푼다:
    //   1) Reader 층: env 를 안쪽으로 흘려보낸다.
    //   2) Option 층: Some 면 다음 단계, None 이면 단락.
    public static K<ROF<Env>, B> Bind<A, B>(K<ROF<Env>, A> ma, Func<A, K<ROF<Env>, B>> f) =>
        new ReaderOption<Env, B>(env =>
            ma.As().Run(env) switch                 // ← Reader 층을 env 로 실행
            {
                Option<A>.Some s => f(s.Value).As().Run(env),  // ← Option 층: Some → 다음
                _                => Option<B>.None.Instance     // ← None → 단락
            });
}

public static class ReaderOptionExtensions
{
    public static ReaderOption<Env, A> As<Env, A>(this K<ROF<Env>, A> fa) => (ReaderOption<Env, A>)fa;
}
