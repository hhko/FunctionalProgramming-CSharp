using Ch17.Traits;

namespace Ch17.Functions;

// Writable 모듈 — tell / listen / pass 의 generic 어휘.
public static class Writable
{
    public static K<M, Unit> tell<M, W>(W item)
        where M : Writable<M, W> where W : Monoid<W> =>
        M.Tell(item);

    public static K<M, (A Value, W Output)> listen<M, W, A>(K<M, A> ma)
        where M : Writable<M, W> where W : Monoid<W> =>
        M.Listen(ma);

    public static K<M, A> pass<M, W, A>(K<M, (A Value, Func<W, W> Function)> action)
        where M : Writable<M, W> where W : Monoid<W> =>
        M.Pass(action);

    // censor — 출력에 변형 함수 f 를 적용한다. Pass 가 요구하는 (결과, 변형함수) 쌍 싣기를
    // 대신 해 주는 사용자 입구. censor(f, ma) = Pass(Bind(ma, a => Pure((a, f)))).
    public static K<M, A> censor<M, W, A>(Func<W, W> f, K<M, A> ma)
        where M : Writable<M, W>, Monad<M> where W : Monoid<W> =>
        M.Pass(M.Bind(ma, a => M.Pure((a, f))));
}
