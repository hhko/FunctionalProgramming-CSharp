# Part 12 — Real-world (실무 예제 — 실전 적용)

## 12부의 배경

기초 ~ 11부에서 우리는 함수형의 어휘 (Functor ~ Traversable), 효과 (Reader ~ Eff), 견고함 (Schedule / Resource / 동시성 / 스트리밍), 테스트를 각각 직접 구현하며 익혔습니다. 하지만 실무는 이 도구들을 하나씩 쓰지 않습니다. 한 기능 안에서 검증 · 의존성 주입 · 오류 처리 · 자원 · 동시성 · 스트리밍 · 테스트가 동시에 얽힙니다.

12부의 출발점은 그동안 따로 익힌 도구들을 **하나의 실전 코드로 합성** 하는 것입니다. 책 목표 4 (실무의 자신감) 가 완성되는 자리입니다.

- 도메인 모델링 — 잘못된 상태를 타입으로 표현 불가능하게 만드는 설계 + Validation 으로 입력을 도메인 타입으로 끌어올리기.
- 효과 기반 애플리케이션 — `Eff<RT>` + `Has<RT, Trait>` 다중 능력 DI (IConsole / IClock / IStore) 로 부수 효과를 격리한 앱.
- 데이터 파이프라인 — 재시도 (Schedule) · 자원 (bracket) · lazy 스트림 · 동시 집계 (Atom) 를 결합한 스트리밍 처리.
- 종합 capstone — 위를 모두 모은 소형 애플리케이션 + 11부 테스트 표준.

**따로 익힌 추상들이 한 코드에서 자연스럽게 합성된다는 것** — 그것이 함수형 설계가 약속한 결말이라는 것 — 이 12부의 결정적 마무리입니다.

---

## 12부의 목표

12부를 마치면 독자는 다음을 갖게 됩니다.

- "잘못된 상태를 표현 불가능하게" 만드는 도메인 모델링 + Validation end-to-end 파이프라인 작성.
- `Eff<RT>` + `Has<RT, Trait>` 로 의존성을 주입하고, 능력 인터페이스 (IConsole / IClock / IStore) 로 콘솔 · 시간 · 저장 부수 효과를 격리한 애플리케이션 작성.
- 재시도 (Schedule) · 자원 (bracket) · lazy 스트림 · 동시 집계 (Atom) 를 결합한 실전 데이터 파이프라인 구성.
- 기초 ~ 11부의 도구를 한 소형 프로젝트로 합성하고 11부 표준으로 테스트.
- LanguageExt v5 샘플 (`Newsletter` / `CardGame` / `BlazorApp` / `EffectsExamples`) 을 익숙한 패턴의 변형으로 읽어내기.

따로 배운 추상을 실전 코드 한 편으로 합성하고 테스트하는 능력이 12부 (그리고 이 책 전체) 의 도달점입니다.

---

## 12부의 무대 — 합성되는 전부

| 시나리오 | 동원되는 도구 | 어느 Part 에서 |
|---|---|---|
| 도메인 모델링 + 검증 | Validation 누적 / applicative / Monoid | Part 1 ~ 3 |
| 의존성 주입 + 부수 효과 격리 | `Eff<RT>` / `Has` 다중 능력 DI | Part 7 |
| 재시도 · 자원 | Schedule (재시도) / bracket (자원 개폐) | Part 8 |
| 동시 · 스트리밍 처리 | Atom (동시 집계) / lazy 스트림 | Part 9 ~ 10 |
| 검증 | 테스트 더블 / 동작 검증 | Part 11 |

12부에는 새 추상이 없습니다. 기초의 한 동사 (모든 값과 함수를 합성 가능한 Elevated World 로 lift 한다) 가 실무 규모에서 실제로 합성되는 것을 보는 자리입니다.

---

## 12부의 학습 흐름

| 장 | 두 축에서의 자리 | 한 줄 |
|---|---|---|
| 39장 | 도메인을 타입으로 | 도메인 모델링 + Validation end-to-end |
| 40장 | 부수 효과를 격리한 앱 | `Eff<RT>` + `Has` 다중 능력 DI |
| 41장 | 데이터 파이프라인 | 재시도 · 자원 · lazy 스트림 · 동시 집계 결합 |
| 42장 | 전부를 한 편으로 | 종합 capstone + 11부 테스트 |

---

## 4개 장의 구성

### 39장 — 도메인 모델링 & 검증 파이프라인

"잘못된 상태를 컴파일되지 않게" 만드는 함수형 도메인 모델링을 다룹니다. strongly-typed 도메인 값 (예: 검증된 `Email` / `Age`) 을 정의하고, 기초의 `Validation` (applicative 누적) 으로 원시 입력을 도메인 타입으로 끌어올리는 end-to-end 파이프라인을 작성합니다. 잘못된 값이 도메인 경계를 넘지 못하게 만드는 설계를 봅니다.

### 40장 — 효과 기반 애플리케이션

`Eff<RT>` + `Has<RT, IConsole>` / `Has<RT, IClock>` / `Has<RT, IStore>` 로 의존성을 주입하고, 능력 인터페이스로 콘솔 · 시간 · 저장 부수 효과를 앱 경계로 격리한 노트 앱을 작성합니다. 7부의 런타임 주입과 11부의 테스트 더블이 같은 설계의 양면임을 실제 앱에서 확인합니다.

### 41장 — 동시성·스트리밍 실전

10부의 lazy 스트림과 8부의 Schedule (재시도) / bracket (소스의 안전한 개폐) 를 결합해 대용량 입력 → 변환 → 적재 데이터 파이프라인을 구성합니다. 필요한 자리에 9부의 Atom 으로 동시 집계를 더합니다. 효과 · 자원 · 스트림 · 동시성이 한 파이프라인으로 합성되는 자리입니다.

### 42장 — 종합 capstone

따로 익힌 도구를 주문 접수 서비스 한 편으로 합성합니다 (검증 + 효과·DI + 테스트 더블). 코드와 테스트를 함께 작성하며, 마지막으로 LanguageExt v5 샘플 (`Newsletter` / `CardGame` / `BlazorApp`) 을 펼쳐 이 책에서 손으로 만든 패턴의 변형으로 읽어내는 것으로 책 전체를 마무리합니다.

---

## 12부의 코드

본문 예제는 모두 `code/Part12-RealWorld/` 에 실행 가능한 형태로 들어 있습니다 (외부 패키지 의존 0).

```bash
dotnet run --project code/Part12-RealWorld/Ch39-Domain-Modeling/Ch39.csproj
dotnet run --project code/Part12-RealWorld/Ch40-Effectful-Application/Ch40.csproj
dotnet run --project code/Part12-RealWorld/Ch41-Streaming-Pipeline/Ch41.csproj
dotnet run --project code/Part12-RealWorld/Ch42-Capstone/Ch42.csproj
```

코드 예제 요약 — Ch39 은 강타입 도메인(`Username`/`Email`/`Age` 스마트 생성자) + `Validation` applicative 누적으로 모든 오류를 한 번에 보고합니다. Ch40 은 `Eff<RT>` + `Has<RT, IConsole/IClock/IStore>` 다중 능력 DI 로 노트 앱을 짜고 테스트 런타임으로 검증합니다. Ch41 은 retry(Schedule) + bracket(Resource) + 동시 집계(Atom) + lazy 스트리밍을 한 파이프라인으로 결합합니다. Ch42 capstone 은 검증(기초) + 효과·DI(7부) + 테스트 더블(11부) 을 주문 접수 서비스 한 편으로 합성합니다. 새 추상 없이, 기초의 한 동사가 실무 규모에서 합성됩니다.

LanguageExt v5 의 `Samples/`(Newsletter·CardGame·BlazorApp·EffectsExamples)를 펼치면 이 책에서 손으로 만든 패턴(`Eff<RT>`/`Has`/`Validation`/Pipes)의 변형으로 읽힙니다.

---

## 12부의 진입점

11부를 마쳤다면 Ch39 — 도메인 모델링 & 검증 파이프라인 부터 시작합니다. 가장 바깥 (입력 검증) 에서 시작해 안쪽 (효과 · 자원 · 스트림) 으로 들어가며, 마지막 capstone 에서 전부가 한 편으로 합성됩니다.
