using Ch41.Types;

namespace Ch41.Challenges;

// 챌린지 정답 — 파이프라인 결과로 평균 계산 (동시 집계 합 / 유효 개수).
public static class Average
{
    public static double Of(IReadOnlyList<string> records)
    {
        var r = Pipeline.Run(records, maxRetries: 5, flakyUntilAttempt: 1);
        return r.Valid == 0 ? 0 : (double)r.Sum / r.Valid;
    }
}
