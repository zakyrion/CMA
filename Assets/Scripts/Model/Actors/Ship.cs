using CMA;
using CMA.Messages;

namespace Model
{
    public class Ship : Actor<Ship.ShipId>
    {
        public Ship() : base(new ShipId(0))
        {
        }

        public Ship(ShipId key, IMessageManager manager) : base(key, manager)
        {
        }

        protected override void Subscribe()
        {
            SubscribeMessage<Main.GameOver>(OnGameOver);
        }

        private void OnGameOver(Main.GameOver message)
        {
            SendMessage(new View.Ship.Die());
            Owner.RemoveActor(TypedKey);
        }

        public class ShipId
        {
            public ShipId(int key)
            {
                Key = key;
            }

            public int Key { get; protected set; }

            public override bool Equals(object obj)
            {
                var keyObj = obj as ShipId;

                if (keyObj != null)
                    return keyObj.Key == Key;

                return base.Equals(obj);
            }
        }
    }
}