using Ch07.Traits;
using Ch07.Types;

namespace Ch07.Tests;

// 본문 §7.9.4 가짜 Monad 반례 — 실행 가능한 버전.
//
// BogusBind 는 진짜 Bind 와 시그니처가 한 글자도 다르지 않다.
// 다만 꺼낸 값을 다음 함수에 먹이지 않고 *버린다* (f 를 호출하지 않고 늘 Nothing).
// 컴파일은 통과하지만 좌항등 법칙 Bind(Pure(a), f) ≡ f(a) 를 깬다.
// 타입이 막지 못하는 약속을 세 법칙이 막는다는 것을 코드로 보인다.
public static class MonadCounterexample
{
    // 가짜 — 값을 버린다. f 를 호출하지 않고 늘 Nothing.
    public static K<MyMaybeF, B> BogusBind<A, B>(K<MyMaybeF, A> ma, Func<A, K<MyMaybeF, B>> f) =>
        MyMaybe<B>.Nothing.Instance;

    // 진짜 / 가짜에 좌항등을 적용해 결과를 비교한다 (a = 3, f = n => Pure(n + 1)).
    //   좌항등: Bind(Pure(a), f) ≡ f(a)  →  f(3) = Just(4)
    // 반환: (진짜 결과, 가짜 결과, 기대값 f(3)).
    public static (K<MyMaybeF, int> real, K<MyMaybeF, int> bogus, K<MyMaybeF, int> expected) LeftIdentityCompare()
    {
        Func<int, K<MyMaybeF, int>> f = n => MyMaybeF.Pure(n + 1);
        var real     = MyMaybeF.Bind(MyMaybeF.Pure(3), f);   // Just(4)
        var bogus    = BogusBind(MyMaybeF.Pure(3), f);        // Nothing — 값을 버림
        var expected = f(3);                                  // Just(4)
        return (real, bogus, expected);
    }

    // 진짜는 좌항등 통과 (f(a) 와 같음), 가짜는 위반 (f(a) 와 다름).
    public static bool RealHoldsBogusBreaks()
    {
        var (real, bogus, expected) = LeftIdentityCompare();
        return Eq(real, expected) && !Eq(bogus, expected);
    }

    static bool Eq(K<MyMaybeF, int> x, K<MyMaybeF, int> y) =>
        (x.As(), y.As()) switch
        {
            (MyMaybe<int>.Just a, MyMaybe<int>.Just b)   => a.Value == b.Value,
            (MyMaybe<int>.Nothing, MyMaybe<int>.Nothing) => true,
            _                                            => false
        };
}
