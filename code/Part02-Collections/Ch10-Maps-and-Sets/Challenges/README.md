# Ch10 챌린지

키-값 / 집합 컨테이너의 trait 부착을 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① Set 은 왜 (무제약) Functor 가 아닌가

`WhySetIsNotFunctor.md` — `Map<Key,V>` 에는 Functor 가 깔끔히 붙는데 `Set<A>` 에는 안 붙는 두 근거(시그니처 제약 충돌 + 모양 보존 위반)를 설명한다.

**노리는 능력** — trait 부착의 *경계* 를 본다. 시그니처와 법칙 양쪽에서 왜 안 되는지 설명할 수 있어야 한다.

## 심화 챌린지 (선택)

### ② "키 변환" 은 왜 Functor Map 이 아닌가

`KeyMapping.cs` — Functor 의 `Map` 은 *값만* 바꾸고 키는 보존했다. 키 자체를 바꾸는 `MapKeys` 는 (1) 충돌 가능, (2) 재정렬 필요 → Functor 의 자리가 아니다. 충돌이 실제로 일어나는 것을 시연한다.

**노리는 능력** — 키-값 컨테이너에서 Functor 가 *왜 값에만 작용하는지* 를 반례로 굳힌다.

## 실행

```bash
dotnet run --project code/Part2-Collections/Ch10-Maps-and-Sets/Ch10.csproj
```
