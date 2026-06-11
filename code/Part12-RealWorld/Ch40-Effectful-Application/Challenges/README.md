# Ch40 챌린지

효과 기반 애플리케이션을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 다중 능력 조합 (Console + Clock + Store)

`NoteApp.cs` — 노트를 *시각과 함께* 저장하고 목록을 출력하는 워크플로. `where RT : Has<RT, IConsole>, Has<RT, IClock>, Has<RT, IStore>` 로 세 능력을 조합 요구하되, 효과 코드는 구현(Live/Memory)을 모른다.

**노리는 능력** — 5부 `Eff<RT>` + `Has` DI 가 실무 앱으로 확장됨을 본다. 능력을 타입으로 조합한다.

## 심화 챌린지 (선택)

### ② 테스트 런타임으로 앱 전체 검증

`Tests/AppTests.cs` — `MemoryConsole`/`FixedClock`/`MemoryStore` 를 주입해 앱 워크플로를 *부수 작용 없이 결정적으로* 검증한다 (콘솔 출력 + 저장소 상태 + 반환값).

**노리는 능력** — 9부 테스트 더블이 실무 앱 테스트로 이어짐을 본다.

## 실행

```bash
dotnet run --project code/Part10-RealWorld/Ch40-Effectful-Application/Ch40.csproj
```
