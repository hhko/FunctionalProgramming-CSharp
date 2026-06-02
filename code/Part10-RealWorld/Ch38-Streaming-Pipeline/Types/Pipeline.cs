namespace Ch38.Types;

// 원자적 누산기 (7부 Atom) — 동시 집계를 락 없이 안전하게.
public sealed class AtomicLong
{
    long value;
    public long Value => Interlocked.Read(ref value);
    public void Add(long delta) => Interlocked.Add(ref value, delta);
}

// 실전 데이터 파이프라인 — 6부 retry(Schedule) + bracket(Resource) + 7부 Atom(동시 집계) +
// 8부 스트리밍(lazy 변환) 을 한 자리에서 결합한다.
public static class Pipeline
{
    public sealed record Report(long Sum, int Valid, int ConnectAttempts, List<string> Events);

    public static Report Run(IReadOnlyList<string> rawRecords, int maxRetries, int flakyUntilAttempt)
    {
        var events = new List<string>();

        // ① 재시도(Schedule) — 불안정한 연결을 최대 maxRetries 회 재시도.
        var attempts = 0;
        bool Connect() { attempts++; return attempts >= flakyUntilAttempt; }
        var connected = Connect();
        for (var i = 0; i < maxRetries && !connected; i++) connected = Connect();
        if (!connected) throw new InvalidOperationException("연결 실패");

        // ② 자원(bracket) — 소스를 열고 반드시 닫는다 (예외에도).
        events.Add("open source");
        try
        {
            // ③ 스트리밍 변환(lazy) — 파싱 성공만 추림.
            var parsed = rawRecords
                .Select(r => (ok: int.TryParse(r, out var v), value: v))
                .Where(x => x.ok)
                .Select(x => x.value)
                .ToList();

            // ④ 동시 집계(Atom) — 여러 스레드가 락 없이 합산.
            var sum = new AtomicLong();
            Parallel.ForEach(parsed, v => sum.Add(v));

            return new Report(sum.Value, parsed.Count, attempts, events);
        }
        finally
        {
            events.Add("close source");
        }
    }
}
