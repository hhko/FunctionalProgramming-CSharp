using Ch07.Functions;
using Ch07.Traits;
using Ch07.Types;

namespace Ch07.Challenges;

// 7장 §7.11 챌린지 3 개의 정답 코드.
//
// 각 정답은 본문이 "손으로 따라가 보라" 고 한 결과를 *실행 가능한 코드* 로 옮긴 것.
// Program.cs 에서 호출해 콘솔로 확인할 수 있다.
public static class MonadChallenges
{
    // 공용 파싱 함수 — 정수면 Just, 아니면 Nothing (a → E<b> 유형).
    public static K<MyMaybeF, int> ParseInt(string s) =>
        int.TryParse(s, out var n)
            ? MyMaybeF.Pure(n)
            : MyMaybe<int>.Nothing.Instance;

    // ── 챌린지 1 — Bind 사슬을 시그니처 따라 추적 ─────────────────────────
    //
    // ParseInt("12").Bind(n => ParseInt("3").Bind(m => Pure(n * m))).
    // Just(12) → Just(3) → Pure(36) = Just(36).
    public static K<MyMaybeF, int> TraceBind() =>
        ParseInt("12").Bind(n =>
            ParseInt("3").Bind<MyMaybeF, int, int>(m =>
                MyMaybeF.Pure(n * m)));

    // 중간 단계를 ParseInt("x") 로 바꾸면 그 자리에서 단락.
    // ParseInt("x") = Nothing → 바깥 Bind 가 f 를 *호출조차 안 함* → Nothing.
    public static K<MyMaybeF, int> TraceBindShortCircuit() =>
        ParseInt("x").Bind(n =>
            ParseInt("3").Bind<MyMaybeF, int, int>(m =>
                MyMaybeF.Pure(n * m)));

    // ── 챌린지 2 — f >=> g 를 Bind 만으로 직접 작성 ──────────────────────
    //
    // Kleisli.Then 과 같은 정의지만, 챌린지가 요구한 a => Bind(f(a), g) 를 손으로 적은 것.
    // 라이브러리 헬퍼 없이 Bind 한 번으로 두 World-crossing 함수를 잇는다.
    public static Func<A, K<MyMaybeF, C>> KleisliByBind<A, B, C>(
        Func<A, K<MyMaybeF, B>> f,
        Func<B, K<MyMaybeF, C>> g) =>
        a => MyMaybeF.Bind(f(a), g);

    // 좌항등 (pure >=> f ≡ f) 확인 — pure 를 앞에 붙인 합성이 f 와 같은 값을 낸다.
    //   (pure >=> f)(a) = Bind(Pure(a), f) = f(a)   ← 좌항등 법칙.
    public static (K<MyMaybeF, int> viaCompose, K<MyMaybeF, int> viaDirect) LeftIdentityDemo(int a)
    {
        Func<int, K<MyMaybeF, int>> f       = n => ParseInt((n + 1).ToString());
        Func<int, K<MyMaybeF, int>> pure     = MyMaybeF.Pure;
        var pureThenF = KleisliByBind(pure, f);
        return (pureThenF(a), f(a));   // 두 값이 같아야 한다.
    }

    // ── 챌린지 3 — LINQ ↔ 중첩 Bind 동치 ────────────────────────────────
    //
    // §7.6.1 의 LINQ 예제. C# 컴파일러가 from-from-select 를 SelectMany 호출로 바꾼다.
    public static K<MyMaybeF, int> ViaLinq() =>
        from a in ParseInt("3")
        from b in ParseInt("4")
        select a + b;

    // 같은 계산을 중첩 Bind 로 직접 작성. SelectMany 정의
    //   Bind(ma, a => Bind(bind(a), b => Pure(project(a, b))))
    // 를 손으로 펼친 형태 — ViaLinq 와 *정확히 같은 값* 을 낸다.
    public static K<MyMaybeF, int> ViaNestedBind() =>
        MyMaybeF.Bind(ParseInt("3"), a =>
            MyMaybeF.Bind(ParseInt("4"), b =>
                MyMaybeF.Pure(a + b)));
}
