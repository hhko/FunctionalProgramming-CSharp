using Ch20.Functions;
using Ch20.Traits;
using Ch20.Types;

namespace Ch20.Challenges;

// 챌린지 ① 정답 — StateT<Stack, OptionF, A>: 상태 + 실패 두 효과를 한 스택에.
//
// push 는 상태만 바꾸고, pop 은 빈 스택이면 *실패(None)* 한다.
// 상태 스레딩(StateT)과 실패(내부 Option)가 동시에 작동한다.
public static class Stack
{
    public static K<StateTF<List<int>, OptionF>, Unit> Push(int x) =>
        Stateful.modify<StateTF<List<int>, OptionF>, List<int>>(s => [x, .. s]);

    // 빈 스택이면 내부 Option 이 None — 전체 계산이 단락된다.
    public static K<StateTF<List<int>, OptionF>, int> Pop =>
        new StateT<List<int>, OptionF, int>(s =>
            s.Count == 0
                ? Option<(int, List<int>)>.None.Instance
                : new Option<(int, List<int>)>.Some((s[0], s.Skip(1).ToList())));
}
