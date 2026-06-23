namespace Ch07.Tests;

// 최소 property 검증 — 법칙은 특정 값이 아니라 *모든 입력* 에 대한 약속이므로,
// 임의 입력 count 개로 성질(prop)이 성립하는지 검사한다. (의존성 0.)
//
// 3 장 3.10.6절에서 처음 만든 도구를 이 장 세 법칙 검증에 그대로 재사용한다.
// 생성기를 Functor·Monad 로 키우고 실패를 *최소 반례로 축소(shrinking)* 하는 본격
// 도구, 그리고 실무 도구 (CsCheck / FsCheck) 로의 이행은 11 부에서 다룬다.
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
