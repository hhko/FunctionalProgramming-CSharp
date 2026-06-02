using Ch29.Types;

namespace Ch29.Challenges;

// 챌린지 정답 — 메시지 송수신 시나리오로 인과성 추적.
// p1 이 이벤트 → 메시지 전송, p2 가 수신(merge) → p2 의 후속 이벤트는 p1 의 이벤트 이후다.
public static class Causality
{
    public static (VectorClock P1Send, VectorClock P2After, VectorClock.Ordering Order) Scenario()
    {
        var p1 = new VectorClock().Increment("p1");   // p1 에서 이벤트 발생, 메시지 전송
        var p2 = new VectorClock().Increment("p2");   // p2 에서 독립 이벤트
        var p2Recv = p2.Merge(p1).Increment("p2");    // p2 가 p1 메시지 수신 후 후속 이벤트
        // p1 전송 시점은 p2 수신 후 시점보다 Before (인과 선행).
        return (p1, p2Recv, p1.Compare(p2Recv));
    }
}
