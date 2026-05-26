# 챌린지 ② 정답 — Foldable 인지 분류

본문 §4.9.2 의 다섯 후보를 분류한 정답.

| 후보 | 시그니처 | Foldable? | 해당 trait | 근거 |
|---|---|---|---|---|
| (a) `Reverse` | `List<A> → List<A>` | ✗ | 컨테이너 특화 | 결과가 `E<a>` (List) 라 ④ 유형. Foldable 의 `E<a> → b` 형태가 아님. Functor 도 아님 (변환 함수 없이 순서만 뒤집음). |
| (b) `Length` | `List<A> → int` | ✓ | **Foldable** | `E<a> → int`. 컨테이너 소비 + 한 값 (개수) 누적. `Count` 의 다른 이름. |
| (c) `Average` | `List<int> → double` | ✓ | **Foldable** | `E<int> → double`. 컨테이너 소비 + 한 값 (평균) 누적. `Sum` 과 `Count` 의 합성으로 정의 가능. |
| (d) `Lookup` | `(Dictionary<K, V>, K) → Option<V>` | ✗ | Monad (`bind`) | 출력이 `Option<V>` (Elevated). 두 번째 인자 `K → Option<V>` 가 ② 유형 (World-crossing). 6장 Monad 의 자리. |
| (e) `Find` | `(Func<A, bool>, List<A>) → A?` | ✓ | **Foldable** | `E<a> → a?`. 컨테이너 소비 + 한 원소 (또는 default) 누적. 단락 회로 fold 의 정통 예 — 술어 만족하는 첫 원소를 만나면 종료. |

## 자주 헷갈리는 자리

### (a) Reverse 가 왜 Foldable 이 아닌가
- 시그니처가 `List<A> → List<A>` 라 ④ 유형 (`E<a> → E<a>`).
- 컨테이너 모양이 *보존* 됨 (길이 같음), 단지 원소 순서만 다름.
- Foldable 은 *컨테이너를 소비* 하는 약속이라 결과에 `E` 가 다시 등장하면 안 됨.
- *Reverse 를 Foldable 로 정의할 수는 있다* — `FoldRight((x, acc) => acc.Concat([x]), [], xs)`. 결과 시퀀스가 *우연히* 리스트일 뿐, 시그니처상 `E<a>` 를 돌려준다는 약속이 없음. 결과 타입을 *`E<a>` 로 못 박는* 변환은 Filterable / 컨테이너 특화의 자리.

### (d) Lookup 이 왜 Foldable 이 아닌가
- 두 번째 인자 `K` 가 Normal 입력, 출력 `Option<V>` 가 Elevated 결과.
- 시그니처 자체가 *② 유형* (`a → E<b>`) — Normal 에서 Elevated 로 *건너가는* 함수.
- Foldable 은 *Elevated → Normal* (③ 유형) 의 자리. 방향이 *정반대*.
- 이 건너감을 *합성 가능하게* 만드는 도구가 8장의 `bind` (Monad).

### (e) Find 가 왜 Foldable 인가
- 시그니처 `(p, E<a>) → a?` — 출력이 *Normal 의 한 값* (찾은 원소 또는 default).
- step 함수가 `(x, acc) => p(x) ? x : acc` 또는 `FoldWhile` 류 단락 회로.
- 컨테이너 모든 원소를 *방문* 할 필요는 없다 (술어 만족 즉시 종료) — `Foldable` 의 단락 가능성이 핵심.
- 일반적으로 `Foldable` 의 자유 함수 카탈로그에 `Find` 가 들어 있다 (§4.7.2 의 표).

## 학습 메시지

세 기준이 *동시에* 만족되어야 Foldable 의 `Fold` 자리:
1. *시그니처가 `E<a> → b`* 형태인가?
2. *컨테이너 모양이 소비* 되는가?
3. *step 함수로 누적* 가능한가?

하나라도 어긋나면 *다른 trait* — Functor (④), Filterable (`E<a> → E<a>`), Monad (②), Traversable (`T<E<a>> ↔ E<T<a>>`) 중 하나.
