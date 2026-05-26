using Ch05.Traits;

namespace Ch05.Functions;

// 어떤 Foldable F 든 받는 일반 함수 (본문 §4.5.1 ~ §4.5.2).
//
// trait 메서드 (F.FoldLeft / F.FoldRight) 위에서 자유 함수가 자동 동작.
// MyList / MyMaybe / 미래의 어떤 Foldable 든 같은 호출로 처리.
public static class SumAnyFoldable
{
    // int 컨테이너의 합 — 어떤 F 든.
    public static int Run<F>(K<F, int> fa)
        where F : Foldable<F> =>
        F.FoldLeft<int, int>((acc, n) => acc + n, 0, fa);
}

// Count, All, Any 도 같은 패턴 — 어떤 F 든 동작.
public static class FoldableHelpers
{
    // 어떤 A 든 원소 개수.
    public static int CountAny<F, A>(K<F, A> fa)
        where F : Foldable<F> =>
        F.Count(fa);

    // 어떤 A 든 모든 원소가 술어 만족.
    public static bool AllAny<F, A>(Func<A, bool> p, K<F, A> fa)
        where F : Foldable<F> =>
        F.All(p, fa);

    // 어떤 A 든 한 원소라도 술어 만족.
    public static bool AnyAny<F, A>(Func<A, bool> p, K<F, A> fa)
        where F : Foldable<F> =>
        F.Any(p, fa);
}

// 합성 헬퍼 — 두 함수의 수학적 합성 g ∘ f.
// §4.6 의 일관성 법칙 검증과 §4.7 의 자유 함수 합성에서 사용.
public static class Compose
{
    public static Func<A, C> Of<A, B, C>(Func<B, C> g, Func<A, B> f) =>
        x => g(f(x));
}
