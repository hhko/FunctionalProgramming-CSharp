namespace Ch11.Traits;

// 1-인자 컨테이너 마커 — `K<in F, A>` 는 LanguageExt v5 의 K 와 정합.
//
// F 의 `in` (contravariant) 표기가 `Natural<out F, in G>` 의 variance 반전 규칙을 가능하게 한다:
//   - Transform 의 매개변수 `K<F, A>` 안 F: contravariant context × 매개변수 자리 = covariant → out F OK.
//   - Transform 의 반환 `K<G, A>` 안 G: contravariant context × 반환 자리 = contravariant → in G OK.
// 즉 K 의 contravariant 표기가 Natural 의 source/target 카테고리적 어휘를 컴파일러 검사에 통과시킨다.
public interface K<in F, A> { }
