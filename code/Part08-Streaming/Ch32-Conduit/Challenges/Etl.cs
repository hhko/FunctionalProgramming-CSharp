using Ch32.Types;

namespace Ch32.Challenges;

// 챌린지 정답 — 로그 파일 ETL: 파싱(실패 제외) → 임계값 필터 → 합산. 자원은 안전하게 개폐.
public static class Etl
{
    public static (int Total, List<string> Events) SumAboveThreshold(
        IReadOnlyList<string> lines, int threshold)
    {
        var events = new List<string>();
        var conduit =
            Conduit.Choose<string, int>(s => (int.TryParse(s, out var v), v))   // 파싱 성공만
                   .Then(Conduit.Filter<int>(x => x >= threshold));             // 임계값 이상
        var total = Pipeline.RunOverFile(lines, conduit, xs => xs.Sum(), events);
        return (total, events);
    }
}
