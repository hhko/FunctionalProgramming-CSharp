using Ch36.Functions;
using Ch36.Types;

namespace Ch36.Challenges;

// 테스트 대상 효과 — 이름을 묻고, 읽고, 인사한다. RT 가 콘솔 능력을 가지면 동작.
// 같은 코드가 LiveConsole(실제) 또는 MemoryConsole(테스트) 어느 런타임에서도 실행된다.
public static class Greeter
{
    public static K<ReaderTF<RT>, string> Ask<RT>() where RT : Has<RT, IConsole> =>
        from _1 in Eff.WriteLine<RT>("이름이 무엇인가요?")
        from name in Eff.ReadLine<RT>()
        from _2 in Eff.WriteLine<RT>($"안녕하세요, {name}!")
        select name;
}
