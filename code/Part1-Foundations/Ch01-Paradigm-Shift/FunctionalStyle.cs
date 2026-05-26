namespace Ch01;

// 본문 §1.4 — 함수형 (함수의 합성).
//
// *불변* + *합성* + *추상* 의 세 결합. §1.4.5 의 *같은 문제, 세 패러다임의 풀이* 가
// 세 풀이의 *모양 차이* 를 직접 비교하는 자리.
public static class FunctionalStyle
{
    public static void Run()
    {
        Console.WriteLine("== §1.4 함수형 — 함수의 합성 ==");

        // §1.4.1 — Aggregate (fold) 한 줄. 상태 변수 없음.
        var numbers = new[] { 1, 2, 3, 4, 5 };
        int sum = numbers.Aggregate(0, (acc, n) => acc + n);
        Console.WriteLine($"  numbers.Aggregate(0, +) = {sum}     (acc 는 *루프 변수가 아니라 함수 매개변수*)");
        Console.WriteLine($"  numbers.Sum()           = {numbers.Sum()}     (LINQ 의 Sum 은 같은 의미의 syntactic sugar)");

        // §1.4.5 — 같은 문제 (짝수의 제곱합) 를 세 패러다임으로.
        Console.WriteLine();
        Console.WriteLine("  [§1.4.5 같은 문제, 세 패러다임의 풀이 — 짝수의 제곱합]");
        var data = new[] { 1, 2, 3, 4, 5, 6 };
        int expected = 4 + 16 + 36;  // 2^2 + 4^2 + 6^2
        Console.WriteLine($"    명령형:    {SumOfSquaresOfEvensImperative(data),3}   (for + if + 상태 변수)");
        Console.WriteLine($"    객체 지향: {new EvenSquareSum(data).Compute(),3}   (책임 분리, 내부는 여전히 명령형)");
        Console.WriteLine($"    함수형:    {SumOfSquaresOfEvensFunctional(data),3}   (.Where / .Select / .Sum — 한 줄)");
        Console.WriteLine($"    (기대값:   {expected,3})");
    }

    // 명령형 — for + if + 상태 변수
    private static int SumOfSquaresOfEvensImperative(int[] numbers)
    {
        int sum = 0;
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] % 2 == 0)
                sum += numbers[i] * numbers[i];
        }
        return sum;
    }

    // 객체 지향 — 책임을 클래스 + 작은 메서드 (IsEven / Square) 로 분리.
    // 내부는 여전히 명령형 절차.
    private sealed class EvenSquareSum
    {
        private readonly int[] _numbers;
        public EvenSquareSum(int[] numbers) => _numbers = numbers;

        public int Compute()
        {
            int sum = 0;
            foreach (var n in _numbers)
                if (IsEven(n))
                    sum += Square(n);
            return sum;
        }

        private static bool IsEven(int n) => n % 2 == 0;
        private static int  Square(int n) => n * n;
    }

    // 함수형 — 단계마다 *함수 이름이 의미를 명시*.
    private static int SumOfSquaresOfEvensFunctional(int[] numbers) =>
        numbers
            .Where(n => n % 2 == 0)
            .Select(n => n * n)
            .Sum();
}
