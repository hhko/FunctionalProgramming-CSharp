namespace Ch09.Tests;

// 최소 property 검증 — 법칙은 특정 값이 아니라 *모든 입력* 에 대한 약속이므로,
// 임의 입력 count 개로 성질(prop)이 성립하는지 검사한다. 3장 §3.7.1 의 도구 그대로. (의존성 0.)
//
// 생성기를 Functor·Monad 로 키우고 실패를 *최소 반례로 축소(shrinking)* 하는 본격
// 도구, 그리고 효과 코드의 테스트는 11부에서 다룬다. 여기서는 그 씨앗만.
public static class Property
{
    // gen 으로 만든 임의 입력 count 개에 대해 prop 가 모두 참이면 true.
    // seed 고정 — 같은 실행은 같은 입력 (재현 가능).
    public static bool ForAll<A>(Func<Random, A> gen, Func<A, bool> prop, int count = 100, int seed = 42)
    {
        var rng = new Random(seed);
        for (var i = 0; i < count; i++)
            if (!prop(gen(rng)))      // 한 입력이라도 깨지면 실패
                return false;
        return true;
    }
}
