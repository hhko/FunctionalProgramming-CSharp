using Ch25.Traits;

namespace Ch25.Types;

// Eff<A> — *런타임 없는 효과*. 지연된 계산이 Run 되면 Fin<A> (성공/실패) 를 낸다.
//   개념적으로 Eff<A> ≈ IO<Fin<A>> — 23장 IO 의 지연 + 24장 Fin 의 오류 포획을 합친 것.
//
// LanguageExt v5 에서 Eff<A> 는 Eff<MinRT, A> (빈 런타임) 의 얇은 래퍼다. 여기선 런타임이
// 필요 없는 경우의 효과를 보인다. 런타임 주입은 26장 Eff<RT, A> 에서.
public sealed class Eff<A> : K<EffF, A>
{
    readonly Func<Fin<A>> run;
    internal Eff(Func<Fin<A>> run) => this.run = run;

    public Fin<A> Run() => run();

    // 부수 효과 thunk — 예외가 나면 자동으로 Fail(Error) 로 포획.
    public static Eff<A> Effect(Func<A> thunk) =>
        new(() =>
        {
            try { return new Fin<A>.Succ(thunk()); }
            catch (Exception e) { return new Fin<A>.Fail(Error.New(e)); }
        });
}

public sealed class EffF : Monad<EffF>, Fallible<EffF>, Alternative<EffF>, Final<EffF>
{
    public static K<EffF, A> Pure<A>(A value) => new Eff<A>(() => new Fin<A>.Succ(value));

    public static K<EffF, B> Map<A, B>(Func<A, B> f, K<EffF, A> fa) =>
        new Eff<B>(() => fa.As().Run() switch
        {
            Fin<A>.Succ s => new Fin<B>.Succ(f(s.Value)),
            Fin<A>.Fail e => new Fin<B>.Fail(e.Error),
            _ => throw new InvalidOperationException()
        });

    public static K<EffF, B> Apply<A, B>(K<EffF, Func<A, B>> mf, K<EffF, A> ma) =>
        Bind(mf, f => Map(f, ma));

    public static K<EffF, B> Bind<A, B>(K<EffF, A> ma, Func<A, K<EffF, B>> f) =>
        new Eff<B>(() => ma.As().Run() switch
        {
            Fin<A>.Succ s => f(s.Value).As().Run(),
            Fin<A>.Fail e => new Fin<B>.Fail(e.Error),   // 실패 단락
            _ => throw new InvalidOperationException()
        });

    public static K<EffF, A> Fail<A>(Error error) => new Eff<A>(() => new Fin<A>.Fail(error));

    public static K<EffF, A> Catch<A>(K<EffF, A> fa, Func<Error, K<EffF, A>> handler) =>
        new Eff<A>(() => fa.As().Run() switch
        {
            Fin<A>.Fail e => handler(e.Error).As().Run(),
            var succ => succ
        });

    // Alternative — Empty 는 곧 실패, Choose 는 첫째 성공이면 그대로·실패면 둘째 시도 (고르기).
    public static K<EffF, A> Empty<A>() => new Eff<A>(() => new Fin<A>.Fail(Error.New("Eff.Empty")));

    public static K<EffF, A> Choose<A>(K<EffF, A> fa, K<EffF, A> fb) =>
        new Eff<A>(() => fa.As().Run() switch
        {
            Fin<A>.Fail => fb.As().Run(),   // 첫째 실패 → 둘째
            var succ => succ                 // 첫째 성공 → 둘째 미평가
        });

    // Final — fa 를 Run 한 뒤 성공·실패와 무관하게 @finally 를 Run 하고 fa 결과를 돌려준다.
    public static K<EffF, A> Finally<A, X>(K<EffF, A> fa, K<EffF, X> @finally) =>
        new Eff<A>(() =>
        {
            var result = fa.As().Run();
            @finally.As().Run();             // 정리는 성공·실패 무관 실행
            return result;
        });
}

public static class EffExtensions
{
    public static Eff<A> As<A>(this K<EffF, A> fa) => (Eff<A>)fa;
}
