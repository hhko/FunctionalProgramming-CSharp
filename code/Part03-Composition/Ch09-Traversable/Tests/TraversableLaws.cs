using Ch09.Functions;
using Ch09.Traits;
using Ch09.Types;

namespace Ch09.Tests;

// Traversable 의 법칙을 학습용 헬퍼로 묶은 모듈.
//
// xUnit + Shouldly 로 옮기려면 각 호출을 [Fact] 로 감싸고 ShouldBe 단언만 추가하면 된다.
// 본 학습용 csproj 는 콘솔 데모이므로 bool 반환으로 통과 / 실패를 표시한다.
//
// 본문 §9.10 의 세 법칙 (자연성 / 항등 / 합성) 중 *항등* 의 학습용 버전과
// §9.8 의 sequence = traverse(id) 등식을 검증한다. 정밀한 형식화와 property-based
// 자동 검증은 9부로 넘어간다 (본문 §9.15 테스트 디딤돌).
public static class TraversableLaws
{
    // ① 항등 법칙 (학습용 버전) — Traverse(Pure, t) 가 t 를 보존한다.
    //
    //   각 원소 a 를 *효과 없는* 끌어올림 (a => F.Pure(a)) 으로 변환하면,
    //   Traverse 는 바깥 구조 T 의 모양을 그대로 둔 채 전체를 F.Pure 로 감싼다.
    //   곧 Traverse(Pure, t) ≡ F.Pure(t) 가 성립한다.
    //
    //   probe 는 비교를 위해 K<T, A> 의 원소를 순서대로 꺼내는 함수
    //   (예: MyList 면 Items). T 의 모양 보존을 원소 나열로 확인한다.
    public static bool IdentityHolds<T, F, A>(
        K<T, A>                        ta,
        Func<K<T, A>, IEnumerable<A>>  probe)
        where T : Traversable<T>
        where F : Applicative<F>
    {
        // 좌변 — Traverse(Pure, t). 항등 효과로 끌어올린 뒤 층을 뒤집는다.
        K<F, K<T, A>> lhs = Traversable.traverse<T, F, A, A>(a => F.Pure(a), ta);

        // 우변 — Pure(t). 바깥 구조를 통째로 한 번 끌어올린 것.
        K<F, K<T, A>> rhs = F.Pure(ta);

        // F 안의 T 를 꺼내 원소 나열로 비교. 두 값 모두 F.Pure 안에 같은 T 를 담아야 한다.
        IEnumerable<A>? left  = ExtractInner<T, F, A>(lhs, probe);
        IEnumerable<A>? right = ExtractInner<T, F, A>(rhs, probe);
        return left is not null
            && right is not null
            && left.SequenceEqual(right);
    }

    // ② sequence = traverse(id) 등식 — 두 표기가 같은 결과를 낸다.
    //
    //   sequence 는 Traversable 의 virtual default 로, traverse 에 항등 함수를 넘긴 것이다.
    //   여기서는 trait virtual default (T.Sequence) 와 traverse(x => x) 를 직접 비교해
    //   두 경로가 같은 K<F, K<T, A>> 를 냄을 확인한다.
    public static bool SequenceEqualsTraverseId<T, F, A>(
        K<T, K<F, A>>                  tfa,
        Func<K<T, A>, IEnumerable<A>>  probe)
        where T : Traversable<T>
        where F : Applicative<F>
    {
        // 좌변 — virtual default sequence.
        K<F, K<T, A>> viaSequence = Traversable.sequence<T, F, A>(tfa);

        // 우변 — traverse(id). 변환 함수 자리에 항등 함수 (x => x) 를 넘긴 것.
        K<F, K<T, A>> viaTraverseId = Traversable.traverse<T, F, K<F, A>, A>(x => x, tfa);

        IEnumerable<A>? left  = ExtractInner<T, F, A>(viaSequence, probe);
        IEnumerable<A>? right = ExtractInner<T, F, A>(viaTraverseId, probe);
        return left is not null
            && right is not null
            && left.SequenceEqual(right);
    }

    // F.Pure 안에 담긴 K<T, A> 를 꺼내 probe 로 원소를 나열한다.
    //
    // F 가 MyMaybe 일 때만 직접 분기해 꺼낸다. F 가 Nothing 이면 null 을 돌려
    // *항등 / 등식 자리에서는 발생하지 않아야 할* 단락을 호출 측이 감지하게 한다.
    // 다른 Applicative F 로 확장하려면 이 추출기만 늘리면 된다.
    static IEnumerable<A>? ExtractInner<T, F, A>(
        K<F, K<T, A>>                  fta,
        Func<K<T, A>, IEnumerable<A>>  probe)
        where T : Traversable<T>
        where F : Applicative<F>
    =>
        fta switch
        {
            MyMaybe<K<T, A>>.Just j  => probe(j.Value),
            MyMaybe<K<T, A>>.Nothing => null,
            _ => throw new InvalidOperationException(
                     "ExtractInner 는 MyMaybeF 만 지원합니다. 다른 F 는 분기를 추가하십시오.")
        };

    // MyList 원소 나열 probe — 비교에 쓰는 표준 추출기.
    public static IEnumerable<A> ProbeList<A>(K<MyListF, A> ta) => ta.As().Items;
}
