# Part 6 — Robust Effects (견고한 효과)

## 8부의 배경

7부는 `IO` / `Eff` 로 효과를 값으로 인코딩하고 실행을 미루는 핵심을 세웠습니다. 하지만 실무 효과는 실패하고, 자원을 잡고, 관측되어야 합니다. 네트워크 호출은 재시도가 필요하고, 파일 핸들은 반드시 닫혀야 하며, 운영 환경에서는 추적 (tracing) 이 필요합니다.

8부의 출발점은 7부의 효과를 **견고하게 (robust)** 만드는 세 가지 도구입니다. 모두 `IO` / `Eff` 위에 조합으로 얹힙니다.

- `Schedule` — 언제 다시 시도/반복할지를 기술하는 (잠재적으로 무한한) 시간 간격의 스트림. `retry` / `repeat` 와 결합합니다.
- `Resource` / `bracket` — 반드시 해제되는 자원 수명 관리. 획득 (acquire) 과 해제 (release) 를 한 쌍으로 묶습니다.
- `Activity` (OpenTelemetry) — 효과를 분산 추적으로 관측. .NET `System.Diagnostics.Activity` 와 정합합니다.

**효과를 값으로 다루기 때문에 재시도 · 자원 · 추적을 모두 조합 가능한 별도 값으로 얹을 수 있다는 것** 이 8부의 결정적 통찰입니다. 명령형에서 흩어지던 횡단 관심사가 함수형에서는 합성됩니다.

---

## 8부의 목표

8부를 마치면 독자는 다음을 갖게 됩니다.

- `Schedule` 을 시간 간격의 스트림으로 보는 발상 + `recurs` / `spaced` / `exponential` / `fibonacci` 의 직접 구현.
- 스케줄 결합 — union `|` (둘 중 짧은 쪽) 과 intersect `&` (둘 중 긴 쪽), 그리고 `retry` / `repeat` 결합.
- `Resource` / `bracket` 으로 예외가 나도 반드시 해제되는 자원 수명을, 계층적 자원 추적 (parent/child) 과 함께 직접 구현.
- `Activity` 기반 분산 추적을 효과에 얹어 관측 가능 한 코드를 작성.
- 위 세 도구가 모두 7부 `IO` / `Eff` 위에 조합으로 얹히는 패턴.

실무 효과를 재시도 · 자원 안전 · 관측 가능하게 만드는 능력이 8부의 도달점입니다.

---

## 8부의 무대 — 효과 위에 얹는 도구

| 도구 | 시그니처 (개념) | 의미 |
|---|---|---|
| **Schedule** | (무한) `Duration` 스트림 | 재시도 / 반복의 시점을 기술. `Semigroup<Schedule>`. |
| **Resource** | `acquire × release` | 자원의 획득–해제를 한 쌍으로. 예외에도 해제 보장. |
| **Activity** | `IO` 를 감싸는 span | 효과 구간을 분산 추적 (OpenTelemetry) 으로 관측. |

세 도구 모두 효과를 바꾸지 않고 감쌉니다. `effect.Retry(schedule)`, `bracket(acquire, release, use)`, `activity("span", effect)` 처럼 7부 효과에 합성됩니다.

---

## 8부의 학습 흐름

| 장 | 두 축에서의 자리 | 한 줄 |
|---|---|---|
| 27장 | 재시도 · 반복 | `Schedule` — 시간 간격 스트림 + union `|` / intersect `&` |
| 28장 | 자원 수명 | `Resource` / `bracket` — 예외에도 해제 보장 + 계층 추적 |
| 29장 | 관측성 | `Activity` / OpenTelemetry — 효과 구간 분산 추적 |

---

## 3개 장의 구성

### 27장 — Schedule / 재시도와 반복

`Schedule` 은 다음에 언제 시도할지를 담은 (잠재적으로 무한한) `Duration` 스트림입니다. 학습용 `MySchedule` 을 직접 정의해 `recurs(n)` / `spaced(d)` / `exponential(d)` / `fibonacci` 를 구현하고, union `|` (둘 중 짧은 간격) 과 intersect `&` (둘 중 긴 간격) 으로 스케줄을 합성합니다. `effect.Retry(schedule)` / `effect.Repeat(schedule)` 이 효과에 얹히는 자리를 봅니다.

### 28장 — Resource & bracket / 자원 수명

`bracket` 은 획득–사용–해제 세 단계를 한 값으로 묶어, 사용 중 예외가 나도 해제를 보장합니다. `Resources` 클래스의 계층적 추적 (parent/child, `Acquire` / `Release` / `ReleaseAll`) 을 직접 구현하며, `EnvIO` 가 자원을 운반하는 7부의 그림과 연결합니다. `using` 의 함수형·효과적 대응입니다.

### 29장 — Observability / Activity · OpenTelemetry

효과를 관측 가능하게 만듭니다. `Activity` 효과 (`LanguageExt.Sys` 의 `Sys.Diag.Activity`) 로 효과 구간을 span 으로 감싸 분산 추적을 남깁니다. .NET `System.Diagnostics.Activity` / OpenTelemetry 와 어떻게 정합하는지, 그리고 추적이 효과를 바꾸지 않고 감싸는 횡단 관심사임을 봅니다.

---

## 8부의 코드

본문 예제는 모두 `code/Part08-RobustEffects/` 에 실행 가능한 형태로 들어 있습니다 (외부 패키지 의존 0).

```bash
dotnet run --project code/Part08-RobustEffects/Ch27-Schedule/Ch27.csproj
dotnet run --project code/Part08-RobustEffects/Ch28-Resource/Ch28.csproj
dotnet run --project code/Part08-RobustEffects/Ch29-Observability/Ch29.csproj
```

코드 예제 요약 — Ch27 는 `Schedule` 을 `Duration` 스트림으로 구현해 `recurs`/`spaced`/`exponential` 과 union `|`(짧은 간격·긴 길이)·intersect `&`(긴 간격·짧은 길이) 합성을 보이고, `recurs(5) & exponential(10ms)` 를 실패하는 효과에 `retry` 로 얹습니다. Ch28 는 `bracket`(획득–사용–해제)이 예외에도 해제를 보장 함과 `Resources` 의 LIFO 해제를 보입니다. Ch29 은 `Activity`/`Tracer` 로 효과 구간을 중첩 span 트리로 추적하되 결과를 바꾸지 않는 횡단 관심사임을 보입니다. 세 도구 모두 7부 효과 위에 조합으로 얹힙니다.

학습용 구현은 LanguageExt v5 의 발상과 정합합니다 (`Schedule` 의 union/intersect, `bracket` 의 finally 보장, `Activity` 추적). 단, `Schedule` 은 `IEnumerable<Duration>` 으로, 추적은 `System.Diagnostics.Activity` 대신 자체 `Tracer` 로 단순화했습니다 (외부 의존 0).

---

## 8부의 진입점

7부를 마쳤다면 Ch27 — Schedule 부터 시작합니다. 재시도 정책을 값으로 보는 발상이 8부의 "효과 위에 조합으로 얹는다" 는 주제를 가장 선명하게 보여 줍니다.
