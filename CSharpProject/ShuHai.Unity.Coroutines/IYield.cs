namespace ShuHai.Unity.Coroutines
{
    public interface IYield
    {
        bool IsDone { get; }

        void Start();
        void Stop();
        void Update();
    }
}