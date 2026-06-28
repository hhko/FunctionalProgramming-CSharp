# Ch29 챌린지

관측성(Activity / 분산 추적)을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 효과를 감싸는 추적 (결과 불변)

`Program.cs` 의 추적 예제 — `activity(name, effect)` 로 효과 구간을 span 으로 감싼다. 중첩 호출이 부모-자식 트리를 만들고, *추적은 효과의 결과를 바꾸지 않는다* (횡단 관심사).

**노리는 능력** — 관측성이 효과를 *바꾸지 않고 감싸는* 도구임을 본다. `System.Diagnostics.Activity` / OpenTelemetry 의 발상과 정합.

## 심화 챌린지 (선택)

### ② 예외 안전한 구간 종료

`Tests/TracingLaws.cs` — 구간 안에서 예외가 나도 span 이 올바르게 닫혀 다음 추적이 오염되지 않음을 확인한다 (28장 bracket 의 finally 와 같은 보장).

**노리는 능력** — 추적·자원·재시도가 모두 "효과 위에 조합으로 얹히는" 8부의 공통 주제임을 정리한다.

## 실행

```bash
dotnet run --project code/Part08-RobustEffects/Ch29-Observability/Ch29.csproj
```
