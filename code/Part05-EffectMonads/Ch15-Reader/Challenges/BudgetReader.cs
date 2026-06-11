using Ch15.Functions;
using Ch15.Traits;
using Ch15.Types;

namespace Ch15.Challenges;

// 챌린지 ① 정답 — 여러 Reader 를 LINQ 로 합성해 *전역 변수 없이* 의존성을 주입.
//
// 환경(AppConfig)은 단 한 번 Run 에서 주입되고, 합성된 모든 단계가 *같은 환경* 을 공유한다.
// 명령형이라면 config 를 전역에 두거나 매 함수에 인자로 넘겼을 자리다.
public sealed record AppConfig(decimal TaxRate, string Currency);

public static class BudgetReader
{
    // 환경에서 세율을 읽어 세금 계산.
    public static K<ReaderF<AppConfig>, decimal> Tax(decimal amount) =>
        Readable.asks<ReaderF<AppConfig>, AppConfig, decimal>(c => amount * c.TaxRate);

    // 환경에서 통화 기호를 읽어 포맷.
    public static K<ReaderF<AppConfig>, string> Format(decimal amount) =>
        Readable.asks<ReaderF<AppConfig>, AppConfig, string>(c => $"{amount:0.##}{c.Currency}");

    // 두 Reader 를 LINQ 로 합성 — config 를 한 번도 명시적으로 넘기지 않는다.
    public static K<ReaderF<AppConfig>, string> TotalWithTax(decimal amount) =>
        from tax in Tax(amount)
        from line in Format(amount + tax)
        select line;
}
