# Ch09 챌린지

순서 컬렉션에 1부의 trait 을 부착하는 챌린지다. *필수* 두 개 + *심화* 한 개.

## 필수 챌린지

### ① `MyLst<A>` — cons 연결 리스트에 같은 Monad 부착

`MyLst.cs` — `MySeq` 는 `IEnumerable` 백킹이었지만 `MyLst` 는 *재귀적 cons 구조* (`Cons` / `Nil`) 다.

**노리는 능력** — 자료 *표현* 이 완전히 달라도 *같은 trait* (Functor / Applicative / Monad / Foldable) 가 그대로 붙음을 확인. 추상은 표현이 아니라 *시그니처가 약속하는 동작* 에 달려 있다.

**검증 포인트**
- `MySeq` 와 `MyLst` 에 대해 *같은 generic 함수* (`Monad.bind`, `FoldableExtensions.Count`) 가 동작하는가
- Monad 세 법칙 (좌·우 항등, 결합) 이 `MyLst` 에서도 성립하는가

### ② 시퀀스 연산 분류

`ClassifyOps.md` — `Select` / `Where` / `Aggregate` / `SelectMany` / `First` 가 각각 *어느 trait 의 자리* 인지 분류한다.

**노리는 능력** — LINQ 의 익숙한 메서드들이 사실 Functor / Monad / Foldable 의 별명이었음을 시그니처로 본다.

## 심화 챌린지 (선택)

### ③ `ZipSeq<A>` — 시퀀스의 두 번째 Applicative

`ZipSeq.cs` — `MySeq.Apply` 는 *데카르트 곱* 이지만, 시퀀스에는 *zip (짝 맞춤)* 이라는 또 하나의 적법한 Applicative 가 있다.

**왜 흥미로운가** — 같은 자료 타입에 *서로 다른 Applicative* 가 둘 존재할 수 있다 (Haskell 의 `[]` vs `ZipList`). 단, zip 의 `Pure` 는 무한 반복이라야 법칙이 성립하므로 학습용으로는 `Apply` 만 시연한다.

## 실행

```bash
dotnet run --project code/Part2-Collections/Ch09-Sequences/Ch09.csproj
```

`Program.cs` 의 콘솔 데모가 챌린지 코드를 모두 실행한다.

## xUnit 으로 옮기려면

9부 (Ch33~35) 에서 `Tests/` 의 bool 헬퍼를 그대로 `[Fact]` + Shouldly 로 감싸는 표준을 다룬다.
