# Ch34 챌린지

속성 기반 테스트(property-based)를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① 생성기 + 축소(shrinking)

`Types/Prop.cs` — `Gen<A>` 로 무작위 입력을 만들고 `Prop.ForAll` 로 성질을 N회 검사한다. 실패하면 *축소* 로 최소 반례를 찾는다. `Program.cs` 에서 "모든 n < 100" 의 최소 반례가 **100** 으로 줄어듦을 본다.

**노리는 능력** — 특정 예시가 아니라 *모든 입력에 성립하는 성질* 을 검사하는 발상, 그리고 shrinking 이 디버깅을 돕는 이유를 본다. (실무: CsCheck / FsCheck.)

## 심화 챌린지 (선택)

### ② 정렬의 성질

`SortProp.cs` — 올바른 정렬은 *길이 보존 + 오름차순* 성질을 만족한다. 틀린 "정렬"(중복 제거)은 길이 보존 성질을 어겨 반례가 잡힌다.

**노리는 능력** — 함수의 *성질* 을 명세로 삼아 구현을 검증하는 패턴. 합법칙 검증(33장)이 property-based 의 한 사례임을 본다.

## 실행

```bash
dotnet run --project code/Part9-FunctionalTesting/Ch34-Property-Based/Ch34.csproj
```
