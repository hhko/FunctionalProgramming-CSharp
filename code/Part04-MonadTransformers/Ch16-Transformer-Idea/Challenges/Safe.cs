using Ch16.Traits;
using Ch16.Types;

namespace Ch16.Challenges;

// 챌린지 ① 정답 — OptionT<ManyF, A> 위의 안전한 계산.
//
// 내부 모나드 Many (비결정성) + Option 층 (실패) 이 *한 스택* 으로 합쳐진다.
// 각 갈래가 독립적으로 성공/실패할 수 있다 — 결과는 Many<Option<int>>.
public static class Safe
{
    // 0 이면 실패(None), 아니면 100/x (Some). 두 효과를 한 값에 담는다.
    public static K<OptionTF<ManyF>, int> Recip(int x) =>
        x == 0
            ? new OptionT<ManyF, int>(ManyF.Pure<Option<int>>(Option<int>.None.Instance))
            : OptionTF<ManyF>.Pure(100 / x);
}
