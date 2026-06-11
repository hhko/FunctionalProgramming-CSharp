namespace Ch15.Traits;

// Readable — *환경 의존* 효과의 trait. LanguageExt v5 의 Readable.Trait.cs 와 정합.
//
//   Asks  : (Env → A) → K<M, A>     환경에서 값을 뽑아 계산으로
//   Ask   : K<M, Env>               환경 전체를 그대로 (Asks(identity))
//   Local : (Env → Env) → K<M,A> → K<M,A>   국소적으로 변형된 환경에서 실행
//
// 이 세 멤버가 "전역 변수 없이 의존성을 주입" 하는 함수형 DI 의 토대다.
public interface Readable<M, Env> where M : Readable<M, Env>
{
    static abstract K<M, A> Asks<A>(Func<Env, A> f);

    static virtual K<M, Env> Ask =>
        M.Asks(e => e);

    static abstract K<M, A> Local<A>(Func<Env, Env> f, K<M, A> ma);
}
