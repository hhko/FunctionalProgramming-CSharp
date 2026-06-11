# Ch15 챌린지

환경 의존 효과 모나드(Reader)를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 전역 변수 없는 의존성 주입

`BudgetReader.cs` — 세율·통화 기호를 담은 `AppConfig` 를 *전역에 두지 않고* Reader 로 주입한다. 여러 Reader 를 LINQ `from-from-select` 로 합성해도 환경은 단 한 번 `Run(config)` 에서 공유된다.

**노리는 능력** — `Bind` 가 *같은 환경을 다음 단계로 암묵 전달* 하는 것을 코드로 본다. config 를 매 함수에 넘기던 명령형 패턴이 사라진다.

## 심화 챌린지 (선택)

### ② `Local` 로 환경 국소 변경

`Program.cs` 의 Local 예제 — 특정 하위 계산만 *변형된 환경* 에서 실행한다 (예: 세율을 일시적으로 0 으로). 바깥 계산의 환경은 그대로다.

**노리는 능력** — `Local : (Env→Env) → K<M,A> → K<M,A>` 가 어떻게 *스코프가 한정된* 환경 변경을 주는지 본다.

## 실행

```bash
dotnet run --project code/Part3-EffectMonads/Ch15-Reader/Ch15.csproj
```
