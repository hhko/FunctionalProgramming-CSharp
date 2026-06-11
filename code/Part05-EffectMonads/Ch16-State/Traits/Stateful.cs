namespace Ch16.Traits;

// Stateful — *상태 스레딩* 효과의 trait. LanguageExt v5 의 Stateful.Trait.cs 와 정합.
//
//   Get    : K<M, S>               현재 상태를 값으로
//   Put    : S → K<M, Unit>        상태를 통째로 교체
//   Modify : (S → S) → K<M, Unit>  상태를 함수로 변형
//   Gets   : (S → A) → K<M, A>     상태에서 일부를 뽑아 값으로
//
// 가변 필드 없이 상태 변화를 표현하는 함수형 도구. Get 은 Gets(identity) 로 유도된다.
public interface Stateful<M, S> where M : Stateful<M, S>
{
    static abstract K<M, Unit> Put(S value);
    static abstract K<M, Unit> Modify(Func<S, S> modify);
    static abstract K<M, A> Gets<A>(Func<S, A> f);

    static virtual K<M, S> Get =>
        M.Gets(s => s);
}
