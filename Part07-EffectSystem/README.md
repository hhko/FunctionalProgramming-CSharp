# Part 7 — Effect System (IO 와 효과 런타임)

## 7부의 배경

5부는 효과를 모나드로 인코딩했고, 6부는 그 효과들을 한 스택으로 쌓았습니다. 그런데 가장 흔한 효과, 곧 실제 부수 효과 (콘솔 출력, 파일 읽기, 네트워크 호출, 시간 조회) 은 아직 다루지 않았습니다.

7부의 출발점은 함수형이 부수 효과 (side-effect) 를 순수하게 다루는 핵심 발상, 곧 **효과를 즉시 실행하지 않고 값으로 인코딩** 하는 것이고, `IO<A>` 와 `Eff` 가 그 발상의 구현체입니다. 콘솔 출력도, 파일 읽기도, 예외도 일단 값이 되면 합성·재시도·테스트의 대상이 됩니다. 이것이 책 목표 4 (실무의 자신감) 의 정면입니다.

- `IO<A>` — 부수 효과를 즉시 실행하지 않고 DSL 노드 (`IOBind` / `IOApply` / `IOCatch` …) 의 트리로 인코딩 합니다. `EnvIO` (취소 토큰 · 자원 · 동기화 컨텍스트) 위에서 `Run` 될 때 비로소 실행됩니다. 자유 모나드 / 연속 (continuation) 인코딩과 스택 안전성을 직접 구현으로 봅니다.
- `Error` / `Fin<A>` / `Fallible<F>` — 예외를 값으로 다루는 함수형 오류 모델.
- `Eff<A>` — `Eff<MinRT, A>` 의 얇은 래퍼. 런타임 없이 IO + 오류를 다루는 효과.
- `Eff<RT, A> = ReaderT<RT, IO, A>` — **6부 변환기의 회수**. 런타임 `RT` 를 환경으로 주입하고 (`Has<RT, Trait>` DI), IO 효과를 안쪽에 품습니다.

**`Eff<RT, A>` 가 사실 `ReaderT<RT, IO, A>` 라는 것** — 즉 6부에서 쌓은 변환기가 실무 효과 시스템의 골격이었다는 것 — 이 7부의 결정적 통찰입니다.

---

## 7부의 목표

7부를 마치면 독자는 다음을 갖게 됩니다.

- `IO<A>` 가 값으로 인코딩된 효과 프로그램임을 — 즉 부수 효과가 `Run` 전까지 일어나지 않음을 — DSL 노드의 직접 구현으로 이해.
- `EnvIO` 가 실행 시점에 운반하는 것 (취소 · 자원 · 동기화 컨텍스트) 과 트램폴린 기반 스택 안전성.
- `Error` / `Fin<A>` / `Fallible<F>` 로 예외를 값으로 다루는 함수형 오류 모델의 직접 구현.
- `Eff<A>` 와 `Eff<RT, A>` 의 관계, 그리고 `Eff<RT, A> = ReaderT<RT, IO, A>` 인 이유.
- `Has<RT, Trait>` 패턴으로 런타임을 통한 의존성 주입을 시그니처 단계에서 설계.

`Run` 전까지 아무 부수 효과도 일어나지 않는 효과 프로그램을 직접 짜고 읽어내는 능력이 7부의 도달점입니다.

---

## 7부의 무대 — 실행을 미룬 Elevated World

| 시민 | 시그니처 (개념) | 의미 |
|---|---|---|
| **IO\<A\>** | `EnvIO → A` (DSL 인코딩) | `Run` 될 때 부수 효과를 수행하고 `A` 를 돌려주는 효과 프로그램. |
| **Fin\<A\>** | `Error ⊕ A` | 성공 `A` 또는 실패 `Error` — 예외의 값 표현. |
| **Eff\<A\>** | `Eff<MinRT, A>` | 런타임 없는 효과 (IO + 오류). |
| **Eff\<RT, A\>** | `ReaderT<RT, IO, A>` | 런타임 `RT` 주입 + IO 효과. 6부 변환기의 회수. |

기초의 비유가 정점에 닿습니다. Elevated World 로 lift 한다는 한 동사가, 여기서는 부수 효과를 값으로 lift 해 실행을 미룬다는 의미가 됩니다.

---

## 7부의 학습 흐름

| 장 | 두 축에서의 자리 | 한 줄 |
|---|---|---|
| 23장 | 효과의 인코딩 | `IO<A>` DSL 노드 + `EnvIO` + `Run` + 스택 안전성 |
| 24장 | 함수형 오류 모델 | `Error` / `Fin<A>` / `Fallible<F>` — 예외를 값으로 |
| 25장 | 런타임 없는 효과 | `Eff<A> = Eff<MinRT, A>` + `Alternative` / `MonoidK` / `Final` |
| 26장 | 런타임 주입 효과 | `Eff<RT, A> = ReaderT<RT, IO, A>` + `Has<RT, Trait>` DI |

---

## 4개 장의 구성

### 23장 — IO\<A\> — 지연 효과의 인코딩

`IO<A>` 는 부수 효과를 즉시 실행하지 않고 DSL 노드 (`IOBind` / `IOApply` / `IOCatch` / `IOFail` …) 의 트리로 인코딩합니다. 학습용 `MyIO<A>` 를 직접 정의하고 `Run(EnvIO)` 에서 그 트리를 해석하는 인터프리터를 구현합니다. 깊은 `bind` 체인이 스택을 터뜨리지 않도록 트램폴린으로 스택 안전성을 확보하는 자리를 봅니다.

### 24장 — Error · Fin · Fallible

함수형은 예외를 던지지 않고 값으로 다룹니다. `Error` (구조화된 오류), `Fin<A>` (성공 ⊕ 실패), `Fallible<F>` trait (`Fail` / `Catch`) 을 직접 구현합니다. 기초의 `MyValidation` 이 효과 시스템 안의 오류로 확장되는 자리이며, `try/finally` 의 함수형 대응 `Final<F>.Finally` 도 다룹니다.

### 25장 — Eff\<A\> — 런타임 없는 효과

`Eff<A>` 는 `Eff<MinRT, A>` 의 얇은 래퍼입니다 (`MinRT` 는 빈 런타임). IO + 오류를 다루되 의존성 주입이 필요 없는 경우의 효과입니다. `Alternative` / `MonoidK` / `Final` 인스턴스를 직접 부착하며 4부의 선택 / 결합 추상이 효과에서도 작동함을 확인합니다.

### 26장 — Eff\<RT, A\> = ReaderT\<RT, IO, A\>

7부의 정점입니다. `Eff<RT, A>` 가 사실 `ReaderT<RT, IO, A>` (6부에서 쌓은 변환기) 임을 직접 구현으로 확인합니다. 런타임 `RT` 를 환경으로 주입하고, `Has<RT, ConsoleIO>` 같은 제약으로 컴파일러가 검증하는 의존성 주입을 설계합니다. 8부의 견고함 (Schedule / Resource) 과 12부의 실무 예제가 모두 이 타입 위에 섭니다.

---

## 7부의 코드

본문 예제는 모두 `code/Part07-EffectSystem/` 에 실행 가능한 형태로 들어 있습니다 (외부 패키지 의존 0).

```bash
dotnet run --project code/Part07-EffectSystem/Ch23-IO/Ch23.csproj
dotnet run --project code/Part07-EffectSystem/Ch24-Error-Fin-Fallible/Ch24.csproj
dotnet run --project code/Part07-EffectSystem/Ch25-Eff/Ch25.csproj
dotnet run --project code/Part07-EffectSystem/Ch26-Eff-Runtime/Ch26.csproj
```

코드 예제 요약 — Ch23 은 `IO<A>` 를 DSL 노드 트리 + 트램폴린 인터프리터로 구현해 **100,000 단계 Bind 가 스택을 터뜨리지 않음** 과 Run 전 지연·`EnvIO` 취소를 보입니다. Ch24 은 `Error`/`Fin`(Succ·Fail)/`Fallible`(Fail·Catch) 로 예외를 값으로 다루고 첫 실패에서 단락함을, `Try` 로 예외→Error 포획을 보입니다. Ch25 는 `Eff<A> ≈ IO<Fin<A>>` (런타임 없는 효과) 의 지연·오류 포획·Catch 폴백을. Ch26 은 **`Eff<RT,A> = ReaderT<RT, IO, A>`** 와 `Has<RT, IConsole>` 능력 기반 DI 로, 같은 효과 코드를 `TestConsole` 런타임에 주입해 부수 효과 없이 결정적으로 실행합니다 (11부 테스트 런타임 미리보기). 모든 효과가 모나드 법칙을 통과합니다.

학습용 구현은 LanguageExt v5 와 정합합니다 (`Eff<RT,A>` 가 `ReaderT<RT, IO, A>` 인 점, `Fin`/`Error`/`Fallible`/`Has` 시그니처). 단, IO 는 라이브러리의 타입화 DSL 대신 object 소거 노드 + 트램폴린으로, `Eff<A>` 는 `Eff<MinRT,A>` 래퍼 대신 `IO<Fin<A>>` 형태로 단순화했습니다.

---

## 7부의 진입점

6부를 마쳤다면 Ch23 — IO\<A\> 부터 시작합니다. "효과가 값으로 인코딩되어 `Run` 전까지 일어나지 않는다" 는 발상이 7부 전체를 떠받칩니다.
