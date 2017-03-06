public struct RequestKey
{
    public string ResultKey { get; private set; }
    public string MessageKey { get; private set; }

    public RequestKey(string resultKey, string messageKey) : this()
    {
        ResultKey = resultKey;
        MessageKey = messageKey;
    }
}