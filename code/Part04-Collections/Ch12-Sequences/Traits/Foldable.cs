namespace Ch12.Traits;

// Foldable — 5장 정의 그대로 (FoldRight + FoldLeft + virtual 자유 함수들).
//
// 실무 시퀀스에서 Sum / Count / Any / ToList 가 모두 이 두 abstract 위에서 자란다.
public interface Foldable<F> where F : Foldable<F>
{
    static abstract B FoldRight<A, B>(Func<A, B, B> f, B seed, K<F, A> fa);
    static abstract B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<F, A> fa);

    static virtual int Count<A>(K<F, A> fa) =>
        F.FoldLeft<A, int>((acc, _) => acc + 1, 0, fa);

    static virtual bool IsEmpty<A>(K<F, A> fa) =>
        F.FoldRight<A, bool>((_, _) => false, true, fa);

    static virtual bool All<A>(Func<A, bool> p, K<F, A> fa) =>
        F.FoldRight<A, bool>((a, acc) => p(a) && acc, true, fa);

    static virtual bool Any<A>(Func<A, bool> p, K<F, A> fa) =>
        F.FoldRight<A, bool>((a, acc) => p(a) || acc, false, fa);

    static virtual A? First<A>(K<F, A> fa) =>
        F.FoldRight<A, A?>((a, _) => a, default, fa);

    static virtual IEnumerable<A> ToList<A>(K<F, A> fa) =>
        F.FoldRight<A, List<A>>((a, acc) => { acc.Insert(0, a); return acc; }, new List<A>(), fa);
}
