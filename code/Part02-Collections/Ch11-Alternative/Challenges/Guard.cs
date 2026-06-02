using Ch11.Traits;

namespace Ch11.Challenges;

// 챌린지 ② (심화) — guard. Alternative 위에서 자라는 고전 헬퍼.
//
//   guard(true)  = Pure(unit)   (성공 — 계속 진행)
//   guard(false) = Empty<Unit>() (실패 — 가지 쳐냄)
//
// Applicative.Pure 와 MonoidK.Empty 두 능력을 동시에 쓴다.
// Monad 의 Bind 와 결합하면 LINQ where 절 (조건 필터) 의 일반형이 된다.
public readonly record struct Unit
{
    public static readonly Unit Default = new();
    public override string ToString() => "()";
}

public static class Guards
{
    public static K<F, Unit> guard<F>(bool condition)
        where F : Alternative<F> =>
        condition ? F.Pure(Unit.Default) : F.Empty<Unit>();
}
