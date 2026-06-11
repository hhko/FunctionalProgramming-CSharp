using Ch09.Functions;
using Ch09.Traits;
using Ch09.Types;

namespace Ch09.Tests;

// 본문 §9.7.1 — 안쪽 효과 F 를 MyValidation 으로 바꾸면 같은 Traverse 가 누적한다.
//
// MyMaybe 로 [3, 5, 7] 을 traverse 하면 첫 홀수에서 단락해 Nothing 한 건뿐이지만,
// MyValidation 으로 바꾸면 세 오류가 모두 모인다. Traverse 코드는 한 글자도 바뀌지 않는다.
public static class ValidationTraverse
{
    // 짝수면 Valid, 홀수면 Invalid(오류 1건). MyMaybe 의 evenCheck 와 같은 판정, 효과만 MyValidation.
    public static Func<int, K<MyValidationF<string>, int>> EvenV =>
        n => n % 2 == 0
            ? MyValidationF<string>.Pure(n)
            : new MyValidation<string, int>.Invalid([$"{n} 은(는) 홀수"]);

    // [3, 5, 7] 을 traverse — 셋 다 홀수라 오류 3 건 누적.
    public static MyValidation<string, K<MyListF, int>> TraverseAllOdd() =>
        (MyValidation<string, K<MyListF, int>>)
            Traversable.traverse<MyListF, MyValidationF<string>, int, int>(
                EvenV, new MyList<int>([3, 5, 7]));

    // 누적된 오류 건수 (3 건 기대). 단락하는 MyMaybe 였다면 정보는 1 건뿐.
    public static int AccumulatedErrorCount() =>
        TraverseAllOdd() switch
        {
            MyValidation<string, K<MyListF, int>>.Invalid e => e.Errors.Count,
            _ => 0
        };
}
