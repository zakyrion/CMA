using System;
using CMA;
using CMA.Core;
using CMA.Messages;
using UnityEngine;

public class PerformanceTest : Actor
{
    private int _currentLap;

    private int _finished;

    private long _tiks;
    private IDeliveryHelper _receiver;
    private long _timeStamp;

    public PerformanceTest() : base(new SingleThreadController())
    {
        
    }

    private void DoActorTest()
    {
        _timeStamp = DateTime.Now.Ticks;

        if (_currentLap == 0)
        {
            for (var i = 0; i < Starter.testCount; i++)
            {
                var message = new Message(new PlusActor.MathAction(i));
                message.Init("", "");
                message.SetCluster("");
            }

            Debug.Log($"Create All Messages: {DateTime.Now.Ticks - _timeStamp}");

            _timeStamp = DateTime.Now.Ticks;

            var testObj = new Message(new PlusActor.MathAction(0));
            var random = new System.Random();
            
            var manager = new MessageManager();
            manager.Receive<PlusActor.MathAction>((action, message) => {
                var result = random.NextDouble() * action.Data;
                result += 1;
            });

            /*var handler = new MessageHandler<PlusActor.MathAction>((action, message) => {
                var result = random.NextDouble() * action.Data;
                result += 1;
            });*/

            for (var i = 0; i < Starter.testCount; i++)
            {
                //handler.Invoke(testObj);
                manager.Responce(testObj);
            }
            Debug.Log($"Do All Action: {DateTime.Now.Ticks - _timeStamp}");
            //return;
            _timeStamp = DateTime.Now.Ticks;
        }

        if (_currentLap < Starter.testLaps)
        {
            /*_receiver.PushMessage(new PlusActor.MathActionCount(Starter.testCount / Starter.testLaps), Adress);
            for (var i = 0; i < Starter.testCount / Starter.testLaps; i++)
                _receiver.PushMessage(new PlusActor.MathAction(Starter.testLaps + i), Adress);*/

            for (var j = 0; j < Starter.testLaps; j++)
            {
                var adress = $"PerformanceTest/{j}/!";
                Send(new PlusActor.MathActionCount(Starter.testCount / Starter.testLaps), adress);

                for (var i = 0; i < Starter.testCount / Starter.testLaps; i++)
                    Send(new PlusActor.MathAction(j * Starter.testLaps + i), adress);
            }
            _currentLap++;
        }
        else
        {
            Debug.Log($"Average: { _tiks / Starter.testLaps}");
        }
    }

    protected override void Subscribe()
    {
        Receive<FinishCalculation>(OnFinishCalculation);
        Receive<Start>(OnStart);
    }

    private void OnStart(IMessage obj)
    {
        AskDeliveryHelper("PerformanceTest", Cluster.Name, EDeliveryType.ToChildern, receiver =>
            {
                _receiver = receiver;
                DoActorTest();
            });
    }

    private void OnFinishCalculation(FinishCalculation finishCalculation, IMessage message)
    {
        _finished++;
        if (_finished == Starter.testLaps)
        {
            _finished = 0;
            _tiks += DateTime.Now.Ticks - _timeStamp;
            Debug.Log($"Finish Actor lap: {DateTime.Now.Ticks - _timeStamp}");
            DoActorTest();
        }
    }

    public class FinishCalculation : Container<int>
    {
        public FinishCalculation(int data) : base(data)
        {
        }
    }

    public class Start
    {
    }
}