using Ch04.Traits;

namespace Ch04.Challenges;

// 챌린지 ① — Const<C, A> Functor.
//
// 실제 값은 C 한 개만 들고, 두 번째 타입 매개변수 A 는 *phantom* — 시그니처에만 등장.
// Functor.Map 의 변환 함수 f 는 *한 번도 호출되지 않는다*. 결과는 같은 C value 를 든 Const<C, B>.
//
// 이게 흥미로운 이유 — *함수 호출이 0 회여도 Functor 법칙은 자동 성립*. MyMaybe.Nothing.Map(f)
// 와 같은 발상. applicative 가 효과만 따로 모으는 발상의 출발점.
public sealed class Const<C, A>(C value) : K<ConstF<C>, A>
{
    public C Value { get; } = value;

    public override string ToString() => $"Const({Value})";
}

public sealed class ConstF<C> : Functor<ConstF<C>>
{
    public static K<ConstF<C>, B> Map<A, B>(Func<A, B> f, K<ConstF<C>, A> fa)
    {
        // 핵심 — f 는 호출하지 않는다. 같은 Value 로 Const<C, B> 를 새로 만든다.
        var c = fa.As();
        return new Const<C, B>(c.Value);
    }
}

public static class ConstExtensions
{
    public static Const<C, A> As<C, A>(this K<ConstF<C>, A> fa) => (Const<C, A>)fa;
}
