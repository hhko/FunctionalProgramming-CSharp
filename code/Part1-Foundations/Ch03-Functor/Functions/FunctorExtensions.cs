using Ch03.Traits;

namespace Ch03.Functions;

// FunctorExtensions — K<F, A> 위 extension methods (라이브러리 Functor.Extensions.cs 미러).
//
// 호출 어법: maybe.Map(n => n.ToString()) — 가장 자연스러운 점 호출.
// generic 추론으로 F, A, B 자동 결정.
public static class FunctorExtensions
{
    public static K<F, B> Map<F, A, B>(this K<F, A> fa, Func<A, B> f)
        where F : Functor<F> =>
        F.Map(f, fa);
}
