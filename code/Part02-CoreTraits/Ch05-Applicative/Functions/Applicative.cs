using Ch05.Traits;

namespace Ch05.Functions;

// Applicative 모듈 — generic 헬퍼 함수 (라이브러리 Applicative.Module.cs 미러).
//
// 호출 어법: Applicative.pure<MyMaybeF, int>(42), Applicative.apply(mf, ma).
public static class Applicative
{
    public static K<F, A> pure<F, A>(A value)
        where F : Applicative<F> =>
        F.Pure(value);

    public static K<F, B> apply<F, A, B>(K<F, Func<A, B>> mf, K<F, A> ma)
        where F : Applicative<F> =>
        F.Apply(mf, ma);
}
