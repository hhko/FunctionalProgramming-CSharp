using Ch31.Challenges;
using Ch31.Functions;
using Ch31.Types;

namespace Ch31.Tests;

public static class STMLaws
{
    // ① 단일 트랜잭션 이체.
    public static bool SingleTransferHolds()
    {
        var a = new Ref<long>(100);
        var b = new Ref<long>(0);
        STMOps.Atomically(Bank.Transfer(a, b, 30));
        return a.Current == 70 && b.Current == 30;
    }

    // ② 동시 이체 — 총액 보존 (돈이 생기거나 사라지지 않는다).
    public static bool ConcurrentConservationHolds()
    {
        var a = new Ref<long>(1000);
        var b = new Ref<long>(1000);
        const long total = 2000;
        Parallel.For(0, 4000, i =>
        {
            long amt = (i % 7) + 1;
            STMOps.Atomically(i % 2 == 0 ? Bank.Transfer(a, b, amt) : Bank.Transfer(b, a, amt));
        });
        return a.Current + b.Current == total;
    }
}
