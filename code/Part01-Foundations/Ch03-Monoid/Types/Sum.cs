using Ch03.Traits;

namespace Ch03.Types;

// 덧셈 Monoid — Empty 는 0, Combine 은 + (instance 의 결합 능력).
public readonly record struct Sum(int Value) : Monoid<Sum>
{
    public static Sum Empty => new(0);
    public Sum Combine(Sum rhs) => new(Value + rhs.Value);

    // operator + 명시 — 구체 타입에서 직접 호출 가능하게.
    // (interface 의 static virtual operator default 는 generic constraint context 안에서만 호출 가능.)
    public static Sum operator +(Sum lhs, Sum rhs) => lhs.Combine(rhs);
}
