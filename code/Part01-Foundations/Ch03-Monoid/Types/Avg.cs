using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Types;

// 평균을 Monoid 로 만드는 패턴 — 자료의 모양을 바꿔 Monoid 어휘 안으로 끌어들이는 자리.
//
// 평균 값 한 개 (double) 는 결합 법칙이 깨져 직접 Monoid 가 아닙니다.
// 그러나 {Total, Count} 두 자리의 자료로 누적하면 두 정수의 덧셈 Monoid 두 개 짝이라
// 자연스럽게 Monoid 가 됩니다. 마지막 단계에 .Value 로 평균을 추출.
//
// 패턴 (Wlaschin, Working with non-monoids) — 먼저 자료를 Monoid 로 설계하고, 원하는 결과는
// 마지막에 추출한다. Avg 는 {Total, Count} 로 누적한 뒤 .Value 로 평균을 뽑는다.
public readonly record struct Avg(int Total, int Count) : Monoid<Avg>
{
    [Pure]
    public static Avg Empty => new(0, 0);

    [Pure]
    public Avg Combine(Avg rhs) => new(Total + rhs.Total, Count + rhs.Count);

    // 최종 추출 — 누적된 자료에서 평균 계산. 빈 경우 (Count == 0) 는 0.0.
    [Pure]
    public double Value => Count == 0 ? 0.0 : (double)Total / Count;

    // 한 값을 입력하는 정통 생성 어법 — Avg.Of(5) == Avg(5, 1).
    [Pure]
    public static Avg Of(int value) => new(value, 1);
}
