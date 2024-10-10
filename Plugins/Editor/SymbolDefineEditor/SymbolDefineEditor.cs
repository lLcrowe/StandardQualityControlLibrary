#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

//edit=>PlayerSettings=>OtherSetting=>Scripting Define Symbols 에서 위의 심볼을 입력해서 처리
//20231111//초기화시 자동화처리
//20240507//심볼처리를 패뜨파인더처럼 온오프해서  제작하기
//20240607//기존 lLcroweToolBox 통합
//20240609//심볼제작기 제작끝

namespace SymbolDefine
{



    /// <summary>
    /// 지정된 정의 기호를 플레이어 설정 정의 기호에 추가기능
    /// 아래의 Symbol 속성에 자신의 정의 기호를 추가하기만 하면 됩니다
    /// </summary>
    //[DefaultExecutionOrder(-5000)]
    //[InitializeOnLoad]
    public class SymbolDefineEditor : EditorWindow
    {
        //규칙은 심플하게 
        //Building_Space


        /// <summary>
        /// 편집기에 추가할 심볼
        /// </summary>
        public string[] symbolArray = new string[]
        {
            //------현재 지정한 심볼들




            //"TimerModule",//따로 리폴로지가 생성됨
            //유틸안에서 세팅해놓게 처리//둘중하나를 쓰도록 권장
            //"Util2D",//2D게임용 유틸
            //"Util3D",//3D게임용 유틸
            
            //"CommandKey",//커맨드키
            //"Shape",
            //"Animation3DSet",//3D 애니메이션 전용 

            //외부 에셋//이름로 심볼정의
            
            "AbilitySystem",
            //"Doozy",//많은게 있을것..

            //"Animancer",ResearchTreeStore
            "Achevement",//"업적",
            "Localize",//MEC&Localize//현지화
            "lLcroweDOTS",//DOTS용
        };

        //에셋 익스텐드구역
        public string[] extendSymbolArray = new string[]
        {
            "MEC",
            "PathFinder",
            "DoTween",
            "DoTweenASM",
            "MasterAudio",
            "Sensor",
            "DamageText",
            "Light2D",
            "Explore2D",
        };


        protected static Vector2 windowMinSize = new Vector2(300, 200);
        protected static Vector2 windowMaxSize = new Vector2(300, 515);
        //[InitializeOnLoadMethod]//여러번튀어나오는문제가 있음

        //각각 윈도우에디터에 써줘야 나오는것
        [MenuItem("lLcroweTool/DefineSymbols")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(SymbolDefineEditor));
            editorWindow.titleContent.text = $"심볼정의 관리자";
            editorWindow.minSize = windowMinSize;
            editorWindow.maxSize = windowMaxSize;
        }


        private Vector2 symbolScrollPos;
        private Vector2 symbolExtentscroll;
        private static List<string> allDefines;
        private void OnEnable()
        {
            //심볼;심볼;심볼;심볼;심볼;심볼;//이렇게 존재 스트링이 존재
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(symbolArray.Except(allDefines));

            //var list = symbolBible.Keys.;
        }

        private void OnGUI()
        {
            ShowSymbolList(ref symbolScrollPos, symbolArray, "개인라이브러리");
            ShowSymbolList(ref symbolExtentscroll, extendSymbolArray, "에셋");
        }

        private void ShowSymbolList(ref Vector2 scroll, string[] symbolArray, string content)
        {
            EditorGUILayout.LabelField(content);
            EditorLineLayout();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            for (int i = 0; i < symbolArray.Length; i++)
            {
                var targetString = symbolArray[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(targetString);
                if (CheckSymbol(targetString))
                {
                    GUI.color = Color.red;
                    Button("삭제", () => RemoveSymbol(targetString));
                }
                else
                {
                    GUI.color = Color.green;
                    Button("추가", () => AddSymbol(targetString));
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }


        bool CheckSymbol(string symbol)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return defines.Contains(symbol);
        }


        void AddSymbol(string symbol)
        {
            //심볼체크
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (defines.Contains(symbol))
            {
                return;
            }
            defines += ";" + symbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }

        void RemoveSymbol(string symbol)
        {
            //심볼체크
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!defines.Contains(symbol))
            {
                return;
            }
            defines = defines.Replace(symbol + ";", "").Replace(";" + symbol, "").Replace(symbol, "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }

        public static void EditorLineLayout(Color color = default, float thickness = 0.5f, int padding = 10, int margin = 0)
        {
            //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            color = color != default ? color : Color.grey;
            Rect r = EditorGUILayout.GetControlRect(false, GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding * 0.5f;

            switch (margin)
            {
                // expand to maximum width
                case < 0:
                    r.x = 0;
                    r.width = EditorGUIUtility.currentViewWidth;

                    break;
                case > 0:
                    // shrink line width
                    r.x += margin;
                    r.width -= margin * 2;

                    break;
            }

            EditorGUI.DrawRect(r, color);
        }


        /// <summary>
        /// 에디터버튼(래핑함)
        /// </summary>
        /// <param name="content">버튼컨텐츠</param>
        /// <param name="action">액션</param>
        public static void Button(string content, System.Action action)
        {
            if (GUILayout.Button(content))
            {
                action?.Invoke();
                SceneView.RepaintAll();
            }
        }

    }
}
#endif