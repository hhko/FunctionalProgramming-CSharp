# Part 9 — Concurrency (동시성)

## 9부의 배경

5 ~ 8부에서 효과를 값으로 다루고 견고하게 만들었습니다. 그런데 효과가 동시에 실행되면 공유 상태에 대한 경쟁 (race) 이 생깁니다. 명령형의 `lock` 은 깜빡 잊기 쉽고, 교착 (deadlock) 을 부르며, 어디서 어떤 락을 잡았는지 추적하기 어렵습니다.

9부의 출발점은 함수형이 동시성을 다루는 방식입니다. **공유 가변 상태를 원자적 참조와 트랜잭션으로 감싸 락 없이 안전하게** 만듭니다.

- `Atom<A>` — Compare-And-Swap (CAS) 기반 원자적 참조. 갱신은 순수 함수 `A → A` 로 표현되고, 충돌 시 자동 재시도됩니다.
- `Ref<A>` / `STM` — Software Transactional Memory. 여러 참조를 하나의 트랜잭션으로 묶어 all-or-nothing 으로 커밋합니다.
- `AtomHashMap` / `VectorClock` — 동시 컬렉션과 분산 인과성 추적.

**갱신을 순수 함수로 표현하기 때문에 충돌 시 그냥 다시 적용하면 된다는 것** — 락 대신 재시도 — 이 9부의 결정적 통찰입니다. 기초의 "효과를 값으로" 가 여기서는 "상태 변화를 함수로" 가 됩니다.

---

## 9부의 목표

9부를 마치면 독자는 다음을 갖게 됩니다.

- `Atom<A>` 의 CAS 루프를 직접 구현 — 갱신 함수 `A → A` 와 충돌 시 재시도의 메커니즘.
- `Ref<A>` 와 `STM<A>` 모나드의 직접 구현 — 여러 참조를 한 트랜잭션으로 묶는 발상.
- 트랜잭션의 격리 (isolation) 와 커밋 / 롤백이 락 없이 어떻게 보장되는지.
- `AtomHashMap` 같은 동시 컬렉션과 `VectorClock` / `VersionVector` 로 분산 인과성을 추적하는 기초.
- 동시성 도구가 7부 `IO` / `Eff` 효과와 결합되는 패턴.

공유 상태를 락 없이 안전하게 다루는 함수형 동시성의 발상과 직접 구현 능력이 9부의 도달점입니다.

---

## 9부의 무대 — 안전한 공유 상태

| 시민 | 시그니처 (개념) | 의미 |
|---|---|---|
| **Atom\<A\>** | `swap : (A → A) → IO<A>` | CAS 기반 원자적 단일 참조. 충돌 시 재시도. |
| **Ref\<A\>** | `STM` 안에서만 읽기 / 쓰기 | 트랜잭션 메모리의 참조. |
| **STM\<A\>** | 트랜잭션 모나드 | 여러 `Ref` 를 all-or-nothing 으로 커밋. |
| **VectorClock** | 부분 순서 | 분산 이벤트의 인과성 추적. |

상태 변화가 순수 함수 (`A → A`) 로 표현되므로, 동시성 충돌은 "다시 적용" 으로 풀립니다. 기초의 trait 어휘 (특히 `Monad`) 가 `STM` 에서 그대로 작동합니다.

---

## 9부의 학습 흐름

| 장 | 두 축에서의 자리 | 한 줄 |
|---|---|---|
| 30장 | 원자적 참조 | `Atom<A>` — CAS 루프 + 순수 갱신 함수 |
| 31장 | 트랜잭션 메모리 | `Ref<A>` / `STM<A>` — 여러 참조를 한 트랜잭션으로 |
| 32장 | 동시 컬렉션 · 인과성 | `AtomHashMap` / `VectorClock` / `VersionVector` |

---

## 3개 장의 구성

### 30장 — Atom\<A\> / CAS 원자성

`Atom<A>` 은 단일 공유 값을 락 없이 안전하게 갱신합니다. 학습용 `MyAtom<A>` 의 `Swap(Func<A, A>)` 를 직접 구현하며 읽기 → 새 값 계산 → CAS → 실패 시 재시도 루프를 봅니다. 갱신이 순수 함수 이기 때문에 재시도가 안전하다는 점 (부수 효과를 넣으면 왜 깨지는지) 을 코드로 확인합니다.

### 31장 — STM & Ref\<A\> / 트랜잭션 메모리

여러 참조를 하나의 원자적 트랜잭션으로 묶는 STM 을 직접 구현합니다. `Ref<A>` 는 `STM` 모나드 안에서만 읽고 쓰며, 트랜잭션 전체가 all-or-nothing 으로 커밋됩니다. `STM<A>` 가 `Monad` 인스턴스이므로 기초의 `bind` / LINQ 어법이 그대로 쓰임을 봅니다. 격리 수준과 충돌 시 롤백·재시도를 다룹니다.

### 32장 — 동시 컬렉션 & 인과성

`AtomHashMap<K, V>` 같은 동시 컬렉션이 4부의 불변 컬렉션을 원자적 참조로 감싼 형태임을 봅니다. 나아가 `VectorClock` / `VersionVector` 로 분산 시스템에서 이벤트의 부분 순서 (인과성) 를 추적하는 기초를 다룹니다. 동시성 도구가 7부 효과와 결합되어 동시 워크로드를 구성하는 패턴으로 마무리합니다.

---

## 9부의 코드

본문 예제는 모두 `code/Part09-Concurrency/` 에 실행 가능한 형태로 들어 있습니다 (외부 패키지 의존 0, .NET 내장 `System.Collections.Immutable` 만 사용).

```bash
dotnet run --project code/Part09-Concurrency/Ch30-Atom/Ch30.csproj
dotnet run --project code/Part09-Concurrency/Ch31-STM/Ch31.csproj
dotnet run --project code/Part09-Concurrency/Ch32-Concurrent-Collections/Ch32.csproj
```

코드 예제 요약 — Ch30 은 `Atom<A>` 를 `Interlocked.CompareExchange` CAS 루프로 구현해 8 스레드 × 10,000 증가가 정확히 80,000 이 됨(락 없는 `int++` 는 유실)을 실증합니다. Ch31 은 `Ref` + 낙관적 `STM` 트랜잭션(읽기 버전 검증 → 충돌 시 전체 재시도)으로 동시 계좌 이체에도 총액이 보존 됨을, `STM<A>` 가 `Monad` 라 LINQ 로 합성됨을 보입니다. Ch32 는 `AtomHashMap`(불변 맵 + CAS) 동시 삽입 무손실과 `VectorClock` 으로 happens-before / concurrent 를 판정합니다. 갱신을 순수 함수로 표현하기에 충돌 시 재적용이 안전하다는 9부의 핵심이 코드로 드러납니다.

학습용 구현은 LanguageExt v5 의 발상과 정합합니다 (`Atom` CAS, `Ref`/`STM` 트랜잭션, `AtomHashMap`, `VectorClock`). 단, STM 은 글로벌 락 기반 낙관적 검증으로, 동시 컬렉션은 `ImmutableDictionary` 기반으로 단순화했습니다.

---

## 9부의 진입점

8부를 마쳤다면 Ch30 — Atom 부터 시작합니다. 단일 원자적 참조의 CAS 루프가 "상태 변화를 순수 함수로" 라는 9부 전체의 발상을 가장 작은 단위로 보여 줍니다.
