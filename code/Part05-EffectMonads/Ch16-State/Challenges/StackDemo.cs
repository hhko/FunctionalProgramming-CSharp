using Ch16.Functions;
using Ch16.Traits;
using Ch16.Types;

namespace Ch16.Challenges;

// 상태가 정수가 아니라 자료 구조(스택)여도 같은 네 동사·같은 Bind 스레딩으로 다뤄진다.
// 스택은 int[] 를 불변으로 취급 — top 은 마지막 원소. push/pop 은 새 배열을 만들어 흘린다.
public static class StackDemo
{
    public static K<StateF<int[]>, Unit> Push(int n) =>
        Stateful.modify<StateF<int[]>, int[]>(s => [.. s, n]);

    public static K<StateF<int[]>, int> Pop() =>
        from s in Stateful.get<StateF<int[]>, int[]>()
        from _ in Stateful.put<StateF<int[]>, int[]>(s[..^1])
        select s[^1];

    // Push(1) · Push(2) · Pop → 값 2, 최종 상태 [1]
    public static K<StateF<int[]>, int> Demo() =>
        from _1 in Push(1)
        from _2 in Push(2)
        from top in Pop()
        select top;
}
