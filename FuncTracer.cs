using System.Collections.Generic;
using System.Diagnostics;

namespace lLCroweTool.QC.EditorOnly
{
    public class FuncTracer
    {
        //함수추적기
        //다른 프로그래머의 코드를 파악할 때 함수 호출 흐름을 쉽게 추적하기 위해 제작
        //예를 들어 A 버튼을 눌렀을 때 캐릭터의 애니메이션 바뀌는데 이걸코드를 쫒다보면서 알 수도 있겠지만 그냥 A 버튼을 눌렀을 때 어떤 함수가 호출됐는지 확인할 수 있으면 편하겠다 생각이 들더라구요
        //호출 => 함수=>함수=>함수=>함수 => 작동부분

        private static string key = "FuncTracer";

#if UNITY_EDITOR && lLcroweToolLogSystem
        //임시로 제작
        public FuncTracer() 
        {
            lLCroweTool.LogSystem.LogManager.Register(key, "FuncTrace_Log", false, true);
        }


        //추적할 체크포인트를 박아야되는 함수
        //문제점은 각 위치마다 박아야되므로 
        //시작점 끝점이 지정되야함

        //끝점이 지정안된상태로 할려면 다른방식을 체크
        //느낌상 클래스하나를 불려왔을떄 해당클래스의 모든함수를 체크후 쓰고 있는가? 하면서 불려와야될듯함

        private static readonly object lockObject = new object();

        public static void TraceFunctionCheckPoint()
        {
            lock (lockObject)
            {
                var stackTrace = new StackTrace();
                StackFrame[] stackFrames = stackTrace.GetFrames();

                if (stackFrames != null)
                {
                    List<string> tempList = new List<string>(stackFrames.Length);

                    int count = stackFrames.Length;


                    foreach (StackFrame frame in stackFrames)
                    {
                        // 메서드 정보 가져오기
                        var method = frame.GetMethod();

                        string content = $"{count}.Method: {method.Name}\n";// 현재 프레임에서 호출된 메서드 이름 출력

                        //content += $"Declaring Type: {method.DeclaringType?.FullName}\n";// 메서드가 정의된 클래스 또는 타입 이름 출력
                        //content += $"File: {frame.GetFileName()}, Line: {frame.GetFileLineNumber()}\n";// 메서드가 포함된 파일 및 라인 번호 출력

                        content = content.Substring(0, content.Length - 1);
                        tempList.Add(content);
                        count--;
                    }

                    tempList.Reverse();
                    LogManager.Log(key, lLcroweUtil.LogIList(tempList));
                }
            }

        }
        #endif
    }
}
