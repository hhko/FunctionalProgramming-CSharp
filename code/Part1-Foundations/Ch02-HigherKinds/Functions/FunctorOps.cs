using Ch02.Traits;

namespace Ch02.Functions;

// 본문 §2.5.6 / §2.8 의 *어떤 Functor 든* 받는 일반 함수들.
//
// 구체 컨테이너 (`MyList`, `MyMaybe`) 를 *몰라도* 동작 — `where F : Functor<F>` 제약이
// `F.Map(...)` 호출을 보장한다. *3-tuple 패턴의 진짜 ROI* — 한 번 정의에 N 개 인스턴스
// 자동 동작.
public static class FunctorOps
{
    // §2.8 — Map 한 번 적용.
    public static K<F, B> Run<F, A, B>(K<F, A> fa, Func<A, B> f)
        where F : Functor<F> =>
        F.Map(f, fa);

    // §2.5.6 — Map 두 번 적용 (체이닝 시 F 가 끝까지 살아 있음을 보여 준다).
    public static K<F, B> ApplyTwice<F, A, B>(K<F, A> fa, Func<A, B> f, Func<B, B> g)
        where F : Functor<F> =>
        F.Map(g, F.Map(f, fa));
}
