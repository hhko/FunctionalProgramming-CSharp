using Ch09.Functions;
using Ch09.Traits;
using Ch09.Types;

namespace Ch09.Challenges;

// 본문 §9.11 의 세 챌린지 정답.
//
//   챌린지 1 — Traverse(evenCheck, [2, 3, 6]) 가 왜 Nothing 인지 Apply 사슬로 추적.
//   챌린지 2 — 안쪽 효과 F 자리에 8장 MyValidation 을 넣으면 누적이 되는 까닭 (개념).
//   챌린지 3 — sequence([Just 1, Just 2, Just 3]) = traverse(x => x, ...) 동일 결과 확인.
//
// 챌린지 2 의 누적은 이 프로젝트의 Tests/ValidationTraverse.cs 에서 MyValidation 으로 실증된다.
// 여기 챌린지 파일에서는 단락(MyMaybe) 추적에 집중하고, 누적 실증은 그 테스트를 참고한다.
public static class TraversableChallenges
{
    // ── 챌린지 1 — 단락 추적 ──────────────────────────────────────────────
    //
    // Traverse(evenCheck, [2, 3, 6]) 의 Apply 사슬:
    //   MyListF.Traverse 는 뒤에서 앞으로 누적한다. 각 원소를 evenCheck 로 변환하면
    //     evenCheck(2) = Just 2,  evenCheck(3) = Nothing,  evenCheck(6) = Just 6.
    //   prepend 를 Apply 로 엮는데, 한 단계라도 인자가 Nothing 이면
    //   MyMaybeF.Apply 의 분기가 Nothing 을 돌려준다 (단락).
    //   곧 3 의 Nothing 이 그 단계의 Apply 결과를 Nothing 으로 만들고,
    //   이후 Apply 도 Nothing 을 전파해 전체가 Nothing 이 된다.
    //
    // 반환값 — Traverse 결과 자체. 호출 측이 Just / Nothing 을 확인한다.
    public static K<MyMaybeF, K<MyListF, int>> ShortCircuit(IEnumerable<int> source)
    {
        K<MyListF, int> nums = new MyList<int>(source);

        Func<int, K<MyMaybeF, int>> evenCheck = n =>
            n % 2 == 0
                ? MyMaybeF.Pure(n)
                : MyMaybe<int>.Nothing.Instance;

        return MyListF.Traverse<MyMaybeF, int, int>(evenCheck, nums);
    }

    // 단락 여부 판정 — Just 면 true (모두 짝수), Nothing 이면 false (홀수 하나라도).
    public static bool AllPass(K<MyMaybeF, K<MyListF, int>> result) =>
        result.As() is MyMaybe<K<MyListF, int>>.Just;

    // ── 챌린지 2 — F 자리에 MyValidation 을 넣으면 누적 (개념) ──────────────
    //
    // [개념 — Ch09 코드에는 MyValidation 이 없어 주석으로만 설명]
    //
    // 안쪽 효과 F 를 8장의 MyValidationF<E> 로 바꾸면, 변환 함수는
    //   parseEven : int → K<MyValidationF<string>, int>
    //     짝수면 Valid(n), 홀수면 Invalid(["{n} 은 홀수"]).
    // Traverse 가 엮는 Apply 가 MyValidation 의 Apply 이므로,
    //   (Invalid fe, Invalid ae) 분기에서 [..fe.Errors, ..ae.Errors] 로 *오류를 모은다*.
    // 따라서 [3, 5, 7] 처럼 셋이 모두 홀수면 오류 3 건이 한 Invalid 에 누적된다.
    //
    // 곧 *같은 Traverse* 가 안쪽 F 의 Apply 분기에 따라
    //   MyMaybe → 단락 (첫 실패에서 전체 Nothing),
    //   MyValidation → 누적 (모든 실패를 모은 Invalid)
    // 으로 갈린다. Traverse 자체는 단락 / 누적을 모른다. F 의 Apply 가 정한다.
    //
    // MyMaybe 로 실증 가능한 *단락* 쪽만 코드로 확인하는 헬퍼:
    //   세 원소가 모두 홀수여도 MyMaybe 는 오류를 모으지 못하고 단일 Nothing 만 낸다.
    public static bool ShortCircuitLosesAllButFirst(IEnumerable<int> allOdd)
    {
        var result = ShortCircuit(allOdd);
        // 홀수가 몇 개든 결과는 단일 Nothing — 오류 개수 정보가 사라진다.
        // MyValidation 이라면 이 자리에 오류가 누적됐을 것이다 (위 개념 설명).
        return !AllPass(result);
    }

    // ── 챌린지 3 — sequence = traverse(id) ────────────────────────────────
    //
    // sequence([Just 1, Just 2, Just 3]) 를 traverse(x => x, ...) 로 다시 적어
    // 두 표기가 같은 결과 Just [1, 2, 3] 을 냄을 확인한다.
    public static (K<MyMaybeF, K<MyListF, int>> ViaSequence,
                   K<MyMaybeF, K<MyListF, int>> ViaTraverseId)
        SequenceVsTraverseId(IEnumerable<int> values)
    {
        // [Just 1, Just 2, Just 3] 형태의 K<MyListF, K<MyMaybeF, int>> 구성.
        var listOfJusts = new MyList<K<MyMaybeF, int>>(
            values.Select(v => (K<MyMaybeF, int>)new MyMaybe<int>.Just(v)));

        // 표기 ① — sequence (Traversable virtual default).
        K<MyMaybeF, K<MyListF, int>> viaSequence =
            Traversable.sequence<MyListF, MyMaybeF, int>(listOfJusts);

        // 표기 ② — traverse(x => x).
        K<MyMaybeF, K<MyListF, int>> viaTraverseId =
            Traversable.traverse<MyListF, MyMaybeF, K<MyMaybeF, int>, int>(x => x, listOfJusts);

        return (viaSequence, viaTraverseId);
    }

    // 두 결과가 같은지 — 둘 다 Just 이고 안쪽 리스트 원소가 순서까지 일치하는지.
    public static bool SameResult(
        K<MyMaybeF, K<MyListF, int>> a,
        K<MyMaybeF, K<MyListF, int>> b)
    =>
        (a.As(), b.As()) switch
        {
            (MyMaybe<K<MyListF, int>>.Just ja, MyMaybe<K<MyListF, int>>.Just jb) =>
                ja.Value.As().Items.SequenceEqual(jb.Value.As().Items),
            (MyMaybe<K<MyListF, int>>.Nothing, MyMaybe<K<MyListF, int>>.Nothing) =>
                true,
            _ => false
        };
}
