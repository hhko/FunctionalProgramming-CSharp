namespace Ch10.Traits;

// Biapplicative — Bifunctor 에 두 인자 버전의 Pure / Apply 를 더한 trait.
//
// 5장 Applicative 가 Functor 에 Pure / Apply 를 더했듯, Biapplicative 는 Bifunctor 에
// 두 자리 모두를 채우는 BiPure 와 두 자리에 각각 함수를 적용하는 BiApply 를 더한다.
//
// BiPure  : L → A → F<L, A>
//   두 값을 각 자리에 그냥 감싼다. (주의: v5 의 Biapplicative 에는 BiPure 가 없고 BiApply
//   한 멤버뿐이다. 이 BiPure 는 데모 입력을 만들기 위한 이 책의 학습용 보조 멤버다.)
//
// BiApply : F<(A → C), (B → D)> → F<A, B> → F<C, D>
//   두 자리에 각각 함수를 담은 컨테이너 + 두 자리에 값을 담은 컨테이너 →
//   두 함수를 동시에 적용한 컨테이너. Applicative 의 Apply 가 두 자리로 늘어난 것.
public interface Biapplicative<F> : Bifunctor<F>
    where F : Biapplicative<F>
{
    // 두 값을 각 자리에 감싸기.
    static abstract K<F, L, A> BiPure<L, A>(L first, A second);

    // 두 자리의 함수를 두 자리의 값에 동시에 적용.
    static abstract K<F, C, D> BiApply<A, B, C, D>(
        K<F, Func<A, C>, Func<B, D>> fab, K<F, A, B> fcd);
}

// Bimonad — 개념 주석 (학습용 코드 미제공).
//
// 1-인자 가족 (Functor → Applicative → Monad) 의 발상이 2-인자로 반복되되, 상속 모양은 다르다.
// 1-인자는 사슬 (Monad : Applicative : Functor) 이지만, 2-인자는 Biapplicative 와 Bimonad 가
// 각각 Bifunctor 를 따로 상속하는 별 모양이다 (Bimonad 는 Biapplicative 를 거치지 않는다).
// Bimonad 는 Bifunctor 를 상속해 두 갈래 각각에 bind 를 약속한다. LanguageExt v5 의
// 실제 멤버는 BindFirst / BindSecond 두 개로, 각각 첫 자리 / 둘째 자리를 World-crossing 으로
// 다음 컨테이너에 잇는다 (BiBind 라는 단일 멤버는 v5 에 없다).
//
// 다만 두 자리의 bind 결과가 같은 모양으로 합쳐져야 해 규칙이 까다롭고, v5 의 정식 trait
// 으로도 좁게 쓰여 이 책에서는 시그니처 직감까지만 짚고 코드는 두지 않는다.
