namespace Ch14.Traits;

// Choice — *선택*. 두 Elevated 후보 중 *성공하는 쪽* 을 고른다.
//
//   Choose : K<F,A> → K<F,A> → K<F,A>
//
// MonoidK 의 Combine 과 시그니처는 같지만 *의미* 가 다르다:
//   Combine — 둘을 *모두* 모은다 (예: 두 시퀀스 concat).
//   Choose  — 둘 중 *하나* 를 고른다 (예: 첫 Just, 첫 비어있지 않은 시퀀스).
// 자료 타입에 따라 둘이 같을 수도 (Seq), 다를 수도 (Maybe) 있다.
public interface Choice<F> where F : Choice<F>
{
    static abstract K<F, A> Choose<A>(K<F, A> fa, K<F, A> fb);
}
