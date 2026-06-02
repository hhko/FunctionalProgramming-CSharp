using Ch21.Functions;
using Ch21.Traits;
using Ch21.Types;

namespace Ch21.Challenges;

// 챌린지 ① 정답 — Fin 으로 회원가입 검증 파이프라인 (첫 실패에서 단락, 오류 이유 보존).
// (4부 Applicative 누적과 대비 — Fin 의 Bind 는 monadic 단락.)
public static class RegisterValidation
{
    static K<FinF, string> NonEmpty(string name) =>
        string.IsNullOrWhiteSpace(name)
            ? new Fin<string>.Fail(Error.New("이름이 비어 있음"))
            : new Fin<string>.Succ(name);

    static K<FinF, int> Adult(int age) =>
        age >= 18 ? new Fin<int>.Succ(age) : new Fin<int>.Fail(Error.New($"미성년 ({age})"));

    public static Fin<string> Validate(string name, int age)
    {
        var prog =
            from n in NonEmpty(name)
            from a in Adult(age)
            select $"{n}({a})";
        return prog.As();
    }
}
