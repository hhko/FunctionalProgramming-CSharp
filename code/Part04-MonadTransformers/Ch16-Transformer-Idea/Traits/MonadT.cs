namespace Ch16.Traits;

// MonadT — *변환기* 의 trait. 변환기 T 는 내부 모나드 M 위에 한 효과 층을 쌓는다.
//
//   Lift : K<M, A> → K<T, A>     내부 모나드의 계산을 변환기 한 층 위로 끌어올린다
//
// 15장에서 손으로 짠 배관 (env / Some·None 을 직접 풀기) 의 *일반화* — 변환기는 그 배관을
// 내부 모나드 M 에 대해 자동으로 제공하고, lift 로 안쪽 효과를 바깥 스택에서 쓰게 해 준다.
// (LanguageExt v5 의 MonadT.Trait.cs 의 Lift 에 해당.)
public interface MonadT<T, M> : Monad<T>
    where T : MonadT<T, M>
    where M : Monad<M>
{
    static abstract K<T, A> Lift<A>(K<M, A> ma);
}
