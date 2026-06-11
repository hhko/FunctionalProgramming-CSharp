using Ch21.Traits;
using Ch21.Types;

namespace Ch21.Challenges;

// 챌린지 ① 정답 — EitherT<string, ManyF, int>: 여러 입력을 파싱하되 *왜 실패했는지* 를 남긴다.
//
// 각 갈래(Many)가 독립적으로 Right(값) 또는 Left("에러 메시지") 가 된다.
public static class Parse
{
    public static K<EitherTF<string, ManyF>, int> Positive(int x) =>
        x > 0
            ? EitherTF<string, ManyF>.Pure(x)
            : EitherTF<string, ManyF>.Fail<int>($"{x} 은(는) 양수가 아님");
}
