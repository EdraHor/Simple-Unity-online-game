using System;
using System.Collections.Generic;
using UnityEngine;

internal class UnityThread : MonoBehaviour
{
    internal static UnityThread instance;
    readonly Queue<Action> jobs = new Queue<Action>();

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        while (jobs.Count > 0)
            jobs.Dequeue().Invoke();
    }

    internal void AddJob(Action newJob)
    {
        jobs.Enqueue(newJob);
    }
}