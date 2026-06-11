namespace Ch15.Traits;

// K<F, A> — 익숙한 마커. 3부에서 F 는 *효과를 인코딩한 모나드의 태그* 다.
// 12장의 F 는 ReaderF<Env> — "환경 Env 에 의존하는 계산" 이라는 효과.
public interface K<in F, A>;
