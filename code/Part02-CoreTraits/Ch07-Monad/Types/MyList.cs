using Ch07.Traits;

namespace Ch07.Types;

// 두 번째 Monad 인스턴스 — List 의 비결정성 (§7.3.4).
//
// MyMaybe 의 bind 가 *단락* (한 단계 실패면 멈춤) 이라면,
// MyList 의 bind 는 *비결정성* — 각 원소마다 여러 결과를 내고 모두 이어붙인다.
// 같은 Bind 시그니처가 자료 구조에 따라 다른 얼굴을 보인다.
public sealed record MyList<A>(IReadOnlyList<A> Items) : K<MyListF, A>;

public sealed class MyListF : Monad<MyListF>
{
    public static K<MyListF, B> Map<A, B>(Func<A, B> f, K<MyListF, A> fa) =>
        new MyList<B>(fa.As().Items.Select(f).ToArray());

    // Pure — 원소 하나짜리 리스트 (singleton). List 세계로 끌어올리는 가장 단순한 형태.
    public static K<MyListF, A> Pure<A>(A value) =>
        new MyList<A>(new[] { value });

    // Apply — 함수 목록 × 값 목록의 모든 조합 (cartesian product).
    public static K<MyListF, B> Apply<A, B>(K<MyListF, Func<A, B>> mf, K<MyListF, A> ma) =>
        new MyList<B>(
            (from f in mf.As().Items
             from a in ma.As().Items
             select f(a)).ToArray());

    // Bind — 각 원소에 f 를 적용(각자 리스트)하고 모두 이어붙임.
    // 곧 map 후 flatten (concatMap). [1,2,3].Bind(n => [n, n*10]) = [1,10,2,20,3,30].
    public static K<MyListF, B> Bind<A, B>(K<MyListF, A> ma, Func<A, K<MyListF, B>> f) =>
        new MyList<B>(
            ma.As().Items.SelectMany(a => f(a).As().Items).ToArray());
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyListExtensions
{
    public static MyList<A> As<A>(this K<MyListF, A> fa) => (MyList<A>)fa;
}
