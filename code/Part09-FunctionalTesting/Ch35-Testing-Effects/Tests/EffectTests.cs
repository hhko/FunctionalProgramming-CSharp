using Ch35.Challenges;
using Ch35.Functions;
using Ch35.Types;

namespace Ch35.Tests;

// 효과 코드의 *결정적* 테스트 — MemoryConsole 더블을 런타임에 주입해 실제 콘솔 없이 검증.
// (xUnit 으로 옮기면 각 메서드를 [Fact] 로 감싸고 con.Output.ShouldBe(...) 만 붙이면 된다.)
public static class EffectTests
{
    public static bool GreetingOutputsCorrectly()
    {
        var con = new MemoryConsole(["철수"]);
        var name = Eff.Run(Greeter.Ask<AppRT>(), new AppRT(con));
        return name == "철수"
            && con.Output.SequenceEqual(["이름이 무엇인가요?", "안녕하세요, 철수!"]);
    }

    public static bool DifferentInputDifferentOutput()
    {
        var con = new MemoryConsole(["영희"]);
        Eff.Run(Greeter.Ask<AppRT>(), new AppRT(con));
        return con.Output[^1] == "안녕하세요, 영희!";
    }

    // 부수 작용이 전혀 없다 — 같은 입력은 항상 같은 출력 (결정적).
    public static bool DeterministicHolds()
    {
        string Run()
        {
            var con = new MemoryConsole(["민수"]);
            Eff.Run(Greeter.Ask<AppRT>(), new AppRT(con));
            return string.Join("|", con.Output);
        }
        return Run() == Run();
    }
}
