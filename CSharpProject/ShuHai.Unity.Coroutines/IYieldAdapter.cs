namespace ShuHai.Unity.Coroutines
{
    public interface IYieldAdapter
    {
        IYield ToYield(object yieldObject);
    }
}