namespace Ch11.Traits;

// NaturalTransformation — 컨테이너 자체를 다른 컨테이너로 옮기는 변환.
//
// Transform : K<F, A> → K<G, A>
//   - 값 타입 A 는 입력과 출력에서 같음 — 컨테이너 F 만 G 로 바뀜.
//   - Functor 의 map 이 안의 값을 바꾼다면, NaturalTransformation 은 구조 자체를 바꿈.
//
// 자연성 법칙: Transform(F.Map(f, fa)) == G.Map(f, Transform(fa))
//
// variance — `out F` (source, 변환의 출발), `in G` (target, 변환의 도착).
// 카테고리 이론의 source/target 비대칭을 어휘로 보존 (Haskell `Nat f g = forall a. f a -> g a`
// 의 f=source, g=target).
//
// 일반 variance 규칙으로는 `<in F, out G>` 가 자연 (F=매개변수, G=반환) 인데 v5 는 반대로 표기.
// 컴파일을 통과시키는 핵심은 K 마커의 contravariant 표기 `K<in F, A>`:
//   - Transform 매개변수 K<F, A> 안 F : contravariant context × 매개변수 위치 = covariant → out F OK.
//   - Transform 반환 K<G, A> 안 G : contravariant context × 반환 위치 = contravariant → in G OK.
// 즉 K 의 contravariant 표기가 variance 반전을 만들어 Natural 의 source/target 어휘를 가능하게 한다.
//
// 시그니처는 LanguageExt v5 의 `Natural<out F, in G>` 와 정확히 정합.
public interface Natural<out F, in G>
{
    static abstract K<G, A> Transform<A>(K<F, A> fa);
}
