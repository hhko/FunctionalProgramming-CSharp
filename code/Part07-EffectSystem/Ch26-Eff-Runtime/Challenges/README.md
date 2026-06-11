# Ch26 챌린지

`Eff<RT,A> = ReaderT<RT, IO, A>` 와 `Has<RT,Trait>` DI 를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 같은 효과 코드, 다른 런타임 (DI + 테스트 용이성)

`Program.cs` — 콘솔 능력을 쓰는 *하나의 효과 코드* 를 `AppRT(LiveConsole)` 와 `AppRT(TestConsole)` 두 런타임으로 각각 Run 한다. 효과 코드는 한 줄도 바뀌지 않는다.

**노리는 능력** — `Eff<RT,A>` 가 `ReaderT<RT, IO, A>` (4부 변환기) 임을, 그리고 `Has<RT, IConsole>` 제약이 *능력 기반 DI* 를 컴파일타임에 보장함을 본다. `TestConsole` 로 부수 작용 없이 결정적으로 테스트할 수 있다 (9부 미리보기).

## 심화 챌린지 (선택)

### ② 능력 추가

`IClock` 같은 새 능력을 정의하고 `AppRT` 가 `Has<AppRT, IClock>` 도 구현하게 한 뒤, 시간에 의존하는 효과를 작성하라.

**노리는 능력** — 런타임에 능력을 *조합* 해 쌓는 패턴 (`where RT : Has<RT,IConsole>, Has<RT,IClock>`) 을 익힌다. 10부 실무 앱의 토대다.

## 실행

```bash
dotnet run --project code/Part5-EffectSystem/Ch26-Eff-Runtime/Ch26.csproj
```
