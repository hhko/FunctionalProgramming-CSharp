namespace Ch08.Domain;

// 본문 §8.9.1 또 다른 도메인 — 서버 설정. 회원가입 폼과 같은 누적 패턴을 적용.
public readonly record struct Host(string Value);
public readonly record struct Port(int Value);
public readonly record struct TimeoutSeconds(int Value);

public sealed record ServerConfig(Host Host, Port Port, TimeoutSeconds Timeout)
{
    public override string ToString() => $"ServerConfig({Host.Value}:{Port.Value}, {Timeout.Value}s)";
}
