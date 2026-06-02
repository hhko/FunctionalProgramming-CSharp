using Ch39.Functions;
using Ch39.Types;

namespace Ch39.Challenges;

// 종합 capstone — 주문 접수 서비스. 책 전체의 도구를 한 흐름에 모은다:
//   · 도메인 검증 (1부/36장 Validation applicative 누적)
//   · 효과 + 능력 기반 DI (5부/37장 Eff<RT> + Has)
//   · 테스트 더블로 검증 (9부)
// 각 요청을 검증해, 유효하면 저장+승인 로그, 무효면 거부 로그. 승인 건수를 돌려준다.
public static class OrderService
{
    static K<ReaderTF<RT>, int> ProcessOne<RT>(string id, decimal amount)
        where RT : Has<RT, IConsole>, Has<RT, IStore> =>
        OrderValidation.Validate(id, amount) switch
        {
            Validation<Order>.Valid v =>
                from _1 in Eff.SaveOrder<RT>(v.Value)
                from _2 in Eff.Print<RT>($"승인: {v.Value.Id} ({v.Value.Amount})")
                select 1,
            Validation<Order>.Invalid e =>
                from _ in Eff.Print<RT>($"거부: {string.Join("; ", e.Errors)}")
                select 0,
            _ => ReaderTF<RT>.Pure(0)
        };

    public static K<ReaderTF<RT>, int> ProcessAll<RT>(IEnumerable<(string Id, decimal Amount)> requests)
        where RT : Has<RT, IConsole>, Has<RT, IStore>
    {
        K<ReaderTF<RT>, int> acc = ReaderTF<RT>.Pure(0);
        foreach (var req in requests)
        {
            var captured = req;
            acc = from n in acc
                  from m in ProcessOne<RT>(captured.Id, captured.Amount)
                  select n + m;
        }
        return acc;
    }
}
