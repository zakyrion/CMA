using System;
using System.Collections;
using CMA;
using CMA.Messages;
using UnityEngine;
using Random = System.Random;

public class Starter : MonoBehaviour
{
    public const int testCount = 100000;
    public const int testLaps = 10;
    private readonly Random _random = new Random();
    private MailBox mailbox;

    private void Awake()
    {
        mailbox = new MailBox(new Adress("PerformanceTest"), new ActorSystem());
        mailbox.AddActor(new PerformanceTest());

        for (var i = 0; i < testLaps; i++)
            mailbox.AddActor(new PlusActor(), $"PerformanceTest/{i}");
    }

    void Start()
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        yield return new WaitForEndOfFrame();

        var timeStamp = DateTime.Now.Ticks;

        for (var j = 0; j < testLaps; j++)
        {
            timeStamp = DateTime.Now.Ticks;

            for (var i = 0; i < testCount; i++)
            {
                var result = _random.NextDouble() * testCount;
                result += 1;
            }

            Debug.Log($"Finish test: {DateTime.Now.Ticks - timeStamp}");
        }

        //yield return new WaitForSeconds(5);

        mailbox.Actor.Send(new PerformanceTest.Start(), "PerformanceTest/!");
    }

    void OnDestroy()
    {
        mailbox.Actor.Send(new Kill(), "PerformanceTest/!");
    }
}