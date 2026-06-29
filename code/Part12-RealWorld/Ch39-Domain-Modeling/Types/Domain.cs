namespace Ch39.Types;

// ─────────────────────────────────────────────────────────────────────────
// 계층 도메인 — 검증된 값 객체가 또 검증된 값 객체를 품는다.
//
//   잎 값 객체   (Username · Email · PhoneNumber · Street · City · PostalCode)
//        └ 복합 값 객체 (ContactInfo · Address) — 자체 Pure→Apply 체인으로 조립
//                └ 집합체   (Customer) — Pure→Apply 체인으로 조립
//
// 모든 계층이 같은 어법을 쓴다. private 생성자로 직접 생성을 막고, Create 만이
// 검증을 통과한 경우에만 값을 내준다. 복합 값 객체와 집합체는 자기 하위 Create 들을
// Pure(커리한 생성자) 에서 시작하는 Apply 체인으로 결합한다 (3부 Ch08 의 그 형태).
// 하위에서 누적된 오류가 상위 Apply 체인으로 그대로 합류한다.
// ─────────────────────────────────────────────────────────────────────────

// ── 잎 값 객체 ─────────────────────────────────────────────────────────────
// 원시 string 을 검증을 통과해야만 존재하는 도메인 타입으로 끌어올린다.

public sealed record Username
{
    public string Value { get; }
    private Username(string value) => Value = value;        // 옆문 잠금
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

public sealed record PhoneNumber
{
    public string Value { get; }
    private PhoneNumber(string value) => Value = value;
    public static Validation<string, PhoneNumber> Create(string raw) =>
        raw.All(c => char.IsDigit(c) || c is '-' or '+') && raw.Count(char.IsDigit) >= 9
            ? Validation<string, PhoneNumber>.Success(new PhoneNumber(raw))
            : Validation<string, PhoneNumber>.Fail($"phone '{raw}' 은 숫자 9자 이상이어야 함");
}

public sealed record Street
{
    public string Value { get; }
    private Street(string value) => Value = value;
    public static Validation<string, Street> Create(string raw) =>
        raw.Trim().Length > 0
            ? Validation<string, Street>.Success(new Street(raw.Trim()))
            : Validation<string, Street>.Fail("street 는 비어 있을 수 없음");
}

public sealed record City
{
    public string Value { get; }
    private City(string value) => Value = value;
    public static Validation<string, City> Create(string raw) =>
        raw.Trim().Length > 0
            ? Validation<string, City>.Success(new City(raw.Trim()))
            : Validation<string, City>.Fail("city 는 비어 있을 수 없음");
}

public sealed record PostalCode
{
    public string Value { get; }
    private PostalCode(string value) => Value = value;
    public static Validation<string, PostalCode> Create(string raw) =>
        raw.Length == 5 && raw.All(char.IsDigit)
            ? Validation<string, PostalCode>.Success(new PostalCode(raw))
            : Validation<string, PostalCode>.Fail($"postal '{raw}' 은 숫자 5자리여야 함");
}

// ── 복합 값 객체 ───────────────────────────────────────────────────────────
// 관련 잎 값 객체를 묶은 한 계층. 자체 Pure→Apply 체인으로 자기 잎들을 검증·조립한다.
// 이 계층 안에서 잎들의 오류가 먼저 누적된다.

public sealed record ContactInfo
{
    public Email Email { get; }
    public PhoneNumber Phone { get; }
    private ContactInfo(Email email, PhoneNumber phone) => (Email, Phone) = (email, phone);

    // 두 잎 Create 를 Lift2 로 결합 — 둘 다 틀리면 이 계층에서 오류 2개가 먼저 모인다.
    // 5장의 Lift2 가 (Curry → Pure → Apply → Apply) 를 캡슐화하므로 평범한 2인자 생성자만 넘긴다.
    public static Validation<string, ContactInfo> Create(string email, string phone) =>
        Validation.Lift2((Email e, PhoneNumber p) => new ContactInfo(e, p),
            Email.Create(email),
            PhoneNumber.Create(phone));
}

public sealed record Address
{
    public Street Street { get; }
    public City City { get; }
    public PostalCode Postal { get; }
    private Address(Street street, City city, PostalCode postal) =>
        (Street, City, Postal) = (street, city, postal);

    // 세 잎 Create 를 Lift3 로 결합 — 이 계층에서 최대 오류 3개가 먼저 모인다.
    public static Validation<string, Address> Create(string street, string city, string postal) =>
        Validation.Lift3((Street s, City c, PostalCode p) => new Address(s, c, p),
            Street.Create(street),
            City.Create(city),
            PostalCode.Create(postal));
}

// ── 집합체 ─────────────────────────────────────────────────────────────────
// 잎 하나(Username) 와 복합 값 객체 둘(ContactInfo · Address) 을 묶은 최상위.
// 이 타입이 존재하면 모든 계층의 모든 불변식이 만족됨이 보장된다.

public sealed record Customer
{
    public Username Name { get; }
    public ContactInfo Contact { get; }
    public Address Address { get; }
    private Customer(Username name, ContactInfo contact, Address address) =>     // 옆문 잠금 — 다른 계층과 같은 어법
        (Name, Contact, Address) = (name, contact, address);

    // 집합체도 같은 Lift3 — 잎 하나와 복합 값 객체 둘을 한 번에 결합한다.
    // 하위 복합 값 객체 안에서 누적된 오류가 이 Lift3 의 Apply 체인으로 그대로 합류한다.
    public static Validation<string, Customer> Create(
        string name, string email, string phone, string street, string city, string postal) =>
        Validation.Lift3((Username n, ContactInfo c, Address a) => new Customer(n, c, a),
            Username.Create(name),                  // 잎 1개
            ContactInfo.Create(email, phone),       // 복합 (하위 오류 누적이 합류)
            Address.Create(street, city, postal));  // 복합 (하위 오류 누적이 합류)
}
