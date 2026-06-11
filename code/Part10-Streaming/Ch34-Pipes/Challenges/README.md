# Ch34 챌린지

Pipes(Producer/Consumer/Pipe)를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 역압 (backpressure)

`Program.cs` — 무한 `Producer.Tapped` 에 `Take(3)` 파이프를 끼우면 상류가 정확히 3개만 생산한다. Consumer 가 당긴 만큼만 Producer 가 일한다 (코루틴 기반 당김 모델).

**노리는 능력** — Pipes 가 reactive 구독(push)과 달리 *당김(pull)* 기반이라 역압이 자연히 생김을 본다.

## 심화 챌린지 (선택)

### ② 파이프라인으로 집계

`WordCount.cs` — 생산 → `Map`(정규화) `Then` `Filter`(빈 단어 제거) → `Consumer`(개수) 를 합성한다. 각 역할이 독립적이고 재사용 가능하다.

**노리는 능력** — `producer.Through(pipe).Run(consumer)` (= `producer | pipe | consumer`) 합성으로 파이프라인을 조립하는 패턴을 익힌다.

## 실행

```bash
dotnet run --project code/Part8-Streaming/Ch34-Pipes/Ch34.csproj
```
