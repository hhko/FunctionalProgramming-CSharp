namespace Ch02.Traits;

// 이 책의 *기술적 출발점*. Higher-kinded 표현을 위한 빈 인터페이스.
//
// "F 라는 컨테이너 안에 A 라는 자료가 있다" 는 신호만 컴파일러에 준다.
public interface K<in F, A>;
