using Ch11.Functions;
using Ch11.Tests;
using Ch11.Types;

namespace Ch11.Challenges;

// 본문 §11.7 직접 해보기 — 챌린지 정답 코드.
public static class NaturalChallenges
{
    // 챌린지 1 — Transform 은 값을 건드리지 않고 컨테이너만 옮긴다.
    public static (MyMaybe<int> head, MyList<int> singleton) ContainerOnly()
    {
        var head      = (MyMaybe<int>)ListToMaybe.Transform<int>(MyList<int>.Of(1, 2, 3));  // Just(1)
        var singleton = (MyList<int>)MaybeToList.Transform<int>(MyMaybe<int>.Of(7));         // [7]
        return (head, singleton);
    }

    // 챌린지 2 — 진짜 ListToMaybe 는 자연성 법칙을 지킨다 (두 경로 동일).
    public static bool RealKeepsNaturality() =>
        NaturalityLaws.ListToMaybeNaturality<int, int>(MyList<int>.Of(1, 2, 3), x => x + 1);

    // 챌린지 3 — 값을 들여다보는 가짜는 자연성을 깬다 (두 경로 상이).
    public static bool BogusBreaksNaturality() =>
        NaturalityCounterexample.BreaksNaturality(MyList<int>.Of(1, 2, 3));
}
