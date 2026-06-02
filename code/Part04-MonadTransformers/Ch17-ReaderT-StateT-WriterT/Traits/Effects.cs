namespace Ch17.Traits;

// 3부의 효과 trait 들이 변환기에서도 그대로 부착된다 (시그니처 동일).
public interface Readable<M, Env> where M : Readable<M, Env>
{
    static abstract K<M, A> Asks<A>(Func<Env, A> f);
    static virtual K<M, Env> Ask => M.Asks(e => e);
    static abstract K<M, A> Local<A>(Func<Env, Env> f, K<M, A> ma);
}

public interface Stateful<M, S> where M : Stateful<M, S>
{
    static abstract K<M, Unit> Put(S value);
    static abstract K<M, Unit> Modify(Func<S, S> modify);
    static abstract K<M, A> Gets<A>(Func<S, A> f);
    static virtual K<M, S> Get => M.Gets(s => s);
}
