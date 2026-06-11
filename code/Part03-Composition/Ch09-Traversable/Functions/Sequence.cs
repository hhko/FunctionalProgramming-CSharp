using Ch09.Traits;
using Ch09.Types;

namespace Ch09.Functions;

// Sequence — Traversable trait 의 virtual default 가 이미 정의돼 있다.
//   K<T, K<F, A>>  →  K<F, K<T, A>>
//
// 본 파일은 그 위에 *실전 사용 패턴* 두 개를 헬퍼로 묶어 elevated world 어휘를
// 손에 익히게 만든다.
//
//   * SequenceListOption — List<Option<A>> 를 Option<List<A>> 로. 한 개라도
//                          None 이면 전체 None (단락). 모두 Some 이면 Some(list).
//
//   * TraverseListWith   — 변환 함수 f : A → K<F, B> 를 리스트 전체에 적용 후
//                          결과를 elevated 안의 리스트로 모은다. fold + applicative
//                          으로 합성된 가장 일반적 패턴.
public static class Sequence
{
    // List<Option<A>> → Option<List<A>>.
    // Sequence 는 Traversable virtual default 인데, C# static virtual 은 *인터페이스 이름* 으로
    // 호출해야 한다 — Traversable<MyListF>.Sequence(...). 등식 sequence = traverse(id) 를
    // 직접 쓰면 더 단순하다.
    public static K<MyMaybeF, K<MyListF, A>> SequenceListOption<A>(
        K<MyListF, K<MyMaybeF, A>> listOfOptions)
    =>
        MyListF.Traverse<MyMaybeF, K<MyMaybeF, A>, A>(x => x, listOfOptions);

    // 변환 + 시퀀스 — K<List, A> 를 받아 *각 원소를 f 로 변환* 한 뒤 elevated 안에 모은다.
    // 동일 결과를 Traverse 가 한 번에 낸다 — Sequence + Map 보다 효율적.
    public static K<F, K<MyListF, B>> TraverseListWith<F, A, B>(
        Func<A, K<F, B>> f, K<MyListF, A> list)
        where F : Applicative<F>
    =>
        MyListF.Traverse<F, A, B>(f, list);

    // 항등 등식 — Sequence = Traverse(id).
    //
    // 이 등식이 *왜 Sequence 가 별도 함수가 아닌가* 의 답. id 함수를 Traverse 에
    // 넘기면 Sequence 가 된다. 본 식이 elevated world 어휘의 *우아함* 이다.
    public static K<F, K<MyListF, A>> SequenceEqualsTraverseId<F, A>(
        K<MyListF, K<F, A>> tfa)
        where F : Applicative<F>
    =>
        MyListF.Traverse<F, K<F, A>, A>(x => x, tfa);
}
