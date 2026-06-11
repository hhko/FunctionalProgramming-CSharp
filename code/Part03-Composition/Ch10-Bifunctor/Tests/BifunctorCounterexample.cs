using Ch10.Traits;
using Ch10.Types;

namespace Ch10.Tests;

// 본문 §10.7.1 가짜 Bifunctor 반례 — 실행 가능한 버전.
//
// 두 자리를 뒤바꾸는(swap) 가짜 BiMap. Pair<int, int> 전용 데모.
// 두 함수 + 컨테이너를 받는 모양은 흉내내지만, 두 자리를 뒤섞어
// 항등 법칙 BiMap(id, id, p) == p 를 깬다. 시그니처가 막지 못하는
// 약속을 법칙이 막는다는 것을 코드로 보인다.
public static class BifunctorCounterexample
{
    // 가짜 — 두 자리를 뒤바꿔 담는다.
    public static Pair<int, int> BogusBiMap(Func<int, int> first, Func<int, int> second, Pair<int, int> p) =>
        new(second(p.Second), first(p.First));   // First / Second 자리가 뒤바뀜

    // 진짜 PairF.BiMap 과 가짜에 *항등 함수* 를 걸어 결과를 비교한다.
    // 반환: (진짜 결과, 가짜 결과) — 예: (Pair(3, 5), Pair(5, 3)).
    public static (Pair<int, int> real, Pair<int, int> bogus) IdentityCompare(Pair<int, int> p)
    {
        var real  = (Pair<int, int>)PairF.BiMap<int, int, int, int>(x => x, x => x, p);
        var bogus = BogusBiMap(x => x, x => x, p);
        return (real, bogus);
    }

    // 진짜는 항등 법칙 통과(원본 보존), 가짜는 위반(자리 뒤바뀜).
    public static bool RealHoldsBogusBreaks(Pair<int, int> p)
    {
        var (real, bogus) = IdentityCompare(p);
        return real.Equals(p) && !bogus.Equals(p);
    }

    // 본문 §10.7.2 또 다른 가짜 — 첫 함수를 첫 자리에 *두 번* 적용.
    // 항등 함수면 id(id(x))=x 라 *항등 법칙은 통과*하지만, 합성 단계에서 함수 적용
    // 순서가 어긋나 *합성 법칙을 깬다*. 두 법칙이 따로 필요하다는 증거.
    public static Pair<int, int> BogusBiMapTwice(Func<int, int> first, Func<int, int> second, Pair<int, int> p) =>
        new(first(first(p.First)), second(p.Second));   // first 를 두 번

    // 항등은 통과 / 합성은 깸 을 한 자리에서 보인다 (first=n+1, g=n*2).
    // 한 번에 합성(g∘f 를 두 번)한 첫 자리 = 18, 나눠 적용(f 두 번 → g 두 번)한 첫 자리 = 20.
    public static (Pair<int, int> identity, int onceComposed, int splitApplied) CompositionBreaks()
    {
        var p = new Pair<int, int>(3, 5);
        Func<int, int> f = n => n + 1;
        Func<int, int> g = n => n * 2;

        var idResult = BogusBiMapTwice(x => x, x => x, p);                                  // 항등 → Pair(3, 5)
        var once     = BogusBiMapTwice(n => g(f(n)), x => x, p).First;                      // 한 번에 합성 → 18
        var split    = BogusBiMapTwice(g, x => x, BogusBiMapTwice(f, x => x, p)).First;     // 나눠 적용 → 20
        return (idResult, once, split);
    }

    // 항등은 통과(원본 보존)하나 합성은 깸(onceComposed != splitApplied).
    public static bool IdentityHoldsCompositionBreaks()
    {
        var (idResult, once, split) = CompositionBreaks();
        return idResult.Equals(new Pair<int, int>(3, 5)) && once != split;
    }
}
