namespace Ch37.Types;

// Atom<T> — *원자적 참조*. 갱신은 순수 함수 `T → T` 로 표현하고, 충돌 시 CAS 루프로 자동 재시도합니다.
// (LanguageExt v5 Atom 의 축소판. Interlocked.CompareExchange 로 락 없이 안전합니다.)
//
// 테스트 관점 — 동시성 코드의 결과는 보통 *비결정적* 입니다. 하지만 갱신 함수가 *순수* 하면
// "몇 번을 동시에 증가해도 최종값은 증가 횟수와 같다" 라는 *결정적 불변식* 이 성립합니다.
// 이 불변식이 동시성 toy 를 결정적으로 단언할 수 있게 하는 핵심입니다.
public sealed class Atom<T> where T : class
{
    T value;

    public Atom(T initial) => value = initial;

    // 현재 값 — 다른 스레드의 갱신이 보이도록 Volatile 로 읽습니다.
    public T Value => Volatile.Read(ref value);

    // Swap — 현재 값을 읽고 f 적용 후 CAS. 그 사이 다른 스레드가 바꿨으면 fresh 값으로 재시도합니다.
    // 갱신 횟수(재시도 포함이 아니라 *성공* 횟수)는 곧 적용된 f 의 횟수와 같습니다.
    public T Swap(Func<T, T> f)
    {
        while (true)
        {
            var current = Volatile.Read(ref value);
            var next = f(current);
            if (Interlocked.CompareExchange(ref value, next, current) == current)
                return next;          // CAS 성공 — 그 사이 아무도 안 바꿈
            // CAS 실패 → 누군가 먼저 바꿨다. 루프가 fresh current 로 f 를 다시 적용.
        }
    }
}

// 불변 카운터 — Atom 에 담을 값. 갱신은 새 인스턴스 생성(with)으로, 순수성을 지킵니다.
public sealed record Counter(long N);
