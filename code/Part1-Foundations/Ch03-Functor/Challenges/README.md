# Ch03 챌린지

본 디렉토리에는 [`§3.9`](../../../../Part1-Foundations/Ch03-Functor.md#39-직접-해보기--ch03-챌린지) 의 챌린지 정답이 들어 있다. *필수 챌린지* 두 개 + *심화 챌린지* 두 개로 구성된다.

## 필수 챌린지

### ① `Tree<A>` Functor 부착 + 두 법칙 검증

`TreeFunctor.cs` — 이진 트리 `Tree<A>` 에 Functor 인스턴스를 부착한다. List / Maybe 와 달리 *재귀 자료 구조* 가 핵심.

**노리는 능력** — *3-tuple 패턴을 재귀 자료에도 그대로 적용*. *§3.6 의 두 법칙이 내 구현에서 성립함을 코드로 검증*.

**검증 포인트**
- 모양 보존 — 입력 트리의 *구조 (Branch / Leaf 배치)* 가 출력에서 그대로 유지되는가
- 항등 법칙 — `Map(x => x, tree)` 가 원본과 같은가
- 합성 법칙 — `Map(g, Map(f, tree)) == Map(g ∘ f, tree)` 가 성립하는가

### ② Functor 인지 아닌지 분류

`FunctorClassify.md` — 다섯 후보 메서드 시그니처를 보고 *Functor 의 `Map` 인지 / 다른 trait 의 자리인지* 분류한다.

**노리는 능력** — *§3.8.1 시그니처 분류표를 새 메서드에 직접 적용*. *모양 보존 + 원소 개수 + 변환 함수 + 시그니처 유형* 네 기준으로 판정.

**검증 포인트** — 정답을 보기 전에 종이에 답 + 근거를 적은 뒤 비교. *근거가 같은 자리* 를 짚었는지가 중요.

## 심화 챌린지 (선택)

### ③ `Const<C, _>` Functor

`ConstFunctor.cs` — 두 번째 매개변수 `A` 가 *phantom* (시그니처에만 등장, 실제로 안 들고 있음) 인 컨테이너에 Functor 부착.

**왜 흥미로운가** — `Map` 의 변환 함수 `f` 가 *한 번도 호출되지 않는다*. *함수 호출 0회여도 두 법칙이 자동 성립* 하는 경계 사례. 19장 *Applicative 효과 분리* 의 출발점.

### ④ `Pair<L, _>` Functor

`PairFunctor.cs` — 두 값 (Left, Right) 중 *Right 만* 변환되는 Functor.

**왜 흥미로운가** — 두 매개변수 중 *한쪽만 변환하는* 패턴이 일반화되면 *양쪽 모두 변환할 수 있는 trait* 가 된다. 17장 *Bifunctor* 의 도입.

## 실행

```bash
dotnet run --project code/Part1-Foundations/Ch03-Functor/Ch03.csproj
```

`Program.cs` 의 콘솔 데모가 챌린지 코드를 *모두 실행* 한다. 데모 출력으로 각 챌린지의 결과를 확인할 수 있다.

## xUnit 으로 옮기려면

학습용 csproj 는 콘솔 출력으로 검증을 보여 준다. xUnit 으로 옮기려면 새 테스트 csproj 를 추가하고 `Tests/FunctorLaws.cs` 의 헬퍼를 그대로 `[Fact]` 로 감싸면 된다.

```xml
<ItemGroup>
  <PackageReference Include="xunit" Version="2.9.0" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
  <PackageReference Include="Shouldly" Version="4.2.1" />
</ItemGroup>
```
