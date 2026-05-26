using Ch08.Traits;

namespace Ch08.Types;

public sealed class MyList<A>(IEnumerable<A> items) : K<MyListF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();
    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

// MyList 는 Traversable + Functor + Foldable 모두.
public sealed class MyListF : Traversable<MyListF>
{
    public static K<MyListF, B> Map<A, B>(Func<A, B> f, K<MyListF, A> fa) =>
        new MyList<B>(fa.As().Items.Select(f));

    public static S Fold<A, S>(Func<S, A, S> f, S initial, K<MyListF, A> fa)
    {
        var list = fa.As();
        var acc = initial;
        foreach (var item in list.Items)
            acc = f(acc, item);
        return acc;
    }

    // 핵심 — Traverse. 두 kind 의 swap.
    public static K<F, K<MyListF, B>> Traverse<F, A, B>(Func<A, K<F, B>> f, K<MyListF, A> ta)
        where F : Applicative<F>
    {
        var list = ta.As();

        // 빈 리스트 — Pure(빈 리스트).
        K<F, K<MyListF, B>> acc = F.Pure<K<MyListF, B>>(new MyList<B>([]));

        // 뒤에서 앞으로 누적 — 각 원소를 f 로 변환 후 결합.
        foreach (var a in list.Items.Reverse())
        {
            var fb = f(a);

            // (B, K<MyListF, B>) → K<MyListF, B>  의 curried 함수.
            Func<B, Func<K<MyListF, B>, K<MyListF, B>>> prepend =
                head => tail => new MyList<B>([head, ..tail.As().Items]);

            // F.Pure(prepend) → 한 단계씩 Apply.
            var liftedFn = F.Pure(prepend);
            var step1    = F.Apply<B, Func<K<MyListF, B>, K<MyListF, B>>>(liftedFn, fb);
            acc          = F.Apply<K<MyListF, B>, K<MyListF, B>>(step1, acc);
        }

        return acc;
    }
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MyListExtensions
{
    public static MyList<A> As<A>(this K<MyListF, A> fa) => (MyList<A>)fa;
}
