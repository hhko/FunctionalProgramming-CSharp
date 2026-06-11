namespace Ch29.Types;

// Span — 추적의 한 구간 (이름 + 자식 구간들). 분산 추적의 트리 노드.
public sealed class Span(string name)
{
    public string Name { get; } = name;
    public List<Span> Children { get; } = [];
}

// Tracer — 현재 구간을 스택으로 추적하며 span 트리를 만든다.
// System.Diagnostics.Activity / OpenTelemetry 의 발상을 외부 의존 없이 모델링한 것.
public sealed class Tracer
{
    public Span Root { get; } = new("trace");
    readonly Stack<Span> stack;

    public Tracer() { stack = new Stack<Span>(); stack.Push(Root); }

    // activity — 효과를 *감싸* span 으로 추적한다. 결과는 바꾸지 않는다 (횡단 관심사).
    public A Activity<A>(string name, Func<A> body)
    {
        var span = new Span(name);
        stack.Peek().Children.Add(span);   // 현재 구간의 자식으로
        stack.Push(span);
        try
        {
            return body();                 // 효과 실행 (결과 그대로)
        }
        finally
        {
            stack.Pop();                   // 구간 종료 (예외에도)
        }
    }

    // 트리를 들여쓰기로 출력.
    public string Render()
    {
        var sb = new System.Text.StringBuilder();
        void Walk(Span s, int depth)
        {
            sb.AppendLine($"{new string(' ', depth * 2)}- {s.Name}");
            foreach (var c in s.Children) Walk(c, depth + 1);
        }
        foreach (var c in Root.Children) Walk(c, 0);
        return sb.ToString().TrimEnd();
    }

    // 트리의 이름들을 깊이우선으로 나열 (검증용).
    public List<string> Names()
    {
        var names = new List<string>();
        void Walk(Span s) { foreach (var c in s.Children) { names.Add(c.Name); Walk(c); } }
        Walk(Root);
        return names;
    }
}
