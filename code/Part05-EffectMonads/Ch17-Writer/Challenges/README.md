# Ch17 챌린지

누적 출력 효과 모나드(Writer)를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 계산 과정 로그 추적

`TracedMath.cs` — 각 연산이 결과와 함께 한 줄의 로그를 `Tell` 한다. LINQ 로 이으면 로그가 *Monoid 로 누적* 된다 (1부 Ch02 Monoid 의 회수). logger 를 전역/인자로 들고 다니던 명령형 패턴이 사라진다.

**노리는 능력** — `Bind` 가 두 단계의 출력을 `Combine` 으로 합치고 `Pure` 의 출력이 `Empty` 임을 본다. 누적이 *Monoid 법칙* 위에 선다.

## 심화 챌린지 (선택)

### ② Listen / Pass

`Program.cs` 의 Listen·Pass 예제 — `Listen` 으로 하위 계산이 "말한" 출력을 값으로 들여다보고, `Pass` 로 출력을 후처리(검열)한다.

**노리는 능력** — Writer 가 단순 누적을 넘어 *출력 자체를 다루는* 도구 (`Listen`/`Pass`) 를 가짐을 본다.

## 실행

```bash
dotnet run --project code/Part3-EffectMonads/Ch17-Writer/Ch17.csproj
```
