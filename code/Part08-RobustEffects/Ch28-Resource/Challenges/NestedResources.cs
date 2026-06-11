using Ch28.Types;

namespace Ch28.Challenges;

// 챌린지 ② 정답 — 중첩 bracket. 안쪽 자원이 *먼저* 닫힌다 (LIFO), 예외에도 보장.
public static class NestedResources
{
    public static List<string> OpenTwo()
    {
        var log = new List<string>();
        Resource.Bracket(
            () => { log.Add("open outer"); return "outer"; },
            outer => Resource.Bracket(
                () => { log.Add("open inner"); return "inner"; },
                inner => { log.Add($"use {outer}+{inner}"); return inner.Length; },
                _ => log.Add("close inner")),
            _ => log.Add("close outer"));
        return log;
    }
}
