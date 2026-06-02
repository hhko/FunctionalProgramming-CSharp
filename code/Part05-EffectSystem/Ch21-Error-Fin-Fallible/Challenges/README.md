# Ch21 챌린지

함수형 오류 모델(Error/Fin/Fallible)을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① Fin 파이프라인의 단락

`Program.cs` 의 검증 파이프라인 — 여러 단계를 `Bind` 로 잇고 한 단계가 `Fail(Error)` 면 전체가 *그 오류로* 단락한다. 예외를 던지지 않고 오류가 값으로 흐른다.

**노리는 능력** — `Bind` 가 첫 실패에서 단락하는 monadic 오류 처리를 본다. (4부 Applicative 누적과 대비 — Fin 의 Bind 는 단락.)

## 심화 챌린지 (선택)

### ② Try 로 예외를 값으로 + Catch 복구

`Try.Run` 으로 예외를 던지는 코드를 `Fin` 으로 포획하고, `Fallible.Catch` 로 실패를 복구한다.

**노리는 능력** — 명령형 `try/catch` 가 `Try` (예외→Error) + `Catch` (복구) 두 함수형 도구로 분리됨을 본다.

## 실행

```bash
dotnet run --project code/Part5-EffectSystem/Ch21-Error-Fin-Fallible/Ch21.csproj
```
