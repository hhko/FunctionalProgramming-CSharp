using Ch22.Functions;
using Ch22.Traits;
using Ch22.Types;

namespace Ch22.Challenges;

// 챌린지 ② 정답 — Catch 로 폴백 체인 (1차 실패 → 2차 시도 → 최종 기본값).
// 재시도/폴백의 토대: 실패를 값으로 받아 다음 효과로 넘긴다.
public static class WithFallback
{
    public static Fin<int> ParseWithFallback(string primary, string secondary, int defaultValue)
    {
        K<EffF, int> attempt =
            Fallibles.@catch<EffF, int>(
                Eff<int>.Effect(() => int.Parse(primary)),
                _ => Fallibles.@catch<EffF, int>(
                    Eff<int>.Effect(() => int.Parse(secondary)),
                    _ => EffF.Pure(defaultValue)));

        return attempt.As().Run();
    }
}
