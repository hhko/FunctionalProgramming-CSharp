using Ch12.Traits;

namespace Ch12.Types;

// Reader<Env, A> — 가장 단순한 효과 모나드. 내부는 그냥 *함수* `Env → A` 다.
// (LanguageExt v5 의 `Reader<Env, A>(Func<Env, A> runReader)` 와 동일한 발상.)
//
// 효과 = "환경에 의존". Run(env) 하기 전까지는 아무 값도 없다 — env 가 주입될 때 계산된다.
public sealed class Reader<Env, A>(Func<Env, A> run) : K<ReaderF<Env>, A>
{
    public A Run(Env env) => run(env);
}

// 태그 타입 — 환경 Env 를 고정한 채 Monad + Readable 을 호스트.
public sealed class ReaderF<Env> : Monad<ReaderF<Env>>, Readable<ReaderF<Env>, Env>
{
    public static K<ReaderF<Env>, B> Map<A, B>(Func<A, B> f, K<ReaderF<Env>, A> fa) =>
        new Reader<Env, B>(env => f(fa.As().Run(env)));

    public static K<ReaderF<Env>, A> Pure<A>(A value) =>
        new Reader<Env, A>(_ => value);

    // Apply — 같은 환경을 함수 쪽과 값 쪽 *양쪽에* 흘려보낸다.
    public static K<ReaderF<Env>, B> Apply<A, B>(K<ReaderF<Env>, Func<A, B>> mf, K<ReaderF<Env>, A> ma) =>
        new Reader<Env, B>(env => mf.As().Run(env)(ma.As().Run(env)));

    // Bind — 첫 계산을 env 로 실행하고, 그 결과로 만든 다음 계산을 *같은 env* 로 실행.
    public static K<ReaderF<Env>, B> Bind<A, B>(K<ReaderF<Env>, A> ma, Func<A, K<ReaderF<Env>, B>> f) =>
        new Reader<Env, B>(env => f(ma.As().Run(env)).As().Run(env));

    // Readable.Asks — 환경 → 값 함수를 그대로 Reader 로.
    public static K<ReaderF<Env>, A> Asks<A>(Func<Env, A> f) =>
        new Reader<Env, A>(f);

    // Readable.Local — 변형된 환경에서 ma 를 실행.
    public static K<ReaderF<Env>, A> Local<A>(Func<Env, Env> f, K<ReaderF<Env>, A> ma) =>
        new Reader<Env, A>(env => ma.As().Run(f(env)));
}

public static class ReaderExtensions
{
    public static Reader<Env, A> As<Env, A>(this K<ReaderF<Env>, A> fa) => (Reader<Env, A>)fa;
}
