# Ch23 챌린지

지연 효과 IO 와 스택 안전성을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 깊은 Bind 체인의 스택 안전성

`Program.cs` 의 스택 안전 예제 — 100,000 단계의 `Bind` 체인을 만들어 `Run` 한다. 트램폴린 인터프리터가 *C# 재귀 대신 힙 위의 연속 스택* 으로 해석하므로 `StackOverflowException` 이 나지 않는다.

**노리는 능력** — IO 가 "값으로 인코딩된 효과 프로그램" 이라 *해석 방식* 을 우리가 정한다는 것. 일반 `Func` 합성이라면 깊은 중첩이 스택을 터뜨리지만, 노드 트리 + 트램폴린은 안전하다.

## 심화 챌린지 (선택)

### ② EnvIO 의 취소

`Program.cs` 의 취소 예제 — `EnvIO` 에 이미 취소된 `CancellationToken` 을 실어 Run 하면 인터프리터가 각 단계에서 취소를 확인해 중단한다.

**노리는 능력** — `Run` 시점의 환경(`EnvIO`)이 실행을 *제어* 함을 본다. 5부 뒤쪽의 자원·런타임이 여기에 얹힌다.

## 실행

```bash
dotnet run --project code/Part5-EffectSystem/Ch23-IO/Ch23.csproj
```
