using Ch13.Types;

namespace Ch13.Challenges;

// 챌린지 ② (심화) — "키를 바꾸는 것" 은 왜 Functor Map 이 아닌가.
//
// Functor 의 Map 은 값 V 만 바꾸고 키는 보존했다. 그렇다면 *키* 를 바꾸는 연산은?
// 그건 Functor 의 자리가 아니다 — 키 변환은 (1) 충돌 가능 (서로 다른 키가 같은 키로),
// (2) 재정렬 필요. 그래서 별도 함수로 둔다. 여기서 충돌이 실제로 일어나는 것을 시연한다.
public static class KeyMapping
{
    // 키를 변환하는 자유 함수 (Functor Map 이 아님 — 이름도 일부러 다르게).
    // 충돌하면 *나중 값이 이긴다* (정책을 명시해야 한다는 점이 핵심).
    public static MyMap<KeyB, V> MapKeys<KeyA, KeyB, V>(
        MyMap<KeyA, V> map,
        Func<KeyA, KeyB> keyF)
        where KeyA : notnull
        where KeyB : notnull
    {
        var dict = new Dictionary<KeyB, V>();
        foreach (var p in map.Pairs)
            dict[keyF(p.Key)] = p.Value;   // 충돌 시 덮어씀 (정책)
        return new MyMap<KeyB, V>(dict);
    }
}
