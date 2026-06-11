namespace Ch37.Types;

// Source — `IEnumerable<int>` 기반의 작은 *effectful 스트림*. 원소를 흘려보내면서 *부수 효과*
// (여기서는 흘러간 원소를 trace 에 기록)를 남깁니다. (LanguageExt v5 SourceT 의 축소판.)
//
// 테스트 관점 — 스트림은 lazy 라 "언제 무엇이 흘렀는가" 가 흐릿합니다. effect 를 trace 로 모으면
// "생성 시퀀스 == 기대 골든" 으로 결정적으로 단언할 수 있습니다 (골든 테스트).
public static class Source
{
    // From — 0..count-1 을 흘리되, 각 원소를 흘릴 때 trace 에 기록(effect)합니다.
    // IEnumerable 의 lazy 평가라, 소비될 때 비로소 effect 가 일어납니다.
    public static IEnumerable<int> From(int count, List<int> trace)
    {
        for (var i = 0; i < count; i++)
        {
            trace.Add(i);              // effect — 이 원소가 흘렀음을 기록
            yield return i;
        }
    }

    // Map — 각 원소에 f 를 적용한 새 스트림 (모양 보존, 게으름 유지).
    public static IEnumerable<int> Map(IEnumerable<int> source, Func<int, int> f)
    {
        foreach (var x in source)
            yield return f(x);
    }

    // Filter — predicate 를 만족하는 원소만 통과 (게으름 유지).
    public static IEnumerable<int> Filter(IEnumerable<int> source, Func<int, bool> predicate)
    {
        foreach (var x in source)
            if (predicate(x))
                yield return x;
    }
}
