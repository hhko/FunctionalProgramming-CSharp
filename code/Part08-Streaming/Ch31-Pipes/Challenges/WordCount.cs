using Ch31.Types;

namespace Ch31.Challenges;

// 챌린지 정답 — 파이프라인으로 단어 수 세기 (생산 → 정규화/필터 → 집계).
// producer(줄) → Map(소문자 단어로) → Filter(빈 문자열 제거) → Consumer(개수).
public static class WordCount
{
    public static int Count(params string[] words) =>
        Producer.Of(words)
            .Through(Pipe.Map<string, string>(w => w.Trim().ToLowerInvariant())
                .Then(Pipe.Filter<string>(w => w.Length > 0)))
            .Run(Consumer.ToList<string>())
            .Count;
}
