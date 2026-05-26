# Ch05 챌린지

5장 §5.9 의 챌린지 정답 코드.

| 파일 | 챌린지 | 본문 자리 |
|---|---|---|
| `../Types/MyList.cs` | 필수 ① — MyList cartesian Applicative + 법칙 검증 | §5.9.1 |
| (Program.cs 안 `MyLift4` 부분) | 필수 ② — Lift4 직접 작성 + 회원가입 폼 누적 | §5.9.2 |
| `EitherApplicative.cs` | 심화 B — `MyEither` 단락 Applicative | §5.9.3.B |
| `PairApplicative.cs` | 심화 C — `Pair<W, A>` Writer 의 단순형 Applicative | §5.9.3.C |

`A. ConstApplicativeF<M>` (심화 A) 는 `IMonoid<M>` 의 추상이 필요해 본 챕터 범위 밖.
8장 (Traversable) 이후 부록에서 다룬다.

각 챌린지 정답을 `dotnet run` 으로 직접 호출하려면 `Program.cs` 의 데모 절에서 인스턴스를
사용하는 호출을 추가한다.
