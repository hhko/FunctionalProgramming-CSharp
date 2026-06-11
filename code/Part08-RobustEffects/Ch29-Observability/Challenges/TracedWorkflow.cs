using Ch29.Types;

namespace Ch29.Challenges;

// 챌린지 정답 — 여러 단계를 추적하며 합성. 추적은 결과를 바꾸지 않고 트리만 남긴다.
public static class TracedWorkflow
{
    public static (int Result, List<string> Spans) Run()
    {
        var tracer = new Tracer();
        var result = tracer.Activity("checkout", () =>
        {
            var price = tracer.Activity("price", () => 1000);
            var tax = tracer.Activity("tax", () => price / 10);
            return price + tax;
        });
        return (result, tracer.Names());
    }
}
