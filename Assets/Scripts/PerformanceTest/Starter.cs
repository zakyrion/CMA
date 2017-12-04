using System;
using System.Collections;
using System.Collections.Generic;
using CMA;
using CMA.Core;
using CMA.Messages;
using UnityEngine;
using Random = System.Random;

public class Starter : MonoBehaviour
{
    public const int testCount = 100000;
    public const int testLaps = 10;
    private readonly List<PlusActor> _actors = new List<PlusActor>();
    private readonly Random _random = new Random();
    private ICluster cluster;
    private IActor test;

    private void Awake()
    {
        var system = new ActorSystem();
        cluster = new Cluster("Main", new SingleThreadController());
        system.AddCluster(cluster);
        test = new PerformanceTest();
        cluster.AddActor(test, "PerformanceTest");

        for (var i = 0; i < testLaps; i++)
        {
            var actor = new PlusActor();
            _actors.Add(actor);
            cluster.AddActor(actor, $"PerformanceTest/{i}");
        }
    }

    private void Start()
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

        yield return new WaitForSeconds(2);

        test.PushMessage(new Message(new PerformanceTest.Start()));
    }

    private void OnDestroy()
    {
        cluster.Quit();
    }
}