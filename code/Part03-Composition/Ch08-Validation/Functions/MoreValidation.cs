using Ch08.Domain;
using Ch08.Traits;
using Ch08.Types;

namespace Ch08.Functions;

// 본문 §8.4.1 칸 하나에 규칙 여럿 — 틀린 규칙을 모두 누적.
//
// 한 필드(비밀번호)에 길이·숫자·대문자 세 규칙을 건다. 셋 다 통과해야 Valid,
// 틀린 규칙은 모두 누적된다. 회원가입 폼의 칸-사이 누적과 같은 Apply 가
// 한 칸 안에서도 그대로 쓰인다.
public static class PasswordRules
{
    // 규칙 하나 — 통과면 Valid(""), 실패면 Invalid(오류 1건). 운반 값은 쓰지 않는다.
    static K<MyValidationF<DomainError>, string> Rule(bool ok, string msg) =>
        ok ? MyValidationF<DomainError>.Pure("")
           : new MyValidation<DomainError, string>.Invalid([new DomainError("password", msg)]);

    public static K<MyValidationF<DomainError>, Password> Strong(string raw)
    {
        // 세 규칙을 Apply 로 누적 — 통과 시 raw 로 Password 조립.
        Func<string, string, string, Password> keep = (_, _, _) => new Password(raw);
        return MyValidationF<DomainError>.Pure(Curry.Of(keep))
            .Apply(Rule(raw.Length >= 8, "8 자 이상"))
            .Apply(Rule(raw.Any(char.IsDigit), "숫자 1 자 이상"))
            .Apply(Rule(raw.Any(char.IsUpper), "대문자 1 자 이상"));
    }
}

// 본문 §8.9.1 또 다른 도메인 — 서버 설정 검증.
//
// 회원가입 폼과 *같은* Pure → Apply 누적 패턴을 다른 도메인에 그대로 적용한다.
// 세 칸(host / port / timeout)이 서로 독립이라 오류가 누적된다.
public static class ConfigValidator
{
    public static K<MyValidationF<DomainError>, Host> ValidateHost(string raw) =>
        !string.IsNullOrWhiteSpace(raw) && raw.Contains('.')
            ? MyValidationF<DomainError>.Pure(new Host(raw))
            : new MyValidation<DomainError, Host>.Invalid([new DomainError("host", "비어 있지 않고 '.' 포함")]);

    public static K<MyValidationF<DomainError>, Port> ValidatePort(int raw) =>
        raw is >= 1 and <= 65535
            ? MyValidationF<DomainError>.Pure(new Port(raw))
            : new MyValidation<DomainError, Port>.Invalid([new DomainError("port", "1-65535 범위")]);

    public static K<MyValidationF<DomainError>, TimeoutSeconds> ValidateTimeout(int raw) =>
        raw is >= 1 and <= 300
            ? MyValidationF<DomainError>.Pure(new TimeoutSeconds(raw))
            : new MyValidation<DomainError, TimeoutSeconds>.Invalid([new DomainError("timeout", "1-300 초")]);

    // Pure → 3 × Apply (fluent). 세 칸 독립이라 오류가 누적된다.
    public static K<MyValidationF<DomainError>, ServerConfig> Parse(string host, int port, int timeout)
    {
        Func<Host, Port, TimeoutSeconds, ServerConfig> ctor = (h, p, t) => new ServerConfig(h, p, t);
        return MyValidationF<DomainError>.Pure(Curry.Of(ctor))
            .Apply(ValidateHost(host))
            .Apply(ValidatePort(port))
            .Apply(ValidateTimeout(timeout));
    }
}
