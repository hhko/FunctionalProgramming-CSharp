# Ch35 챌린지

Conduit 와 실전 파이프라인을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 자원 안전한 ETL 파이프라인

`Etl.cs` — 로그 라인을 파싱(실패 제외) → 임계값 필터 → 합산하는 `Conduit` 을, *열고 반드시 닫는* 자원 파이프라인 위에서 실행한다. sink 가 예외를 던져도 닫힘이 보장된다 (8부 bracket 결합).

**노리는 능력** — 스트리밍 변환(`Conduit.Then`)과 자원 수명(open/close 보장)이 한 파이프라인으로 합성됨을 본다.

## 심화 챌린지 (선택)

### ② Pipes 와 Conduit 의 차이

`Program.cs` 의 설명 — Conduit 은 변환 단계를 `IEnumerable<I> → IEnumerable<O>` 하나로 합치고(34장 Pipes 는 Producer/Consumer/Pipe 세 역할로 분리). 어느 자리에서 무엇을 쓰는지 비교하라.

**노리는 능력** — 두 스트리밍 추상의 트레이드오프(역할 분리 vs 단일 변환관)를 구분한다.

## 실행

```bash
dotnet run --project code/Part10-Streaming/Ch35-Conduit/Ch35.csproj
```
