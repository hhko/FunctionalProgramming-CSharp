using Ch31.Functions;
using Ch31.Traits;
using Ch31.Types;

namespace Ch31.Challenges;

// 챌린지 ① 정답 — 계좌 이체를 한 트랜잭션으로. 출금과 입금이 *함께* 커밋된다 (all-or-nothing).
// 중간 상태(돈이 한쪽에서 빠졌는데 아직 안 들어온)가 다른 트랜잭션에 절대 보이지 않는다.
public static class Bank
{
    public static K<STMF, Unit> Transfer(Ref<long> source, Ref<long> dest, long amount) =>
        from f in STMOps.Read(source)
        from _ in STMOps.Write(source, f - amount)
        from t in STMOps.Read(dest)
        from __ in STMOps.Write(dest, t + amount)
        select Unit.Default;
}
