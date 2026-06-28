using Ch22.Types;

namespace Ch22.Traits;

// MonadIO — *IO 효과를 품을 수 있는* 모나드. LanguageExt v5 의 MonadIO.Trait.cs 와 정합.
//
//   LiftIO : IO<A> → K<M, A>     IO 계산을 스택 (또는 IO 자신) 안으로 끌어올린다
//
// 일반 lift 는 "바로 아래 한 층" 만 올리지만, LiftIO 는 *스택 맨 안쪽의 IO* 를 어디서든 올린다.
// 이 한 멤버가 7부 Eff<RT,A> = ReaderT<RT, IO, A> 가 IO 를 품는 메커니즘이다.
public interface MonadIO<M> : Monad<M> where M : MonadIO<M>
{
    static abstract K<M, A> LiftIO<A>(IO<A> ma);
}
