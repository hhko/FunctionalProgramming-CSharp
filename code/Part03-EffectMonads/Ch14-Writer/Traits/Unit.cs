namespace Ch14.Traits;

// Unit — Tell 처럼 *출력만 남기고 돌려줄 값이 없는* 계산의 결과 타입.
public readonly record struct Unit
{
    public static readonly Unit Default = new();
    public override string ToString() => "()";
}
