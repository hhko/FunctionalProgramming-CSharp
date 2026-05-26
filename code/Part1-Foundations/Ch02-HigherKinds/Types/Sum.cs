using Ch02.Traits;

namespace Ch02.Types;

// 정수 덧셈 모노이드 — `Add = +`, `Empty = 0`.
//
// 3-tuple 패턴의 가장 단순한 형태 — 자료 (`Sum`) + trait 구현 (자체 정적 멤버) 만.
// Monoid 는 *높은 카인드 아님* 이므로 별도의 태그 타입 (`SumF` 등) 이 필요 없다.
public readonly record struct Sum(int Value) : Monoid<Sum>
{
    public static Sum Add(Sum a, Sum b) => new(a.Value + b.Value);

    public static Sum Empty => new(0);

    public override string ToString() => $"Sum({Value})";
}
