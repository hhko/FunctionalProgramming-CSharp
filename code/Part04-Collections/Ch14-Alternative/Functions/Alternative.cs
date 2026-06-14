using Ch14.Traits;

namespace Ch14.Functions;

// 모듈 함수 — Choose / Combine / Empty / oneOf 를 어떤 F 든 받는 generic 으로.
public static class Alternatives
{
    public static K<F, A> empty<F, A>()
        where F : MonoidK<F> =>
        F.Empty<A>();

    public static K<F, A> combine<F, A>(K<F, A> lhs, K<F, A> rhs)
        where F : SemigroupK<F> =>
        F.Combine(lhs, rhs);

    public static K<F, A> choose<F, A>(K<F, A> fa, K<F, A> fb)
        where F : Choice<F> =>
        F.Choose(fa, fb);

    public static K<F, A> oneOf<F, A>(params K<F, A>[] options)
        where F : Alternative<F> =>
        F.OneOf(options);

    // fallback — fa 가 성공하면 그 값, 실패(Empty)면 기본값을 Pure 로 올려 돌려준다.
    // x ?? value 의 Elevated 일반형. v5 Alternative.Option 과 정합 (fa | Pure(value)).
    public static K<F, A> option<F, A>(A value, K<F, A> fa)
        where F : Alternative<F> =>
        F.Choose(fa, F.Pure(value));
}

// 확장 어법 — fa.Choose(fb), fa.Combine(fb).
public static class AlternativeExtensions
{
    public static K<F, A> Combine<F, A>(this K<F, A> lhs, K<F, A> rhs)
        where F : SemigroupK<F> =>
        F.Combine(lhs, rhs);

    public static K<F, A> Choose<F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Choice<F> =>
        F.Choose(fa, fb);
}
