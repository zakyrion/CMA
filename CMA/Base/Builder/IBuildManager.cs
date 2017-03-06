namespace CMA
{
    public interface IBuildManager
    {
        void SubscribeBuilder(IBuilder builder);
        T Build<T>();
        T Build<T>(IMessage message);
    }
}