using Ch09.Traits;
using Ch09.Types;

Console.WriteLine("================================================");
Console.WriteLine("8장 — Traversable");
Console.WriteLine("================================================");
Console.WriteLine();

Console.WriteLine("== 예제 1 — Traverse 로 *각 원소가 검증을 통과하는지* ==");

K<MyListF, int> nums = new MyList<int>([2, 4, 6]);

// 짝수면 Just, 홀수면 Nothing 반환하는 함수.
Func<int, K<MyMaybeF, int>> evenCheck = n =>
    n % 2 == 0
        ? MyMaybeF.Pure(n)
        : MyMaybe<int>.Nothing.Instance;

K<MyMaybeF, K<MyListF, int>> result = MyListF.Traverse<MyMaybeF, int, int>(evenCheck, nums);
ShowMaybeOfList(result);

Console.WriteLine();
Console.WriteLine("== 예제 2 — 한 원소가 홀수 — 결과는 Nothing ==");

K<MyListF, int> mixed = new MyList<int>([2, 3, 6]);
K<MyMaybeF, K<MyListF, int>> result2 = MyListF.Traverse<MyMaybeF, int, int>(evenCheck, mixed);
ShowMaybeOfList(result2);

static void ShowMaybeOfList(K<MyMaybeF, K<MyListF, int>> r)
{
    switch (r.As())
    {
        case MyMaybe<K<MyListF, int>>.Just j:
            Console.WriteLine($"  Just({j.Value.As()})");
            break;
        case MyMaybe<K<MyListF, int>>.Nothing:
            Console.WriteLine($"  Nothing");
            break;
    }
}
