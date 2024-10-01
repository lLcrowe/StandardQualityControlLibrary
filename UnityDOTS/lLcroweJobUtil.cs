#if lLcroweDOTS

using System;
using System.Collections;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace lLCroweTool.DOTS
{
    /// <summary>
    /// Job용 유틸들
    /// </summary>
    public static class lLcroweJobUtil
    {
        public static int GetCPUCoreAmount()
        {
            //유니티 잡스레드는 메인스레드1, 랜더스레드1, 나머지워커스레드로 처리

            int coreCount = Environment.ProcessorCount - 1;
            Debug.Log("현재 사용 가능한 코어 수: " + coreCount);
            return coreCount;
        }


        public static string GetThreadID => $"ThreadID:{Thread.CurrentThread.ManagedThreadId}";

        public static string GetFrameCount => $"FrameCount:{Time.frameCount}";


        //많이쓰는 클래스면 폴링고려해야함

        /// <summary>
        /// 잡을 동작 시킬때 상태를 알려고 제작한 클래스
        /// </summary>
        [System.Serializable]
        public class JobStateClass
        {
            public bool isRun = false;
            public string value;

            public float deltaTime;
            private float prevDeltaTime;

            public float delta;
            private float prevTime;

            public void Update(MonoBehaviour monoBehaviour)
            {
                if (isRun)
                {
                    prevDeltaTime = Time.deltaTime;
                    return;
                }
                

                //델타타임 동기화
                deltaTime = Time.deltaTime + prevDeltaTime;
                prevDeltaTime = 0;

                delta = Time.time - prevTime;
                prevTime = Time.time;

                var job = new ParallelJob();
                job.Init(100);
                monoBehaviour.StartCoroutine(RunParallelJob(job, this, 100, 50));
            }
        }

        public static IEnumerator RunJob(Job job)
        {
            Debug.Log($"start JOB : {GetThreadID}, {GetFrameCount}");
            var handle = job.Schedule();

            while (!handle.IsCompleted)
            {
                yield return null;
            } 

            handle.Complete();
            Debug.Log($"end JOB : {GetThreadID}, {GetFrameCount}");
            job.Dispose();
        }

        public static IEnumerator RunParallelJob(ParallelJob job, JobStateClass isRun, int allLoopAmount, int runloopAmount)
        {
            //여기서 병렬처리하는걸 자동화시켜야됨
            isRun.isRun = true;
            Debug.Log($"start pJOB : {GetThreadID}, {GetFrameCount}");

            //반복을 몇번하는가//한번에 몇개씩 작업을 할것인가
            var handle = job.Schedule(allLoopAmount, runloopAmount);

            while (!handle.IsCompleted)
            {
                yield return null;
            }

            handle.Complete();
            isRun.value = job.GetValue();

            Debug.Log($"end pJOB : {GetThreadID}, {GetFrameCount}");
            job.Dispose();
            isRun.isRun = false;
        }

        public interface ICustomJob
        {
            void Init(int amount);
            void Dispose();
        }

        /// <summary>
        /// 편의성을 위해 제작됨
        /// </summary>
        public interface ICustomSystem
        {
            void OnCreat(ref SystemState state);
            void OnUpdate(ref SystemState state);
            void OnDestroy(ref SystemState state);
        }



        //멀티스레드//그냥 스레드하나 더뽑아서 처리해주는
        public struct Job : IJob, ICustomJob
        {
            //char[] jobName;
            int test;

            public Job(string name)
            {
                //  jobName = name.ToCharArray();
                test = 0;
            }

            public void Dispose()
            {
                
            }

            public void Execute()
            {   
                for (int i = 0; i < 10; i++)
                {
                    test++;                    
                }
                Debug.Log($"job : {test},  ID: {GetThreadID}");
            }

            public void Init(int amount)
            {

            }
        }

        //다중 멀티스레드//아직 전체적인게 더 필요
        public struct ParallelJob : IJobParallelFor, ICustomJob
        {
            //char[] jobName;
            NativeArray<int> jobArray;
            int test;
            public void Init(int amount)
            {
                //현재 컴퓨터쓰레드 체크후 분산시키는 방법을 체크하자
                jobArray = new NativeArray<int>(amount, Allocator.TempJob);
                test = 0;
            }

            //index는 병렬처리할때 내부에서 처리하는 구역에서 index부분을 작동시켜줌
            //var handle = job.Schedule(10, 5);//10의 배열크기가 있고 반복은 5씩해주겠다            
            //index는 배열크기만큼 작동됨. 순서가 보장되지않음. 단지 0 1 2 3 4 5 6 7 8 9 가 나옴
            //job의 스트럭트는 새로 생성되서 기본값으로 초기화됨. 
            //계산후에 배열값을 가져올시 밑과같이 나오지만
            //10 
            //21
            //32
            //43
            //54
            //15
            //26
            //37
            //48
            //59

            //계산될시 보면
            //정렬안했을시    //정렬시
            //0 10          //0 10
            //5 15          //1 21
            //1 21          //2 32
            //6 26          //3 43
            //2 32          //4 54
            //7 37          //5 15
            //3 43          //6 26
            //8 48          //7 37
            //4 54          //8 48
            //9 59          //9 59
            public void Execute(int index)
            {   
                for (int i = 0; i < 10; i++)
                {
                    test++;
                }
                jobArray[index] = test + index;

                //이게 다 따로되는거 같은데
                Debug.Log($"index : {index}, ID: {GetThreadID}");
                //처음시작시 스레드아이디가져올수 있나
            }

            public void Dispose()
            {
                jobArray.Dispose();
            }

            public string GetValue()
            {
                string value = "";
                foreach (var item in jobArray)
                {
                    value += $"{item}\n";
                }
                return value;
            }
        }

        //기본적인 싸이클
        //1. 결과데이터들을 집어넣을 원본 해야할것들을

        //public void Sample()
        //{
        //    // 결과를 저장할 단일 float 형식의 네이티브 배열을 생성합니다. 이 예제는 설명을 위해 작업이 완료될 때까지 기다립니다.
        //    NativeArray<float> result = new NativeArray<float>(1, Allocator.TempJob);

        //    // 작업 데이터를 설정합니다.
        //    MyJob jobData = new MyJob();
        //    jobData.a = 10;
        //    jobData.b = 10;
        //    jobData.result = result;

        //    // 작업을 예약합니다.
        //    JobHandle handle = jobData.Schedule();

        //    // 작업이 완료될 때까지 기다립니다.
        //    handle.Complete();

        //    //근데 예제가 크게 두종류다.
        //    //코루틴사용해서 complete될떄까지 넘기는 방식. 최소 한프레임은 작동함
        //    //LateUpdate에서 complete될떄까지 대기. 끝없는 백어택

        //    // 모든 네이티브 배열의 복사본이 동일한 메모리를 가리키므로 "당신의" 네이티브 배열에서 결과에 액세스할 수 있습니다.
        //    float aPlusB = result[0];

        //    // 결과 배열에 할당된 메모리를 해제합니다.
        //    result.Dispose();
        //}

        //Job시스템이 자동적으로 잡혀서 작동됨
        //public partial struct TestSystem : ISystem, ICustomSystem
        //{
        //    public void OnCreat(ref SystemState state)
        //    {
                
        //    }

        //    public void OnDestroy(ref SystemState state)
        //    {
                
        //    }

        //    public void OnUpdate(ref SystemState state)
        //    {
        //        Debug.Log("Test");
        //    }
        //}
    }
}
#endif