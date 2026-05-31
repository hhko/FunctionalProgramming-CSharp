namespace Ch06.Traits;

// Foldable — Elevated World 의 구조를 Normal World 의 한 값으로 끌어내림.
//
// abstract 두 개 (FoldRight + FoldLeft) + virtual 다수 패턴.
// 자료 타입은 두 추상만 정의하면, 나머지 자유 함수 (Count, All, Any, First, ToList ...) 가 자동.
public interface Foldable<F> where F : Foldable<F>
{
    // 핵심 abstract 1 — 오른쪽으로 접기 (step 이 한 원소를 받고 누적이 따라온다).
    static abstract B FoldRight<A, B>(Func<A, B, B> f, B seed, K<F, A> fa);

    // 핵심 abstract 2 — 왼쪽으로 접기 (누적이 먼저, step 이 한 원소를 합친다).
    static abstract B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<F, A> fa);

    // virtual 1 — 원소 개수.
    static virtual int Count<A>(K<F, A> fa) =>
        F.FoldLeft<A, int>((acc, _) => acc + 1, 0, fa);

    // virtual 2 — 비어 있는가.
    static virtual bool IsEmpty<A>(K<F, A> fa) =>
        F.FoldRight<A, bool>((_, _) => false, true, fa);

    // virtual 3 — 모든 원소가 predicate 만족.
    static virtual bool All<A>(Func<A, bool> p, K<F, A> fa) =>
        F.FoldRight<A, bool>((a, acc) => p(a) && acc, true, fa);

    // virtual 4 — 한 원소라도 predicate 만족.
    static virtual bool Any<A>(Func<A, bool> p, K<F, A> fa) =>
        F.FoldRight<A, bool>((a, acc) => p(a) || acc, false, fa);

    // virtual 5 — 첫 원소 (없으면 default).
    static virtual A? First<A>(K<F, A> fa) =>
        F.FoldRight<A, A?>((a, _) => a, default, fa);

    // virtual 6 — 모든 원소를 시퀀스로.
    static virtual IEnumerable<A> ToList<A>(K<F, A> fa) =>
        F.FoldRight<A, List<A>>((a, acc) => { acc.Insert(0, a); return acc; }, new List<A>(), fa);
}
