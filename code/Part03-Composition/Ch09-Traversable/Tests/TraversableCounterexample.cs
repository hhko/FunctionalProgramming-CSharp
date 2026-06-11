using Ch09.Traits;
using Ch09.Types;

namespace Ch09.Tests;

// 본문 §9.10.1 의 *가짜 Traversable* 반례 — 실행 가능한 버전.
//
// BogusTraverse 는 진짜 MyListF.Traverse 와 *시그니처가 한 글자도 다르지 않다*.
// 다만 Reverse() 를 빼 앞에서부터 prepend 하므로 *순서가 뒤집힌다*.
// 컴파일은 통과하지만 항등 법칙 (Traverse(Pure, t) ≡ Pure(t)) 을 깬다.
// 시그니처가 막지 못하는 약속을 법칙이 막는다는 것을 코드로 보인다.
public static class TraversableCounterexample
{
    // 가짜 — Reverse 없이 앞에서부터 prepend. 시그니처는 MyListF.Traverse 와 동일.
    public static K<F, K<MyListF, B>> BogusTraverse<F, A, B>(Func<A, K<F, B>> f, K<MyListF, A> ta)
        where F : Applicative<F>
    {
        var list = ta.As();
        K<F, K<MyListF, B>> acc = F.Pure<K<MyListF, B>>(new MyList<B>([]));

        foreach (var a in list.Items)   // Reverse() 제거 — 앞에서부터 prepend
        {
            var fb = f(a);
            Func<B, Func<K<MyListF, B>, K<MyListF, B>>> prepend =
                head => tail => new MyList<B>([head, ..tail.As().Items]);
            var step1 = F.Apply<B, Func<K<MyListF, B>, K<MyListF, B>>>(F.Pure(prepend), fb);
            acc       = F.Apply<K<MyListF, B>, K<MyListF, B>>(step1, acc);
        }
        return acc;
    }

    // 진짜 Traverse 와 가짜 BogusTraverse 에 *항등 효과* (n => Pure(n)) 를 걸어
    // 결과 목록을 꺼낸다. 진짜는 입력 순서를, 가짜는 뒤집힌 순서를 낸다.
    // 반환: (진짜 결과, 가짜 결과) — 예: ([1, 2, 3], [3, 2, 1]).
    public static (IReadOnlyList<int> real, IReadOnlyList<int> bogus) IdentityOrder(IEnumerable<int> xs)
    {
        K<MyListF, int> input = new MyList<int>(xs);
        Func<int, K<MyMaybeF, int>> pure = MyMaybeF.Pure;   // 항등 효과

        var realResult  = MyListF.Traverse<MyMaybeF, int, int>(pure, input);
        var bogusResult = BogusTraverse<MyMaybeF, int, int>(pure, input);

        return (Extract(realResult), Extract(bogusResult));
    }

    // 진짜는 항등 법칙 통과 (입력 순서 보존), 가짜는 위반 (순서 뒤집힘).
    public static bool RealKeepsOrderBogusBreaks(IEnumerable<int> xs)
    {
        var input = xs.ToList();
        var (real, bogus) = IdentityOrder(input);
        return real.SequenceEqual(input)                              // 진짜 — 순서 보존
            && bogus.SequenceEqual(input.AsEnumerable().Reverse());   // 가짜 — 뒤집힘
    }

    static IReadOnlyList<int> Extract(K<MyMaybeF, K<MyListF, int>> r) =>
        r switch
        {
            MyMaybe<K<MyListF, int>>.Just j => j.Value.As().Items,
            _ => []
        };
}
