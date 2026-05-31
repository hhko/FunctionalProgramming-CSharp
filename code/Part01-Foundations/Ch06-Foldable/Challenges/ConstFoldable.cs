using Ch06.Traits;

namespace Ch06.Challenges;

// 심화 챌린지 ① — Const<C, A> Foldable.
//
// 실제 값은 C 한 개만 들고, 두 번째 타입 매개변수 A 는 phantom — 시그니처에만 등장.
// Foldable 의 step 함수 는 한 번도 호출되지 않는다 . 결과는 항상 seed.
//
// 이게 흥미로운 이유 — step 호출이 0 회여도 일관성 법칙이 자동 성립.
// MyMaybe.Nothing.FoldLeft(...) 와 같은 발상. 19장의 applicative 효과 분리의 출발점.
public sealed class Const<C, A>(C value) : K<ConstF<C>, A>
{
    public C Value { get; } = value;

    public override string ToString() => $"Const({Value})";
}

public sealed class ConstF<C> : Foldable<ConstF<C>>
{
    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<ConstF<C>, A> fa) =>
        seed;   // ← step 호출 없음, seed 그대로

    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<ConstF<C>, A> fa) =>
        seed;   // ← step 호출 없음, seed 그대로
}

public static class ConstExtensions
{
    public static Const<C, A> As<C, A>(this K<ConstF<C>, A> fa) => (Const<C, A>)fa;
}
