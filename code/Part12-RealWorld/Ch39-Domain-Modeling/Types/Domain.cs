namespace Ch39.Types;

// 강타입 도메인 — *검증을 통과해야만* 생성되는 값 (잘못된 상태를 표현 불가능하게).
// private 생성자 + 스마트 생성자(Validation 반환) 로 불변식을 타입 경계에 가둔다.
public sealed record Username
{
    public string Value { get; }
    private Username(string value) => Value = value;
    public static Validation<string, Username> Create(string raw) =>
        raw.Trim().Length >= 3
            ? Validation<string, Username>.Success(new Username(raw.Trim()))
            : Validation<string, Username>.Fail($"username '{raw}' 은 3자 이상이어야 함");
}

public sealed record Email
{
    public string Value { get; }
    private Email(string value) => Value = value;
    public static Validation<string, Email> Create(string raw) =>
        raw.Contains('@') && raw.Contains('.')
            ? Validation<string, Email>.Success(new Email(raw))
            : Validation<string, Email>.Fail($"email '{raw}' 형식이 올바르지 않음");
}

public sealed record Age
{
    public int Value { get; }
    private Age(int value) => Value = value;
    public static Validation<string, Age> Create(int raw) =>
        raw is >= 0 and <= 150
            ? Validation<string, Age>.Success(new Age(raw))
            : Validation<string, Age>.Fail($"age {raw} 은 0~150 범위여야 함");
}

// 검증된 도메인 객체 — 이 타입이 존재하면 세 필드가 모두 유효함이 보장된다.
public sealed record User(Username Name, Email Email, Age Age);

public static class Registration
{
    // 세 필드를 동시에 검증 — 모든 오류를 누적해서 보고 (applicative).
    public static Validation<string, User> Register(string name, string email, int age) =>
        Validation.Map3(
            Username.Create(name),
            Email.Create(email),
            Age.Create(age),
            (n, e, a) => new User(n, e, a));
}
