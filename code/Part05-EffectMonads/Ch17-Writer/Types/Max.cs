using Ch17.Traits;

namespace Ch17.Types;

// Max — Combine 이 '합치기' 가 아니라 '고르기' 인 Monoid. 단계별 최댓값 누적.
//   Empty   = int.MinValue (어떤 값과 Max 해도 그 값이 이기는 항등원)
//   Combine = 둘 중 큰 값을 고름
// Writer 의 W 자리에 Log·Sum 대신 들어가도 WriterF 의 Pure·Bind 코드는 한 줄도 안 바뀐다.
// Combine 이 누적이 아니라 선택이어도 결합 법칙·항등원을 지키면 정당한 Monoid 다.
public sealed class Max(int n) : Monoid<Max>
{
    public int Value { get; } = n;

    public static Max Empty => new(int.MinValue);
    public Max Combine(Max rhs) => new(Value >= rhs.Value ? Value : rhs.Value);

    public static Max Of(int n) => new(n);

    public override string ToString() => Value.ToString();
}
