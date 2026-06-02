using Ch13.Functions;
using Ch13.Traits;
using Ch13.Types;

namespace Ch13.Challenges;

// 챌린지 ① 정답 — 상태로 *고유 ID 생성기*. 가변 카운터 필드 없이 State 로 푼다.
//
// fresh 는 현재 카운터를 값으로 돌려주고 상태를 1 증가시킨다.
// 여러 fresh 를 LINQ 로 이으면 카운터가 *자동으로 실려* 매번 다른 ID 가 나온다.
public static class FreshId
{
    // 현재 값을 반환하고 카운터를 +1.
    public static K<StateF<int>, int> Fresh =>
        from n in Stateful.get<StateF<int>, int>()
        from _ in Stateful.put<StateF<int>, int>(n + 1)
        select n;

    // 이름 목록에 ID 를 매긴다 (상태 = 다음 ID).
    public static K<StateF<int>, List<(int Id, string Name)>> Label(IEnumerable<string> names)
    {
        K<StateF<int>, List<(int, string)>> acc =
            StateF<int>.Pure(new List<(int, string)>());

        foreach (var name in names)
        {
            var captured = name;
            acc = from list in acc
                  from id in Fresh
                  select Append(list, (id, captured));
        }
        return acc;
    }

    static List<(int, string)> Append(List<(int, string)> xs, (int, string) x)
    {
        xs.Add(x);
        return xs;
    }
}
