# Ch29 챌린지

동시 컬렉션과 인과성을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 동시 삽입 무손실

`Program.cs` — `AtomHashMap` 에 여러 스레드가 동시에 삽입해도 항목이 유실되지 않는다 (CAS 가 불변 맵 교체를 보호). 2부 불변 컬렉션을 27장 원자적 참조로 감싼 형태.

**노리는 능력** — 락 없는 동시 컬렉션이 "불변 자료 + 원자적 참조" 의 조합임을 본다.

## 심화 챌린지 (선택)

### ② VectorClock 으로 인과성 추적

`Causality.cs` — 메시지 송수신 시나리오에서 `VectorClock` 으로 *happens-before* 와 *concurrent* 를 구분한다. 송신 이벤트는 수신 후 이벤트보다 `Before` 다.

**노리는 능력** — 분산 이벤트의 *부분 순서* 를 시계 비교로 판정하는 기초를 익힌다 (인과 무관 = Concurrent).

## 실행

```bash
dotnet run --project code/Part7-Concurrency/Ch29-Concurrent-Collections/Ch29.csproj
```
