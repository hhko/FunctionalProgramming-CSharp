# 39장. 도메인 모델링과 검증 — 검증된 값이 또 검증된 값을 품는다 (계층 강타입 도메인 + 계층-교차 누적)

> **이 장의 목표** — 이 장을 마치면 검증된 값 객체가 또 검증된 값 객체를 품는 계층 도메인을 설계하고, 3부에서 만든 `Validation` 의 applicative 누적이 한 계층이 아니라 모든 계층에서 동시에 동작하는 end-to-end 파이프라인을 작성할 수 있습니다. 잎 값 객체 (`Username` · `Email` · `PhoneNumber` · `Street` · `City` · `PostalCode`) 를 스마트 생성자로 만들고, 관련 잎을 묶은 복합 값 객체 (`ContactInfo` · `Address`) 가 자기 잎들을 자체 `Apply` 체인으로 검증·조립하며, 집합체 `Customer` 가 잎 하나와 복합 값 객체 둘을 다시 `Apply` 체인으로 조립합니다. 핵심은 계층을 가로지른 누적입니다. 서로 다른 계층에 박힌 잘못된 `username` · `email` · `postal` 을 넣으면, 하위 복합 값 객체 안에서 먼저 누적된 오류가 상위 `Apply` 체인으로 그대로 합류해 한 번에 모입니다. 곧 `Apply` 가 한 계층이 아니라 모든 계층에서 합성·누적하며, 1부에서 익힌 함수형의 본질 (모든 값을 합성 가능한 Elevated World 로 끌어올림) 이 도메인 계층 전체에 적용되는 자리입니다. 3부 Ch08 의 평평한 다인자 검증을 계승하되, 이 장은 그 검증을 계층으로 끌어올립니다.

> **이 장의 핵심 어휘**
>
> - **잘못된 상태를 표현 불가능하게 (make illegal states unrepresentable)**: 검증을 통과하지 못한 값이 아예 타입으로 존재할 수 없게 만드는 설계 원칙
> - **강타입 도메인 (strongly-typed domain)**: 원시 `string` 대신 의미가 박힌 도메인 타입 (`Username` · `Email` · `PhoneNumber`) 으로 값을 표현하는 모델링
> - **잎 값 객체 (leaf value object)**: 원시값 하나를 검증해 담은 가장 작은 도메인 타입 (`Username` · `Email` · `PhoneNumber` · `Street` · `City` · `PostalCode`)
> - **복합 값 객체 (composite value object)**: 관련 잎 값 객체를 묶은 한 계층의 값 객체. 자기 잎들을 자체 `Pure`→`Apply` 체인으로 검증·조립합니다 (`ContactInfo` · `Address`)
> - **집합체 (aggregate)**: 잎과 복합 값 객체를 묶은 최상위 도메인 객체 (`Customer`)
> - **계층-교차 누적 (cross-level accumulation)**: 서로 다른 계층에 박힌 오류가, 하위 값 객체 안에서 먼저 누적된 뒤 상위 `Apply` 체인으로 합류해 한 목록에 모이는 것
> - **스마트 생성자 (smart constructor)**: 생성자를 `private` 으로 닫고, 불변식을 검사해 통과해야만 값을 내주는 `static Create` 메서드
> - **`Validation<E, A>`**: 3부에서 만든 applicative 누적 검증, 성공 `Valid` 또는 실패 `Invalid` 의 두 케이스
> - **`Apply` 체인**: 2부 Applicative 의 `Apply` 를 이어, `Pure` 로 끌어올린 커리한 생성자에 독립 검증을 한 칸씩 채워 결합하는 어법. 모두 성공해야 성공이고 아니면 양쪽 오류를 모으며, 인자를 한 칸 더 채우려면 `.Apply(...)` 한 줄만 잇습니다
> - **primitive obsession (원시 타입 남용)**: 도메인 개념을 원시 타입 그대로 들고 다녀 잘못된 값이 어디서든 새는 안티패턴

> 이 장을 마치면 할 수 있게 되는 것
> - [ ] 원시 `string` 을 그대로 들고 다닐 때 잘못된 값이 왜 어디서든 새는지 설명할 수 있습니다.
> - [ ] 필드를 `if` 로 하나씩 검사하면 왜 사용자가 오류를 하나씩만 보게 되는지 설명할 수 있습니다.
> - [ ] private 생성자 + 스마트 생성자가 어떻게 잘못된 상태를 표현 불가능하게 만드는지 설명할 수 있습니다.
> - [ ] 관련 잎을 묶어 복합 값 객체로 끌어올리는 것이 왜 도메인을 더 또렷하게 만드는지 설명할 수 있습니다.
> - [ ] 복합 값 객체가 자기 잎들을 `Pure`→`Apply` 체인으로 검증·조립하는 과정을 따라갈 수 있습니다.
> - [ ] `Customer.Create("ab", "no-at", "010-1234-5678", "1 Main St", "Seoul", "bad")` 이 서로 다른 세 계층의 오류를 한 목록에 모으는 과정을 손계산으로 추적할 수 있습니다.
> - [ ] `Apply` 가 한 계층이 아니라 모든 계층에서 합성·누적한다는 것이 무슨 뜻인지 답할 수 있습니다.
> - [ ] `Customer` 값이 손에 있다는 것이 왜 모든 계층 불변식의 컴파일 타임 보증인지 설명할 수 있습니다.

---

## 39.1 이 장에서 다루는 것 — 도메인을 타입으로

11부까지 우리는 함수형의 도구를 하나씩 직접 손으로 만들며 익혔습니다. 1~3부에서 5 개 trait 와 Monoid, 그리고 `Validation` 을 만들었고, 5부에서 Reader · State · Writer 를, 7부에서 `Eff<RT>` 와 `Has` DI 를, 8부에서 Schedule 과 Resource 를, 9부에서 Atom 과 STM 을, 10부에서 스트리밍을, 11부에서 테스트 더블과 런타임 교체를 만들었습니다. 도구는 다 손에 있습니다. 그런데 실무는 이 도구들을 하나씩 떼어 쓰지 않습니다. 한 기능 안에서 검증과 의존성 주입과 오류 처리와 자원과 스트리밍이 동시에 얽힙니다.

12부의 출발점은 이 도구들이 한 실무 코드로 합성되는 자리입니다. 그래서 이 장에는 새 추상이 없습니다. 한 문장으로 잡습니다. 3부에서 만든 `Validation` 의 applicative 누적을, 실무에서 가장 먼저 부딪히는 자리 (사용자 입력을 도메인 타입으로 바꾸는 입구) 에 합성합니다. 입력 검증은 모든 애플리케이션의 가장 바깥 경계이고, 그래서 12부의 가장 바깥 장입니다.

먼저 이 장이 무슨 일을 하는지 그림 하나로 잡습니다. 보통의 코드는 사용자 입력을 `string name`, `string email` 처럼 원시 타입 그대로 받아 들고 다닙니다. 그러다 어딘가에서 `name` 이 빈 문자열인지, `email` 에 `@` 가 있는지를 그때그때 검사합니다. 이 장이 만드는 것은 다릅니다. 검증을 통과해야만 존재하는 타입을 먼저 만들되, 한 계층에서 멈추지 않습니다. 잎 값 객체 (`Username` · `Email` · `PhoneNumber` 같은 원시값 하나짜리) 를 만들고, 관련된 잎을 묶어 복합 값 객체 (`ContactInfo` · `Address`) 로 끌어올리고, 그 위에 집합체 `Customer` 를 둡니다. 원시 입력을 이 계층으로 끌어올리는 입구 하나 (`Customer.Create`) 를 통과한 값은 이미 모든 계층에서 검증됐으므로, 이후 코드는 다시 검사할 필요가 없습니다. 잘못된 값은 입구에서 막혀 도메인 경계 안으로 들어오지 못합니다.

여기서 12부 전체를 꿰는 한 줄이 나옵니다. 1부에서 익힌 함수형의 본질 (모든 값과 함수를 합성 가능한 Elevated World 로 끌어올림) 이, 12부에서는 실무 규모로 확장됩니다. 3부에서 `Validation` 으로 작은 값 하나를 검증해 끌어올리던 그 동사를, 이 장에서는 고객 등록 폼 전체에 동시에 겁니다. 그것도 평평한 한 줄이 아니라 잎 → 복합 값 객체 → 집합체의 계층 전체에 겁니다. 같은 어휘, 실무의 무대입니다.

지금 모든 것을 외우지 않아도 됩니다. 이 장이 끝날 때 손에 남는 것은 두 가지입니다. 검증을 통과해야만 도메인 타입이 존재한다는 설계 하나와, 그 검증의 누적이 한 계층이 아니라 계층 전체에서 동시에 동작한다는 발상 하나입니다. 이 장에 등장하는 어휘를 한 줄씩만 미리 짚어 둡니다. 스마트 생성자는 생성자를 `private` 으로 닫고 검증을 통과해야만 값을 내주는 `Create` 메서드입니다. 복합 값 객체는 관련 잎을 묶어 자체 `Apply` 체인으로 검증·조립하는 한 계층의 값 객체입니다. `Validation<E, A>` 는 3부에서 만든 검증 타입으로, 성공 `Valid` 또는 실패 `Invalid` 입니다. 모두 본문에서 코드와 함께 다시 천천히 풀므로, 여기서는 이름과 한 줄 뜻만 스쳐 두면 됩니다.

---

## 39.2 왜 필요한가 — 원시 타입과 첫 오류에서 멈춤

강타입 도메인을 보이기 전에, 원시 타입을 그대로 들고 다니거나 검증을 순진하게 짜면 어디서 막히는지부터 부딪혀 봅니다. 설계 원칙을 먼저 외우지 않고 손에 잡히는 불편을 먼저 겪는 것이 이 장의 순서입니다.

흔한 작업을 하나 떠올립니다. 고객 등록 폼에서 이름 · 이메일 · 전화번호 · 주소를 받습니다. 명령형이나 평범한 객체 지향으로 적으면 이렇게 시작하게 됩니다.

```csharp
// 원시 타입을 그대로 들고 다니는 고객 등록
public sealed record Customer(
    string Name, string Email, string Phone, string Street, string City, string Postal);

Customer Register(
    string name, string email, string phone, string street, string city, string postal) =>
    new Customer(name, email, phone, street, city, postal);   // ← 아무 검증 없이 그대로 생성된다
```

문제가 둘입니다.

**첫째, 잘못된 값이 어디서든 샙니다.** `Customer` 의 필드가 모두 원시 `string` 이라, `new Customer("", "엉터리", "abc", "", "", "x")` 도 멀쩡히 만들어집니다. 빈 이름, `@` 없는 이메일, 자릿수가 모자란 우편번호가 아무 저항 없이 `Customer` 안으로 들어갑니다. 이 `Customer` 를 받는 코드는 그 값이 검증됐는지 알 길이 없어, 쓰는 자리마다 다시 `if (string.IsNullOrEmpty(customer.Name))` 를 검사하게 됩니다. 검증이 한 입구에 모이지 않고 코드 전체로 흩어집니다. 이렇게 도메인 개념을 원시 타입 그대로 들고 다녀 잘못된 값이 어디서든 새는 것을 primitive obsession (원시 타입 남용) 이라 부릅니다. 게다가 `string` 여섯 개가 평평하게 나열돼 있어, 이메일과 전화번호가 한 묶음 (연락처) 이고 거리 · 도시 · 우편번호가 한 묶음 (주소) 이라는 도메인 구조가 타입에 전혀 드러나지 않습니다.

OO 직감으로 옮기면 익숙합니다. `string` 으로 모든 것을 표현하던 코드가 `EmailAddress`, `PhoneNumber`, `Address` 같은 값 객체 (value object) 로 옮겨 가는 그 리팩터링이 정확히 이 불편을 푸는 길입니다. 원시 타입은 의미를 잃은 채 떠다니지만, 도메인 타입은 그 안에 불변식 (이메일은 `@` 를 포함한다) 을 담을 수 있고, 관련 값을 한 묶음으로 모을 수도 있습니다.

**둘째, 검증을 순진하게 짜면 첫 오류에서 멈춥니다.** 그래서 검증을 넣기로 합니다. 가장 먼저 떠오르는 모양은 `if` 로 하나씩 검사하다 틀리면 곧장 던지거나 돌려보내는 것입니다.

```csharp
// 첫 오류에서 멈추는 검증 — 사용자는 오류를 하나씩만 본다
Customer Register(string name, string email, string postal, /* … */)
{
    if (name.Trim().Length < 3)               throw new ArgumentException("이름이 너무 짧음");
    if (!email.Contains('@'))                 throw new ArgumentException("이메일 형식 오류");  // ← 여기 도달 못 할 수도
    if (postal.Length != 5)                   throw new ArgumentException("우편번호 형식 오류");
    return new Customer(/* … */);
}
```

`Register("ab", "엉터리", "x", …)` 을 부르면 어떻게 될까요. 첫 `if` 에서 이름이 짧다고 곧장 던집니다. 그러면 이메일과 우편번호는 검사조차 못 합니다. 사용자는 "이름이 너무 짧음" 한 줄만 보고, 고쳐서 다시 제출하면 그제야 "이메일 형식 오류" 를 봅니다. 세 군데가 다 틀렸는데 오류를 하나씩 세 번에 나눠 보게 됩니다. 등록 폼에서 가장 답답한 경험입니다.

이 두 불편의 공통점은 하나입니다. 검증이 타입과 분리되어 있고 (원시 타입을 그대로 들고 다님), 검증이 순차적이라 첫 실패에서 멈춥니다. 우리가 바라는 것은 분명합니다. 검증을 통과한 값만 도메인 타입으로 존재하게 하고 싶고 (그것도 관련 값을 묶은 계층으로), 여러 필드를 동시에 검사해 틀린 것을 한 번에 다 보여 주고 싶습니다.

> **흔한 함정** — 검증은 입구에서 한 번만 하면 되지 않냐고 여기는 것입니다.
>
> 입구에서 검증을 했다고 해도, `Customer` 의 필드가 여전히 원시 `string` 이면 그 보증이 타입에 남지 않습니다. 입구를 통과한 `Customer` 와 검증 없이 `new Customer(...)` 로 만든 `Customer` 가 같은 타입이라, 컴파일러는 둘을 구분하지 못합니다. 그래서 이 `Customer` 를 받는 다음 함수는 "이게 검증된 값인가" 를 알 수 없어 방어적으로 다시 검사하거나, 아니면 검사를 빼고 잘못된 값에 노출됩니다. 이 장의 강타입 도메인은 검증의 결과를 타입에 새겨, `Username` 값이 손에 있다는 것 자체가 "이미 검증됐다" 는 보증이 되게 만듭니다. 검증을 한 번만 하는 것을 넘어, 그 한 번의 결과가 타입에 영원히 남게 하는 것이 핵심입니다.

다음 절에서 그 강타입 도메인이 어떤 구조인지 봅니다.

---

## 39.3 계층 도메인 — 값 객체가 값 객체를 품는다

이제 검증을 통과해야만 존재하는 타입을 만드는데, 한 계층에서 멈추지 않습니다. 핵심 발상은 두 단계입니다. 첫째, 원시값을 검증된 잎 값 객체로 끌어올립니다 (스마트 생성자). 둘째, 관련된 잎을 묶어 또 하나의 값 객체로 만듭니다. 이메일과 전화번호는 따로 떠다니는 두 값이 아니라 연락처라는 한 개념의 두 면이고, 거리 · 도시 · 우편번호는 주소라는 한 개념의 세 면입니다. 이렇게 관련 필드를 묶어 중첩 값 객체로 끌어올리면, 도메인이 더 또렷해지고 검증도 그 묶음 단위로 모입니다.

먼저 스마트 생성자의 발상을 일상의 비유로 짧게 잡습니다. 출입증 발급 창구를 떠올립니다. 건물 안으로 들어가려면 반드시 창구에서 신원을 확인받고 출입증을 받아야 합니다. 옆문으로 몰래 들어갈 수는 없습니다 (옆문이 잠겨 있으니까요). 그래서 건물 안에서 누군가 출입증을 들고 있다면, 그 사람은 이미 신원 확인을 통과한 사람입니다. 다시 확인할 필요가 없습니다. 스마트 생성자가 그 창구이고, `private` 생성자가 잠긴 옆문이며, 도메인 타입 값이 출입증입니다.

먼저 잎 값 객체입니다. 이름을 표현하는 `Username` 입니다.

```csharp
// 잎 값 객체 — 검증을 통과해야만 생성되는 값 (잘못된 상태를 표현 불가능하게).
// private 생성자 + 스마트 생성자(Validation 반환) 로 불변식을 타입 경계에 가둔다.
public sealed record Username
{
    public string Value { get; }
    private Username(string value) => Value = value;        // ← 생성자를 닫는다 (옆문 잠금)

    public static Validation<string, Username> Create(string raw) =>
        raw.Trim().Length >= 3                              // ← 불변식: 3자 이상
            ? Validation<string, Username>.Success(new Username(raw.Trim()))
            : Validation<string, Username>.Fail($"username '{raw}' 은 3자 이상이어야 함");
}
```

한 줄씩 읽습니다. `Username` 은 `Value` 하나를 품은 record 입니다. 결정적인 곳은 생성자 앞의 `private` 입니다. 이 한 단어가 외부에서 `new Username(...)` 을 부르는 길을 막습니다. 그러면 `Username` 값을 어떻게 얻을까요. 유일한 입구가 `static Create` 입니다. `Create` 는 원시 `string` 을 받아 불변식 (`raw.Trim().Length >= 3`, 곧 공백을 떼고 3자 이상) 을 검사합니다. 통과하면 `Success` 로 `Username` 값을 감싸 내주고, 못 통과하면 `Fail` 로 오류 메시지를 내줍니다. 곧 `Create` 만이 `Username` 을 만들 수 있고, `Create` 는 검증을 통과해야만 만들어 줍니다.

나머지 잎도 모두 같은 골격입니다. `private` 생성자로 직접 생성을 막고, `Create` 가 각자의 불변식을 검사해 `Validation` 을 돌려줍니다.

```csharp
public sealed record Email
{
    public string Value { get; }
    private Email(string value) => Value = value;
    public static Validation<string, Email> Create(string raw) =>
        raw.Contains('@') && raw.Contains('.')              // ← 불변식: @ 와 . 둘 다 포함
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
```

같은 모양으로 `Street` · `City` (각각 비어 있지 않음) 와 `PostalCode` (숫자 5자리) 까지 모두 여섯 잎을 둡니다. 여기까지가 한 계층입니다. 원시 `string` 을 검증을 통과해야만 존재하는 잎 값 객체로 끌어올렸습니다.

이제 한 계층 올라갑니다. 이메일과 전화번호를 묶은 복합 값 객체 `ContactInfo` 입니다. 결정적인 곳은 `ContactInfo` 가 자기 잎들을 2부에서 만든 `Lift2` 로 검증·조립한다는 점입니다. `Lift2` 는 평범한 2인자 생성자 `(e, p) => new ContactInfo(e, p)` 를 받아 `Curry → Pure → Apply` 를 한 줄에 캡슐화하므로, 커리한 생성자를 손으로 적지 않아도 됩니다.

```csharp
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
```

`Address` 도 같은 모양으로 세 잎 (`Street` · `City` · `PostalCode`) 을 `Lift3` 로 묶습니다.

```csharp
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
```

두 복합 값 객체 모두 2부에서 본 `Lift` 어법 그대로입니다. `Lift2`/`Lift3` 가 `Pure(커리한 생성자).Apply(..).Apply(..)` 를 안에 캡슐화하니, 호출부는 평범한 다인자 생성자만 넘기면 됩니다. 차이는 묶는 대상이 폼의 평평한 칸이 아니라 한 도메인 개념의 잎들이라는 데 있습니다. 곧 보겠지만 이 `Lift` 가 안에서 굴리는 `Apply` 체인 덕분에 한 복합 값 객체 안에서 두 잎이 모두 틀리면 그 오류가 이 계층에서 먼저 모입니다. 이렇게 복합 값 객체가 손에 있다는 것은 그 안의 모든 잎이 검증을 통과했다는 뜻입니다.

여기서 스마트 생성자가 왜 예외를 던지지 않고 `Validation` 을 반환하는지 짚어 둡니다. 예외로 검증하면 (`throw new ArgumentException(...)`), 실패가 시그니처에 보이지 않습니다. `Username Create(string raw)` 라는 시그니처만 봐서는 이 함수가 실패할 수 있는지 알 수 없고, 본문을 읽거나 문서를 봐야 압니다. 게다가 예외는 한 번에 하나만 던져, 곧 보게 될 오류 누적과 합쳐지지 않습니다. 반면 `Validation<string, Username> Create(string raw)` 는 실패 가능성이 반환 타입에 드러나고 (3부에서 본 효과를 값으로), 그래서 `Apply` 체인으로 다른 검증과 합성됩니다. 실패를 던지는 사건이 아니라 다루는 값으로 바꾸는 것이 함수형 검증의 출발점입니다.

손으로 한번 따라갑니다. 잎 스마트 생성자가 값을 어떻게 거르는지 봅니다.

```
원시 입력                  Create (검증)                결과
─────────                  ───────────                  ────

"alice"     ──Username.Create──▶  Trim 후 5자 ≥ 3 ?  예  ──▶  Valid(Username "alice")
                                                                    │
                                                              private ctor 로만 생성
                                                              (옆문 잠김 — 우회 불가)

"ab"        ──Username.Create──▶  Trim 후 2자 ≥ 3 ?  아니오 ──▶  Invalid(["username 'ab' …"])
                                                                    │
                                                              Username 값이 만들어지지 않음
                                                              잘못된 값은 경계 밖에 머문다
```

`"alice"` 는 불변식을 통과해 `Username` 값으로 끌어올려지고, `"ab"` 는 통과하지 못해 `Username` 이 되지 못한 채 오류로 남습니다. 통과한 값만 도메인 안으로 들어오고, 통과하지 못한 값은 경계 밖에 머뭅니다.

![스마트 생성자: 원시 입력이 검증을 통과해야만 잎 값 객체로 끌어올려지고, private 생성자가 우회를 차단한다](./images/Ch39-Domain-Modeling/01-smart-constructor.svg)

**그림 39-1. 스마트 생성자: 검증을 통과해야만 도메인 타입이 된다** — 원시 입력 (`string`) 이 `Create` 라는 입구를 거칩니다. 불변식을 통과하면 `Valid` 로 감싼 잎 값 객체 (`Username` · `Email` · `PhoneNumber`) 가 나오고, 통과하지 못하면 `Invalid` 로 오류가 나옵니다. `private` 생성자가 `Create` 를 거치지 않는 직접 생성 (우회) 을 원리적으로 막아, 잘못된 값은 도메인 경계 안으로 들어오지 못합니다. OO 의 값 객체 (value object) 가 생성자에서 불변식을 검사하는 그 발상과 같되, 예외 대신 `Validation` 을 돌려줍니다.

> **흔한 함정** — 생성자를 `public` 으로 두면 검증이 우회됩니다.
>
> 스마트 생성자의 힘은 전적으로 `private` 생성자에서 나옵니다. 만약 `Username` 의 생성자를 `public` 으로 열어 두면, 누군가 `Create` 를 거치지 않고 `new Username("")` 으로 빈 이름을 만들 수 있습니다. 그 순간 "`Username` 값이 있다 = 검증됐다" 는 보증이 깨지고, 강타입 도메인의 모든 약속이 무너집니다. 출입증 비유로는 옆문이 열려 있는 셈입니다. 검증을 통과하지 않은 값이 출입증을 달고 건물 안을 돌아다니게 됩니다. record 의 기본 생성자가 `public` 이라는 점도 주의할 자리입니다. record 를 쓰되 생성자는 반드시 `private` 으로 닫고 입구를 `Create` 하나로 좁혀야, 불변식이 타입 경계에 갇힙니다. 복합 값 객체 `ContactInfo` · `Address` 도 같은 이유로 생성자를 `private` 으로 닫고 입구를 `Create` 하나로 둡니다.

---

## 39.4 집합체 — Apply 가 모든 계층에서 누적한다

복합 값 객체까지 만들었으니, 이제 잎 하나 (`Username`) 와 복합 값 객체 둘 (`ContactInfo` · `Address`) 을 묶어 집합체 `Customer` 를 만들 차례입니다. 그런데 단순히 묶는 게 아니라, 잎과 복합 값 객체를 동시에 굴려 틀린 것을 한 번에 다 모으고 싶습니다. 그 일을 하는 도구가 2부에서 익힌 `Apply` 입니다. 3부에서 만든 `Validation` 에 그 `Apply` 를 붙여, 커리한 `Customer` 생성자에 세 결과를 한 칸씩 채워 넣습니다.

먼저 3부에서 만든 `Validation` 을 한 줄로 상기합니다. `Validation<E, A>` 는 성공 `Valid(A)` 또는 실패 `Invalid(오류 목록)` 의 두 케이스를 가진 타입이었고, 그 핵심은 여러 검증을 결합할 때 첫 실패에서 멈추지 않고 모든 실패를 모은다는 데 있었습니다. 이 장의 코드는 라이브러리를 쓰지 않고 그 누적 구조를 `string` 오류 버전으로 다시 손에 둡니다.

```csharp
// Validation<E, A> — applicative *누적* 검증 (3부 MyValidation 의 실전판).
// Apply 가 여러 필드를 동시에 검증하며 *모든 오류를 모은다* (첫 오류에서 멈추지 않음).
public abstract record Validation<E, A>
{
    public sealed record Valid(A Value) : Validation<E, A>;
    public sealed record Invalid(IReadOnlyList<E> Errors) : Validation<E, A>;

    public static Validation<E, A> Success(A value) => new Valid(value);
    public static Validation<E, A> Fail(E error) => new Invalid([error]);   // ← 오류 하나를 목록으로 감쌈

    public Validation<E, B> Map<B>(Func<A, B> f) =>
        this switch
        {
            Valid v => new Validation<E, B>.Valid(f(v.Value)),   // Valid 면 f 적용
            Invalid i => new Validation<E, B>.Invalid(i.Errors), // Invalid 면 오류 그대로 통과
            _ => throw new InvalidOperationException()
        };
}
```

한 줄씩 읽습니다. `Validation<E, A>` 는 `Valid` (성공한 값 하나) 와 `Invalid` (오류 목록) 의 두 케이스입니다. `Success` 는 값을 `Valid` 로 감싸고, `Fail` 은 오류 하나를 목록 `[error]` 로 감싸 `Invalid` 로 만듭니다. 여기서 `Fail` 이 오류를 곧장 목록에 담는 것을 눈여겨봅니다. 오류가 목록이라야 나중에 여러 오류를 이어 붙여 모을 수 있기 때문입니다. `Map` 은 2부 Functor 의 그 끌어올림입니다. `Valid` 면 안의 값에 `f` 를 적용하고, `Invalid` 면 오류를 그대로 통과시킵니다. `Success` 는 2부 Applicative 의 `Pure` 와 같은 자리로, Normal 값을 `Valid` 한 칸으로 끌어올립니다.

이제 누적의 핵심, `Apply` 입니다.

```csharp
public static class Validation
{
    static IReadOnlyList<E> Errors<E, A>(Validation<E, A> v) =>
        v is Validation<E, A>.Invalid i ? i.Errors : [];     // Invalid 면 오류 목록, Valid 면 빈 목록

    // Pure — Normal 값을 Valid 한 칸으로 끌어올림 (2부 Applicative 의 Pure).
    public static Validation<E, A> Pure<E, A>(A value) => new Validation<E, A>.Valid(value);

    // Apply — 2부 Applicative 의 그 Apply. 갇힌 함수 Validation<E, Func<A,B>> 에
    // 인자 Validation<E, A> 를 한 칸 채워 Validation<E, B> 를 낸다.
    // 둘 다 Valid 면 함수 적용, 아니면 *양쪽 오류 누적*.
    public static Validation<E, B> Apply<E, A, B>(
        this Validation<E, Func<A, B>> vf, Validation<E, A> va) =>
        (vf, va) switch
        {
            (Validation<E, Func<A, B>>.Valid f, Validation<E, A>.Valid a) =>
                new Validation<E, B>.Valid(f.Value(a.Value)),                // 둘 다 Valid → 함수에 인자 적용
            _ => new Validation<E, B>.Invalid([.. Errors(vf), .. Errors(va)]) // 아니면 양쪽 오류 합침
        };

    // Lift2 / Lift3 — 5장의 Lift 그대로. 평범한 다인자 함수 (A, B) → C 를 받아
    // Curry → Pure → Apply 를 한 번에 캡슐화한다. 호출부가 커리한 생성자를 손으로 적지 않아도 된다.
    public static Validation<E, C> Lift2<E, A, B, C>(
        Func<A, B, C> f, Validation<E, A> fa, Validation<E, B> fb) =>
        Pure<E, Func<A, Func<B, C>>>(a => b => f(a, b))     // 커리는 여기서 한 번만
            .Apply(fa)
            .Apply(fb);

    public static Validation<E, D> Lift3<E, A, B, C, D>(
        Func<A, B, C, D> f, Validation<E, A> fa, Validation<E, B> fb, Validation<E, C> fc) =>
        Pure<E, Func<A, Func<B, Func<C, D>>>>(a => b => c => f(a, b, c))
            .Apply(fa)
            .Apply(fb)
            .Apply(fc);
}
```

`Apply` 를 한 줄로 읽습니다. 갇힌 함수 `vf` (`Validation<E, Func<A, B>>`) 와 인자 `va` (`Validation<E, A>`) 를 받아, 둘 다 `Valid` 일 때만 함수에 인자를 적용해 `Valid` 를 냅니다. 그 외의 모든 경우 (한쪽이라도 `Invalid`) 에는 `[.. Errors(vf), .. Errors(va)]` 로 양쪽의 오류 목록을 이어 붙여 `Invalid` 를 냅니다. 결정적인 곳이 이 이어 붙임입니다. 함수 쪽이 이미 오류를 들고 있어도 멈추지 않고 인자 쪽의 오류까지 모읍니다. `vf` 가 실패고 `va` 도 실패면 두 오류가 다 모이고, 한쪽만 실패면 그 하나만 모입니다.

그 아래 `Lift2`/`Lift3` 가 2부에서 만든 그 `Lift` 입니다. 평범한 다인자 생성자 `(a, b) => f(a, b)` 를 받아, 안에서 한 번만 커리한 뒤 `Pure` 로 끌어올리고 `Apply` 를 인자 수만큼 잇습니다. 곧 `Curry → Pure → Apply` 사슬을 한 줄에 캡슐화한 것입니다. 그래서 오류 누적은 그대로 이 `Lift` 안에서 `Apply` 가 돌며 일어납니다. 한 인자라도 `Invalid` 면 위 `Apply` 가 `[.. Errors(vf), .. Errors(va)]` 로 양쪽 오류를 이어 붙입니다.

집합체도 2부의 `Lift` 어법 그대로입니다. 평범한 3인자 생성자 `(n, c, a) => new Customer(n, c, a)` 를 `Lift3` 에 넘기면, 안에서 커리·`Pure`·`Apply` 세 단계가 한 번에 처리됩니다. 매번 커리한 생성자를 손으로 적지 않습니다.

```csharp
// 집합체 — 잎 하나(Username) 와 복합 값 객체 둘(ContactInfo · Address) 을 묶은 최상위.
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
```

한 줄씩 읽습니다. `Customer` 도 다른 계층과 똑같이 `private` 생성자로 옆문을 잠그고 입구를 `Create` 하나로 둡니다. `Create` 는 평범한 3인자 생성자 `(n, c, a) => new Customer(n, c, a)` 를 `Lift3` 에 넘기고, 잎 하나 (`Username.Create`) 와 복합 값 객체 둘 (`ContactInfo.Create` · `Address.Create`) 을 인자로 줍니다. `Lift3` 안에서는 그 생성자가 커리돼 `Pure` 로 한 칸 끌어올려진 뒤, 세 결과가 `Apply` 로 한 칸씩 채워집니다. 인자가 들어올 때마다 갇힌 함수의 빈자리가 하나씩 줄어, 마지막 `Apply` 에서 `Customer` 가 완성됩니다. 곧 이항 `Apply` 하나를 사슬로 이어 삼항을 처리하는 것이고, 2부에서 본 applicative 의 본질 (이항 `Apply` 를 사슬로 이어 n-항을 처리) 이 `Lift3` 안에 그대로 있습니다.

여기서 2부와 결정적으로 갈리는 점이 나옵니다. 이 자리가 이 장에서 가장 손으로 펼쳐 볼 가치가 있는 곳입니다. 2부의 `Lift` 는 평평한 칸들의 오류를 모았습니다. 이 장의 `Lift3` 가 받는 `ContactInfo.Create(..)` 와 `Address.Create(..)` 는 그 자체가 또 `Lift` 의 결과입니다. 곧 하위 복합 값 객체가 자기 계층에서 이미 누적한 오류를, 상위 `Lift3` 의 `Apply` 체인이 그대로 합류시킵니다.

손계산으로 펼쳐 봅니다. 잘못된 곳이 서로 다른 세 계층에 박힌 `Customer.Create("ab", "no-at", "010-1234-5678", "1 Main St", "Seoul", "bad")` 를 추적합니다.

```
잘못된 곳은 서로 다른 세 계층에 박혀 있다:
  - username "ab"  → 집합체 직속 잎
  - email "no-at"  → ContactInfo(복합) 안의 잎
  - postal "bad"   → Address(복합) 안의 잎
  - phone/street/city 는 유효.

[1단계] 하위 복합 값 객체가 먼저 자기 계층에서 누적한다 (Lift 가 안에서 Apply 를 굴린다).

  Username.Create("ab")
     → Invalid(["username 'ab' 은 …"])                            (잎 1개)

  ContactInfo.Create("no-at", "010-1234-5678")
     = Lift2((e,p) => new ContactInfo(e,p), Email.Create("no-at"), PhoneNumber.Create("010-…"))
       · Email.Create("no-at") = Invalid(["email 'no-at' …"])     (@,. 없음)
       · PhoneNumber.Create("010-1234-5678") = Valid(…)           (숫자 9+개)
       · Lift2 안: Pure(f).Apply(Invalid[email]) → Invalid(["email …"])
       · .Apply(Valid phone) → 함수쪽 Invalid + 값쪽 Valid → 오류 그대로 통과
     → Invalid(["email 'no-at' …"])                               (이 계층 1개)

  Address.Create("1 Main St", "Seoul", "bad")
     = Lift3((s,c,p) => new Address(s,c,p), Street.Create, City.Create, PostalCode.Create("bad"))
       · Street.Create = Valid · City.Create = Valid · PostalCode.Create("bad") = Invalid(["postal 'bad' …"])
       · Lift3 안: Valid·Valid 단계는 함수만 채워지고, 마지막 Apply(Invalid[postal]) 에서 오류 합류
     → Invalid(["postal 'bad' …"])                                (이 계층 1개)

[2단계] 집합체 Lift3 의 Apply 체인이 세 결과를 한 칸씩 채우며 하위 누적을 그대로 합류한다.

  Customer.Create = Lift3((n,c,a) => new Customer(n,c,a), …)
    Pure(f)                                           오류 []
      .Apply(Username.Create("ab"))                   → Invalid(["username …"])           (1개)
      .Apply(ContactInfo.Create(..))                  → [.. username, .. email]           (2개)
      .Apply(Address.Create(..))                      → [.. username, email, .. postal]   (3개)

결과: Invalid(["username 'ab' 은 …", "email 'no-at' …", "postal 'bad' …"])  ← 3개
      ▲ 서로 다른 세 계층에서 발원한 오류가 한 목록에 모였다
```

오류 3개가 서로 다른 세 계층 (집합체 직속 잎 · `ContactInfo` 안의 잎 · `Address` 안의 잎) 에서 발원했는데도, 하위 복합 값 객체 안에서 누적된 것이 상위 `Lift3` 의 `Apply` 체인으로 합류해 한 목록에 모입니다. 이것이 `Apply` 가 한 계층이 아니라 모든 계층에서 합성·누적한다는 뜻입니다.

OO 직감으로 다리를 놓으면, 이것이 폼 검증 라이브러리의 동작과 같습니다. 잘 만든 등록 폼은 제출 버튼을 누르면 이름 · 이메일 · 주소의 오류를 빨간 글씨로 한꺼번에 보여 줍니다. 하나 고치면 다음 오류가 나오는 게 아니라, 틀린 것을 처음부터 다 보여 줍니다. 게다가 이메일이 연락처 섹션에 있고 우편번호가 주소 섹션에 있어도, 섹션을 가리지 않고 한꺼번에 보여 줍니다. `Apply` 체인의 계층-교차 누적이 정확히 그 동작을 타입 수준에서 만듭니다.

여기서 왜 누적이 가능한지를 앞서 익힌 것과 이어 짚어 둡니다. 한 계층 안의 검증들은 서로 독립입니다. `Email` 검증이 `PhoneNumber` 검증의 결과를 필요로 하지 않고, `Username` 검증이 `ContactInfo` 검증의 결과를 필요로 하지 않습니다. 서로 의존하지 않으니 모두 동시에 굴려 오류를 모을 수 있습니다. 이것이 applicative (누적) 가 monad (단락) 와 갈리는 자리입니다. 만약 둘째 검증이 첫째 검증의 결과를 입력으로 받아야 한다면 (의존), 첫째가 실패한 순간 둘째를 굴릴 입력이 없어 멈출 수밖에 없습니다. 그것이 monadic `Bind` 의 단락이고, 그래서 이 장의 `Validation` 은 `Bind` 를 아예 정의하지 않았습니다. "독립이라 모은다, 의존이라 멈춘다", 이 한 줄이 둘을 가릅니다.

![합성 가능한 Elevated World: Create 가 원시 문자열을 Normal World 에서 Elevated World 로 끌어올리고, 그 안에서 같은 Lift 가 잎·복합·집합체 모든 계층을 합성하며 오류를 누적한다](./images/Ch39-Domain-Modeling/02-accumulate-errors.svg)

**그림 39-2. 합성 가능한 Elevated World: Create 로 끌어올리고 Lift 로 합성한다** — `Customer.Create("ab", "no-at", "010-…", "1 Main St", "Seoul", "bad")` 한 입력을 추적합니다. 아래 Normal World 의 원시 문자열 여섯 개는 그대로는 검증도 합성도 되지 않습니다. `Create` 가 각 문자열을 검증하며 Elevated World 로 끌어올리면 (파란 화살표가 두 세계를 건넙니다), 잘못된 셋 (`username` · `email` · `postal`) 은 빨간 잎이 됩니다. Elevated World 안에서는 같은 `Lift` 가 모든 계층을 합성합니다. 두 잎이 `Lift2` 로 `ContactInfo` (email 오류 1개) 가 되고, 세 잎이 `Lift3` 로 `Address` (postal 오류 1개) 가 되며, `Username` 잎과 두 복합 값 객체가 다시 `Lift3` 로 `Customer` 가 됩니다. 그 과정에서 오류가 `[]`→`[username]`→`[username, email]`→`[username, email, postal]` 로 누적돼 `Invalid(3개)` 가 됩니다. 한 번 끌어올린 값은 어느 계층이든 같은 `Lift` 로 합성되고, 오류조차 함께 합성(누적)됩니다.

> **미리보기입니다** — 다음 절에서 이 계층-교차 누적이 실제 데모로 굴러갑니다.
>
> `Customer.Create("alice", "alice@example.com", "010-1234-5678", "1 Main St", "Seoul", "04524")` 는 모두 유효해 `Valid(Customer)` 를 내고, 위 손계산의 입력은 서로 다른 세 계층이 틀려 `Invalid(3개)` 를 내고, 이메일만 틀린 입력은 `Invalid(1개)` 만 냅니다. 한 복합 값 객체 안에서 두 잎이 모두 틀리면 그 계층에서 오류 2개가 먼저 모이는 것까지 다음 절에서 손으로 확인합니다. 강타입 계층 도메인과 계층-교차 누적이 한 함수 `Customer.Create` 안에서 어떻게 합쳐지는지가 이 장의 payoff 입니다.

---

## 39.5 end-to-end payoff — Customer.Create 파이프라인

이제 강타입 계층 도메인과 계층-교차 누적이 한 입구 `Customer.Create` 로 합쳐지는 자리를 봅니다. `Customer.Create` 는 원시 입력 여섯 개를 받아, 잎을 `Create` 로 검증하고, 두 복합 값 객체 (`ContactInfo` · `Address`) 를 조립하고, 그 셋을 `Lift3` 로 집합체 `Customer` 에 채웁니다. 모두 `Valid` 일 때만 `Customer` 가 만들어지고, 하나라도 `Invalid` 면 어느 계층의 오류든 모두 누적됩니다. 이 한 함수가 앞서 본 두 불편을 동시에 풉니다. 원시 입력이 계층 도메인 타입으로 끌어올려지고 (강타입), 여러 계층의 오류가 한 번에 모입니다 (계층-교차 누적).

세 가지 입력으로 실제 데모를 따라갑니다. 코드의 `Program.cs` 가 돌리는 그대로입니다.

**유효한 입력.** `Customer.Create("alice", "alice@example.com", "010-1234-5678", "1 Main St", "Seoul", "04524")`.

```
Username.Create("alice")              → Valid(Username "alice")
ContactInfo.Create("alice@…", "010-…") → Valid(ContactInfo …)     (email · phone 둘 다 Valid)
Address.Create("1 Main St", "Seoul", "04524") → Valid(Address …)  (street · city · postal 셋 다 Valid)

Lift3 의 Apply 체인: 세 Valid 를 만나 생성자가 끝까지 적용
결과: Valid(Customer(alice, alice@example.com, Seoul 04524))
출력: "Valid(alice, alice@example.com, Seoul 04524)"
```

모든 계층의 검증이 통과해 `Customer` 가 만들어졌습니다. 이 `Customer` 값이 손에 있다는 것은 6개 잎 불변식과 2개 복합 불변식이 모두 만족됐다는 컴파일 타임 보증입니다. 이후 어떤 코드도 이 `Customer` 의 이름이 빈 문자열인지, 이메일에 `@` 가 있는지 다시 검사할 필요가 없습니다.

**서로 다른 세 계층이 틀린 경우.** `Customer.Create("ab", "no-at", "010-1234-5678", "1 Main St", "Seoul", "bad")`. 앞 절에서 손으로 펼친 그 입력입니다.

```
Username.Create("ab")     → Invalid(["username 'ab' 은 3자 이상이어야 함"])    (집합체 직속 잎)
ContactInfo.Create("no-at", "010-…")
                          → Invalid(["email 'no-at' 형식이 올바르지 않음"])     (복합 안에서 1개 누적)
Address.Create("1 Main St", "Seoul", "bad")
                          → Invalid(["postal 'bad' 은 숫자 5자리여야 함"])      (복합 안에서 1개 누적)

Lift3 의 Apply 체인: 하위 복합 누적이 상위로 합류, 세 계층 오류가 한 목록에
결과: Invalid(["username …", "email …", "postal …"])
출력: "Invalid([username 'ab' 은 3자 이상이어야 함 / email 'no-at' 형식이 올바르지 않음 / postal 'bad' 은 숫자 5자리여야 함])"
```

세 오류가 모두 모였습니다. 첫 오류 (`Username`) 에서 멈추지 않았고, 서로 다른 세 계층에 박혀 있었는데도 한 목록에 모였습니다. 사용자는 세 군데가 틀렸다는 것을 한 번에 봅니다.

**일부만 틀린 경우.** `Customer.Create("alice", "bad", "010-1234-5678", "1 Main St", "Seoul", "04524")`. 이메일만 틀렸습니다.

```
Username.Create("alice")  → Valid(Username "alice")                          (유효)
ContactInfo.Create("bad", "010-…")
                          → Invalid(["email 'bad' 형식이 올바르지 않음"])      (복합 안 email 만 Invalid)
Address.Create(…)         → Valid(Address …)                                 (유효)

Lift3 의 Apply 체인: 유효한 두 자리의 Errors 는 빈 목록 → 이어 붙여도 email 오류 하나만 남음
결과: Invalid(["email 'bad' 형식이 올바르지 않음"])
출력: "Invalid([email 'bad' 형식이 올바르지 않음])"
```

이메일 오류 하나만 보고됐습니다. 유효한 자리 (`Username` · `Address`) 는 오류 없이 통과하고, 실패한 하나만 모습니다. `Apply` 의 이어 붙임에서 유효한 검증의 오류 목록이 빈 목록 (`[]`) 이라, 이어 붙여도 실패한 하나만 남기 때문입니다. 모든 오류를 모으되, 유효한 것까지 오류로 잡지는 않습니다.

마지막으로 한 복합 값 객체 안에서 두 잎이 모두 틀리는 경우를 보조로 확인합니다. `ContactInfo.Create("no-at", "abc")` 는 email 과 phone 이 둘 다 틀려, 이 계층에서 오류 2개가 먼저 모입니다 (`Invalid(["email 'no-at' …", "phone 'abc' …"])`). 누적이 집합체뿐 아니라 복합 값 객체 안에서도 똑같이 일어남을 보여 줍니다.

데모 출력은 입력을 차례로 보여 줍니다.

```
== 계층 도메인 등록 (계층-교차 누적) ==
  유효 입력        → Valid(alice, alice@example.com, Seoul 04524)
  계층-교차 3 오류 → Invalid([username 'ab' 은 3자 이상이어야 함 / email 'no-at' 형식이 올바르지 않음 / postal 'bad' 은 숫자 5자리여야 함])
  email 만 오류    → Invalid([email 'bad' 형식이 올바르지 않음])
  → 잘못된 username + email + postal 이 서로 다른 계층에 있어도 한 번에 모인다.

== 한 복합 값 객체 안에서의 누적 ==
  email·phone 둘 다 오류 → Invalid([email 'no-at' 형식이 올바르지 않음 / phone 'abc' 은 숫자 9자 이상이어야 함])
```

이 데모가 이 장의 payoff 입니다. `Customer.Create` 라는 한 입구가 원시 입력을 받아, 유효하면 검증된 `Customer` 로 끌어올리고, 틀리면 어느 계층의 오류든 한 번에 돌려줍니다. 앞서 본 두 불편 (잘못된 값이 어디서든 샘 · 오류를 하나씩만 봄) 이 강타입 계층 도메인과 계층-교차 누적의 합성으로 동시에 풀립니다. 잘못된 값은 `Customer` 경계를 못 넘고, 사용자는 모든 계층의 오류를 한 번에 봅니다.

같은 패턴이 계층 없는 도메인에도 그대로 옮겨 갑니다. 이 장의 챌린지가 주문 검증인데, 계층 없이 세 필드를 한 면에서 누적하는 구조라 계층 모델과 좋은 비교가 됩니다. `Order` 도 다른 계층과 똑같이 `private` 생성자로 옆문을 잠그고 입구를 `Create` 하나로 두며, 그 `Create` 가 `Lift3` 로 세 검증을 누적합니다.

```csharp
// 챌린지 정답 — 주문 검증. 계층 없는 평평한 도메인에도 같은 어법이 그대로 간다.
// 다른 계층과 동일하게: private 생성자로 직접 생성을 막고, Create 만이 Pure→Apply 누적으로 값을 내준다.
public sealed record Order
{
    public int Quantity { get; }
    public decimal Price { get; }
    public decimal DiscountRate { get; }
    private Order(int quantity, decimal price, decimal discountRate) =>     // 옆문 잠금
        (Quantity, Price, DiscountRate) = (quantity, price, discountRate);

    static Validation<string, int> CheckQuantity(int q) =>
        q > 0 ? Validation<string, int>.Success(q) : Validation<string, int>.Fail("수량은 양수여야 함");

    static Validation<string, decimal> CheckPrice(decimal p) =>
        p >= 0 ? Validation<string, decimal>.Success(p) : Validation<string, decimal>.Fail("가격은 음수일 수 없음");

    static Validation<string, decimal> CheckDiscount(decimal d) =>
        d is >= 0 and <= 1 ? Validation<string, decimal>.Success(d) : Validation<string, decimal>.Fail("할인율은 0~1");

    // Customer.Create 와 같은 Lift3 — 5장의 Lift 가 Curry → Pure → Apply 를 캡슐화한다.
    public static Validation<string, Order> Create(int quantity, decimal price, decimal discountRate) =>
        Validation.Lift3((int q, decimal p, decimal d) => new Order(q, p, d),
            CheckQuantity(quantity),
            CheckPrice(price),
            CheckDiscount(discountRate));
}
```

`Order.Create` 는 `Customer.Create` 와 같은 `Lift3` 누적을 쓰되, 검증 대상이 평평한 세 필드 (수량 `> 0` · 가격 `>= 0` · 할인율 `0~1`) 입니다. `Order.Create(0, -5, 2)` 처럼 셋 다 틀린 입력을 넣으면 세 오류가 모두 모입니다 (`Invalid(["수량은 양수여야 함", "가격은 음수일 수 없음", "할인율은 0~1"])`). 같은 `Lift3` 누적이 계층 있는 도메인 (`Customer`) 에도, 계층 없는 평평한 도메인 (`Order`) 에도 그대로 옮겨 간다는 것이 이 비교의 의도입니다.

> **더 깊이 (처음엔 건너뛰어도 됩니다)** — 계층이 깊어지면 어떻게 합칩니까.
>
> `Lift` 어법은 계층 깊이에 무관합니다. 각 계층은 자기 자리에서 `LiftN(생성자, 검증1, …, 검증N)` 으로 자기 하위를 누적하고, 상위 계층은 그 결과를 다시 한 인자로 받아 합류시킬 뿐입니다. 곧 한 계층에서 인자가 N 개면 `LiftN` 한 번이고 (안에서 `Pure` 한 번에 `Apply` 가 N 번), 계층이 한 단 깊어지면 그 하위가 또 자기 `Lift` 를 갖습니다. 새 메서드를 만들 필요가 없습니다. 2부의 applicative 가 이항 `Apply` 하나로 모든 arity 를 처리한 그 구조가, 계층 트리의 모든 마디에서 똑같이 반복됩니다. v5 라이브러리도 같은 어법을 쓰는데, `fun(생성자).Map(첫 검증).Apply(둘째 검증).Apply(셋째 검증)` 처럼 `Apply` 를 잇거나, 튜플 확장 `(emailV, phoneV).Apply((e, p) => ...)` 으로 한 번에 결합합니다. 이 장의 학습용 코드가 쓰는 `Lift` 가 바로 그 실무 어법의 가장 단순한 형태입니다.

---

## 39.6 법칙으로 다지기 — 계층-교차 누적의 핵심 성질

11부까지 새 추상마다 그 의미를 검사로 확인했습니다. 이 장에도 확인할 것이 있는데, Functor 나 Monad 의 대수 법칙은 아닙니다. 이 장이 다지는 것은 강타입 계층 도메인 + 계층-교차 누적이 약속한 네 가지 동작입니다. 모든 계층이 유효하면 `Customer` 에 도달하는가, 서로 다른 계층의 오류가 모두 합류하는가, 일부만 틀리면 그 오류만 모이는가, 한 복합 값 객체 안에서도 누적이 일어나는가입니다. 이 넷이 "검증을 통과한 값만 도메인 타입으로 존재하고, `Apply` 가 모든 계층에서 오류를 모은다" 는 이 장의 약속이 코드로 정말 그러한지입니다.

네 검사는 `bool` 함수 넷입니다.

```csharp
public static class DomainTests
{
    // ① 모두 유효 → Valid(Customer).
    public static bool ValidRegistration() =>
        Customer.Create("alice", "alice@example.com", "010-1234-5678", "1 Main St", "Seoul", "04524")
            is Validation<string, Customer>.Valid;

    // ② 계층을 가로지른 오류가 *모두* 누적 (잘못된 username + email + postal → 3개).
    //    하위 복합 값 객체 안에서 누적된 오류가 상위 Apply 체인으로 그대로 합류한다.
    public static bool AccumulatesAcrossLevels() =>
        Customer.Create("ab", "no-at", "010-1234-5678", "1 Main St", "Seoul", "bad")
            is Validation<string, Customer>.Invalid { Errors.Count: 3 };

    // ③ 일부만 오류 → 그 오류만 (email 한 칸만 틀림).
    public static bool PartialErrors() =>
        Customer.Create("alice", "bad", "010-1234-5678", "1 Main St", "Seoul", "04524")
            is Validation<string, Customer>.Invalid { Errors.Count: 1 };

    // ④ 한 복합 값 객체 안에서 두 잎이 모두 틀리면 그 계층에서 2개가 먼저 모인다.
    public static bool LeafErrorsAccumulateWithinComposite() =>
        ContactInfo.Create("no-at", "abc")
            is Validation<string, ContactInfo>.Invalid { Errors.Count: 2 };
}
```

네 검사를 한 줄씩 읽습니다.

첫째, **유효 입력의 도달** (`ValidRegistration`) — 모든 계층이 유효한 입력이 `Valid` 인지 봅니다. 검증을 통과한 입력이 집합체 `Customer` 까지 끌어올려짐을 확인합니다. 39.5 의 첫 데모를 그대로 단언으로 옮긴 것입니다.

둘째, **계층-교차 누적** (`AccumulatesAcrossLevels`) — 서로 다른 세 계층 (직속 잎 `username` · `ContactInfo` 안의 `email` · `Address` 안의 `postal`) 이 틀린 입력의 오류 개수가 정확히 3 인지 봅니다 (`Errors.Count: 3`). 넷 중 가장 중요한 단언입니다. 만약 이 검증이 monadic `Bind` 였거나, 하위 복합 값 객체의 누적이 상위로 합류하지 않았다면 오류가 더 적게 나왔을 것입니다. 개수가 3 이라는 것이 `Apply` 가 한 계층이 아니라 모든 계층에서 누적해 합류시킨다는 핵심 성질을 정확히 증명합니다.

셋째, **부분 오류** (`PartialErrors`) — 이메일만 틀린 입력의 오류 개수가 정확히 1 인지 봅니다 (`Errors.Count: 1`). 유효한 자리 (`Username` · `Address`) 는 오류 없이 통과하고 실패한 하나만 보고됨을 확인합니다. 모든 오류를 모으되 유효한 것까지 잡지는 않음을 단언합니다.

넷째, **복합 안 누적** (`LeafErrorsAccumulateWithinComposite`) — 한 복합 값 객체 `ContactInfo.Create("no-at", "abc")` 안에서 email 과 phone 두 잎이 모두 틀린 경우의 오류 개수가 정확히 2 인지 봅니다 (`Errors.Count: 2`). 누적이 집합체뿐 아니라 복합 값 객체라는 하위 계층 안에서도 똑같이 일어남을 단언합니다.

데모 출력은 넷 다 통과입니다.

```
== 검증 ==
  유효 등록 : 통과
  계층-교차 누적(3) : 통과
  부분 오류(1) : 통과
  복합 안 누적(2) : 통과

모든 검증 통과 [OK]
```

이 네 검사가 12부 축의 한 면을 굳힙니다. 12부의 약속은 "따로 익힌 도구가 한 실무 코드로 합성된다" 는 것이고, 이 장에서는 그 도구가 3부의 `Validation` 입니다. 네 검사는 그 `Validation` 의 누적이 강타입 계층 도메인과 합성됐을 때 약속대로 (유효는 도달 · 계층을 가로질러 다 모음 · 일부 틀리면 그것만 · 복합 안에서도 누적) 동작함을 코드로 단언합니다. 새 추상을 검증하는 게 아니라, 이미 익힌 추상이 계층 도메인 자리에서 제대로 합성됐는지를 검증하는 것입니다.

> **챌린지 — 복합 값 객체를 하나 더 추가하기**
>
> `Customer` 에 결제 정보 복합 값 객체 `PaymentInfo(CardNumber, Expiry)` 를 더합니다. `PaymentInfo.Create` 를 `ContactInfo` 와 같은 `Lift2` 로 짜고, `Customer.Create` 의 집합체 결합을 `Lift3` 에서 `Lift4` 로 늘려 `PaymentInfo.Create(..)` 를 한 인자로 더 받습니다. 잎 → 복합 → 집합체 세 계층에서 잘못된 잎을 하나씩 박았을 때 오류가 정확히 그 개수만큼 한 목록에 모이는지 예측하고 확인합니다. 복합 값 객체를 늘려도 집합체 결합은 `Lift` 의 인자 한 칸만 늘 뿐임을 확인합니다. 정답 코드는 `Challenges/` 에 있습니다. 위 `Order` 검증은 같은 `Lift` 누적이 계층 없는 평평한 도메인에도 그대로 옮겨감을 보이는 비교용으로 남깁니다.

> **더 깊이 (처음엔 건너뛰어도 됩니다)** — 왜 applicative 법칙을 형식적 property 로 검증하지 않을까요.
>
> 2부에서 `MyValidation` 의 `Apply` 가 applicative 법칙 (항등 · 합성 · 준동형) 을 만족함을 property 로 확인했습니다. 이 장의 `Validation` 도 같은 applicative 누적 구조이고 같은 `Apply` 를 쓰니 같은 법칙을 만족하는데, 이 장의 네 검사는 그 대수 법칙을 형식적 property (`ForAll`) 로 굴리지 않고 구체적인 `is` 패턴과 개수 확인으로 대신합니다. 까닭은 이 장이 다지려는 것이 추상의 대수 구조가 아니라 강타입 계층 도메인 + 계층-교차 누적이 한 코드로 합성된 동작이기 때문입니다. 특히 둘째 검사 (`Errors.Count: 3`) 는 어떤 applicative 법칙으로도 직접 표현되지 않는, "monadic 단락이 아니라 모든 계층에서 누적" 이라는 이 합성에만 있는 성질입니다. 12부는 새 추상을 증명하는 자리가 아니라 익힌 추상을 합성하는 자리라, 검사도 대수 법칙이 아니라 합성된 동작을 단언합니다. 입문 단계에서는 "이 장의 핵심은 법칙이 아니라 누적이 계층 도메인과 제대로 합쳐졌는가" 라고만 알아 두면 충분합니다.

---

## 39.7 더 깊이 — 학습용 Validation 은 v5 의 단순화판입니다

> **더 깊이 (처음엔 건너뛰어도 됩니다)** — 이 절은 학습용 `Validation<E, A>` 와 LanguageExt v5 의 실제 `Validation<F, A>` 사이의 거리를 정직하게 짚는 자리입니다. 처음 읽을 때는 건너뛰어도 이 장의 발상을 이해하는 데 지장이 없습니다.

학습용 `Validation<E, A>` 는 누적 검증의 뼈대만 남긴 것이고, LanguageExt v5 의 실제 `Validation<F, A>` 는 같은 발상에 여러 겹을 더 입혔습니다. 정직하게 짚어 둡니다.

**계층 누적은 v5 의 실전 패턴입니다.** 먼저 이 장의 계층-교차 누적이 학습용 단순화가 아니라 v5 의 실제 어법임을 짚습니다. LanguageExt v5 `CreditCardValidation` 샘플의 `CreditCard.Validate` 는 `fun(CreditCardDetails.Make).Map(ValidateCardNumber(..)).Apply(ValidateExpiryDate(..)).Apply(ValidateCVV(..))` 로 복합 값 객체 `CreditCardDetails(CardNumber, Expiry, CVV)` 를 조립합니다. 이 장의 `ContactInfo` · `Address` 조립과 같은 형태입니다. 게다가 `ValidateCardNumber` 안에서 다시 `(ValidateAllDigits, ValidateLength).Apply(..)` 로 잎 수준 누적을 하므로, v5 도 검증된 값 객체가 또 검증을 품는 계층 누적을 그대로 씁니다. 이 장이 보인 것은 그 실전 패턴의 가장 단순한 골격입니다.

**오류 타입과 누적 메커니즘.** 학습용은 오류 타입 `E` 를 자유롭게 두고 (`string` 을 씀), `IReadOnlyList<E>` 에 직접 담아 `[.. a, .. b]` 의 목록 이어 붙임으로 누적합니다. v5 의 `Validation<F, A>` 는 오류 타입 `F` 에 `Monoid<F>` 제약을 강제합니다. 곧 오류 타입이 합쳐질 수 있어야 한다 (`Combine` 과 항등원을 가져야 한다) 는 조건입니다. v5 의 표준 `F` 는 `Error` 또는 `Seq<Error>` 입니다. 여기서 1부와 이어지는 다리가 하나 보입니다. 학습용의 목록 이어 붙임 (빈 목록 = 항등원, concat = 결합) 이 사실 1부에서 배운 Monoid 의 실전 적용입니다. v5 가 `Monoid<F>` 제약을 둔 까닭이 바로 이것입니다. 오류를 누적하려면 오류 타입이 합쳐질 수 있어야 (Monoid 여야) 하기 때문입니다. 학습용은 그 Monoid 를 목록으로 고정해 제약 없이 단순화한 것입니다.

**누적의 위임.** 학습용 `Apply` 의 `[.. Errors(vf), .. Errors(va)]` 는 목록을 직접 이어 붙입니다. v5 의 `Apply` 는 오류 타입 `F` 의 `Combine` 에 누적을 위임합니다. `F` 가 `Seq<string>` 이면 결국 목록 이어 붙임과 같은 결과지만, 추상화 계층이 한 겹 더 있습니다. 학습용이 누적을 손으로 적은 자리를, v5 는 Monoid 의 `Combine` 호출로 대신합니다.

**결합기의 형태.** 학습용도 v5 처럼 `Apply` 체인 (`.Map(...).Apply(...).Apply(...)`) 으로 임의 arity 를 처리합니다. 다만 학습용은 `Apply` 를 `Validation` 전용 확장 메서드로 직접 둔 반면, v5 는 trait 기반 `Applicative.Apply` 라 모든 Elevated 타입이 같은 `Apply` 를 공유하고, 거기에 튜플 확장 `(va, vb, vc).Apply((a, b, c) => ...)`, 누적을 위한 `&` 연산자, 고르기를 위한 `|` 연산자까지 더합니다. 학습용에는 `K<F, A>` trait 도, 튜플 `Apply` 도, 연산자도 없이 한 타입의 `Apply` 하나만 둡니다.

**monadic 단락의 부재.** 학습용은 `Bind` 를 아예 정의하지 않아 누적 한쪽만 보입니다. v5 는 누적 (`Apply`) 과 단락 (`Bind` · `SelectMany`) 을 둘 다 제공합니다. 검증이 서로 의존할 때 (예: 시작일이 유효해야 종료일을 시작일과 비교) 는 v5 에서 `from .. in` 으로 monadic 폴백을 씁니다. 학습용은 독립 검증의 누적만 보여 단순화했습니다.

**타입 구조.** v5 `Validation<F, A>` 의 두 케이스 이름은 `Success` · `Fail` 이고, `Fail` 은 단일 `F` 값 하나를 보관합니다 (`F` 가 Monoid 라 내부적으로 결합). 학습용의 케이스 이름은 `Valid` · `Invalid` 이고, `Invalid` 는 `IReadOnlyList<E>` 를 직접 보관합니다. 이름과 내부 표현이 다르지만, "성공 아니면 누적된 실패" 라는 골격은 같습니다.

![학습용 Validation 과 v5 Validation 비교](./images/Ch39-Domain-Modeling/03-validation-v5-bridge.svg)

**그림 39-3. 학습용 Validation 과 v5 Validation: 같은 뼈대, v5 가 더 입힘** — 실패 케이스 · 누적 · n-항 결합 · 단락 네 면에서 둘을 나란히 둡니다. 학습용의 목록 이어붙임이 곧 1부 Monoid 의 실전이고, v5 는 그 위에 `Monoid<F>` 제약 · `&` (누적) / `|` (고르기) 연산자 · monadic 폴백을 더했습니다.

입문 단계에서는 이 거리를 외울 필요가 없습니다. "검증을 값으로 표현하고, 여러 검증을 동시에 굴려 오류를 모은다" 는 뼈대는 학습용과 v5 가 같고, 나머지는 그 뼈대에 Monoid 제약 · trait · 튜플 확장 · monadic 폴백을 더한 것이라고만 알아 두면 충분합니다.

---

## 39.8 Elevated World 어휘로 다시 읽기

이 절은 이 장의 도구를 1부 비유에 맞춰 다시 읽는 자리입니다. 먼저 매핑부터 둡니다.

| 39장 도구 | Elevated World 어휘 |
|---|---|
| 원시 입력 (`string`) | Normal World 의 평범한 값 |
| 잎 값 객체 (`Username` · `Email` · `PhoneNumber`) | 검증을 통과한 Elevated World 의 시민 |
| 복합 값 객체 (`ContactInfo` · `Address`) | 검증된 시민을 묶어 다시 끌어올린 한 계층 위의 시민 |
| 스마트 생성자 `Create` | Normal 원시값을 검증된 Elevated 도메인으로 끌어올림 (실패 가능) |
| `Validation<E, A>` | 실패를 값으로 인코딩한 Elevated 컨테이너 |
| `Apply` 체인 | 같은 계층의 Elevated 시민을 한 칸씩 결합 (실패는 모두 모음) |
| 계층-교차 누적 | 하위 계층에서 모인 실패가 상위 `Apply` 체인으로 합류 |
| `Customer` 값이 존재함 | 모든 계층의 끌어올림이 성공했다는 컴파일 타임 보증 |

1부에서 함수형의 본질을 한 문장으로 적었습니다. 모든 값과 함수를 합성 가능한 Elevated World 로 끌어올리는 것. 이 장은 그 끌어올림이 실무 입력 검증에 닿되, 한 계층이 아니라 계층 전체로 닿는 자리입니다. 원시 `string` "alice" 는 Normal World 의 평범한 값이고, `Username.Create("alice")` 는 그것을 검증된 `Username` 으로 끌어올리는 것입니다. 다만 이 끌어올림은 실패할 수 있어, 결과가 `Username` 이 아니라 `Validation<string, Username>` 입니다. 곧 끌어올림의 성공 여부를 값으로 담은 Elevated 시민입니다.

여기서 1부의 끌어올림으로 이 장의 자리를 정확히 짚습니다. `Create` 가 원시값을 검증된 잎으로 끌어올리고, 복합 값 객체의 `Apply` 체인이 잎들을 한 칸씩 결합해 한 계층 위의 시민으로 다시 끌어올리며, 집합체의 `Apply` 체인이 잎과 복합 값 객체를 결합해 최상위 시민을 냅니다. 결정적인 점은 이 결합이 실패를 모으되 계층을 가로질러 모은다는 것입니다. 하위 복합 값 객체 안에서 모인 실패가 상위 `Apply` 체인으로 합류해, 서로 다른 계층의 오류가 한 목록에 모입니다. 그리고 모든 계층의 끌어올림이 성공해야만 `Customer` 라는 검증된 시민이 나옵니다. 곧 `Customer` 값이 손에 있다는 것은 모든 계층의 끌어올림이 성공했다는 뜻이고, 이것이 잘못된 상태를 표현 불가능하게 만든다는 설계의 정체입니다. 잘못된 값으로는 끌어올림이 실패해 `Customer` 가 아예 만들어지지 않으니까요.

이 합성이 12부 축의 핵심입니다. 명령형은 검증과 타입을 뗄 수 없어, 원시 타입을 들고 다니며 잘못된 값에 어디서든 노출됐습니다. 함수형은 검증의 결과를 타입에 새겨 (강타입 계층 도메인), 검증을 통과한 값만 도메인 시민으로 존재하게 합니다. 그리고 그 검증을 3부의 `Validation` 누적으로 합성하되, `Apply` 가 한 계층이 아니라 계층 트리 전체에서 실패를 모읍니다. 새 추상 없이, 앞서 만든 끌어올림과 누적이 계층 도메인이라는 한 자리에서 합쳐지는 것이 이 장이 보인 전부입니다.

비유는 여기까지가 역할입니다. `Validation` 이 정확히 어떻게 오류를 모으는지는 `Apply` 의 `[.. Errors(vf), .. Errors(va)]` 와 그 시그니처가 정합니다. 비유가 머리에 그림을 그려 주는 동안 시그니처가 진실을 정합니다.

---

## 39.9 Q&A — 자기 점검

> **Q1. 원시 `string` 을 그대로 들고 다니면 무엇이 문제입니까?** (39.2절)

잘못된 값이 어디서든 샙니다. `Customer` 의 필드가 원시 타입이면 `new Customer("", "엉터리", "abc", "", "", "x")` 도 멀쩡히 만들어져, 빈 이름 · `@` 없는 이메일 · 자릿수가 모자란 우편번호가 아무 저항 없이 들어갑니다. 이 `Customer` 를 받는 코드는 그 값이 검증됐는지 알 수 없어 쓰는 자리마다 다시 검사하거나, 검사를 빼고 잘못된 값에 노출됩니다. 검증이 한 입구에 모이지 않고 코드 전체로 흩어집니다. 이렇게 도메인 개념을 원시 타입 그대로 들고 다니는 것을 primitive obsession (원시 타입 남용) 이라 부릅니다. 게다가 평평한 `string` 나열은 이메일·전화번호가 한 묶음 (연락처) 이라는 도메인 구조도 가리지 못합니다. 이 장의 강타입 계층 도메인은 검증 결과를 타입에 새겨, `Username` 값이 있다는 것 자체를 검증의 보증으로 만들고, 관련 잎을 복합 값 객체로 묶어 구조까지 타입에 드러냅니다.

> **Q2. 스마트 생성자는 어떻게 잘못된 상태를 표현 불가능하게 만듭니까?** (39.3절)

생성자를 `private` 으로 닫고, 유일한 입구 `Create` 가 불변식을 통과한 경우에만 값을 내주기 때문입니다. `Username` 의 생성자가 `private` 이라 외부에서 `new Username(...)` 을 부를 수 없고, `Create("alice")` 만이 검증 (3자 이상) 을 통과해야 `Username` 을 만들어 줍니다. 그래서 `Username` 값이 손에 있다는 것은 곧 그 값이 검증을 통과했다는 뜻입니다. 같은 잠금이 복합 값 객체에도 적용돼, `ContactInfo` 값이 있다는 것은 그 안의 모든 잎이 검증을 통과했다는 뜻이고, `Customer` 값이 있다는 것은 모든 계층이 검증을 통과했다는 뜻입니다. 잘못된 값으로는 어느 계층도 만들 길이 아예 없습니다. 출입증 비유로는 잠긴 옆문 (`private` 생성자) 때문에 창구 (`Create`) 를 통과한 사람만 출입증 (도메인 타입) 을 들고 있는 셈입니다.

> **Q3. 스마트 생성자는 왜 예외가 아니라 `Validation` 을 반환합니까?** (39.3절)

실패를 시그니처에 드러내고 다른 검증과 합성하기 위해서입니다. 예외로 검증하면 (`throw`), `Username Create(string)` 시그니처만 봐서는 실패 가능성이 보이지 않아 본문을 읽어야 압니다. 게다가 예외는 한 번에 하나만 던져 오류 누적과 합쳐지지 않습니다. 반면 `Validation<string, Username> Create(string)` 은 실패 가능성이 반환 타입에 드러나고 (3부에서 본 효과를 값으로), `Apply` 체인으로 다른 검증과 합성됩니다. 실패를 던지는 사건이 아니라 다루는 값으로 바꾸는 것이 함수형 검증의 출발점입니다.

> **Q4. `Customer.Create` 는 왜 커리한 생성자를 손으로 적지 않고 `Lift3` 를 씁니까?** (39.4절)

`Lift3` 가 `Curry → Pure → Apply` 사슬을 안에 캡슐화하기 때문입니다. 2부에서 만든 `Lift3` 는 평범한 3인자 생성자 `(n, c, a) => new Customer(n, c, a)` 를 받아, 안에서 한 번만 커리한 뒤 `Pure` 로 한 칸 끌어올리고 `.Apply(Username.Create(..))` · `.Apply(ContactInfo.Create(..))` · `.Apply(Address.Create(..))` 로 세 인자를 한 칸씩 채웁니다. 인자가 N 개면 `Pure` 한 번에 `Apply` 가 N 번인 그 구조가 `Lift3` 안에 그대로 있습니다. 호출부는 커리한 `n => c => a => ...` 를 매번 손으로 적지 않고 평범한 다인자 생성자만 넘기면 됩니다. 복합 값 객체를 넷 이상으로 늘리면 `Lift4` 처럼 인자 한 칸을 더 받는 `Lift` 를 쓸 뿐입니다. 이는 3부 Ch08 의 `Pure(커리한 생성자).Apply(..).Apply(..)` 형과 한 글자도 다르지 않고 (그 사슬을 `Lift` 가 캡슐화한 것뿐), 2부 applicative 가 이항 `Apply` 하나로 모든 arity 를 처리한 그 구조와 같습니다.

> **Q5. 서로 다른 계층의 오류가 어떻게 한 목록에 모입니까?** (39.4절, 39.5절)

하위 복합 값 객체가 자기 계층에서 먼저 누적한 오류를, 상위 `Lift3` 의 `Apply` 체인이 그대로 합류시키기 때문입니다. `Customer.Create("ab", "no-at", "010-…", "1 Main St", "Seoul", "bad")` 에서 잘못된 곳은 세 계층에 박혀 있습니다. `Username.Create("ab")` 는 직속 잎이라 오류 1개를 내고, `ContactInfo.Create("no-at", …)` 는 자체 `Lift2` 안에서 email 오류 1개를 먼저 모으며, `Address.Create(…, "bad")` 는 자체 `Lift3` 안에서 postal 오류 1개를 먼저 모읍니다. 집합체 `Customer.Create` 의 `Lift3` 는 이 세 결과를 한 칸씩 받으며 `[]`→`[username]`→`[username, email]`→`[username, email, postal]` 로 이어 붙여, 서로 다른 세 계층의 오류를 한 `Invalid(3개)` 목록에 모읍니다. `Apply` 가 한 계층이 아니라 모든 계층에서 합성·누적한다는 뜻입니다.

> **Q6. applicative 누적은 monadic 단락과 왜 다릅니까?** (39.4절)

검증이 서로 독립인가 의존인가에 따라 갈립니다. 한 계층 안의 검증들 (`Email` 과 `PhoneNumber`, 또는 `Username` 과 `ContactInfo` 와 `Address`) 은 서로 독립입니다. 하나가 다른 하나의 결과를 필요로 하지 않아, 모두 동시에 굴려 모든 오류를 모을 수 있습니다 (applicative 누적). 반대로 둘째 검증이 첫째 검증의 결과를 입력으로 받아야 한다면 (의존), 첫째가 실패한 순간 둘째를 굴릴 입력이 없어 멈출 수밖에 없습니다 (monadic 단락). 그래서 이 장의 `Validation` 은 `Bind` 를 아예 정의하지 않고 누적만 보였습니다. "독립이라 모은다, 의존이라 멈춘다", 이 한 줄이 둘을 가릅니다.

> **Q7. `Customer` 값이 손에 있다는 것이 왜 검증의 보증입니까?** (39.5절, 39.8절)

`Customer` 를 만들 유일한 길이 모든 계층의 `Apply` 체인이 `Valid` 를 만나는 경우뿐이기 때문입니다. `Customer` 의 세 필드가 `Username` · `ContactInfo` · `Address` 인데, 이 셋은 각자의 `Create` 를 통과해야만 존재하고, 두 복합 값 객체는 다시 자기 잎들이 모두 `Create` 를 통과해야만 존재합니다. 그래서 `Customer` 값이 손에 있다는 것은 6개 잎 불변식과 2개 복합 불변식이 모두 만족됐다는 컴파일 타임 보증입니다. 이후 어떤 코드도 이 `Customer` 의 이름이 빈 문자열인지, 이메일에 `@` 가 있는지 다시 검사할 필요가 없습니다. 검증을 한 번 한 결과가 타입에 영원히 남아, 재검증이 사라집니다.

> **Q8. 일부만 틀리면 유효한 자리도 오류로 잡힙니까?** (39.5절)

아닙니다. 실패한 자리의 오류만 모입니다. 이메일만 틀린 입력에서 `Username` 과 `Address` 는 `Valid` 라 오류 목록이 빈 목록 (`[]`) 이고, `ContactInfo` 만 email 오류로 `Invalid` 입니다. `Apply` 의 이어 붙임 `[.. Errors(vf), .. Errors(va)]` 에서 유효한 검증의 빈 목록은 이어 붙여도 아무것도 더하지 않으므로, 결과는 이메일 오류 하나뿐입니다 (`Invalid(1개)`). 모든 오류를 모으되, 유효한 것까지 오류로 잡지는 않습니다.

> **Q9. 이 장의 `Validation` 은 LanguageExt v5 와 무엇이 다릅니까?** (39.7절)

뼈대도 결합 어법도 같고, 차이는 더 좁은 자리로 옮겨졌습니다. 이 장의 학습용도 v5 처럼 `Pure`→`Apply` 체인 (`Pure(생성자).Apply(..).Apply(..)`) 으로 검증을 결합하므로, 결합기의 모양은 v5 와 다르지 않습니다. 차이는 네 자리에 있습니다. 첫째, 누적의 바탕입니다. 학습용은 오류를 `IReadOnlyList<E>` 에 담아 목록을 직접 이어 붙이지만, v5 `Validation<F, A>` 는 오류 타입 `F` 에 `Monoid<F>` 제약을 두고 `Combine` 에 누적을 위임합니다 (표준 `F` 는 `Error` 또는 `Seq<Error>`). 둘째, 추상의 폭입니다. 학습용 `Apply` 는 `Validation` 전용이지만, v5 는 trait 기반 `Applicative.Apply` 라 모든 Elevated 타입이 같은 `Apply` 를 공유하고, 튜플 확장 `(va, vb, vc).Apply(...)` 과 누적의 `&` · 고르기의 `|` 연산자까지 더합니다. 셋째, 단락의 유무입니다. 학습용은 `Bind` 가 없어 누적만 보이지만 v5 는 누적 (`Apply`) 과 단락 (`Bind`) 을 둘 다 제공합니다. 넷째, 케이스 이름입니다. 학습용은 `Valid` · `Invalid`, v5 는 `Success` · `Fail` 입니다. 곧 "검증을 값으로 만들고 `Apply` 체인으로 실패를 모은다" 는 핵심은 학습용과 v5 가 같고, 차이는 누적을 Monoid 의 `Combine` 에 위임하는가, trait 기반 `Apply` 와 `&` · `|` 연산자와 `Bind` 를 더하는가, 케이스 이름을 무엇으로 두는가로 좁혀집니다.

---

## 39.10 요약

- **이 장은 강타입 계층 도메인과 3부 `Validation` 의 applicative 누적을 한 코드로 합성합니다.** 새 추상이 아니라, 따로 익힌 두 도구가 계층 도메인 모델링이라는 자리에서 합쳐지는 것을 보는 12부의 첫 장입니다. 3부 Ch08 의 평평한 다인자 검증을 계층으로 끌어올립니다 (39.1절).
- **원시 타입을 그대로 들고 다니면 잘못된 값이 어디서든 샙니다.** 평평한 `string` 필드는 검증을 우회한 값과 검증된 값을 구분하지 못하고, 관련 값이 한 묶음이라는 도메인 구조도 가리지 못합니다. `if` 로 순차 검증하면 첫 오류에서 멈춰 사용자가 오류를 하나씩만 봅니다 (39.2절).
- **스마트 생성자가 잎을 잎 값 객체로, 묶음을 복합 값 객체로 끌어올립니다.** 생성자를 `private` 으로 닫고 `Create` 가 불변식을 통과한 경우에만 값을 내주면, 도메인 타입 값이 손에 있다는 것 자체가 검증의 보증이 됩니다. 복합 값 객체 (`ContactInfo` · `Address`) 는 자기 잎들을 자체 `Pure`→`Apply` 체인으로 검증·조립합니다 (39.3절).
- **`Apply` 가 한 계층이 아니라 모든 계층에서 누적합니다.** 집합체 `Customer` 는 `Pure` 로 끌어올린 커리한 생성자에 잎 하나와 복합 값 객체 둘을 `Apply` 로 채웁니다. 하위 복합 값 객체가 자기 계층에서 먼저 누적한 오류가 상위 `Apply` 체인으로 그대로 합류해, 서로 다른 계층의 오류가 한 목록에 모입니다. 검증이 서로 독립이라 누적이 가능합니다 (의존이면 monadic 단락) (39.4절).
- **`Customer.Create` 한 입구가 두 불편을 동시에 풉니다.** 원시 입력을 검증된 계층 `Customer` 로 끌어올리고, 잘못된 값은 `Customer` 경계를 못 넘으며, 사용자는 어느 계층의 오류든 한 번에 봅니다. 같은 `Lift3` 누적이 계층 없는 평평한 주문 검증에도 그대로 옮겨 갑니다 (39.5절).
- **네 검사가 합성된 동작을 다집니다.** 유효 입력의 도달 · 계층-교차 누적 (개수 3) · 부분 오류 (개수 1) · 복합 안 누적 (개수 2) 가, 3부의 `Validation` 누적이 강타입 계층 도메인과 약속대로 합성됐는지를 코드로 단언합니다. `Username` 값이 있음은 검증을 통과한 Elevated 시민이고, `Customer` 가 존재함은 모든 계층의 끌어올림이 성공했다는 보증입니다 (39.6절, 39.8절).

---

## 39.11 다음 장으로 — 검증된 값으로 부수 효과를 일으키되 테스트 가능하게

이 장에서 12부의 첫 합성을 만났습니다. 강타입 계층 도메인 (잎 값 객체 → 복합 값 객체 → 집합체) 으로 잘못된 상태를 표현 불가능하게 만들고, 3부의 `Validation` applicative 누적으로 원시 입력을 검증된 계층 도메인 타입으로 끌어올리는 `Customer.Create` 파이프라인을 짰습니다. 모든 계층이 유효하면 `Customer` 가 만들어지고, 틀리면 어느 계층의 오류든 한 번에 모였습니다. 잘못된 값이 도메인 경계를 못 넘는 입구를 만든 것입니다.

그런데 검증된 `Customer` 를 만든 다음에는 무엇을 할까요. 실무에서는 그 `Customer` 를 콘솔에 출력하거나, 데이터베이스에 저장하거나, 환영 메일을 보냅니다. 모두 부수 효과입니다. 이 장의 `Customer.Create` 는 순수한 검증이라 부수 효과가 없었지만, 검증된 값을 받아 실제 일을 하려면 부수 효과를 일으켜야 합니다. 그런데 부수 효과를 그냥 일으키면 (`Console.WriteLine` · 데이터베이스 호출을 코드에 직접 박으면), 그 코드는 테스트할 수 없습니다. 테스트가 진짜 콘솔에 출력하고 진짜 데이터베이스에 쓰게 되니까요.

다음 장은 이 자리를 풉니다. 7부에서 만든 `Eff<RT>` 와 `Has<RT, Trait>` DI, 그리고 11부에서 만든 테스트 더블을 합성해, 부수 효과를 앱 경계로 격리한 애플리케이션을 짭니다. 콘솔 · 파일 · 시간 같은 효과를 능력 (`Has`) 으로 추상화해 런타임에 주입하면, 실제 앱에서는 진짜 구현을, 테스트에서는 가짜 구현을 끼울 수 있습니다. 7부의 런타임 주입과 11부의 테스트 더블이 같은 설계의 양면임을 실제 앱에서 확인합니다. 이 장에서 만든 검증된 도메인 타입이 그 앱의 입구가 됩니다. 다음 장은 [40장 효과 기반 애플리케이션](./Ch40-Effectful-Application.md) 입니다.
