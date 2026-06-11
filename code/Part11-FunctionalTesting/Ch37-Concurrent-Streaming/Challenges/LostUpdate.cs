using Ch37.Types;

namespace Ch37.Challenges;

// 챌린지 ① 정답 — 비원자 카운터로 *잃어버린 갱신* 을 관찰한다.
// 평범한 long n; n++ 를 1000 개 스레드로 동시 증가하면 갱신이 유실되어 최종값이 1000 *이하* 로 흔들린다.
// 그 결과는 *비결정적* 이라 == 1000 같은 단언의 대상이 아니다. 같은 코드를 Atom 으로 바꾸면 *항상* 1000.
public static class LostUpdate
{
    // 비원자 카운터를 동시 증가 — 잃어버린 갱신으로 최종값이 1000 이하로 나올 수 있다.
    // (필드를 ref 로 넘기려고 배열 한 칸을 쓴다. 락도 Interlocked 도 없는 평범한 ++.)
    public static long NonAtomicFinalValue()
    {
        var box = new long[1];
        Parallel.For(0, 1000, _ => box[0]++);   // 비원자 ++ — 읽기·증가·쓰기가 쪼개져 갱신 유실
        return box[0];
    }

    // 같은 동시 증가를 Atom 으로 — CAS 가 모든 증가를 반영해 *항상* 정확히 1000.
    public static long AtomFinalValue()
    {
        var atom = new Atom<Counter>(new Counter(0));
        Parallel.For(0, 1000, _ => atom.Swap(c => c with { N = c.N + 1 }));
        return atom.Value.N;
    }

    // 검증 포인트 — 비원자는 1000 이하로 흔들릴 *수 있고* (≤ 1000), Atom 은 *항상* 1000.
    // 비원자 결과는 비결정적이라 "< 1000" 을 단언하지 않는다. 단언하는 것은 Atom 의 == 1000 뿐.
    public static bool AtomAlwaysReachesThousand() =>
        AtomFinalValue() == 1000 && NonAtomicFinalValue() <= 1000;
}
