namespace Ch30.Types;

// Atom<A> — *원자적 참조*. 갱신은 순수 함수 `A → A` 로 표현되고, 충돌 시 자동 재시도(CAS 루프).
// (LanguageExt v5 Atom 의 핵심. Interlocked.CompareExchange 로 락 없이 안전.)
//
// 핵심 — 갱신 함수가 *순수* 라서, CAS 충돌로 다시 적용해도 안전하다 (부수 작용이 있으면 깨진다).
public sealed class Atom<A>(A initial) where A : class
{
    A value = initial;

    public A Value => Volatile.Read(ref value);

    // Swap — 현재 값을 읽고 f 적용 후 CAS. 그 사이 다른 스레드가 바꿨으면 재시도.
    public A Swap(Func<A, A> f)
    {
        while (true)
        {
            var current = Volatile.Read(ref value);
            var next = f(current);
            if (Interlocked.CompareExchange(ref value, next, current) == current)
                return next;          // 성공
            // 실패 → 누군가 먼저 바꿨다. 루프가 fresh current 로 f 를 다시 적용.
        }
    }
}

// 불변 카운터 (Atom 에 담을 값). 갱신은 새 인스턴스 생성 (with).
public sealed record Counter(long N);
