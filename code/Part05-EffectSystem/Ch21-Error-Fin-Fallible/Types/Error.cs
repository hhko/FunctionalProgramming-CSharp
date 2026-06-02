namespace Ch21.Types;

// Error — *구조화된 오류 값*. 예외를 던지는 대신 이 값을 흘려보낸다.
// (LanguageExt v5 Error 의 축소판 — 메시지 + 선택적 예외.)
public sealed record Error(string Message, Exception? Exception = null)
{
    public static Error New(string message) => new(message);
    public static Error New(Exception ex) => new(ex.Message, ex);

    public override string ToString() => Message;
}
