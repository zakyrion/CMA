namespace CMA.Core
{
    public static class Core
    {
        private static BuildManager _buildManager = new BuildManager();

        public static void SubscribeBuilder(IBuilder builder)
        {
            _buildManager.SubscribeBuilder(builder);
        }

        public static T Get<T>()
        {
            return _buildManager.Build<T>();
        }

        public static T Get<T>(IMessage message)
        {
            return _buildManager.Build<T>(message);
        }
    }
}