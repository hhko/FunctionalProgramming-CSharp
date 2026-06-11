# Ch07 챌린지

7장 §7.11 의 챌린지 정답 코드.

| 챌린지 | 정답 | 본문 자리 |
|---|---|---|
| 1 — `Bind` 사슬을 시그니처 따라 추적 / 중간 단락 짚기 | `MonadChallenges.cs` — `TraceBind()` / `TraceBindShortCircuit()` | §7.11.1 |
| 2 — `f >=> g` 를 `Bind` 만으로 직접 작성 + 좌항등 설명 | `MonadChallenges.cs` — `KleisliByBind(...)` / `LeftIdentityDemo(...)` | §7.11.2 |
| 3 — LINQ 예제를 중첩 `Bind` 표기로 다시 적고 동치 확인 | `MonadChallenges.cs` — `ViaLinq()` / `ViaNestedBind()` | §7.11.3 |
| 4 — 다섯 시그니처가 어느 trait 자리인지 분류 | `MonadClassify.md` (해설 문서) | §7.11.4 |

챌린지 1 ~ 3 정답은 `Program.cs` 의 챌린지 절에서 호출해 `dotnet run` 으로 결과를 확인할 수 있다. 챌린지 4 는 시그니처 분류라 코드가 아닌 해설 문서 (`MonadClassify.md`) 로 정답을 둔다.
