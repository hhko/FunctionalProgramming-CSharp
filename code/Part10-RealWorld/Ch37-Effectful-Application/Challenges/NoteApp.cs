using Ch37.Functions;
using Ch37.Types;

namespace Ch37.Challenges;

// 효과 기반 앱 — 노트를 시각과 함께 저장하고 목록을 출력한다.
// 세 능력(콘솔/시계/저장소)을 *조합* 으로 요구하지만, 효과 코드는 런타임 구현을 모른다.
public static class NoteApp
{
    public static K<ReaderTF<RT>, Unit> SaveNote<RT>(string key, string text)
        where RT : Has<RT, IConsole>, Has<RT, IClock>, Has<RT, IStore> =>
        from t in Eff.Now<RT>()
        from _1 in Eff.Save<RT>(key, $"{text} @t{t}")
        from _2 in Eff.Print<RT>($"저장: {key}")
        select Unit.Default;

    public static K<ReaderTF<RT>, int> ListNotes<RT>()
        where RT : Has<RT, IStore>, Has<RT, IConsole> =>
        from keys in Eff.Keys<RT>()
        from _ in Eff.Print<RT>($"노트 {keys.Count}개: {string.Join(", ", keys)}")
        select keys.Count;

    // 전체 워크플로 — 두 노트 저장 후 목록.
    public static K<ReaderTF<RT>, int> Run<RT>()
        where RT : Has<RT, IConsole>, Has<RT, IClock>, Has<RT, IStore> =>
        from _1 in SaveNote<RT>("note1", "첫 노트")
        from _2 in SaveNote<RT>("note2", "둘째 노트")
        from count in ListNotes<RT>()
        select count;
}
