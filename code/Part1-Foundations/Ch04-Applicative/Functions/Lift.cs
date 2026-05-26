using Ch04.Traits;

namespace Ch04.Functions;

// Lift — 다인자 *순수 함수* 를 *컨테이너 단계로 승격*.
//
// Lift2 의 의미: (A, B) → C 를 받아 (K<F, A>, K<F, B>) → K<F, C> 로 변환.
//
// Curry → Pure → Apply 의 결합 패턴이 Applicative 의 표준 사용법.
public static class Lift
{
    public static K<F, C> Lift2<F, A, B, C>(Func<A, B, C> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F>
    {
        var curried = Curry.Of(f);                      // (A, B) → C  ⟶  A → (B → C)
        K<F, Func<A, Func<B, C>>> lifted = F.Pure(curried);
        K<F, Func<B, C>> step1 = F.Apply<A, Func<B, C>>(lifted, fa);
        K<F, C> step2 = F.Apply<B, C>(step1, fb);
        return step2;
    }

    public static K<F, D> Lift3<F, A, B, C, D>(Func<A, B, C, D> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F>
    {
        var curried = Curry.Of(f);
        var lifted = F.Pure(curried);
        var step1 = F.Apply<A, Func<B, Func<C, D>>>(lifted, fa);
        var step2 = F.Apply<B, Func<C, D>>(step1, fb);
        var step3 = F.Apply<C, D>(step2, fc);
        return step3;
    }
}
