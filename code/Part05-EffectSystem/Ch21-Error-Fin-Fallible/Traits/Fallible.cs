using Ch21.Types;

namespace Ch21.Traits;

// Fallible — *오류로 실패할 수 있는* 효과의 trait. LanguageExt v5 의 Fallible.Trait.cs 와 정합.
//
//   Fail  : Error → K<F, A>                       오류로 단락
//   Catch : K<F,A> → (Error → K<F,A>) → K<F,A>     실패를 포착해 복구
//
// 예외를 던지지 않고 *값(Error)* 으로 다루는 함수형 오류 모델의 핵심.
public interface Fallible<F> where F : Fallible<F>
{
    static abstract K<F, A> Fail<A>(Error error);
    static abstract K<F, A> Catch<A>(K<F, A> fa, Func<Error, K<F, A>> handler);
}
