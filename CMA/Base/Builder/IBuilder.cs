namespace CMA
{
    public interface IBuilder
    {
        string Key { get; }
        object Build();
        object Build(IMessage message);
    }
}