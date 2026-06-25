using Ch16.Functions;
using Ch16.Traits;
using Ch16.Types;

namespace Ch16.Challenges;

// WithTemp — 하위 계산을 임시로 변형한 상태에서 돌린 뒤 원래 상태로 복원한다.
// 15장 Reader 의 Local 을 State 의 get/put 으로 재현한 모양. 시작에서 get 으로 원래 상태 s 를
// 값으로 붙잡아 두고, 끝에서 put(s) 로 되돌리므로 Bind 가 그 저장값을 마지막까지 실어 나른다.
public static class TempState
{
    public static K<StateF<S>, A> WithTemp<S, A>(Func<S, S> setter, K<StateF<S>, A> op) =>
        from s  in Stateful.get<StateF<S>, S>()       // 원래 상태를 값으로 붙잡음
        from _  in Stateful.put<StateF<S>, S>(setter(s))   // 임시 상태로 교체
        from r  in op                                  // 하위 계산을 그 임시 상태에서
        from _2 in Stateful.put<StateF<S>, S>(s)       // 원래 상태로 복원
        select r;
}
