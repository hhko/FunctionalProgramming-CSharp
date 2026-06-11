using System.Diagnostics.Contracts;
using Ch03.Traits;

namespace Ch03.Tests;

// 결합 법칙 위반 시연 — Combine 이 두 값의 평균을 돌려준다.
//
// Mean 은 *시그니처는 완벽* 하다. Combine 은 같은 타입 두 값을 받아 같은 타입 한 값을
// 돌려주고 (Mean → Mean → Mean), Empty 도 갖춰 Monoid<Mean> 을 구현한다. 그런데 평균은
// *결합 법칙* 을 어긴다 — 묶는 순서가 결과를 바꾼다.
//   (a·b)·c : ((2+4)/2 = 3),  (3+8)/2 = 5.5
//   a·(b·c) : ((4+8)/2 = 6),  (2+6)/2 = 4.0   → 5.5 ≠ 4.0
// 컴파일러는 막을 수 없지만 *Monoid 의 결합 법칙* 을 어긴다.
//
// 본문 §3.7 의 핵심 — 시그니처는 필요조건이지 충분조건이 아니다. 두 법칙 (결합 + 항등) 의
// 실제 성립이 진짜 Monoid 의 약속이라, MonoidLaws.AssociativityHolds 가 false 를 낸다.
// (Sum / Concat 같은 진짜 Monoid 는 true. 다른 장의 BogusListF / BogusApplicativeF 와 같은 결.)
public readonly record struct Mean(double Value) : Monoid<Mean>
{
    // 평균에는 진짜 항등원이 없다 — 0 을 두지만 항등 법칙도 함께 깨진다.
    [Pure]
    public static Mean Empty => new(0.0);

    // 두 값의 평균 — 결합 법칙을 어기는 비결합 연산.
    [Pure]
    public Mean Combine(Mean rhs) => new((Value + rhs.Value) / 2.0);
}
