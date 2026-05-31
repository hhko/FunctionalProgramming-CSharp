using Ch03.Traits;

namespace Ch03.Types;

// 문자열 이어붙이기 Monoid — Empty 는 "", Combine 은 + (instance 의 결합 능력).
public readonly record struct Concat(string Value) : Monoid<Concat>
{
    public static Concat Empty => new("");
    public Concat Combine(Concat rhs) => new(Value + rhs.Value);

    // operator + 명시 — 구체 타입에서 직접 호출 가능하게.
    public static Concat operator +(Concat lhs, Concat rhs) => lhs.Combine(rhs);
}
