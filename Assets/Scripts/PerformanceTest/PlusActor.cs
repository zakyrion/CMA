using System;
using CMA;
using CMA.Messages;

public class PlusActor : Actor
{
    private readonly Random _random = new Random();
    private int _count;
    private int _done;

    protected override void Subscribe()
    {
        Receive<MathActionCount>(OnMathActionCount);
        Receive<MathAction>(OnMathAction);
    }

    private void OnMathActionCount(MathActionCount arg1, IMessage arg2)
    {
        _count = arg1.Data;
        _done = 0;

        /*for (var i = 0; i < _count; i++)
        {
            var result = _random.NextDouble() * _count;
            result += 1;
        }

        Send(new PerformanceTest.FinishCalculation(0), "PerformanceTest/!");*/
    }

    private void OnMathAction(MathAction mathAction, IMessage message)
    {
        var result = _random.NextDouble() * mathAction.Data;
        result += 1;
        _done++;
        if (_count == _done)
            Send(new PerformanceTest.FinishCalculation(0), "PerformanceTest/!");
    }

    public class MathAction : Container<int>
    {
        public MathAction(int data) : base(data)
        {
        }
    }

    public class MathActionCount : Container<int>
    {
        public MathActionCount(int data) : base(data)
        {
        }
    }
}