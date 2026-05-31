using Ch11.Functions;
using Ch11.Types;

namespace Ch11.Tests;

// 자연성 법칙 검증 — Transform 과 Functor map 이 가환.
//
//   Transform(F.Map(f, fa)) == G.Map(f, Transform(fa))
//
// (이 챕터 한정 자기 자료 타입의 map 을 직접 구현해 검증 — 4장 Functor 의 K<F,A> 와 별도.)
public static class NaturalityLaws
{
    // MyList<A> 의 map
    public static MyList<B> ListMap<A, B>(MyList<A> xs, Func<A, B> f) =>
        new(xs.Items.Select(f).ToList());

    // MyMaybe<A> 의 map
    public static MyMaybe<B> MaybeMap<A, B>(MyMaybe<A> m, Func<A, B> f) =>
        m switch
        {
            Just<A> j => MyMaybe<B>.Of(f(j.Val)),
            _ => MyMaybe<B>.Nothing
        };

    // ListToMaybe 의 자연성: Transform(ListMap(xs, f)) == MaybeMap(Transform(xs), f)
    public static bool ListToMaybeNaturality<A, B>(MyList<A> xs, Func<A, B> f)
    {
        var lhs = (MyMaybe<B>)ListToMaybe.Transform<B>(ListMap(xs, f));
        var rhs = MaybeMap((MyMaybe<A>)ListToMaybe.Transform<A>(xs), f);
        return lhs.Equals(rhs);
    }

    // MaybeToList 의 자연성: Transform(MaybeMap(m, f)) == ListMap(Transform(m), f)
    public static bool MaybeToListNaturality<A, B>(MyMaybe<A> m, Func<A, B> f)
    {
        var lhs = (MyList<B>)MaybeToList.Transform<B>(MaybeMap(m, f));
        var rhs = ListMap((MyList<A>)MaybeToList.Transform<A>(m), f);
        return lhs.Items.SequenceEqual(rhs.Items);
    }
}
