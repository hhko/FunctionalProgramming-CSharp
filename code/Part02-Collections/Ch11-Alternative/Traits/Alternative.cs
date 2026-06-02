namespace Ch11.Traits;

// Alternative — Choice + Applicative 의 합류 + 자체 단위원 Empty.
//
// LanguageExt v5 의 Alternative.Trait.cs 와 정합 — 그쪽도
//   `interface Alternative<F> : Choice<F>, Applicative<F> { static abstract K<F,A> Empty<A>(); }`
// 처럼 *Empty 를 직접 선언* 한다 (MonoidK 를 상속하지 *않는다*).
// 대신 자료 타입이 MonoidK 와 Alternative 를 *각각 따로* 구현한다 (라이브러리의 Seq 가 그렇다).
//
// "여러 시도 중 하나를 고르되, 전부 실패하면 Empty" 라는 패턴을 추상화한다.
// 파서 결합자 (`p1 <|> p2`), 대체 값 (`opt ?? fallback`) 의 일반형.
public interface Alternative<F> : Choice<F>, Applicative<F>
    where F : Alternative<F>
{
    // 단위원 — 라이브러리는 Alternative 가 Empty 를 *직접* 선언한다.
    static abstract K<F, A> Empty<A>();

    // virtual — 여러 후보 중 처음으로 성공하는 것 (전부 실패하면 Empty).
    // LanguageExt 의 Alternative.Choice<A>(Seq<K<F,A>>) 에 해당하는 학습용 helper.
    static virtual K<F, A> OneOf<A>(params K<F, A>[] options)
    {
        var acc = F.Empty<A>();
        foreach (var opt in options)
            acc = F.Choose(acc, opt);
        return acc;
    }
}
