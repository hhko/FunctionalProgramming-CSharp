using Ch11.Traits;
using Ch11.Types;

namespace Ch11.Functions;

// MyList → MyMaybe : 첫 원소가 있으면 Just, 없으면 Nothing.
public sealed class ListToMaybe : Natural<MyListF, MyMaybeF>
{
    public static K<MyMaybeF, A> Transform<A>(K<MyListF, A> fa)
    {
        var list = (MyList<A>)fa;
        return list.IsEmpty ? MyMaybe<A>.Nothing : MyMaybe<A>.Of(list.Head);
    }
}

// MyMaybe → MyList : Just 면 원소 1개, Nothing 이면 빈 리스트.
public sealed class MaybeToList : Natural<MyMaybeF, MyListF>
{
    public static K<MyListF, A> Transform<A>(K<MyMaybeF, A> fa) =>
        (MyMaybe<A>)fa switch
        {
            Just<A> j => MyList<A>.Of(j.Val),
            _ => MyList<A>.Empty
        };
}
