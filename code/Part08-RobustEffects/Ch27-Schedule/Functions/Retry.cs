using Ch27.Types;

namespace Ch27.Functions;

// Schedule 을 효과에 *얹는* 함수들. 효과는 Func<Fin<A>> 로 모델링.
// (실무에선 각 Duration 만큼 Thread.Sleep/Task.Delay 하지만, 학습 데모는 간격을 기록만 한다.)
public static class Retry
{
    // 실패하면 schedule 이 정한 횟수만큼 재시도. 성공하면 멈춘다.
    public static (Fin<A> Result, int Attempts, List<double> Delays) RetryFin<A>(
        Func<Fin<A>> action, Schedule schedule)
    {
        var delays = new List<double>();
        var result = action();
        var attempts = 1;
        if (result is Fin<A>.Succ) return (result, attempts, delays);

        foreach (var d in schedule.Durations)
        {
            delays.Add(d.TotalMs);          // 실무: Thread.Sleep(d.Span)
            result = action();
            attempts++;
            if (result is Fin<A>.Succ) break;
        }
        return (result, attempts, delays);
    }

    // 성공하는 한 schedule 횟수만큼 반복하며 결과를 모은다.
    public static List<A> RepeatCollect<A>(Func<A> action, Schedule schedule)
    {
        var results = new List<A> { action() };   // 최초 1회
        foreach (var _ in schedule.Durations)
            results.Add(action());
        return results;
    }
}
