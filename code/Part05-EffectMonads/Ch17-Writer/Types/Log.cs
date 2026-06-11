using Ch17.Traits;

namespace Ch17.Types;

// Log — 구체적인 Monoid 인스턴스. 문자열 줄들의 누적.
//   Empty   = 빈 로그
//   Combine = 두 로그를 이어 붙임
// Writer 의 W 자리에 들어가 "실행 추적 로그" 를 누적한다.
//
// 주의 — record 의 자동 Equals 는 내부 리스트를 *참조* 로 비교하므로, 값 비교가 필요한
// 법칙 검증에서는 Lines 를 직접 비교한다 (Program 의 probe 참조). 여기선 Equals 를 직접 구현.
public sealed class Log(IReadOnlyList<string> lines) : Monoid<Log>
{
    public IReadOnlyList<string> Lines { get; } = lines;

    public static Log Empty => new([]);
    public Log Combine(Log rhs) => new([.. Lines, .. rhs.Lines]);

    public static Log Of(string line) => new([line]);

    public override string ToString() => $"[{string.Join(" | ", Lines)}]";
}
