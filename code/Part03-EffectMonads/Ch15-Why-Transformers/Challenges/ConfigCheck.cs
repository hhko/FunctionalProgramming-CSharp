using Ch15.Functions;
using Ch15.Traits;
using Ch15.Types;

namespace Ch15.Challenges;

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
}
