namespace Ch34.Types;

// Gen<A> — *생성기*. 무작위 입력을 만든다. (CsCheck/FsCheck 의 Gen 발상.)
public sealed class Gen<A>(Func<Random, A> sample)
{
    public A Sample(Random r) => sample(r);
    public Gen<B> Select<B>(Func<A, B> f) => new(r => f(sample(r)));
}

public static class Gen
{
    public static Gen<int> IntRange(int lo, int hi) => new(r => r.Next(lo, hi + 1));

    public static Gen<List<int>> ListOf(Gen<int> elem, int maxLen) =>
        new(r =>
        {
            var n = r.Next(0, maxLen + 1);
            var list = new List<int>(n);
            for (var i = 0; i < n; i++) list.Add(elem.Sample(r));
            return list;
        });
}

// 속성 기반 검증 — 무작위 N 케이스로 성질을 때려 보고, 실패하면 *축소(shrink)* 로 최소 반례를 찾는다.
public static class Prop
{
    public sealed record Result<A>(bool Ok, int Cases, A? Counterexample, int Shrinks);

    public static Result<A> ForAll<A>(
        Gen<A> gen,
        Func<A, bool> property,
        Func<A, IEnumerable<A>> shrink,
        int cases = 200,
        int seed = 42)
    {
        var rng = new Random(seed);
        for (var i = 0; i < cases; i++)
        {
            var x = gen.Sample(rng);
            if (!property(x))
            {
                var (minimal, steps) = Shrink(x, property, shrink);
                return new Result<A>(false, i + 1, minimal, steps);
            }
        }
        return new Result<A>(true, cases, default, 0);
    }

    // 반례를 더 작은 후보로 줄여 나간다 (여전히 실패하는 가장 작은 값).
    static (A Minimal, int Steps) Shrink<A>(A failing, Func<A, bool> property, Func<A, IEnumerable<A>> shrink)
    {
        var current = failing;
        var steps = 0;
        var reduced = true;
        while (reduced)
        {
            reduced = false;
            foreach (var candidate in shrink(current))
            {
                if (!property(candidate))     // 더 작은데도 여전히 실패 → 채택
                {
                    current = candidate;
                    steps++;
                    reduced = true;
                    break;
                }
            }
        }
        return (current, steps);
    }
}
