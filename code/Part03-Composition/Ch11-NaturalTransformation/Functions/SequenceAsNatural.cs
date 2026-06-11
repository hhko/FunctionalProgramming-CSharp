using Ch11.Traits;
using Ch11.Types;

namespace Ch11.Functions;

// §11.6 — sequence 가 NaturalTransformation 임을 코드로 보인다.
//
// 9장의 sequence : MyList<MyMaybe<A>> → MyMaybe<MyList<A>> 를 합성 컨테이너의 눈으로 보면
// 정확히 Transform : K<F, A> → K<G, A> 이다.
//   F = MyList∘MyMaybe (바깥 List, 안 Maybe)
//   G = MyMaybe∘MyList (바깥 Maybe, 안 List)
// 값 A 는 그대로이고 두 층의 안팎만 뒤집힌다.

// 합성 컨테이너 마커 — 두 컨테이너를 통째로 하나의 F / G 로 본다.
public sealed record ListOfMaybe<A>(MyList<MyMaybe<A>> Value) : K<ListOfMaybeF, A>;
public sealed class ListOfMaybeF { }

public sealed record MaybeOfList<A>(MyMaybe<MyList<A>> Value) : K<MaybeOfListF, A>;
public sealed class MaybeOfListF { }

// sequence 는 이 두 합성 컨테이너 사이의 자연 변환이다 (§11.4 의 ListToMaybe 와 같은 Natural 인스턴스).
public sealed class SequenceNat : Natural<ListOfMaybeF, MaybeOfListF>
{
    public static K<MaybeOfListF, A> Transform<A>(K<ListOfMaybeF, A> fa)
    {
        var list = ((ListOfMaybe<A>)fa).Value;       // MyList<MyMaybe<A>>
        var acc = new List<A>();
        foreach (var m in list.Items)
        {
            // 안쪽 효과가 MyMaybe 라, 하나라도 Nothing 이면 전체가 Nothing (단락).
            if (m is Just<A> j) acc.Add(j.Val);
            else return new MaybeOfList<A>(MyMaybe<MyList<A>>.Nothing);
        }
        // 모두 Just 면 값들을 모아 Just(MyList) 로 — 두 층의 안팎이 뒤집힌다.
        return new MaybeOfList<A>(MyMaybe<MyList<A>>.Of(new MyList<A>(acc)));
    }
}
