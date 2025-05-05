//using UnityEngine;
//using UnityEditor;
//using System.Diagnostics;
//using System.Collections.Generic;
//using System;
//using System.Linq;
//using UnityEditor.Compilation;

//// 에디터 폴더 내에 이 스크립트 저장
//public class CompilationAnalyzer
//{
//    private static Dictionary<string, float> assemblyCompileTimes = new Dictionary<string, float>();
//    private static Stopwatch totalStopwatch = new Stopwatch();
//    private static Stopwatch assemblyStopwatch = new Stopwatch();
//    private static DateTime lastCompileTime;

//    [InitializeOnLoadMethod]
//    static void Initialize()
//    {
//        // 컴파일 이벤트 구독
//        CompilationPipeline.compilationStarted += OnCompilationStarted;
//        CompilationPipeline.compilationFinished += OnCompilationFinished;
//        CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
//        CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;

//        // 초기화 로그 제거
//    }

//    private static void OnCompilationStarted(object obj)
//    {
//        // 시작 시 로그 출력하지 않음
//        assemblyCompileTimes.Clear();
//        totalStopwatch.Restart();
//        lastCompileTime = DateTime.Now;
//    }

//    private static void OnAssemblyCompilationStarted(string assemblyName)
//    {
//        assemblyStopwatch.Restart();
//    }

//    private static void OnAssemblyCompilationFinished(string assemblyName, CompilerMessage[] messages)
//    {
//        float time = assemblyStopwatch.ElapsedMilliseconds / 1000f;
//        assemblyCompileTimes[assemblyName] = time;

//        // 오류 메시지만 기록 (정상 진행 과정은 기록하지 않음)
//        int errorCount = messages.Count(m => m.type == CompilerMessageType.Error);
//        if (errorCount > 0)
//        {
//            UnityEngine.Debug.LogError($"어셈블리 '{assemblyName}' 컴파일 중 {errorCount}개의 오류 발생");
//        }
//    }

//    private static void OnCompilationFinished(object obj)
//    {
//        float totalTime = totalStopwatch.ElapsedMilliseconds / 1000f;

//        // 어셈블리별 컴파일 시간을 내림차순으로 정렬
//        var sortedAssemblies = assemblyCompileTimes
//            .OrderByDescending(pair => pair.Value)
//            .ToList();

//        // 결과 로그 문자열 구성
//        System.Text.StringBuilder logBuilder = new System.Text.StringBuilder();
//        logBuilder.AppendLine("\n===== 컴파일 분석 결과 =====");
//        logBuilder.AppendLine($"총 컴파일 시간: {totalTime:F2}초");

//        logBuilder.AppendLine("\n어셈블리별 컴파일 시간 (내림차순):");
//        foreach (var pair in sortedAssemblies)
//        {
//            string marker = pair.Value > 1.0f ? "* " : "  ";
//            logBuilder.AppendLine($"{marker}{pair.Key}: {pair.Value:F2}초");
//        }

//        // 상위 5개 어셈블리 분석
//        if (sortedAssemblies.Count > 0)
//        {
//            logBuilder.AppendLine("\n컴파일 병목 분석:");
//            int count = Math.Min(5, sortedAssemblies.Count);
//            for (int i = 0; i < count; i++)
//            {
//                var assembly = sortedAssemblies[i];
//                float percentage = (assembly.Value / totalTime) * 100;
//                logBuilder.AppendLine($"{i + 1}. {assembly.Key}: {assembly.Value:F2}초 (전체의 {percentage:F1}%)");

//                // 어셈블리 의존성 분석 (가능한 경우)
//                try
//                {
//                    var assemblies = CompilationPipeline.GetAssemblies();
//                    var targetAssembly = assemblies.FirstOrDefault(a => a.name == assembly.Key);
//                    if (targetAssembly != null && targetAssembly.assemblyReferences.Length > 0)
//                    {
//                        logBuilder.AppendLine($"   의존성 개수: {targetAssembly.assemblyReferences.Length}개");
//                    }
//                }
//                catch (Exception)
//                {
//                    // 의존성 분석 오류 무시
//                }
//            }
//        }

//        logBuilder.AppendLine("\n===== 컴파일 분석 완료 =====");

//        // 최종 결과를 한 번에 로그로 출력
//        UnityEngine.Debug.Log(logBuilder.ToString());
//    }

//    [MenuItem("Tools/컴파일 분석기/강제 컴파일 %#F12", false, 100)]
//    public static void ForceRecompile()
//    {
//        // 임시 스크립트 생성
//        string tempFilePath = "Assets/Editor/TempCompileForcer.cs";
//        System.IO.File.WriteAllText(tempFilePath,
//            "// 임시 파일 - 컴파일 강제 유발용\n" +
//            "class TempCompileForcer { static int uniqueID = " + DateTime.Now.Ticks + "; }");

//        // 에셋 데이터베이스 갱신
//        AssetDatabase.Refresh();

//        // 임시 파일 삭제 (다음 프레임에)
//        EditorApplication.delayCall += () => {
//            if (System.IO.File.Exists(tempFilePath))
//            {
//                System.IO.File.Delete(tempFilePath);
//                AssetDatabase.Refresh();
//            }
//        };
//    }
//}