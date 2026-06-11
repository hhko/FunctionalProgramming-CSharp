using Ch29.Types;

namespace Ch29.Tests;

public static class TracingLaws
{
    // ① 추적은 결과를 바꾸지 않는다 (횡단 관심사).
    public static bool ResultUnchangedHolds()
    {
        var tracer = new Tracer();
        var traced = tracer.Activity("op", () => 21 * 2);
        return traced == 42;
    }

    // ② 중첩 호출이 부모-자식 트리를 만든다.
    public static bool NestingHolds()
    {
        var tracer = new Tracer();
        tracer.Activity("parent", () =>
            tracer.Activity("child", () => 0));
        return tracer.Names().SequenceEqual(["parent", "child"]);
    }

    // ③ 예외가 나도 구간이 올바르게 닫혀 다음 추적에 영향 없음.
    public static bool ClosesOnExceptionHolds()
    {
        var tracer = new Tracer();
        try { tracer.Activity<int>("boom", () => throw new InvalidOperationException()); }
        catch (InvalidOperationException) { }
        tracer.Activity("after", () => 0);
        // boom 과 after 둘 다 루트의 자식 (boom 이 스택을 오염시키지 않았다).
        return tracer.Names().SequenceEqual(["boom", "after"]);
    }
}
