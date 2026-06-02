using Ch09.Traits;

namespace Ch09.Challenges;

// 챌린지 ③ (심화) 정답 — ZipSeq. 시퀀스의 *두 번째* Applicative.
//
// MySeq.Apply 는 데카르트 곱 ([+1,+10] <*> [1,2] = [2,3,11,12]) 이다.
// 하지만 시퀀스에는 *또 하나의 적법한 Applicative* 가 있다 — zip (짝 맞춤).
//   [+1,+10] <*> [1,2] = [2, 12]   (i 번째 함수에 i 번째 값)
//
// 같은 자료 타입에 *서로 다른 Applicative* 가 둘 존재할 수 있음을 보여 준다.
// (Haskell 의 [] vs ZipList 와 정확히 같은 구도.) 단, 두 Applicative 의 Pure 가 다르다 —
// zip 의 Pure 는 *무한 반복* 이라야 법칙이 성립하므로, 학습용으로는 Apply 만 시연한다.
public sealed class ZipSeq<A>(IEnumerable<A> items) : K<ZipF, A>
{
    public IEnumerable<A> Items { get; } = items;
    public override string ToString() => $"Zip[{string.Join(", ", Items)}]";
}

public sealed class ZipF : Functor<ZipF>
{
    public static K<ZipF, B> Map<A, B>(Func<A, B> f, K<ZipF, A> fa) =>
        new ZipSeq<B>(fa.As().Items.Select(f));

    // zip 결합 — 함수와 값을 *짝* 으로 맞춰 적용 (둘 중 짧은 길이까지).
    public static K<ZipF, B> Apply<A, B>(K<ZipF, Func<A, B>> mf, K<ZipF, A> ma) =>
        new ZipSeq<B>(mf.As().Items.Zip(ma.As().Items, (f, a) => f(a)));
}

public static class ZipSeqExtensions
{
    public static ZipSeq<A> As<A>(this K<ZipF, A> fa) => (ZipSeq<A>)fa;
}
