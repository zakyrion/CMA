using System;

public interface IMessage : ICommunication
{
    Type GetType();
}