using Ch40.Challenges;
using Ch40.Functions;
using Ch40.Types;

namespace Ch40.Tests;

public static class AppTests
{
    static (AppRT, MemoryConsole, MemoryStore) Build()
    {
        var con = new MemoryConsole();
        var store = new MemoryStore();
        return (new AppRT(con, new FixedClock(1000), store), con, store);
    }

    // ① 워크플로가 노트 2개를 저장하고 개수를 돌려준다.
    public static bool SavesTwoNotes()
    {
        var (rt, _, store) = Build();
        var count = Eff.Run(NoteApp.Run<AppRT>(), rt);
        return count == 2 && store.Keys().SequenceEqual(["note1", "note2"]);
    }

    // ② 시계 능력이 저장 값에 반영된다 (FixedClock(1000) → @t1000).
    public static bool UsesClock()
    {
        var (rt, _, store) = Build();
        Eff.Run(NoteApp.SaveNote<AppRT>("k", "x"), rt);
        return store.Get("k") == "x @t1000";
    }

    // ③ 콘솔 출력이 결정적으로 기록된다.
    public static bool ConsoleOutput()
    {
        var (rt, con, _) = Build();
        Eff.Run(NoteApp.Run<AppRT>(), rt);
        return con.Output.SequenceEqual(["저장: note1", "저장: note2", "노트 2개: note1, note2"]);
    }
}
