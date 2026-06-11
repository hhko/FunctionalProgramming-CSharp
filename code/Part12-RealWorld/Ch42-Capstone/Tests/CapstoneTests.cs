using Ch42.Challenges;
using Ch42.Functions;
using Ch42.Types;

namespace Ch42.Tests;

public static class CapstoneTests
{
    static readonly (string, decimal)[] Requests =
        [("A", 100m), ("", 50m), ("B", 200m), ("C", -5m)];

    static (AppRT rt, MemoryConsole con, MemoryStore store) Build()
    {
        var con = new MemoryConsole();
        var store = new MemoryStore();
        return (new AppRT(con, store), con, store);
    }

    // ① 유효 주문 2건만 승인.
    public static bool AcceptsValidOnly()
    {
        var (rt, _, store) = Build();
        var accepted = Eff.Run(OrderService.ProcessAll<AppRT>(Requests), rt);
        return accepted == 2 && store.Count() == 2;
    }

    // ② 저장소 매출 합계 = 100 + 200.
    public static bool TotalsRevenue()
    {
        var (rt, _, store) = Build();
        Eff.Run(OrderService.ProcessAll<AppRT>(Requests), rt);
        return store.Total() == 300m;
    }

    // ③ 콘솔에 승인 2 + 거부 2 = 4줄.
    public static bool LogsAllOutcomes()
    {
        var (rt, con, _) = Build();
        Eff.Run(OrderService.ProcessAll<AppRT>(Requests), rt);
        return con.Output.Count == 4
            && con.Output.Count(l => l.StartsWith("승인")) == 2
            && con.Output.Count(l => l.StartsWith("거부")) == 2;
    }
}
