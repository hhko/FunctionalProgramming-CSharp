# Part 3 — Effect Monads (효과를 담는 모나드)

## 5부의 배경

기초의 Elevated World 시민들 (`Option` / `List` / `Result`) 은 모두 자료를 담는 컨테이너였습니다. "없을 수 있음", "여러 개", "실패" 는 전부 값의 모양에 대한 효과였습니다.

5부는 한 발 나아갑니다. **효과 그 자체를 모나드로 인코딩** 합니다. "환경에 의존한다", "상태를 읽고 쓴다", "로그를 누적한다" 같은 부수 효과를 (가변 필드나 전역 상태 없이) 순수 함수의 타입에 담습니다. 이것이 함수형이 부수 효과를 다루는 핵심 발상입니다.

- *Reader* — `Env → a`. 환경(설정 / 의존성)을 읽기만 하는 계산.
- *State* — `S → (a, S)`. 상태를 읽고 쓰며 실어 나르는 계산.
- *Writer* — `(a, W)`. 결과와 함께 로그 `W` 를 누적하는 계산 (`W` 는 Monoid).

세 모나드는 각각 학습용 trait (`Readable<M, Env>` / `Stateful<M, S>` / `Writable<M, W>`) 으로 추상화됩니다. 이 trait 들은 LanguageExt v5 의 공식 trait 와 시그니처가 정합하며, 6부의 모나드 변환기가 쌓아 올릴 재료가 됩니다.

---

## 5부의 목표

5부를 마치면 독자는 다음을 갖게 됩니다.

- 부수 효과 (환경 / 상태 / 로그) 를 가변 상태 없이 순수 함수의 타입으로 인코딩하는 발상.
- `Reader<Env, a>` 의 직접 구현 + `Readable<M, Env>` trait (`Asks` / `Ask` / `Local`).
- `State<S, a>` 의 직접 구현 + `Stateful<M, S>` trait (`Get` / `Put` / `Modify` / `Gets`).
- `Writer<W, a>` 의 직접 구현 + `Writable<M, W>` trait (`Tell` / `Listen` / `Pass`), 그리고 기초 `Monoid` 가 `W` 누적에 왜 필요한지.
- 세 모나드가 모두 `Monad<M>` 인스턴스이므로 `bind` / LINQ 로 합성된다는 점, 그리고 하나의 모나드로는 여러 효과를 동시에 담지 못한다는 한계 → 6부 변환기로의 다리.

효과를 모나드로 보는 눈 + 세 trait 을 직접 부착할 수 있는 능력이 5부의 도달점입니다.

---

## 5부의 무대 — 효과를 인코딩한 Elevated World

| 모나드 | 시그니처 (Normal 표현) | 인코딩하는 효과 | trait |
|---|---|---|---|
| **Reader** | `Env → a` | 환경(설정 · 의존성)에 의존 | `Readable<M, Env>` |
| **State** | `S → (a, S)` | 상태를 읽고 씀 | `Stateful<M, S>` |
| **Writer** | `(a, W)` , `W : Monoid` | 로그 · 출력을 누적 | `Writable<M, W>` |

세 시민은 모두 함수 또는 곱의 모양이지만, `K<M, A>` 로 끌어올리면 기초의 모든 동사 (`Map` / `Apply` / `Bind`) 가 그대로 작동합니다. 효과가 타입 안에 있으니 합성이 곧 효과의 합성입니다.

---

## 5부의 학습 흐름

| 장 | 두 축에서의 자리 | 한 줄 |
|---|---|---|
| 15장 | 환경 의존 효과 | `Reader` / `Readable<M, Env>` — `Asks` / `Ask` / `Local` |
| 16장 | 상태 스레딩 효과 | `State` / `Stateful<M, S>` — `Get` / `Put` / `Modify` |
| 17장 | 누적 효과 | `Writer` / `Writable<M, W>` — `Tell` / `Listen` / `Pass` (Monoid 재방문) |
| 18장 | 단일 모나드의 한계 | 여러 효과를 동시에 담지 못하는 벽 → 6부 변환기로의 다리 |

---

## 4개 장의 구성

### 15장 — Reader / 환경 의존

가장 단순한 효과 모나드입니다. `Reader<Env, a>` 는 `Env → a` 함수의 끌어올림이며, `Map` 은 함수 합성, `Bind` 는 같은 환경을 다음 단계로 암묵적으로 전달합니다. 학습용 `Readable<M, Env>` trait (`Asks` / `Ask` / `Local`) 을 직접 정의해 의존성 주입이 어떻게 전역 변수 없이 가능한지 봅니다.

코드 예제 — `BudgetReader` 가 세율·통화 `AppConfig` 를 전역 없이 주입해 여러 Reader 를 LINQ 로 합성하고, `Local` 로 세율을 일시적으로 0 으로 바꿔도 바깥 환경은 그대로임을 보입니다.

### 16장 — State / 상태 스레딩

`State<S, a>` 는 `S → (a, S)` 의 끌어올림입니다. `Bind` 가 새 상태를 다음 단계로 자동으로 실어 나르는 자리를 직접 구현합니다. `Stateful<M, S>` trait (`Get` / `Put` / `Modify` / `Gets`) 로 가변 필드 없이 상태 변화를 표현하는 발상을 손에 잡습니다.

코드 예제 — `FreshId` 가 가변 카운터 없이 고유 ID 생성기를 `State<int, _>` 로 구현하고, 같은 State 계산을 다른 초기 상태로 Run 하면 결과가 달라짐 (상태가 인자가 아니라 효과) 을 보입니다.

### 17장 — Writer / 누적 로그

`Writer<W, a>` 는 결과 `a` 와 출력 `W` 의 곱입니다. `Bind` 가 두 단계의 `W` 를 Monoid 로 합치는 자리에서 기초 Ch03 의 `Monoid` 가 다시 쓰입니다. `Writable<M, W>` trait (`Tell` / `Listen` / `Pass`) 을 직접 구현하며 로그 / 메트릭 누적을 순수하게 표현합니다.

코드 예제 — `TracedMath` 가 `(a+b)*c` 의 각 단계를 `Tell` 로 로그에 누적하고, `Listen` 으로 하위 계산이 말한 출력을 들여다보며, `Pass` 로 로그를 검열합니다. (학습용 `Writer` 는 LanguageExt 의 `Func<W,(A,W)>` 대신 누적의 본질이 또렷한 쌍 표현 `(A, W)` 을 씁니다.)

### 18장 — 왜 변환기가 필요한가

실전 코드는 보통 환경 + 오류 + IO 처럼 여러 효과를 동시에 요구합니다. 하지만 `Reader<Env, Option<a>>` 같은 단순 중첩은 `bind` 가 일반적으로 합성되지 않습니다. 이 합성의 벽을 직접 코드로 부딪쳐 보고, 6부의 모나드 변환기가 왜 필요한지를 동기로 세웁니다.

코드 예제 — `ReaderOption<Env, A>` (내부 `Env → Option<A>`) 의 `Bind` 를 손으로 짜 두 층 (env / Some·None) 을 직접 푸는 배관을 보입니다. 이 배관이 모든 법칙을 만족하는 진짜 모나드임을 확인하되, 효과 쌍마다 반복해야 하는 비용을 체감합니다. 6부 `ReaderT<Env, M, A>` 가 이 배관을 임의의 내부 모나드 M 에 대해 자동 생성합니다.

---

## 5부의 코드

본문 예제는 모두 `code/Part05-EffectMonads/` 에 실행 가능한 형태로 들어 있습니다 (외부 패키지 의존 0).

```bash
dotnet run --project code/Part05-EffectMonads/Ch15-Reader/Ch15.csproj
dotnet run --project code/Part05-EffectMonads/Ch16-State/Ch16.csproj
dotnet run --project code/Part05-EffectMonads/Ch17-Writer/Ch17.csproj
dotnet run --project code/Part05-EffectMonads/Ch18-Why-Transformers/Ch18.csproj
```

각 프로젝트는 기초와 동일한 구성이며 `Program.cs` 의 콘솔 데모가 모든 법칙 검증 결과를 출력합니다. 학습용 trait (`Readable` / `Stateful` / `Writable`) 의 시그니처는 LanguageExt v5 의 공식 trait 와 정합합니다.

---

## 5부의 진입점

4부를 마쳤다면 Ch15 — Reader 부터 시작합니다. 세 효과 모나드 중 가장 단순한 Reader 가 "효과를 타입에 담는다" 는 발상을 가장 선명하게 보여 줍니다.
