using Ch37.Types;

namespace Ch37.Challenges;

// 챌린지 ③ 정답 — 자원 셋(A, B, C)을 열어 종료 로그가 *LIFO* 인지 검사한다.
// A → B → C 로 열면 닫히는 순서는 C → B → A. 중간 자원 사용 중 예외가 나도 셋 다 닫힌다.
public static class NestedScope
{
    // A → B → C 로 열고 ReleaseAll — 종료 로그가 C, B, A 역순(LIFO)인지 본다.
    public static bool ThreeResourcesReleaseLifo()
    {
        var scope = new Scope();
        scope.Acquire("A", () => 1, _ => { });
        scope.Acquire("B", () => 2, _ => { });
        scope.Acquire("C", () => 3, _ => { });
        scope.ReleaseAll();
        return scope.Log.SequenceEqual(
            ["acquire A", "acquire B", "acquire C", "release C", "release B", "release A"]);
    }

    // 중간 자원(B) 사용 중 예외가 나도 이미 연 자원은 모두 LIFO 로 닫힌다.
    // try 안에서 셋을 연 뒤 예외를 던지고, finally 의 ReleaseAll 이 C → B → A 로 닫는다.
    public static bool ReleasesAllOnException()
    {
        var scope = new Scope();
        try
        {
            scope.Acquire("A", () => 1, _ => { });
            scope.Acquire("B", () => 2, _ => { });
            scope.Acquire("C", () => 3, _ => { });
            throw new InvalidOperationException("boom");
        }
        catch (InvalidOperationException) { /* 예외는 정상적으로 전파됩니다 */ }
        finally
        {
            scope.ReleaseAll();
        }
        return scope.Log.SequenceEqual(
            ["acquire A", "acquire B", "acquire C", "release C", "release B", "release A"]);
    }
}
