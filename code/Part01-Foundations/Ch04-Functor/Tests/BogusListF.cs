using Ch04.Traits;

namespace Ch04.Tests;

// 항등 법칙 위반 시연 — Map 이 입력과 무관하게 빈 결과를 반환.
//
// BogusListF.Map 은 *시그니처는 완벽* 한데 *항등 함수* 를 매핑해도 *원본과 다른 빈 리스트* 를
// 돌려준다. 컴파일러는 막을 수 없지만 *Functor 의 모양 보존 직관* 을 어긴다.
//
// 본문 §3.6.5 의 핵심 — 시그니처 외에 추가되는 *Functor 의 사용 계약* (두 법칙) 이 필요한 이유.
public sealed class BogusList<A>(IEnumerable<A> items) : K<BogusListF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();

    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

public sealed class BogusListF : Functor<BogusListF>
{
    public static K<BogusListF, B> Map<A, B>(Func<A, B> f, K<BogusListF, A> fa) =>
        new BogusList<B>([]);                  // ← 입력 무시, 항상 빈 결과
}

public static class BogusListExtensions
{
    public static BogusList<A> As<A>(this K<BogusListF, A> fa) => (BogusList<A>)fa;
}
