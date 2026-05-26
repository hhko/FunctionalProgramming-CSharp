using Ch04.Traits;

namespace Ch04.Functions;

// LiftN — Lift.cs 의 보강. 두 가지를 추가한다.
//
//   1. Lift4 — 4 인자 함수의 끌어올림. 기존 Lift2/Lift3 의 자연스러운 일반화.
//   2. Curried 형태 — 즉시 적용 대신 *E<a> → E<b> → … → E<r> 함수 그 자체* 를 돌려준다.
//      이 형태가 F# Elevated World 시리즈의 lift 어휘에 더 가깝다.
public static class LiftN
{
    // 4 인자 함수 끌어올림 — 즉시 적용. apply 4 번.
    public static K<F, E> Lift4<F, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        K<F, A> fa, K<F, B> fb, K<F, C> fc, K<F, D> fd)
        where F : Applicative<F>
    {
        Func<A, Func<B, Func<C, Func<D, E>>>> curried =
            a => b => c => d => f(a, b, c, d);
        var lifted = F.Pure(curried);
        var step1 = F.Apply<A, Func<B, Func<C, Func<D, E>>>>(lifted, fa);
        var step2 = F.Apply<B, Func<C, Func<D, E>>>(step1, fb);
        var step3 = F.Apply<C, Func<D, E>>(step2, fc);
        var step4 = F.Apply<D, E>(step3, fd);
        return step4;
    }

    // Curried 형태: (a, b) → c 를 E<a> → E<b> → E<c> 로 끌어올린다.
    // 즉시 적용 대신 *함수 그 자체* 가 elevated world 에서 살아 있다는 점이 lift 의 진짜 의미.
    public static Func<K<F, A>, Func<K<F, B>, K<F, C>>> Lift2Curried<F, A, B, C>(
        Func<A, B, C> f)
        where F : Applicative<F>
    =>
        fa => fb => Lift.Lift2(f, fa, fb);

    public static Func<K<F, A>, Func<K<F, B>, Func<K<F, C>, K<F, D>>>> Lift3Curried<F, A, B, C, D>(
        Func<A, B, C, D> f)
        where F : Applicative<F>
    =>
        fa => fb => fc => Lift.Lift3(f, fa, fb, fc);
}
