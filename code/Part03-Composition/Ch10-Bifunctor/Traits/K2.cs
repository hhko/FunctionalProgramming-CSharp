namespace Ch10.Traits;

// 2-인자 컨테이너 마커 — `K<in F, L, A>` 가 `F<L, A>` 를 Order 0 어휘로 인코딩한다.
//
// 4장의 `K<in F, A>` 가 한 인자 컨테이너 (`F<A>`) 의 마커였다면, 이 K 는 두 인자 컨테이너
// (`F<L, A>`, 예: `Either<L, R>`, `Pair<A, B>`) 의 마커.
//
// F 의 `in` (contravariant) 표기 — brand F 는 컨테이너 종류를 분류하는 type-level dispatch 어휘
// 라 contravariant 위치가 자연. 또 이 contravariant 가 `Natural<out F, in G>` (11장) 같은
// 카테고리적 trait variance 의 반전 규칙을 만들어 v5 의 source/target 어휘를 가능하게 한다.
// 시그니처는 LanguageExt v5 의 `K<in F, A, B>` 와 정확히 정합.
public interface K<in F, L, A> { }
