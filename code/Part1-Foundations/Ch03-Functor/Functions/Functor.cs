using Ch03.Traits;

namespace Ch03.Functions;

// Functor 모듈 — generic 헬퍼 함수 (라이브러리 Functor.Module.cs 미러).
//
// 호출 어법: Functor.map<MyMaybeF, int, string>(n => n.ToString(), maybe).
// 라이브러리 패턴 — generic 제약 (where F : Functor<F>) 으로 자동 디스패치.
public static class Functor
{
    public static K<F, B> map<F, A, B>(Func<A, B> f, K<F, A> fa)
        where F : Functor<F> =>
        F.Map(f, fa);
}
