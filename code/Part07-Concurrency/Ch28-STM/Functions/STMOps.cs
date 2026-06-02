using Ch28.Traits;
using Ch28.Types;

namespace Ch28.Functions;

public static class MonadExtensions
{
    public static K<M, B> Select<M, A, B>(this K<M, A> ma, Func<A, B> f)
        where M : Monad<M> => M.MapDefault(f, ma);

    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M> => M.SelectMany(ma, bind, project);
}

public static class STMOps
{
    static readonly object gate = new();

    // 참조 읽기 — 자기 트랜잭션의 쓰기를 먼저 보고, 아니면 커밋된 값을 읽으며 버전을 기록.
    public static K<STMF, A> Read<A>(Ref<A> r) =>
        new STM<A>(t =>
        {
            if (t.Writes.TryGetValue(r, out var w)) return (A)w!;
            t.Reads[r] = r.Version;     // 검증용 스냅샷
            return r.Current;
        });

    // 참조 쓰기 — 로그에만 기록 (커밋 전엔 실제 참조 불변).
    public static K<STMF, Unit> Write<A>(Ref<A> r, A value) =>
        new STM<Unit>(t => { t.Writes[r] = value; return Unit.Default; });

    // atomically — 낙관적 트랜잭션. 본문 실행 → 글로벌 락에서 읽은 버전 검증 → 통과면 일괄 적용,
    // 충돌이면 *전체 재시도* (all-or-nothing). 본문 중 예외가 나면 쓰기가 적용되지 않는다.
    public static A Atomically<A>(K<STMF, A> stm)
    {
        while (true)
        {
            var txn = new Txn();
            var result = stm.As().Run(txn);
            lock (gate)
            {
                var valid = txn.Reads.All(kv => kv.Key.Version == kv.Value);
                if (valid)
                {
                    foreach (var (r, v) in txn.Writes) { r.Apply(v); r.Version++; }
                    return result;
                }
            }
            // 검증 실패 → 다른 트랜잭션이 먼저 커밋. 처음부터 다시.
        }
    }
}
