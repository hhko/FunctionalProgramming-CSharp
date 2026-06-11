using Ch14.Traits;

namespace Ch14.Types;

// MySeq — 9장의 시퀀스에 *선택·결합* 추상을 부착.
//
// 결정적 대비 — 시퀀스에서 Combine 과 Choose 가 *다르다*:
//   Combine (MonoidK) = 두 시퀀스 concat (모두 모음).
//   Choose  (Choice)  = 첫 비어있지 않은 시퀀스 (하나만 고름).
public sealed class MySeq<A>(IEnumerable<A> items) : K<SeqF, A>
{
    public IReadOnlyList<A> Items { get; } = items.ToList();
    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

// 라이브러리의 `Seq : …, MonoidK<Seq>, Alternative<Seq>` 와 동일하게
// MonoidK 와 Alternative 를 *각각 따로* 구현한다. 두 인터페이스가 모두 Empty 를
// 요구하지만 시그니처가 같아 하나의 static 메서드가 둘 다 만족시킨다.
public sealed class SeqF : MonoidK<SeqF>, Alternative<SeqF>
{
    public static K<SeqF, B> Map<A, B>(Func<A, B> f, K<SeqF, A> fa) =>
        new MySeq<B>(fa.As().Items.Select(f));

    public static K<SeqF, A> Pure<A>(A value) =>
        new MySeq<A>([value]);

    public static K<SeqF, B> Apply<A, B>(K<SeqF, Func<A, B>> mf, K<SeqF, A> ma) =>
        new MySeq<B>(mf.As().Items.SelectMany(f => ma.As().Items.Select(f)));

    // MonoidK.Empty — 빈 시퀀스 (Combine 의 항등원).
    public static K<SeqF, A> Empty<A>() =>
        new MySeq<A>([]);

    // SemigroupK.Combine — concat (둘을 모두 모음).
    public static K<SeqF, A> Combine<A>(K<SeqF, A> lhs, K<SeqF, A> rhs) =>
        new MySeq<A>([.. lhs.As().Items, .. rhs.As().Items]);

    // Choice.Choose — 첫 비어있지 않은 쪽 (하나만 고름).
    public static K<SeqF, A> Choose<A>(K<SeqF, A> fa, K<SeqF, A> fb) =>
        fa.As().Items.Count > 0 ? fa : fb;
}

public static class MySeqExtensions
{
    public static MySeq<A> As<A>(this K<SeqF, A> fa) => (MySeq<A>)fa;
}
