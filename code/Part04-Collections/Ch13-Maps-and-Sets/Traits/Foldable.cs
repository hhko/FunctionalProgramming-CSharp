namespace Ch13.Traits;

// Foldable — 5장 정의 그대로. Map / Set 에서는 *값* 들을 한 값으로 끌어내린다.
//
// 예: 가격 맵의 합계, 집합의 원소 개수. 키는 접지 않고 값만 접는다.
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
}
