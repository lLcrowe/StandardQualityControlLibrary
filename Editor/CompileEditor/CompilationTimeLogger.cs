//using UnityEditor;
//using UnityEditor.Compilation;
//using UnityEngine;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Text;
//using Debug = UnityEngine.Debug;

//[InitializeOnLoad]
//public static class CompilationTimeLogger
//{
//    private static Stopwatch globalStopwatch;
//    private static Dictionary<string, double> assemblyCompileTimes = new();

//    static CompilationTimeLogger()
//    {
//        CompilationPipeline.compilationStarted += OnCompilationStarted;
//        CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
//        CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
//        CompilationPipeline.compilationFinished += OnCompilationFinished;
//    }

//    private static Dictionary<string, Stopwatch> activeAssemblyTimers = new();

//    private static void OnCompilationStarted(object _)
//    {
//        globalStopwatch = new Stopwatch();
//        globalStopwatch.Start();
//        assemblyCompileTimes.Clear();
//        activeAssemblyTimers.Clear();
//    }

//    private static void OnAssemblyCompilationStarted(string assemblyPath)
//    {
//        string assemblyName = System.IO.Path.GetFileName(assemblyPath);
//        var sw = new Stopwatch();
//        sw.Start();
//        activeAssemblyTimers.Add(assemblyName, sw);
//    }

//    private static void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
//    {
//        string assemblyName = System.IO.Path.GetFileName(assemblyPath);
//        if (activeAssemblyTimers.TryGetValue(assemblyName, out var sw))
//        {
//            sw.Stop();
//            assemblyCompileTimes[assemblyName] = sw.Elapsed.TotalSeconds;
//        }
//    }

//    private static void OnCompilationFinished(object _)
//    {
//        globalStopwatch.Stop();
//        double totalSec = globalStopwatch.Elapsed.TotalSeconds;

//        StringBuilder sb = new StringBuilder();
//        sb.AppendLine("<color=lime>[컴파일 완료]</color>");
//        sb.AppendLine($"총 소요 시간: <b>{totalSec:F2}초</b>");
//        sb.AppendLine("어셈블리별 컴파일 시간:");

//        foreach (var kvp in assemblyCompileTimes)
//        {
//            sb.AppendLine($"  ⮡ {kvp.Key}: <b>{kvp.Value:F2}초</b>");
//        }

//        Debug.Log(sb.ToString());
//    }

//    [MenuItem("Tools/컴파일 분석기/컴파일 F11")]
//    public static void ForceRecompile()
//    {
//        // 에셋 데이터베이스 갱신
//        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
//    }
//}
