namespace Ch14.Traits;

// Monoid 재방문 (1부 Ch02) — Writer 의 누적이 Monoid 위에서 작동한다.
//
//   Semigroup : 결합 가능한 이항 연산 (Combine)
//   Monoid    : Semigroup + 단위원 (Empty)
//
// Writer 의 Bind 가 두 단계의 출력을 Combine 으로 합치고, Pure 의 출력이 Empty 다.
// self-bound (`W : Monoid<W>`) 로 컴파일러가 "W 가 Monoid 임" 을 검증한다.
public interface Semigroup<A> where A : Semigroup<A>
{
    A Combine(A rhs);
}

public interface Monoid<A> : Semigroup<A> where A : Monoid<A>
{
    static abstract A Empty { get; }
}
