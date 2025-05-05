#if UNITY_EDITOR
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

[InitializeOnLoad]
public static class LogCompilationTimes
{
    static Stopwatch totalStopwatch = new Stopwatch();
    static Dictionary<string, Stopwatch> assemblyStopwatches = new Dictionary<string, Stopwatch>();
    static Dictionary<string, double> assemblyCompileTimes = new Dictionary<string, double>();
    static bool hasCompilationFinished = false;

    static LogCompilationTimes()
    {
        CompilationPipeline.compilationStarted += OnCompilationStarted;
        CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
        CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        CompilationPipeline.compilationFinished += OnCompilationFinished;
    }

    private static void OnCompilationStarted(object obj)
    {
        totalStopwatch.Restart();
        assemblyStopwatches.Clear();
        assemblyCompileTimes.Clear();
        hasCompilationFinished = false;
    }

    private static void OnAssemblyCompilationStarted(string assemblyPath)
    {
        string assemblyName = Path.GetFileName(assemblyPath);
        var sw = new Stopwatch();
        sw.Start();
        assemblyStopwatches[assemblyName] = sw;
    }

    private static void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
    {
        string assemblyName = Path.GetFileName(assemblyPath);

        if (assemblyStopwatches.TryGetValue(assemblyName, out var sw))
        {
            sw.Stop();
            assemblyCompileTimes[assemblyName] = sw.Elapsed.TotalSeconds;
        }
        else
        {
            assemblyCompileTimes[assemblyName] = 0;
        }
    }

    private static void OnCompilationFinished(object obj)
    {
        if (hasCompilationFinished) return;
        hasCompilationFinished = true;

        totalStopwatch.Stop();

        double sum = assemblyCompileTimes.Values.Sum();

        Debug.Log($"[컴파일 완료]\n" +
                  $"총 소요 시간: {totalStopwatch.Elapsed.TotalSeconds:0.00}초\n" +
                  string.Join("\n", assemblyCompileTimes.Select(kv => $"  □ {kv.Key}: {kv.Value:0.00}초")) +
                  $"\n어셈블리별 컴파일 시간 총합: {sum:0.00}초");
    }

    // ==========================
    // 강제 리컴파일 메뉴
    // ==========================

    [MenuItem("Tools/Recompile All Scripts F11")]
    public static void ForceRecompileAll()
    {
        Debug.Log("[스크립트 강제 전체 리컴파일 요청됨]");
        AssetDatabase.Refresh();
        CompilationPipeline.RequestScriptCompilation();
    }

    [MenuItem("Tools/Recompile Assembly-CSharp")]
    public static void ForceRecompileAssemblyCSharp()
    {
        ForceTouchAssembly("Assembly-CSharp");
    }

    [MenuItem("Tools/Recompile Assembly-CSharp-Editor")]
    public static void ForceRecompileAssemblyCSharpEditor()
    {
        ForceTouchAssembly("Assembly-CSharp-Editor");
    }

    private static void ForceTouchAssembly(string assemblyName)
    {
        string[] allScripts = AssetDatabase.FindAssets("t:Script")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => path.EndsWith(".cs"))
            .ToArray();

        // 가장 마지막으로 변경된 파일을 선택 (또는 아무거나)
        foreach (var path in allScripts)
        {
            string asmDef = CompilationPipeline.GetAssemblyNameFromScriptPath(path);
            if (asmDef == assemblyName)
            {
                File.SetLastWriteTime(path, System.DateTime.Now);
                Debug.Log($"[{assemblyName}] 변경된 파일: {path}");
                AssetDatabase.ImportAsset(path);
                return;
            }
        }

        Debug.LogWarning($"[{assemblyName}]에 해당하는 스크립트를 찾을 수 없습니다.");
    }
}
#endif
