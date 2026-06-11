using Ch05.Challenges;
using Ch05.Functions;
using Ch05.Tests;
using Ch05.Traits;
using Ch05.Types;

Console.WriteLine("================================================");
Console.WriteLine("5장 — Applicative / Pure + Apply / LiftN");
Console.WriteLine("================================================");
Console.WriteLine();

// ─── §5.5.1 Curry — 다인자 → 한 인자씩 ───────────────────────────────────
Console.WriteLine("== §5.5.1 — Curry 변환 ==");
Func<int, int, int> add = (a, b) => a + b;
var addCurried = Curry.Of(add);
Console.WriteLine($"  add(2, 3)              = {add(2, 3)}");
Console.WriteLine($"  addCurried(2)(3)       = {addCurried(2)(3)}");
Console.WriteLine($"  부분 적용 — addCurried(10)(7) = {addCurried(10)(7)}");
Console.WriteLine();

// ─── §5.3 첫 인스턴스 — MyMaybe Applicative ──────────────────────────────
// §5.1.1 의 add 를 curried 형태로 → Map(첫 인자) → Apply(둘째 인자) 로 결합.
Console.WriteLine("== §5.3 — MyMaybe.Pure + Apply (add 의 elevated 결합) ==");
Func<int, Func<int, int>> addC  = x => y => x + y;
K<MyMaybeF, int>            five  = MyMaybeF.Pure(5);
K<MyMaybeF, int>            three = MyMaybeF.Pure(3);

// ── 학습용 펼친 형태 — Map (첫 인자) → Apply (둘째 인자) 2단계 사슬
// Step 1: Map 으로 첫 인자 5 적용 → "5 를 더하는 함수" 가 Just 안에
K<MyMaybeF, Func<int, int>> mf = MyMaybeF.Map<int, Func<int, int>>(addC, five);
// Step 2: Apply 로 둘째 인자 3 결합 → Just(8)
K<MyMaybeF, int>            r1 = MyMaybeF.Apply<int, int>(mf, three);
Console.WriteLine($"  [펼친 형태] Map(addC, Just(5)).Apply(Just(3)) = {r1.As()}    ← 2단계 사슬");

// ── 실전 압축형 — Lift2 한 줄 (Curry → Pure → Apply → Apply 의 캡슐화)
K<MyMaybeF, int> r4 = Lift.Lift2<MyMaybeF, int, int, int>((x, y) => x + y, five, three);
Console.WriteLine($"  [압축형]    Lift2((x, y) => x + y, Just(5), Just(3)) = {r4.As()}    ← 1단계, 같은 결과");

K<MyMaybeF, Func<int, int>> nothingF = MyMaybe<Func<int, int>>.Nothing.Instance;
K<MyMaybeF, int>            sd       = nothingF.Apply(three);
Console.WriteLine($"  Nothing.Apply(Just(3))                       = {sd.As()}    ← 단락 (함수가 없음)");
Console.WriteLine();

// ─── §5.5 어떤 Applicative 든 — Lift2 / Lift3 / Lift4 ───────────────────
Console.WriteLine("== §5.5 — Lift2 / Lift3 / Lift4 (MyMaybe) ==");
K<MyMaybeF, int> a = MyMaybeF.Pure(7);
K<MyMaybeF, int> b = MyMaybeF.Pure(3);

K<MyMaybeF, int> sum2 = Lift.Lift2<MyMaybeF, int, int, int>((x, y) => x + y, a, b);
Console.WriteLine($"  Lift2(+, Just(7), Just(3))           = {sum2.As()}");

K<MyMaybeF, int> sum3 = Lift.Lift3<MyMaybeF, int, int, int, int>(
    (x, y, z) => x + y + z, a, b, MyMaybeF.Pure(100));
Console.WriteLine($"  Lift3(+, Just(7), Just(3), Just(100)) = {sum3.As()}");

K<MyMaybeF, string> sum4 = LiftN.Lift4<MyMaybeF, int, int, int, int, string>(
    (w, x, y, z) => $"sum={w + x + y + z}",
    MyMaybeF.Pure(1), MyMaybeF.Pure(2), MyMaybeF.Pure(3), MyMaybeF.Pure(4));
Console.WriteLine($"  Lift4(... ,1,2,3,4)                    = {sum4.As()}");

// 한 측 Nothing — 단락
K<MyMaybeF, int> nothing = MyMaybe<int>.Nothing.Instance;
K<MyMaybeF, int> sumWithNothing = Lift.Lift2<MyMaybeF, int, int, int>(
    (x, y) => x + y, a, nothing);
Console.WriteLine($"  Lift2(+, Just(7), Nothing)             = {sumWithNothing.As()}    ← 단락");
Console.WriteLine();

// ─── §5.4 둘째 인스턴스 — MyValidation Applicative (누적) ───────────────
Console.WriteLine("== §5.4 — MyValidation 누적 vs MyMaybe 단락 ==");
var valid7 = MyValidationF<string>.Pure(7);
var validA = MyValidationF<string>.Pure(3);
K<MyValidationF<string>, int> validSum =
    Lift.Lift2<MyValidationF<string>, int, int, int>((x, y) => x + y, valid7, validA);
Console.WriteLine($"  Lift2(Valid(7), Valid(3))             = {ShowValidation(validSum)}");

K<MyValidationF<string>, int> err1 = new MyValidation<string, int>.Invalid(["나이가 부족"]);
K<MyValidationF<string>, int> err2 = new MyValidation<string, int>.Invalid(["이메일 형식 오류"]);
K<MyValidationF<string>, int> errSum =
    Lift.Lift2<MyValidationF<string>, int, int, int>((x, y) => x + y, err1, err2);
Console.WriteLine($"  Lift2(에러1, 에러2)                    = {ShowValidation(errSum)}   ← 두 에러 *누적*");
Console.WriteLine();

// 회원가입 폼 4 입력 누적 (Lift4)
Console.WriteLine("== §5.4.3 — 회원가입 폼의 4 입력 누적 ==");
var nameErr  = new MyValidation<string, string>.Invalid(["이름이 비어 있습니다"]);
var emailErr = new MyValidation<string, string>.Invalid(["이메일 형식 오류"]);
var ageErr   = new MyValidation<string, int>.Invalid(["나이는 양수"]);
var tierErr  = new MyValidation<string, string>.Invalid(["등급 미지원"]);

K<MyValidationF<string>, string> user = LiftN.Lift4<
    MyValidationF<string>, string, string, int, string, string>(
    (n, e, ag, t) => $"User({n}, {e}, {ag}, {t})",
    nameErr, emailErr, ageErr, tierErr);
Console.WriteLine($"  Lift4(폼 4 입력 모두 실패)             = {ShowValidationS(user)}");
Console.WriteLine();

// ─── §5.9.1 챌린지 ① — MyList cartesian Applicative ─────────────────────
Console.WriteLine("== §5.9.1 (챌린지 ①) — MyList cartesian Applicative ==");
K<MyListF, Func<int, int>> fs = new MyList<Func<int, int>>([n => n + 1, n => n * 10]);
K<MyListF, int>            ns = new MyList<int>([1, 2, 3]);
var listR = MyListF.Apply<int, int>(fs, ns);
Console.WriteLine($"  [+1, *10] × [1, 2, 3] = {listR.As()}   ← 2 × 3 = 6 원소");

K<MyListF, int> sumList = Lift.Lift2<MyListF, int, int, int>(
    (x, y) => x + y,
    new MyList<int>([1, 2]),
    new MyList<int>([10, 20, 30]));
Console.WriteLine($"  Lift2(+, [1,2], [10,20,30]) = {sumList.As()}");
Console.WriteLine();

// ─── §5.6 네 법칙 검증 ─────────────────────────────────────────────────
Console.WriteLine("== §5.6 — 다섯 법칙 검증 (MyMaybe) ==");
K<MyMaybeF, int> sample = new MyMaybe<int>.Just(7);
Func<K<MyMaybeF, int>, IEnumerable<int>> probeMaybeI = m =>
    m.As() switch { MyMaybe<int>.Just j => [j.Value], _ => [] };
Func<K<MyMaybeF, string>, IEnumerable<string>> probeMaybeS = m =>
    m.As() switch { MyMaybe<string>.Just j => [j.Value], _ => [] };

bool id  = ApplicativeLaws.IdentityHolds<MyMaybeF, int>(sample, probeMaybeI);
bool hom = ApplicativeLaws.HomomorphismHolds<MyMaybeF, int, string>(
                n => $"#{n}", 42, probeMaybeS);
bool inter = ApplicativeLaws.InterchangeHolds<MyMaybeF, int, string>(
                MyMaybeF.Pure<Func<int, string>>(n => $"#{n}"), 42, probeMaybeS);
bool comp  = ApplicativeLaws.CompositionHolds<MyMaybeF, int, int, int>(
                MyMaybeF.Pure<Func<int, int>>(n => n + 1),
                MyMaybeF.Pure<Func<int, int>>(n => n * 10),
                new MyMaybe<int>.Just(7),
                m => m.As() switch { MyMaybe<int>.Just j => [j.Value], _ => [] });
bool fnc = ApplicativeLaws.FunctorConsistencyHolds<MyMaybeF, int, string>(
                n => $"#{n}", new MyMaybe<int>.Just(7), probeMaybeS);

Console.WriteLine($"  ① Identity              : {(id   ? "통과" : "실패")}");
Console.WriteLine($"  ② Homomorphism          : {(hom  ? "통과" : "실패")}");
Console.WriteLine($"  ③ Interchange           : {(inter? "통과" : "실패")}");
Console.WriteLine($"  ④ Composition           : {(comp ? "통과" : "실패")}");
Console.WriteLine($"  ⑤ Functor 정합 (Map ≡ Apply∘Pure) : {(fnc ? "통과" : "실패")}");
Console.WriteLine();

// ─── §5.6.7 임의 입력 property — 3장 §3.7.1 의 ForAll 재사용 ─────────────
// 법칙은 특정 값이 아니라 모든 입력의 약속이므로, 임의의 MyMaybe<int> 100 건에
// 검증한다. 컨테이너 입력만으로 변주 가능한 항등·Functor 정합 법칙을 우선한다.
// (함수 인자 표본 x => x + 1 은 고정.)
Console.WriteLine("== §5.6.7 — 임의 입력 100 건 property (ForAll) ==");
Func<Random, K<MyMaybeF, int>> genMaybe = r =>
    r.Next(2) == 0
        ? new MyMaybe<int>.Just(r.Next(-1000, 1000))
        : MyMaybe<int>.Nothing.Instance;
Func<int, int> plusOne = x => x + 1;

bool idAll = Property.ForAll(genMaybe,
    fa => ApplicativeLaws.IdentityHolds<MyMaybeF, int>(
        fa, m => m.As() switch { MyMaybe<int>.Just j => [j.Value], _ => [] }));
bool fncAll = Property.ForAll(genMaybe,
    fa => ApplicativeLaws.FunctorConsistencyHolds<MyMaybeF, int, int>(
        plusOne, fa, m => m.As() switch { MyMaybe<int>.Just j => [j.Value], _ => [] }));

Console.WriteLine($"  ① Identity      (임의 100 건) : {(idAll  ? "통과" : "위반")}");
Console.WriteLine($"  ⑤ Functor 정합  (임의 100 건) : {(fncAll ? "통과" : "위반")}");
Console.WriteLine();

// ─── §5.6.8 가짜 Applicative — 법칙 위반 시연 ──────────────────────────
Console.WriteLine("== §5.6.8 — 가짜 Applicative 의 법칙 위반 ==");
K<BogusApplicativeF, int> bogusFa = new BogusApp<int>([1, 2, 3]);
bool bogusId = ApplicativeLaws.IdentityHolds<BogusApplicativeF, int>(
                bogusFa, fa => fa.As().Items);
Console.WriteLine($"  BogusApplicativeF — Identity   : {(bogusId ? "통과" : "위반 (예상대로)")}");

bool dupHom = ApplicativeLaws.HomomorphismHolds<DupApplicativeF, int, int>(
                n => n * 2, 7, fb => fb.As().Items);
Console.WriteLine($"  DupApplicativeF   — Homomorphism: {(dupHom ? "통과" : "위반 (예상대로)")}");
Console.WriteLine();

// ─── §5.9.3.B 심화 — MyEither 단락 Applicative ──────────────────────────
Console.WriteLine("== §5.9.3.B (심화) — MyEither 단락 Applicative ==");
var leftA = new MyEither<string, int>.Left("첫 에러");
var leftB = new MyEither<string, int>.Left("둘째 에러");
K<MyEitherF<string>, int> eitherSum = Lift.Lift2<MyEitherF<string>, int, int, int>(
    (x, y) => x + y, leftA, leftB);
Console.WriteLine($"  Lift2(Left(첫), Left(둘)) = {ShowEither(eitherSum)}    ← *첫 에러만* — 단락");
Console.WriteLine();

// ─── §5.9.3.C 심화 — Pair Writer Applicative ────────────────────────────
Console.WriteLine("== §5.9.3.C (심화) — Pair Writer Applicative ==");
var p1 = new Pair<int>("[step1]", 7);
var p2 = new Pair<int>("[step2]", 3);
K<PairF, int> pairSum = Lift.Lift2<PairF, int, int, int>((x, y) => x + y, p1, p2);
Console.WriteLine($"  Lift2(p1, p2)    = {pairSum.As()}    ← 로그 monoid 누적");
Console.WriteLine();

Console.WriteLine("================================================");
Console.WriteLine("5장 완료 — 6장 Foldable 로 진행합니다");
Console.WriteLine("================================================");

// ─── 출력 헬퍼 ─────────────────────────────────────────────────────────
static string ShowValidation(K<MyValidationF<string>, int> v) =>
    v.As() switch
    {
        MyValidation<string, int>.Valid x   => $"Valid({x.Value})",
        MyValidation<string, int>.Invalid e => $"Invalid({string.Join(" + ", e.Errors)})",
        _ => "?"
    };

static string ShowValidationS(K<MyValidationF<string>, string> v) =>
    v.As() switch
    {
        MyValidation<string, string>.Valid x   => $"Valid({x.Value})",
        MyValidation<string, string>.Invalid e => $"Invalid({string.Join(" + ", e.Errors)})",
        _ => "?"
    };

static string ShowEither(K<MyEitherF<string>, int> e) =>
    e.As() switch
    {
        MyEither<string, int>.Right r => $"Right({r.Value})",
        MyEither<string, int>.Left  l => $"Left({l.Error})",
        _ => "?"
    };
