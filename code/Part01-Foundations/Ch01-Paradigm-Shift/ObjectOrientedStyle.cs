namespace Ch01;

// 객체 지향 (능력의 캡슐화).
//
// 명령형의 *상태가 흩어진다* 를 *상태 변경을 객체 안으로 좁힌다* 로 푼다.
// 그러나 *변경 자체* 가 남아 있어 *동시성 + 객체 그래프* 비용은 그대로다.
public static class ObjectOrientedStyle
{
    public static void Run()
    {
        Console.WriteLine("== 객체 지향 — 능력의 캡슐화 ==");

        // Counter 가 상태를 *내부* 에 가둔다.
        var counter = new Counter();
        counter.Increment();
        counter.Increment();
        counter.Increment();
        Console.WriteLine($"  Counter (3 번 Increment)    = {counter.Value}");
        Console.WriteLine("  ↑ _count 가 private — 외부에서 직접 변경 불가 (변경의 출처가 객체 안으로 좁혀짐)");

        // 4 대 원칙 중 *다형성* — 같은 인터페이스가 다른 동작을 표현.
        Console.WriteLine();
        Console.WriteLine("  [다형성 — IShape.Area() 한 호출, 도형마다 다른 공식]");
        IShape[] shapes = [new Circle(3), new Square(4), new Triangle(3, 4)];
        foreach (var s in shapes)
            Console.WriteLine($"    {s.GetType().Name,-9}.Area() = {s.Area():F2}");

        // 약점: *Sum* 같은 능력이 컬렉션마다 별도 정의됨.
        Console.WriteLine();
        Console.WriteLine("  [약점 — 같은 능력이 컬렉션마다 별도 정의]");
        Console.WriteLine("    MyArrayList.Sum()  — 별도 구현");
        Console.WriteLine("    MyLinkedList.Sum() — 별도 구현 (같은 의미인데도)");
        Console.WriteLine("    → 함수형은 이 한계를 *Foldable* trait 으로 푼다 (6장).");
    }

    private sealed class Counter
    {
        private int _count;                              // ← 상태가 *캡슐화*
        public void Increment() => _count++;             // ← 변경은 메서드를 통해서만
        public int Value => _count;
    }

    private interface IShape { double Area(); }

    private sealed record Circle(double Radius) : IShape
    {
        public double Area() => Math.PI * Radius * Radius;
    }

    private sealed record Square(double Side) : IShape
    {
        public double Area() => Side * Side;
    }

    private sealed record Triangle(double Base, double Height) : IShape
    {
        public double Area() => 0.5 * Base * Height;
    }
}
