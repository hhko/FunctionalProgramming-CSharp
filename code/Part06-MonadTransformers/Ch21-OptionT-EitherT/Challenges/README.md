# Ch21 챌린지

오류·부재 변환기(OptionT/EitherT)와 스택 순서를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① EitherT 는 "왜 실패했는지" 를 남긴다

`Parse.cs` — `EitherT<string, ManyF, int>` 로 여러 입력을 검사하되, 실패 시 `None` 이 아니라 `Left("에러 메시지")` 로 *이유* 를 보존한다. (16장 `OptionT` 의 `None` 과 대비.)

**노리는 능력** — 실패에 정보가 필요할 때 `OptionT` 대신 `EitherT` 를 고르는 판단. 두 변환기는 같은 자리에 서지만 *실패의 표현* 이 다르다.

## 심화 챌린지 (선택)

### ② 스택 순서가 의미를 바꾼다

`Program.cs` 의 설명 — `EitherT<L, ManyF, A> = Many<Either<L, A>>` 는 "각 갈래가 자기 오류로 실패" (비결정 구조 생존). 만약 순서가 반대 (`ManyT<EitherF…>` = `Either<L, Many<A>>`) 였다면 *한 번의 실패가 전체를 죽인다*.

**노리는 능력** — 변환기 스택에서 *어느 효과가 바깥인가* 가 의미를 결정함을 본다.

## 실행

```bash
dotnet run --project code/Part4-MonadTransformers/Ch21-OptionT-EitherT/Ch21.csproj
```
