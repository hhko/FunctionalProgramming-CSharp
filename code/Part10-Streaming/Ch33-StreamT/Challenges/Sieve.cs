using Ch33.Types;

namespace Ch33.Challenges;

// 챌린지 정답 — 무한 스트림으로 소수 거르기 (게으른 평가의 진수).
// From(2) 에서 시작해 각 소수의 배수를 거르며 무한히 흐르되, Take(n) 만큼만 실제 계산된다.
public static class Sieve
{
    public static List<int> Primes(int count)
    {
        StreamT<int> Sift(StreamT<int> s)
        {
            var c = s.Pull()!;                       // 첫 원소 = 소수
            var prime = c.Head;
            // 그 소수의 배수를 거른 나머지를 다시 체질 (지연).
            return StreamT<int>.Cons(() => prime, () => Sift(c.Tail.Filter(x => x % prime != 0)));
        }
        return Sift(Streams.From(2)).Take(count).ToList();
    }
}
