using Ch23.Functions;
using Ch23.Traits;
using Ch23.Types;

namespace Ch23.Challenges;

// 챌린지 ② 정답 — 런타임에 *두 번째 능력* (IClock) 을 추가하고 조합한다.
public interface IClock
{
    long NowTicks();
}

public sealed class FixedClock(long ticks) : IClock
{
    public long NowTicks() => ticks;
}

// 콘솔 + 시계 두 능력을 가진 런타임. static 멤버 이름 충돌(Get)을 *명시적 구현* 으로 푼다.
public sealed record FullRT(IConsole Console, IClock Clock) : Has<FullRT, IConsole>, Has<FullRT, IClock>
{
    static IConsole Has<FullRT, IConsole>.Get(FullRT rt) => rt.Console;
    static IClock Has<FullRT, IClock>.Get(FullRT rt) => rt.Clock;
}

public static class ClockEff
{
    // 시계 능력을 요구하는 효과.
    public static K<ReaderTF<RT, IOF>, long> Now<RT>() where RT : Has<RT, IClock> =>
        from clock in Readable.asks<ReaderTF<RT, IOF>, RT, IClock>(rt => RT.Get(rt))
        from t in IOM.liftIO<ReaderTF<RT, IOF>, long>(new IO<long>(clock.NowTicks))
        select t;

    // 콘솔 + 시계 *둘 다* 쓰는 효과 (FullRT 가 두 능력을 모두 가짐).
    public static long Stamp(FullRT rt)
    {
        K<ReaderTF<FullRT, IOF>, long> prog =
            from now in Now<FullRT>()
            from _ in Eff.WriteLine<FullRT>($"기록 시각 = {now}")
            select now;
        return Eff.Run(prog, rt);
    }
}
