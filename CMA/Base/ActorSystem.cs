﻿using CMA.Messages;

namespace CMA
{
    public abstract class ActorSystem<T> : Compositor<T>
    {
        //выделить запросы в сущьность с ресет евентами, для асинхронности
        //1 - любое событие или реквест может быть обработан в следующими способами:
        //1.1 - выполниться любым актером.
        //1.2 - быть переданым дальше по маркеру или по медиатору
        //1.3 - закончиться неудачей с установкой соответствующего флага
        //2 - реквесты и сообщения наследуются от одного интерфейса чтобы они могли отрабатывать в очереди в том порядке в котором пришли

        protected ActorSystem()
        {
            System = MessageManager;
        }

        protected ActorSystem(IMessageManager messageManager) : base(messageManager)
        {
            System = MessageManager;
        }
    }
}