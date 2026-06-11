using Ch22.Functions;
using Ch22.Traits;
using Ch22.Types;

namespace Ch22.Challenges;

// 챌린지 ② 정답 — ReaderT<int, IOF> 가 Eff<RT> 축소판임을 *작은 앱* 으로 확인.
// 환경(배수)을 읽고 IO 로 값을 얻어 곱한다. Run(env) 는 IO 조립만, io.Run() 에서 실행.
public static class ConfigApp
{
    public static int Compute(int env, Func<int> ioSource)
    {
        K<ReaderTF<int, IOF>, int> program =
            from factor in Readable.asks<ReaderTF<int, IOF>, int, int>(e => e)
            from n in IOM.liftIO<ReaderTF<int, IOF>, int>(new IO<int>(ioSource))
            select factor * n;

        // ReaderT 를 env 로 Run → 안쪽 IO → IO.Run() 으로 실제 실행.
        return program.As().Run(env).As().Run();
    }
}
