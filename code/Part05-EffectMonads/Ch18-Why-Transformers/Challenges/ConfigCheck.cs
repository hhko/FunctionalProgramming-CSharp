using Ch18.Functions;
using Ch18.Traits;
using Ch18.Types;

namespace Ch18.Challenges;

// 챌린지 ② 정답 — 손으로 짠 ReaderOption 을 *실제로 사용* 해 설정을 검증한다.
// 두 키를 차례로 조회하다 하나라도 없으면 None 으로 단락 (env 의존 + 실패가 한 스택에).
public static class ConfigCheck
{
    static K<ROF<Dictionary<string, int>>, int> Lookup(string key) =>
        new ReaderOption<Dictionary<string, int>, int>(
            e => e.TryGetValue(key, out var v)
                ? new Option<int>.Some(v)
                : Option<int>.None.Instance);

    // port + timeout 둘 다 있어야 Some, 아니면 None.
    public static Option<int> SumConfig(Dictionary<string, int> cfg)
    {
        var prog =
            from port in Lookup("port")
            from timeout in Lookup("timeout")
            select port + timeout;
        return prog.As().Run(cfg);
    }

    // lift 데모 — 환경만 읽는 Reader, 상수 Option, 실패 가능 Lookup 을 한 사슬에 섞는다.
    public static Option<int> WithBonus(Dictionary<string, int> cfg)
    {
        var size  = ReaderOption<Dictionary<string, int>, int>
                        .LiftReader(new Reader<Dictionary<string, int>, int>(e => e.Count));
        var bonus = ReaderOption<Dictionary<string, int>, int>
                        .LiftOption(new Option<int>.Some(100));
        var prog =
            from n in size           // LiftReader 로 올린 환경 의존
            from a in Lookup("a")    // 환경 의존 + 실패 가능
            from b in bonus          // LiftOption 으로 올린 상수
            select n + a + b;
        return prog.As().Run(cfg);
    }
}
