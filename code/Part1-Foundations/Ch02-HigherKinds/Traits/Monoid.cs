namespace Ch02.Traits;

// Monoid — Addable + *항등원 Empty*.
//
// 빈 시퀀스도 안전하게 누적 가능하게 만드는 한 조각 — `Add(Empty, x) == x == Add(x, Empty)`.
// `Addable` 만으로는 0 개 원소를 처리할 수 없다.
public interface Monoid<SELF> : Addable<SELF> where SELF : Monoid<SELF>
{
    static abstract SELF Empty { get; }
}
