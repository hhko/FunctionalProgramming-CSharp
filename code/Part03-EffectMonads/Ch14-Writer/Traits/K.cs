namespace Ch14.Traits;

// 14장의 F 는 WriterF<W> — "결과와 함께 W 를 누적하는 계산" 이라는 효과 (W 는 Monoid).
public interface K<in F, A>;
