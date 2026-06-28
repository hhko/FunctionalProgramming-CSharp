# Ch19 챌린지

변환기의 발상과 `lift` 를 다룬다. *필수* 한 개 + *심화* 한 개.

## 필수 챌린지

### ① OptionT 를 내부 모나드 위에 얹기

`Safe.cs` — `OptionT<ManyF, int>` 위에서 안전한 역수 계산. 내부 모나드 `Many` (비결정성) 와 `Option` 층 (실패) 이 한 스택으로 합쳐져, 각 갈래가 독립적으로 성공/실패한다 (결과 `Many<Option<int>>`).

**노리는 능력** — 18장에서 손으로 짠 배관이 *내부 모나드 M 에 대해 한 번만* 작성되어 재사용됨을 본다. `OptionT<M, A>` 의 `Bind` 는 M 이 무엇이든 통한다.

## 심화 챌린지 (선택)

### ② 다른 내부 모나드로 교체

`Program.cs` 를 참고해 `OptionT` 의 내부 모나드를 `Many` 대신 다른 모나드(예: 단일값 Identity)로 바꿔 보라. *같은 OptionT 코드* 가 그대로 동작함을 확인한다.

**노리는 능력** — 변환기가 내부 모나드에 *다형적* 임을 체감한다. 이것이 18장의 고정된 ReaderOption 과의 결정적 차이다.

## 실행

```bash
dotnet run --project code/Part06-MonadTransformers/Ch19-Transformer-Idea/Ch19.csproj
```
