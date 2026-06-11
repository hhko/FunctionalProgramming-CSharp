namespace Ch17.Traits;

// Writable — *누적 출력* 효과의 trait. LanguageExt v5 의 Writable.Trait.cs 와 정합.
//
//   Tell   : W → K<M, Unit>                        출력에 항목 하나를 더한다
//   Listen : K<M,A> → K<M,(A, W)>                   계산이 "말한" 출력을 값으로 함께 본다
//   Pass   : K<M,(A, W→W)> → K<M, A>                출력을 후처리하는 함수를 적용 (필터/검열)
//
// W 는 Monoid — Tell 의 항목들이 Combine 으로 누적되고, 시작은 Empty.
public interface Writable<M, W> where M : Writable<M, W> where W : Monoid<W>
{
    static abstract K<M, Unit> Tell(W item);
    static abstract K<M, (A Value, W Output)> Listen<A>(K<M, A> ma);
    static abstract K<M, A> Pass<A>(K<M, (A Value, Func<W, W> Function)> action);
}
