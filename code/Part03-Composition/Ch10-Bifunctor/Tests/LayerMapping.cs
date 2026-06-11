using Ch10.Functions;
using Ch10.Traits;
using Ch10.Types;

namespace Ch10.Tests;

// 본문 §10.4.1 실전 — 계층 사이 오류 매핑.
//
// 인프라 계층은 Either<DbError, Row> 를 돌려주고, 응용 계층은 Either<DomainError, Dto> 를 원한다.
// 오류는 DbError → DomainError, 성공은 Row → Dto. 두 변환이 한 번에 필요한 자리를 BiMap 한 줄로 닫는다.
// §10.1.1 의 손분해(switch 로 Left/Right 풀어 재조립)와 같은 일을, 분해·재조립 없이 두 함수만으로.
public sealed record DbError(string Code);
public sealed record Row(int Id, string Name);
public sealed record DomainError(string Message);
public sealed record Dto(int Id, string Name);

public static class LayerMapping
{
    // 오류 갈래는 도메인 오류로, 성공 갈래는 Dto 로 — 한 줄에 양쪽 변환.
    public static K<EitherF, DomainError, Dto> ToDomain(K<EitherF, DbError, Row> result) =>
        result.BiMap(
            db  => new DomainError($"조회 실패: {db.Code}"),   // 오류 갈래 변환 (MapFirst 자리)
            row => new Dto(row.Id, row.Name));                 // 성공 갈래 변환 (MapSecond 자리)
}
