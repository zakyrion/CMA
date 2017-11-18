using System;
using System.Collections.Generic;
using CMA;
using CMA.Messages;
using UnityEngine;

public class PerformanceTest : Actor
{
    List<IAdress> _adresses = new List<IAdress>();
    private int _currentLap;

    private int _finished;

    private long _timeStamp;

    private void DoActorTest()
    {
        _timeStamp = DateTime.Now.Ticks;

        if (_currentLap < Starter.testLaps)
            for (var j = 0; j < Starter.testLaps; j++)
            {
                Send(new PlusActor.MathActionCount(Starter.testCount / Starter.testLaps), $"PerformanceTest/{j}/!");
                for (var i = 0; i < Starter.testCount / Starter.testLaps; i++)
                    Send(new PlusActor.MathAction(j * Starter.testLaps + i), _adresses[j]);
            }

        //Debug.Log("Send All");
        _currentLap++;
    }

    protected override void Subscribe()
    {
        Receive<FinishCalculation>(OnFinishCalculation);
        Receive<Start>(OnStart);
    }

    private void OnStart(IMessage obj)
    {
        for (int i = 0; i < Starter.testLaps; i++)
        {
            _adresses.Add(new Adress($"PerformanceTest/{i}/!"));
        }
        DoActorTest();
    }

    private long _tiks;
    private void OnFinishCalculation(FinishCalculation finishCalculation, IMessage message)
    {
        _finished++;
        if (_finished < Starter.testLaps)
        {
            _tiks += DateTime.Now.Ticks - _timeStamp;
            //Debug.Log($"Finish Actor lap: {DateTime.Now.Ticks - _timeStamp}");
            DoActorTest();
        }
        else
        {
            Debug.Log($"Average: { _tiks / Starter.testLaps}");
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