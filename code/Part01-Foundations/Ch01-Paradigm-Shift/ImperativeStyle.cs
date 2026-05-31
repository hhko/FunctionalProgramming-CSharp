namespace Ch01;

// 명령형 (절차의 나열).
//
// 세 약점을 코드로 보인다: 상태 변수의 추적 부담, 캡슐화의 깨짐
// (외부 코드가 Total 갱신을 우회), 동시성과 결합 시 race condition.
public static class ImperativeStyle
{
    public static void Run()
    {
        Console.WriteLine("== 명령형 — 절차의 나열 ==");

        // sum 변수가 매 반복마다 *변경* 된다.
        var numbers = new[] { 1, 2, 3, 4, 5 };
        Console.WriteLine($"  SumImperative([1..5]) = {SumImperative(numbers)}      (= 15)");

        // *상태가 흩어진다* — Order 의 캡슐화 약속이 깨지는 시연.
        Console.WriteLine();
        Console.WriteLine("  [캡슐화 깨짐 — AddItem 우회 시연]");
        var order = new Order();
        order.AddItem(new Item("사과", 1000m));
        order.AddItem(new Item("배",   2000m));
        Console.WriteLine($"    AddItem × 2 → Items.Count = {order.Items.Count}, Total = {order.Total}원   (정상)");

        // 외부 코드가 *AddItem 을 우회* 해 Items 만 직접 수정 → 불변식 (invariant) 깨짐.
        order.Items.Add(new Item("의심거래", 99999m));
        Console.WriteLine($"    Items.Add 직접 호출 → Items.Count = {order.Items.Count}, Total = {order.Total}원");
        Console.WriteLine("    ↑ Items 는 3개인데 Total 은 갱신 안 됨 — *불변식 (invariant) 깨짐*");

        // 명령형 + 동시성 = race condition.
        Console.WriteLine();
        Console.WriteLine("  [동시성 + 명령형 = race condition 시연]");
        RaceConditionDemo();
    }

    private static int SumImperative(int[] numbers)
    {
        int sum = 0;                       // ← 상태 변수의 도입
        for (int i = 0; i < numbers.Length; i++)
        {
            sum = sum + numbers[i];        // ← 매 반복마다 sum 의 값이 *변경*
        }
        return sum;
    }

    private record Item(string Name, decimal Price);

    private sealed class Order
    {
        public List<Item> Items { get; } = new();    // ← 외부에서 수정 가능 (캡슐화 약점)
        public decimal Total { get; private set; }   // ← AddItem 이 갱신하기로 약속

        public void AddItem(Item item)
        {
            Items.Add(item);
            Total += item.Price;
        }
    }

    // 2 개 스레드가 같은 counter 를 100,000 번씩 증가. 락 없으면 *유실* 발생.
    private static void RaceConditionDemo()
    {
        var counter = new Counter();
        const int N = 100_000;
        var t1 = new Thread(() => { for (int i = 0; i < N; i++) counter.IncrementUnsafe(); });
        var t2 = new Thread(() => { for (int i = 0; i < N; i++) counter.IncrementUnsafe(); });
        t1.Start(); t2.Start(); t1.Join(); t2.Join();
        Console.WriteLine($"    기대값: {2 * N:N0}, 실제값: {counter.Value:N0}  (차이 = 유실된 증가)");
    }

    private sealed class Counter
    {
        private int _value;
        public int Value => _value;
        public void IncrementUnsafe()
        {
            // 읽기 → 더하기 → 쓰기 사이에 다른 스레드가 끼어들면 한 번의 증가가 *유실* 된다.
            _value = _value + 1;
        }
    }
}
