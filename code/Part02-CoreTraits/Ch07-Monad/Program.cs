using Ch07.Challenges;
using Ch07.Functions;
using Ch07.Tests;
using Ch07.Traits;
using Ch07.Types;

Console.WriteLine("================================================");
Console.WriteLine("7장 — Monad + LINQ");
Console.WriteLine("================================================");
Console.WriteLine();

// 파싱 함수 — 정수면 Just, 아니면 Nothing.
static K<MyMaybeF, int> ParseInt(string s) =>
    int.TryParse(s, out var n)
        ? MyMaybeF.Pure(n)
        : MyMaybe<int>.Nothing.Instance;

Console.WriteLine("== 예제 1 — Bind 명시 호출 ==");

K<MyMaybeF, int> byBind = ParseInt("3").Bind(a =>
    ParseInt("4").Bind(b =>
        MyMaybeF.Pure(a + b)));
Console.WriteLine($"  ParseInt(\"3\") + ParseInt(\"4\") = {Show(byBind)}");

Console.WriteLine();
Console.WriteLine("== 예제 2 — LINQ from-select (같은 결과) ==");

K<MyMaybeF, int> byLinq =
    from a in ParseInt("3")
    from b in ParseInt("4")
    select a + b;
Console.WriteLine($"  from a in 3 from b in 4 select a+b = {Show(byLinq)}");

Console.WriteLine();
Console.WriteLine("== 예제 3 — 단락 회로 (한 단계 실패) ==");

K<MyMaybeF, int> shortCircuit =
    from a in ParseInt("3")
    from b in ParseInt("xyz")             // ← Nothing 반환
    from c in ParseInt("5")               // ← 실행도 안 됨
    select a + b + c;
Console.WriteLine($"  중간에 \"xyz\" → {Show(shortCircuit)}    (b 이후는 평가 안 됨)");

Console.WriteLine();
Console.WriteLine("== 예제 4 — 다단계 LINQ ==");

K<MyMaybeF, int> threeStep =
    from a in ParseInt("10")
    from b in ParseInt("20")
    from c in ParseInt("30")
    select a + b + c;
Console.WriteLine($"  10 + 20 + 30 = {Show(threeStep)}");

// 동기 함수 — 역수. 0 이면 Nothing (a → E<b> 유형, §7.1).
static K<MyMaybeF, int> Reciprocal(int n) =>
    n == 0
        ? MyMaybe<int>.Nothing.Instance
        : MyMaybeF.Pure(1000 / n);   // 정수 역수 근사 (1000 / n)

Console.WriteLine();
Console.WriteLine("== 예제 5 — Kleisli 합성 >=> (Then) ==");

// f >=> g — 타입이 어긋나 막혔던 ParseInt 와 Reciprocal 이 한 줄로 이어진다 (§7.5).
Func<string, K<MyMaybeF, int>> parse  = MonadChallenges.ParseInt;
Func<int, K<MyMaybeF, int>>    recip  = Reciprocal;
Func<string, K<MyMaybeF, int>> pipe   = parse.Then(recip);
Console.WriteLine($"  (ParseInt >=> Reciprocal)(\"4\")   = {Show(pipe("4"))}    ← 1000/4");
Console.WriteLine($"  (ParseInt >=> Reciprocal)(\"0\")   = {Show(pipe("0"))}    ← 0 이라 Nothing");
Console.WriteLine($"  (ParseInt >=> Reciprocal)(\"xyz\") = {Show(pipe("xyz"))}    ← 파싱 실패라 Nothing");

Console.WriteLine();
Console.WriteLine("== 예제 6 — flatten (중첩 E<E<a>> 평탄화) ==");

// Map(Reciprocal, …) 은 한 겹 더 겹친 MyMaybe<MyMaybe<int>> 를 만든다 (§7.7).
K<MyMaybeF, K<MyMaybeF, int>> nested =
    MyMaybeF.Map<int, K<MyMaybeF, int>>(Reciprocal, MyMaybeF.Pure(4));
K<MyMaybeF, int> flat = Monad.flatten<MyMaybeF, int>(nested);
Console.WriteLine($"  flatten(Just(Just(250)))         = {Show(flat)}    ← 한 겹이 펴짐");

Console.WriteLine();
Console.WriteLine("== 예제 7 — Kleisli 항등원 (pure >=> f ≡ f) ==");

// Id() = Pure 가 합성의 왼/오른 항등원 (§7.5.2).
Func<int, K<MyMaybeF, int>> idK = Kleisli.Id<MyMaybeF, int>();
Func<int, K<MyMaybeF, int>> pureThenRecip = idK.Then(recip);   // pure >=> Reciprocal
Console.WriteLine($"  Reciprocal(5)                    = {Show(recip(5))}");
Console.WriteLine($"  (pure >=> Reciprocal)(5)         = {Show(pureThenRecip(5))}    ← 같은 값 (좌항등)");

Console.WriteLine();
Console.WriteLine("== 예제 8 — 두 번째 Monad: MyList 의 비결정성 (§7.3.4) ==");

K<MyListF, int> xs = new MyList<int>(new[] { 1, 2, 3 });
// 각 n 마다 [n, n*10] 두 결과 → 모두 이어붙임 (bind = map 후 flatten)
K<MyListF, int> fanOut = xs.Bind<MyListF, int, int>(n => new MyList<int>(new[] { n, n * 10 }));
Console.WriteLine($"  [1,2,3].Bind(n => [n, n*10])       = {ShowList(fanOut)}   ← 비결정성(fan-out)");

// bind 이 map 후 flatten 임을 직접 확인 — map 은 [[1,10],[2,20],[3,30]], flatten 은 이어붙임
K<MyListF, K<MyListF, int>> mappedLi = MyListF.Map<int, K<MyListF, int>>(
    n => new MyList<int>(new[] { n, n * 10 }), xs);
K<MyListF, int> flatLi = Monad.flatten<MyListF, int>(mappedLi);
Console.WriteLine($"  flatten(map(n=>[n,n*10], [1,2,3]))  = {ShowList(flatLi)}   ← bind 과 같은 값");
Console.WriteLine($"  같은 Bind 시그니처, 다른 얼굴 — Maybe=단락, List=비결정성");

Console.WriteLine();
Console.WriteLine("== 예제 9 — LINQ 가 List 에서는 모든 조합 (§7.6.4) ==");

// MyMaybe 에서 단락을 만들던 같은 from-from-select 가, List 에서는 cartesian (모든 조합) 을 만든다.
K<MyListF, string> sizes  = new MyList<string>(new[] { "S", "M" });
K<MyListF, string> colors = new MyList<string>(new[] { "Red", "Blue" });
K<MyListF, string> combos =
    from s in sizes
    from c in colors
    select $"{s}-{c}";
Console.WriteLine($"  from s in [S,M] from c in [Red,Blue]  = {ShowList(combos)}");

Console.WriteLine();
Console.WriteLine("== §7.6.3 실전 — 의존 사슬 (조회 → 조회 → 출금) ==");
Console.WriteLine($"  출금 (id=1, 300)   bind : {ShowR(Bank.WithdrawByBind(new UserId(1), 300))}");
Console.WriteLine($"  출금 (id=1, 300)   LINQ : {ShowR(Bank.WithdrawByLinq(new UserId(1), 300))}");
Console.WriteLine($"  잔액 부족 (id=1, 5000)  : {ShowR(Bank.WithdrawByLinq(new UserId(1), 5000))}    ← Withdraw 단락");
Console.WriteLine($"  없는 사용자 (id=99)     : {ShowR(Bank.WithdrawByLinq(new UserId(99), 300))}    ← FindUser 단락");

Console.WriteLine();
Console.WriteLine("== §7.11 챌린지 1 — Bind 사슬 추적 ==");
Console.WriteLine($"  ParseInt(\"12\") >>= n.ParseInt(\"3\") >>= m.Pure(n*m) = {Show(MonadChallenges.TraceBind())}    ← Just(36)");
Console.WriteLine($"  중간을 ParseInt(\"x\") 로 바꾸면                       = {Show(MonadChallenges.TraceBindShortCircuit())}    ← 첫 단계에서 단락");

Console.WriteLine();
Console.WriteLine("== §7.11 챌린지 2 — f >=> g 를 Bind 로 직접 + 좌항등 ==");
var (viaCompose, viaDirect) = MonadChallenges.LeftIdentityDemo(41);
Console.WriteLine($"  (pure >=> f)(41)                 = {Show(viaCompose)}");
Console.WriteLine($"  f(41)                            = {Show(viaDirect)}    ← 같은 값 (Bind(Pure(a), f) ≡ f(a))");

Console.WriteLine();
Console.WriteLine("== §7.11 챌린지 3 — LINQ ↔ 중첩 Bind 동치 ==");
Console.WriteLine($"  ViaLinq()       (from-from-select) = {Show(MonadChallenges.ViaLinq())}");
Console.WriteLine($"  ViaNestedBind() (중첩 Bind)        = {Show(MonadChallenges.ViaNestedBind())}    ← 같은 값");

Console.WriteLine();
Console.WriteLine("== §7.9 — 세 법칙 검증 (MyMaybe) ==");

// 본문 §7.9 의 좌항등 / 우항등 / 결합 을 학습용 헬퍼로 검증.
Func<K<MyMaybeF, int>, IEnumerable<int>> probeI = m =>
    m.As() switch { MyMaybe<int>.Just j => [j.Value], _ => [] };

Func<int, K<MyMaybeF, int>> fLaw = n => MonadChallenges.ParseInt((n + 1).ToString());
Func<int, K<MyMaybeF, int>> gLaw = n => Reciprocal(n);

bool leftId  = MonadLaws.LeftIdentityHolds<MyMaybeF, int, int>(7, fLaw, probeI);
bool rightId = MonadLaws.RightIdentityHolds<MyMaybeF, int>(new MyMaybe<int>.Just(7), probeI);
bool assoc   = MonadLaws.AssociativityHolds<MyMaybeF, int, int, int>(
                   new MyMaybe<int>.Just(7), fLaw, gLaw, probeI);

Console.WriteLine($"  ① 좌항등 Bind(Pure(a), f) ≡ f(a)         : {(leftId  ? "통과" : "실패")}");
Console.WriteLine($"  ② 우항등 Bind(m, Pure)    ≡ m            : {(rightId ? "통과" : "실패")}");
Console.WriteLine($"  ③ 결합   Bind(Bind(m,f),g) ≡ Bind(m,..)  : {(assoc   ? "통과" : "실패")}");

Console.WriteLine();
Console.WriteLine("== §7.9.2 — 세 법칙을 임의 입력 100 건으로 (3장 §3.7.1 ForAll) ==");

// 법칙은 특정 값이 아니라 *모든 입력* 의 약속이므로, 임의의 MyMaybe<int> 100 건에 검증한다.
// World-crossing 표본 함수 f, g 는 고정하고 입력 m 만 변주한다 (a → E<b> 유형).
Func<Random, K<MyMaybeF, int>> genMaybe = r =>
    r.Next(2) == 0
        ? MyMaybeF.Pure(r.Next(-1000, 1000))
        : MyMaybe<int>.Nothing.Instance;
Func<int, K<MyMaybeF, int>> fProp = n => n > 50 ? MyMaybeF.Pure(n + 1) : MyMaybe<int>.Nothing.Instance;
Func<int, K<MyMaybeF, int>> gProp = n => MyMaybeF.Pure(n * 10);

bool leftIdAll = Property.ForAll(genMaybe, m =>
    m.As() is MyMaybe<int>.Just j
        ? MonadLaws.LeftIdentityHolds<MyMaybeF, int, int>(j.Value, fProp, probeI)
        : true);   // 좌항등은 Pure(a) 에서 출발 — Just 인 입력의 값만 a 로 사용
bool rightIdAll = Property.ForAll(genMaybe, m =>
    MonadLaws.RightIdentityHolds<MyMaybeF, int>(m, probeI));
bool assocAll = Property.ForAll(genMaybe, m =>
    MonadLaws.AssociativityHolds<MyMaybeF, int, int, int>(m, fProp, gProp, probeI));

Console.WriteLine($"  ① 좌항등 (임의 100 건) : {(leftIdAll  ? "통과" : "위반")}");
Console.WriteLine($"  ② 우항등 (임의 100 건) : {(rightIdAll ? "통과" : "위반")}");
Console.WriteLine($"  ③ 결합   (임의 100 건) : {(assocAll   ? "통과" : "위반")}");

Console.WriteLine();
Console.WriteLine("== §7.9.4 — 가짜 Monad 반례 (값을 버리는 Bind) ==");

var (realLi, bogusLi, expectedLi) = MonadCounterexample.LeftIdentityCompare();
Console.WriteLine($"  좌항등 기대값 f(3)                = {Show(expectedLi)}");
Console.WriteLine($"  진짜 Bind(Pure(3), f)            = {Show(realLi)}    ← 좌항등 통과");
Console.WriteLine($"  가짜 BogusBind(Pure(3), f)       = {Show(bogusLi)}    ← 값을 버려 위반");
Console.WriteLine($"  진짜 통과 & 가짜 위반?            : {MonadCounterexample.RealHoldsBogusBreaks()}");

Console.WriteLine();
Console.WriteLine("================================================");
Console.WriteLine("7장 완료 — 8장 Validation 으로 진행합니다");
Console.WriteLine("================================================");

static string Show(K<MyMaybeF, int> v) =>
    v.As() switch
    {
        MyMaybe<int>.Just j => $"Just({j.Value})",
        MyMaybe<int>.Nothing => "Nothing",
        _ => "?"
    };

static string ShowList<T>(K<MyListF, T> v) =>
    "[" + string.Join(", ", v.As().Items) + "]";

static string ShowR(K<MyMaybeF, Receipt> v) =>
    v.As() switch
    {
        MyMaybe<Receipt>.Just j => $"Just({j.Value})",
        MyMaybe<Receipt>.Nothing => "Nothing",
        _ => "?"
    };
