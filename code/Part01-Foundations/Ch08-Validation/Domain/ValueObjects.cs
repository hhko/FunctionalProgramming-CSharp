namespace Ch08.Domain;

// 회원가입 폼의 4 개 값 객체. *생성자가 검증을 강제* 하지 않는다 — 검증은 *외부 함수*
// 가 담당하고, 검증 통과한 경우에만 record 생성 (검증자 함수가 막는다).
public readonly record struct Email(string Value);
public readonly record struct Password(string Value);
public readonly record struct Age(int Value);

public enum Tier { Free, Pro, Enterprise }

public sealed record User(Email Email, Password Password, Age Age, Tier Tier);

// 도메인 에러 — 어느 필드의 어떤 문제인지.
public sealed record DomainError(string Field, string Message)
{
    public override string ToString() => $"{Field}: {Message}";
}
