# Ch27 챌린지

재시도/반복 정책(Schedule)을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 재시도 정책을 값으로

`Program.cs` 의 retry 예제 — `Schedule.Recurs(n) | Schedule.Exponential(d)` 같은 *값* 을 만들어 실패하는 효과에 얹는다. 정책(횟수·간격)이 코드 흐름과 분리된 *데이터* 가 된다.

**노리는 능력** — 재시도가 if/loop 가 아니라 *조합 가능한 값* 임을 본다. union `|` / intersect `&` 로 정책을 합성한다.

## 심화 챌린지 (선택)

### ② 정책 합성의 의미

`Tests/ScheduleLaws.cs` — `|` 는 더 짧은 간격·긴 길이, `&` 는 더 긴 간격·짧은 길이를 택함을 확인한다. 예: "최대 5회, 단 지수 백오프" = `recurs(5) & exponential(10ms)`.

**노리는 능력** — 두 조합자의 의미 차이를 정확히 구분하고, 실무 재시도 정책을 합성으로 표현한다.

## 실행

```bash
dotnet run --project code/Part08-RobustEffects/Ch27-Schedule/Ch27.csproj
```
