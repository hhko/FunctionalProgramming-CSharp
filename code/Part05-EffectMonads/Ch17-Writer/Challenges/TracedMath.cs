using Ch17.Functions;
using Ch17.Traits;
using Ch17.Types;

namespace Ch17.Challenges;

// 챌린지 ① 정답 — 계산 과정을 *로그로 추적* 하는 Writer.
//
// 각 연산이 결과와 함께 한 줄의 로그를 Tell 한다. LINQ 로 이으면 로그가 Monoid 로 누적된다.
// 명령형이라면 logger 를 전역/인자로 들고 다녔을 자리다.
public static class TracedMath
{
    static K<WriterF<Log>, int> Step(int result, string message) =>
        from _ in Writable.tell<WriterF<Log>, Log>(Log.Of(message))
        select result;

    public static K<WriterF<Log>, int> Add(int x, int y) =>
        Step(x + y, $"{x} + {y} = {x + y}");

    public static K<WriterF<Log>, int> Mul(int x, int y) =>
        Step(x * y, $"{x} * {y} = {x * y}");

    // (a + b) * c — 각 단계가 로그를 누적.
    public static K<WriterF<Log>, int> Compute(int a, int b, int c) =>
        from sum  in Add(a, b)
        from prod in Mul(sum, c)
        select prod;
}
