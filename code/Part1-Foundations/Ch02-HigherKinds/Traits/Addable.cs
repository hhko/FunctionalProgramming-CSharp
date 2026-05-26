namespace Ch02.Traits;

// 자기 참조 제약 (self-bound) 의 가장 단순한 trait — 두 값을 *더하는* 능력 한 개.
//
// SELF 가 *자기 자신* 을 가리킨다 — `where SELF : Addable<SELF>`. 이게 함수형 trait 의
// 표준 패턴이다 (§2.6.1).
public interface Addable<SELF> where SELF : Addable<SELF>
{
    static abstract SELF Add(SELF a, SELF b);
}
