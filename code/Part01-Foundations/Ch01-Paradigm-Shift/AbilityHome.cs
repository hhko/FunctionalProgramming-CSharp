namespace Ch01;

// 능력은 어디 사는가 (한 차원 비교).
//
// 명령형: 능력 = 함수의 본문 / 객체 지향: 능력 = 객체에 묶인 메서드 / 함수형:
// 능력 = trait (책장) 의 정적 자리. 2장 / 3장 / 4장의 *trait + static abstract* 가 이를 코드로 한다.
public static class AbilityHome
{
    public static void Run()
    {
        Console.WriteLine("== 능력은 어디 사는가 — 한 차원 비교 ==");

        // [명령형] 능력 = 함수 시그니처 자체.
        int Add(int a, int b) => a + b;
        Console.WriteLine($"  명령형:    Add(2, 3) = {Add(2, 3)}");
        Console.WriteLine("             → 능력이 *함수의 본문* 에 산다.");

        // [객체 지향] 능력 = 객체 인스턴스의 메서드.
        var calc = new Calculator();
        Console.WriteLine($"  객체 지향: calc.Add(2, 3) = {calc.Add(2, 3)}");
        Console.WriteLine("             → 능력이 *객체* 에 산다 — List 마다 / Set 마다 별도 정의가 필요.");

        // [함수형 (간략)] 능력 = 타입 자체의 정적 멤버.
        // 본격 trait + static abstract 는 2장 (Higher Kinds) / 3장 (Monoid) / 4장 (Functor) 에서 본다.
        Console.WriteLine($"  함수형:    IntAdder.Add(2, 3) = {IntAdder.Add(2, 3)}");
        Console.WriteLine("             → 능력이 *trait (책장)* 에 산다 — Foldable / Monoid 의 자리.");
    }

    private sealed class Calculator
    {
        public int Add(int a, int b) => a + b;
    }

    private static class IntAdder
    {
        public static int Add(int a, int b) => a + b;
    }
}
