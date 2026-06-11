using Ch11.Functions;
using Ch11.Traits;
using Ch11.Types;

namespace Ch11.Tests;

// 자연성 법칙 검증 — Transform 과 Functor map 이 가환.
//
//   Transform(F.Map(f, fa)) == G.Map(f, Transform(fa))
//
// (이 챕터 한정 자기 자료 타입의 map 을 직접 구현해 검증 — 4장 Functor 의 K<F, A> 와 별도.)
//
// xUnit + Shouldly 로 옮기려면 각 호출을 [Fact] 로 감싸고 ShouldBe 단언만 추가하면 됩니다.
// 본 학습용 csproj 는 콘솔 데모이므로 bool 반환으로 통과 / 실패를 표시합니다.
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

    // 일반 자연성 법칙 — 임의의 변환 N : Natural<F, G> 에 대해 두 경로의 가환을 검증.
    //
    //   lhs = N.Transform(F.map(f, fa))   — 먼저 map, 그다음 Transform
    //   rhs = G.map(f, N.Transform(fa))   — 먼저 Transform, 그다음 map
    //
    // static abstract Transform 은 변환 타입 N (예: ListToMaybe) 에 살므로 제약은 where N : Natural<F, G>.
    // 디스패치도 N.Transform<A>(...) — F / G 태그 타입이 아니라 N 으로 호출합니다.
    // mapF / mapG 는 각 컨테이너의 map (이 챕터는 자체 ListMap / MaybeMap), probe 는 비교용 값 추출기.
    public static bool NaturalityHolds<N, F, G, A, B>(
        K<F, A>                            fa,
        Func<A, B>                         f,
        Func<K<F, A>, Func<A, B>, K<F, B>> mapF,
        Func<K<G, A>, Func<A, B>, K<G, B>> mapG,
        Func<K<G, B>, IEnumerable<B>>      probe)
        where N : Natural<F, G>
    {
        var lhs = N.Transform<B>(mapF(fa, f));   // 먼저 map, 그다음 Transform
        var rhs = mapG(N.Transform<A>(fa), f);   // 먼저 Transform, 그다음 map
        return probe(lhs).SequenceEqual(probe(rhs));
    }

    // ListToMaybe 의 자연성: Transform(ListMap(xs, f)) == MaybeMap(Transform(xs), f)
    public static bool ListToMaybeNaturality<A, B>(MyList<A> xs, Func<A, B> f)
    {
        var lhs = (MyMaybe<B>)ListToMaybe.Transform<B>(ListMap(xs, f));   // 먼저 map, 그다음 Transform
        var rhs = MaybeMap((MyMaybe<A>)ListToMaybe.Transform<A>(xs), f);  // 먼저 Transform, 그다음 map
        return lhs.Equals(rhs);
    }

    // MaybeToList 의 자연성: Transform(MaybeMap(m, f)) == ListMap(Transform(m), f)
    public static bool MaybeToListNaturality<A, B>(MyMaybe<A> m, Func<A, B> f)
    {
        var lhs = (MyList<B>)MaybeToList.Transform<B>(MaybeMap(m, f));    // 먼저 map, 그다음 Transform
        var rhs = ListMap((MyList<A>)MaybeToList.Transform<A>(m), f);     // 먼저 Transform, 그다음 map
        return lhs.Items.SequenceEqual(rhs.Items);
    }
}
