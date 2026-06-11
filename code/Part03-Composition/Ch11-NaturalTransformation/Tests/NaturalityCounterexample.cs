using Ch11.Types;

namespace Ch11.Tests;

// 본문 §11.5.1 가짜 자연 변환 반례 — 실행 가능한 버전.
//
// 진짜 ListToMaybe 는 값을 보지 않고 첫 원소를 꺼낸다.
// 가짜 BogusEvenHead 는 *값을 들여다본다* — 첫 원소가 짝수일 때만 꺼낸다.
// 값에 따라 분기하므로 map 으로 값을 바꾼 뒤와 바꾸기 전의 결과가 갈려
// 자연성 법칙 Transform(map(f, fa)) == map(f, Transform(fa)) 을 깬다.
public static class NaturalityCounterexample
{
    // 가짜 — 값을 들여다본다 (첫 원소가 짝수일 때만 꺼냄). int 전용 데모.
    public static MyMaybe<int> BogusEvenHead(MyList<int> xs) =>
        xs.IsEmpty            ? MyMaybe<int>.Nothing
        : xs.Head % 2 == 0    ? MyMaybe<int>.Of(xs.Head)
                              : MyMaybe<int>.Nothing;

    // 자연성 두 경로를 비교한다 (f = x => x + 1).
    //   lhs = 먼저 map, 그다음 가짜 Transform
    //   rhs = 먼저 가짜 Transform, 그다음 map
    // 반환: (lhs, rhs) — 가짜는 둘이 다르다.
    public static (MyMaybe<int> mapThenTransform, MyMaybe<int> transformThenMap) NaturalityPaths(MyList<int> xs)
    {
        Func<int, int> f = x => x + 1;
        var lhs = BogusEvenHead(NaturalityLaws.ListMap(xs, f));        // 먼저 map, 그다음 가짜
        var rhs = NaturalityLaws.MaybeMap(BogusEvenHead(xs), f);       // 먼저 가짜, 그다음 map
        return (lhs, rhs);
    }

    // 값을 들여다보는 가짜는 자연성을 깬다 (두 경로가 다름).
    public static bool BreaksNaturality(MyList<int> xs)
    {
        var (lhs, rhs) = NaturalityPaths(xs);
        return !lhs.Equals(rhs);
    }
}
