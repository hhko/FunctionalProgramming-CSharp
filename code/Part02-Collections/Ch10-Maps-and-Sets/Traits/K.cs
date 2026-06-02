namespace Ch10.Traits;

// K<F, A> — 익숙한 마커. 10장에서 F 는 *키가 고정된 컨테이너의 태그* (MapF<K>) 다.
//
// 핵심 — Map 의 자리 A 는 *값* 타입이다. 키는 F 안에 갇혀 있어 Map 이 건드리지 않는다.
public interface K<in F, A>;
