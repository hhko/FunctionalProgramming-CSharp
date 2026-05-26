using Ch06.Traits;

namespace Ch06.Functions;

// Kleisli 합성 — 월드 교차 함수의 정식 ∘.
//
// 1 장 의 ② 유형 — a → M<b> — 는 *직접 합성이 안 된다*. f: a → M<b> 와 g: b → M<c>
// 는 어법이 어긋나 이어 붙지 않는다. Bind 가 이 문제를 풀어 *합성을 elevated world
// 안에서* 가능하게 만든다.
//
// >=> 연산자 (Kleisli 합성) 가 정의되면 *normal 함수의 ∘* 과 동일한 우아함이 elevated
// 어휘에 들어온다.
//
//   f : a → M<b>
//   g : b → M<c>
//   f >=> g : a → M<c>     (Kleisli 합성)
public static class Kleisli
{
    // f >=> g  ─ a 를 받아 M<c> 를 낸다. 내부적으로는 Bind 사슬.
    public static Func<A, K<M, C>> Then<M, A, B, C>(
        this Func<A, K<M, B>> f,
        Func<B, K<M, C>> g)
        where M : Monad<M>
    =>
        a => M.Bind(f(a), g);

    // 3 단계 사슬을 한 줄로.
    public static Func<A, K<M, D>> ThenThen<M, A, B, C, D>(
        Func<A, K<M, B>> f,
        Func<B, K<M, C>> g,
        Func<C, K<M, D>> h)
        where M : Monad<M>
    =>
        a => M.Bind(M.Bind(f(a), g), h);

    // 항등 Kleisli 함수 — Pure. Kleisli 합성의 *왼/오른 항등원*.
    //   pure >=> f  ≡  f
    //   f >=> pure  ≡  f
    public static Func<A, K<M, A>> Id<M, A>()
        where M : Monad<M>
    =>
        M.Pure;
}
