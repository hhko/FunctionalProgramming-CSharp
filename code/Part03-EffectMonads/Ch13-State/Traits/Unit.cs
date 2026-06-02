namespace Ch13.Traits;

// Unit — "값이 없음" 을 나타내는 한 점 타입. Put / Modify 처럼 *상태만 바꾸고 돌려줄 값이 없는*
// 계산의 결과 타입으로 쓴다. (void 는 타입 인자로 못 쓰므로 Unit 이 필요.)
public readonly record struct Unit
{
    public static readonly Unit Default = new();
    public override string ToString() => "()";
}
