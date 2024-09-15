#if lLcroweDOTS

using Assets.DOTSGame;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static lLCroweTool.DOTS.lLcroweJobUtil;

public class SampleTestJob : MonoBehaviour
{
    //잡에 레퍼런스가 들어가면 작동안됨
    //array, struct안에 주소들 안됨


    
    public JobStateClass runClass = new ();

    public float deltaTime;
    private float prevDeltaTime;

    public float delta;
    private float prevTime;


    private void Start()
    {
        Debug.Log($"{GetThreadID},{GetFrameCount}");


        //StartCoroutine(RunJob(new Job()));

        var job = new ParallelJob();
        job.Init(5);
        StartCoroutine(RunParallelJob(job, runClass, 5, 1));

        //RunJob1();
        //RunJob2();
        //TestRunJob1();
        //TestRunJob2();

    }

    private void Update()
    {
        if (runClass.isRun)
        {
            prevDeltaTime = Time.deltaTime;
            return;
        }
        Debug.Log(runClass.value);

        //델타타임 동기화
        deltaTime = Time.deltaTime + prevDeltaTime;
        prevDeltaTime = 0;

        delta = Time.time - prevTime;
        prevTime = Time.time;
        var job = new ParallelJob();
        job.Init(5);
        StartCoroutine(RunParallelJob(job, runClass, 100, 50));
    }

    private async void RunAsync()
    {
        //이건나중
        await Task.Delay(1000);
        Debug.Log(GetThreadID);
    }

    //한스레드하나만 참조
    private void RunJob1()
    {
        Debug.Log($"start JOB : {GetThreadID}, {GetFrameCount}");
        Job job = new Job();
        var handle = job.Schedule();
        handle.Complete();

        job.Dispose();
        Debug.Log($"end JOB : {GetThreadID}, {GetFrameCount}");
    }
    private void RunJob2()
    {
        Debug.Log($"start JOB : {GetThreadID}, {GetFrameCount}");
        Job job = new Job();
        job.Run();
        //handle.Complete();

        job.Dispose();
        Debug.Log($"end JOB : {GetThreadID}, {GetFrameCount}");
    }

    //여러스레드에서 계산됨
    private void TestRunJob1()
    {
        Debug.Log($"start RunParallelJob : {GetThreadID}, {GetFrameCount}");
        ParallelJob job = new ParallelJob();
        job.Init(5);
        var handle = job.Schedule(5, 1);
        handle.Complete();

        job.Dispose();
        Debug.Log($"end RunParallelJob : {GetThreadID}, {GetFrameCount}");
    }

    //한스레드에서 계산됨
    private void TestRunJob2()
    {
        Debug.Log($"start RunParallelJob : {GetThreadID}, {GetFrameCount}");
        ParallelJob job = new ParallelJob();
        job.Init(5);
        job.Run(5);
        //handle.Complete();

        job.Dispose();
        Debug.Log($"end RunParallelJob : {GetThreadID}, {GetFrameCount}");
    }

    /// <summary>
    /// Add a WaitForSeconds component to your entity with a delay parameter.
    /// The ScheduleSystem will then wait that many seconds before removing
    /// the component, and add a Ready tag component. You can then process
    /// the Ready component in your own systems.
    /// </summary>
    public struct WaitForSeconds// : IComponentData
    {
        public float time;

        public WaitForSeconds(float delay) => this.time = UnityEngine.Time.time + delay;
    }


}
#endif