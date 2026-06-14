using Ch12.Traits;

namespace Ch12.Types;

// MySeq — *lazy 시퀀스*. 1부의 MyList 가 가리키던 자리의 실무 시민.
//
// MyList 는 생성 시 ToList() 로 즉시 materialize 했지만, 실무 Seq 는 *lazy* 다.
// 내부를 IEnumerable<A> 로 들고, Map / Bind / Apply 가 모두 yield 기반 (LINQ) 이라
// 끝까지 당겨질 때까지 계산이 미뤄진다. (LanguageExt Seq 의 iterator 패턴 참고.)
public sealed class MySeq<A>(IEnumerable<A> items) : K<SeqF, A>
{
    public IEnumerable<A> Items { get; } = items;

    public override string ToString() => $"[{string.Join(", ", Items)}]";
}

// 태그 타입 — 하나의 SeqF 가 Monad + Traversable 을 모두 호스트.
// (Monad ⊃ Applicative ⊃ Functor, Traversable ⊃ Functor + Foldable.)
public sealed class SeqF : Monad<SeqF>, Traversable<SeqF>
{
    // Functor — 안의 값만 변환 (lazy).
    public static K<SeqF, B> Map<A, B>(Func<A, B> f, K<SeqF, A> fa) =>
        new MySeq<B>(fa.As().Items.Select(f));

    // Applicative.Pure — 원소 하나짜리 시퀀스.
    public static K<SeqF, A> Pure<A>(A value) =>
        new MySeq<A>([value]);

    // Applicative.Apply — 데카르트 곱 (함수 × 값).
    public static K<SeqF, B> Apply<A, B>(K<SeqF, Func<A, B>> mf, K<SeqF, A> ma)
    {
        var fs = mf.As();
        var xs = ma.As();
        return new MySeq<B>(Go());

        IEnumerable<B> Go()
        {
            foreach (var f in fs.Items)
                foreach (var x in xs.Items)
                    yield return f(x);
        }
    }

    // Monad.Bind — 각 원소를 시퀀스로 펼쳐 이어 붙임 (= LINQ SelectMany).
    public static K<SeqF, B> Bind<A, B>(K<SeqF, A> ma, Func<A, K<SeqF, B>> f)
    {
        var xs = ma.As();
        return new MySeq<B>(Go());

        IEnumerable<B> Go()
        {
            foreach (var x in xs.Items)
                foreach (var y in f(x).As().Items)
                    yield return y;
        }
    }

    // Foldable.FoldLeft — 앞에서 뒤로 (lazy 시퀀스를 한 번만 순회).
    public static B FoldLeft<A, B>(Func<B, A, B> f, B seed, K<SeqF, A> fa)
    {
        var acc = seed;
        foreach (var a in fa.As().Items)
            acc = f(acc, a);
        return acc;
    }

    // Foldable.FoldRight — 뒤에서 앞으로 (materialize 후 역순 누적, 스택 안전).
    public static B FoldRight<A, B>(Func<A, B, B> f, B seed, K<SeqF, A> fa)
    {
        var buffer = fa.As().Items.ToList();
        var acc = seed;
        for (var i = buffer.Count - 1; i >= 0; i--)
            acc = f(buffer[i], acc);
        return acc;
    }

    // Traversable.Traverse — List<F<B>> 을 F<List<B>> 로 swap (9장 패턴).
    public static K<F, K<SeqF, B>> Traverse<F, A, B>(Func<A, K<F, B>> f, K<SeqF, A> ta)
        where F : Applicative<F>
    {
        K<F, K<SeqF, B>> acc = F.Pure<K<SeqF, B>>(new MySeq<B>([]));

        foreach (var a in ta.As().Items.Reverse())
        {
            var fb = f(a);
            Func<B, Func<K<SeqF, B>, K<SeqF, B>>> prepend =
                head => tail => new MySeq<B>([head, .. tail.As().Items]);

            var liftedFn = F.Pure(prepend);
            var step1    = F.Apply<B, Func<K<SeqF, B>, K<SeqF, B>>>(liftedFn, fb);
            acc          = F.Apply<K<SeqF, B>, K<SeqF, B>>(step1, acc);
        }

        return acc;
    }
}

// LanguageExt 식 확장 메서드 — 다운캐스트 보일러플레이트를 감춘다.
public static class MySeqExtensions
{
    public static MySeq<A> As<A>(this K<SeqF, A> fa) => (MySeq<A>)fa;
}
