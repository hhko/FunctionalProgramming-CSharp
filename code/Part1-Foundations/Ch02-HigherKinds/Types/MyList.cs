using Ch02.Traits;

namespace Ch02.Types;

// 본문 §2.5.1 의 *조각 ② — 자료 타입*.
//
// `K<MyListF, A>` 를 구현해 "MyListF 안에 A 가 있다" 는 type-level 신호를 컴파일러에
// 전달한다. 실제 자료는 `Items` 로 보관.
public sealed class MyList<A>(IEnumerable<A> items) : K<MyListF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();

    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

// 본문 §2.5.1 의 *조각 ③ — 태그 타입*.
//
// `Functor<MyListF>` 를 *직접 구현* 해 `Map` 의 정적 호스트가 된다.
// 호출 형식 — `MyListF.Map(f, xs)` (값의 인스턴스 메서드가 아니라 타입 이름의 정적 메서드).
public sealed class MyListF : Functor<MyListF>
{
    public static K<MyListF, B> Map<A, B>(Func<A, B> f, K<MyListF, A> fa)
    {
        var list = fa.As();
        return new MyList<B>(list.Items.Select(f));
    }
}

// 본문 §2.5.7 — LanguageExt 식 *확장 메서드* 로 다운캐스트 보일러플레이트를 감춘다.
// 사용 — `K<MyListF, A> fa` 에 대해 `fa.As()` 한 줄.
public static class MyListExtensions
{
    public static MyList<A> As<A>(this K<MyListF, A> fa) => (MyList<A>)fa;
}
