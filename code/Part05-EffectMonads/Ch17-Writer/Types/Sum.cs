using Ch17.Traits;

namespace Ch17.Types;

// Sum — 또 다른 구체 Monoid 인스턴스. 정수의 합 누적.
//   Empty   = 0 (더해도 상대를 안 바꾸는 항등원)
//   Combine = 두 수를 더함
// Writer 의 W 자리에 Log 대신 들어가면, 같은 Writer 가 로그가 아니라
// 비용·횟수 같은 수를 누적한다. WriterF 의 Pure·Bind 코드는 한 줄도 바뀌지 않는다.
public sealed class Sum(int n) : Monoid<Sum>
{
    public int Value { get; } = n;

    public static Sum Empty => new(0);
    public Sum Combine(Sum rhs) => new(Value + rhs.Value);

    public static Sum Of(int n) => new(n);

    public override string ToString() => Value.ToString();
}
