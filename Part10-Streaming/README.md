# Part 10 — Streaming (스트리밍)

## 10부의 배경

4부의 컬렉션은 메모리에 모두 올라간 유한한 자료였습니다. 하지만 실무 데이터는 종종 끝이 없거나 (이벤트 스트림, 센서 입력), 메모리에 다 담을 수 없습니다 (대용량 파일, DB 커서). 이런 데이터를 한꺼번에 컬렉션으로 들면 메모리가 터집니다.

10부의 출발점은 **효과적 스트리밍** 입니다. 데이터를 한 조각씩 당겨오며 (pull) 효과 (IO) 와 함께 흘려보냅니다.

- `StreamT<M, A>` — 내부 모나드 `M` 위에서 `A` 를 차례로 흘려보내는 효과적 스트림. 6부 변환기 위에 섭니다.
- Pipes — `Producer<O, A>` (생산) / `Consumer<I, A>` (소비) / `Pipe<I, O, A>` (변환) 를 조합 해 데이터 파이프라인을 만듭니다. 코루틴 기반이라 reactive 구독과 다릅니다.
- `Conduit<I, O, M, A>` — 또 다른 스트리밍 추상.

**스트림이 효과를 품은 lazy 시퀀스이며, Producer / Consumer / Pipe 가 합성 가능한 조각이라는 것** 이 10부의 결정적 발상입니다. 기초의 합성 어휘가 무한 / 대용량 데이터로 확장됩니다.

---

## 10부의 목표

10부를 마치면 독자는 다음을 갖게 됩니다.

- `StreamT<M, A>` 의 직접 구현 — 내부 모나드 `M` 위에서 값을 한 조각씩 흘려보내는 lazy 효과 스트림.
- 당김 기반 (pull-based) 스트리밍이 왜 메모리 안전하고 어떻게 역압 (backpressure) 을 자연히 갖는지.
- Pipes 의 세 역할 — `Producer<O, A>` / `Consumer<I, A>` / `Pipe<I, O, A>` — 과 이들을 잇는 합성 (`|` 등) 의 직접 구현.
- `Conduit` 와 Pipes 의 차이, 그리고 어느 자리에서 무엇을 쓰는지.
- 스트리밍이 5 ~ 8부의 효과 / Schedule / Resource 와 결합되어 실전 데이터 파이프라인을 이루는 패턴.

무한 / 대용량 데이터를 메모리 안전하게 합성하는 스트리밍 추상의 발상과 직접 구현 능력이 10부의 도달점입니다.

---

## 10부의 무대 — 흐르는 Elevated World

| 시민 | 시그니처 | 역할 |
|---|---|---|
| **StreamT\<M, A\>** | 내부 `M` 위의 lazy 시퀀스 | `A` 를 한 조각씩 효과와 함께 흘림 |
| **Producer\<O, A\>** | `O` 를 yield, `A` 로 종료 | 데이터 생산 |
| **Consumer\<I, A\>** | `I` 를 await, `A` 로 종료 | 데이터 소비 |
| **Pipe\<I, O, A\>** | `I → O` 변환 | 생산과 소비 사이 변환 |

스트림은 효과를 품은 컬렉션입니다. 4부의 `Map` / `Filter` / `Fold` 가 그대로 쓰이되, 전체가 메모리에 올라오지 않고 한 조각씩 작동합니다.

---

## 10부의 학습 흐름

| 장 | 두 축에서의 자리 | 한 줄 |
|---|---|---|
| 33장 | 효과적 스트림 | `StreamT<M, A>` — 내부 `M` 위의 lazy 효과 시퀀스 |
| 34장 | 조합 가능한 파이프 | Pipes `Producer` / `Consumer` / `Pipe` 와 합성 |
| 35장 | 실전 파이프라인 | `Conduit` + Schedule / Resource 결합 데이터 파이프라인 |

---

## 3개 장의 구성

### 33장 — StreamT / 효과적 스트림

`StreamT<M, A>` 는 내부 모나드 `M` (보통 `IO`) 위에서 `A` 를 차례로 흘려보내는 lazy 스트림입니다. 학습용 `MyStreamT<M, A>` 를 직접 정의해 한 조각을 당기면 다음이 계산되는 구조를 봅니다. 6부 변환기 위에 서므로 `Map` / `Bind` 가 그대로 작동하고, 무한 스트림이 메모리를 터뜨리지 않는 이유를 코드로 확인합니다.

### 34장 — Pipes / Producer · Consumer · Pipe

스트리밍을 세 역할의 조합으로 봅니다. `Producer<O, A>` 는 값을 yield, `Consumer<I, A>` 는 값을 await, `Pipe<I, O, A>` 는 둘 사이를 변환합니다. 학습용 버전을 직접 구현하고 `producer | pipe | consumer` 합성으로 파이프라인을 잇습니다. 코루틴 기반 당김 모델이 reactive 구독과 어떻게 다른지, 역압이 왜 자연히 생기는지 봅니다.

### 35장 — Conduit & 실전 파이프라인

`Conduit<I, O, M, A>` 라는 또 다른 스트리밍 추상을 다루고 Pipes 와 비교합니다. 그리고 8부의 `Schedule` (주기적 폴링) / `Resource` (스트림 소스의 안전한 개폐) 와 결합해 대용량 파일 → 변환 → 적재 같은 실전 데이터 파이프라인을 구성합니다. 12부 실무 예제의 스트리밍 시나리오로 이어집니다.

---

## 10부의 코드

본문 예제는 모두 `code/Part10-Streaming/` 에 실행 가능한 형태로 들어 있습니다 (외부 패키지 의존 0).

```bash
dotnet run --project code/Part10-Streaming/Ch33-StreamT/Ch33.csproj
dotnet run --project code/Part10-Streaming/Ch34-Pipes/Ch34.csproj
dotnet run --project code/Part10-Streaming/Ch35-Conduit/Ch35.csproj
```

코드 예제 요약 — Ch33 은 당김 기반 lazy `StreamT` 로 무한 스트림을 `Take` 로 유한 소비하고, 당긴 만큼만 부수 효과가 일어남(무한 소수 체 포함)을 보입니다. Ch34 은 `Producer`/`Pipe`/`Consumer` 를 `Through`/`Then`/`Run`(= `producer | pipe | consumer`) 으로 합성하고, 무한 `Producer` + `Take` 로 역압(backpressure) 을 실증합니다. Ch35 는 `Conduit` 변환관과, 열고 반드시 닫는 자원 파이프라인 위 ETL(파싱→필터→합산, 예외에도 close 보장)을 8부 bracket 과 결합해 보입니다.

학습용 구현은 LanguageExt v5 의 발상과 정합합니다 (당김 기반 스트림, Producer/Consumer/Pipe 합성, Conduit). 단, 내부 모나드 M 을 지연 pull 로 고정하고 `Conduit` 을 `IEnumerable` 변환으로 단순화했습니다 (외부 의존 0).

---

## 10부의 진입점

9부를 마쳤다면 Ch33 — StreamT 부터 시작합니다. "효과를 품은 lazy 시퀀스" 라는 한 발상이 10부 전체 (Pipes / Conduit) 의 토대입니다.
