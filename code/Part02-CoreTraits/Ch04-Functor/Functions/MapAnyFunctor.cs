using Ch04.Traits;

namespace Ch04.Functions;

// *어떤 Functor F 든* 받는 일반 함수 (본문 §4.5.1 ~ §4.5.2).
//
// trait 메서드 (F.Map) 위에서 자유 함수가 자동 동작. MyList / MyMaybe / 미래의 어떤
// Functor 든 같은 호출로 처리.
public static class MapAnyFunctor
{
    public static K<F, B> Run<F, A, B>(K<F, A> fa, Func<A, B> f)
        where F : Functor<F> =>
        F.Map(f, fa);
}

// 일반 함수의 *합성* (본문 §4.5.3).
//
// Map 한 개의 정의 위에 수많은 일반 함수가 자라난다는 것을 보여 준다.
// 두 메서드 모두 *어떤 Functor 든* 받는다.
public static class FunctorHelpers
{
    // f 를 두 번 적용 — Map(f ∘ f) 와 같음 (합성 법칙 §4.6.3).
    public static K<F, A> MapTwice<F, A>(K<F, A> fa, Func<A, A> f)
        where F : Functor<F> =>
        F.Map(f, F.Map(f, fa));

    // f 와 g 를 차례로 적용 — Map(g ∘ f) 와 같음.
    public static K<F, C> MapThenMap<F, A, B, C>(K<F, A> fa, Func<A, B> f, Func<B, C> g)
        where F : Functor<F> =>
        F.Map(g, F.Map(f, fa));
}

// 합성 법칙 검증을 위한 작은 헬퍼 (본문 §4.6.4 / §4.7.1 에서 사용).
public static class Compose
{
    public static Func<A, C> Of<A, B, C>(Func<B, C> g, Func<A, B> f) =>
        x => g(f(x));
}
