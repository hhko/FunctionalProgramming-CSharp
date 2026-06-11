using Ch07.Traits;
using Ch07.Types;

namespace Ch07.Functions;

// 본문 §7.6.3 실전 — 의존 사슬 조회 파이프라인.
//
// 각 단계가 앞 단계의 성공 값에 의존하고, 각 단계가 실패할 수 있다 (a → E<b> 유형).
// bind / LINQ 가 성공을 다음 단계로 잇고, 어느 단계든 Nothing 이면 사슬이 단락한다.
public readonly record struct UserId(int Value);
public sealed record User(UserId Id, string Name);
public sealed record Account(UserId Owner, int Balance);
public sealed record Receipt(int Amount, int Remaining)
{
    public override string ToString() => $"출금 {Amount}, 잔액 {Remaining}";
}

public static class Bank
{
    static readonly Dictionary<int, User> Users = new()
    {
        [1] = new User(new UserId(1), "김함수"),
    };

    static readonly Dictionary<int, Account> Accounts = new()
    {
        [1] = new Account(new UserId(1), 1000),
    };

    // 1단계 — 사용자 조회 (없으면 Nothing). UserId → MyMaybe<User>
    public static K<MyMaybeF, User> FindUser(UserId id) =>
        Users.TryGetValue(id.Value, out var u)
            ? MyMaybeF.Pure(u)
            : MyMaybe<User>.Nothing.Instance;

    // 2단계 — 계좌 조회 (없으면 Nothing). User → MyMaybe<Account>
    public static K<MyMaybeF, Account> FindAccount(User user) =>
        Accounts.TryGetValue(user.Id.Value, out var a)
            ? MyMaybeF.Pure(a)
            : MyMaybe<Account>.Nothing.Instance;

    // 3단계 — 출금 (잔액 부족이면 Nothing). (Account, amount) → MyMaybe<Receipt>
    public static K<MyMaybeF, Receipt> Withdraw(Account account, int amount) =>
        amount <= account.Balance
            ? MyMaybeF.Pure(new Receipt(amount, account.Balance - amount))
            : MyMaybe<Receipt>.Nothing.Instance;

    // bind 사슬 — 세 단계를 의존 결합으로 잇는다.
    public static K<MyMaybeF, Receipt> WithdrawByBind(UserId id, int amount) =>
        FindUser(id).Bind(user =>
        FindAccount(user).Bind<MyMaybeF, Account, Receipt>(account =>
        Withdraw(account, amount)));

    // LINQ 사슬 — 같은 파이프라인의 from-from-select 표기.
    public static K<MyMaybeF, Receipt> WithdrawByLinq(UserId id, int amount) =>
        from user    in FindUser(id)
        from account in FindAccount(user)
        from receipt in Withdraw(account, amount)
        select receipt;
}
