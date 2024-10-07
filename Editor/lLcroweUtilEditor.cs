#if UNITY_EDITOR
using lLCroweTool.Dictionary;
using lLCroweTool.QC.Curve;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    public static class lLcroweUtilEditor
    {
        //=========================================================================================================
        //에디터에서만 작동될구역
        //=========================================================================================================
                
        private static int tempDamage;
      
        private static bool buffApply;
        private static int buffPercentValue;
 
        private static string targetEffectSound;

        /// <summary>
        /// newTonsoft Json, LitJson보다 유니티내장Json이 휠씬빠르다
        /// </summary>
        //public static void Json()
        //{
        //    //https://wergia.tistory.com/164
        //    JsonUtility.FromJson<GameObject>(,);//Json데이터로 특정타입 객체로 만들기
        //    JsonUtility.FromJsonOverwrite();//JSON 데이터에서 읽어 객체의 데이터를 덮어씁니다
        //    JsonUtility.ToJson();//Object를 Json데이터로 제작
        //}

        //Resources가 여러곳에 있으면 나중에 다합쳐진다.

        /// <summary>
        /// 데이터복사
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="original">컴포넌트  타입</param>
        /// <param name="target">붙여넣을게임오브젝트</param>
        /// <returns>복사한 컴포넌트</returns>
        public static T GetCopyOf<T>(T original, T target) where T : Object
        {
            System.Type type = original.GetType();

            System.Reflection.FieldInfo[] fields = type.GetFields();

            //foreach (System.Reflection.FieldInfo field in fields)
            //{
            //    field.SetValue(copy, field.GetValue(original));
            //}

            for (int i = 0; i < fields.Length; i++)
            {
                System.Reflection.FieldInfo field = fields[i];
                field.SetValue(target, field.GetValue(original));
            }

            return target as T;
        }

        /// <summary>
        /// 데이터를 적재할 폴더이름
        /// </summary>
        private static string customFolderName = "Resources";//"GameData";

        /// <summary>
        /// 데이터생성 함수
        /// </summary>
        /// <param name="newDataObject">해당되는 신규데이터</param>
        /// <param name="fileName">데이터파일 이름</param>
        /// <param name="folderName">지정할 폴더이름</param>
        /// <param name="tag">꼬리표</param>
        /// <param name="isUseDisplay">안내창을 표시할건지 여부</param>
        public static void CreateDataObject<T>(ref T newDataObject, string fileName, string folderName, string tag, bool isUseDisplay = true) where T : Object
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.Log("데이터오브젝트 이름을 정해주세요");
                return;
            }

            //데이터에셋 새롭게 만들기
            //해당폴더가 있는가?
            if (!AssetDatabase.IsValidFolder("Assets/A0"))
            {
                //폴더가 없으면
                Debug.Log("\n" + "경로에 해당되는 폴더가 없어서 폴더를 추가시켯했습니다.");
                AssetDatabase.CreateFolder("Assets", "A0");
            }

            //해당폴더가 있는가?
            if (!AssetDatabase.IsValidFolder($"Assets/A0/{customFolderName}"))
            {
                //폴더가 없으면
                Debug.Log("\n" + "경로에 해당되는 폴더가 없어서 폴더를 추가시켯했습니다.");
                AssetDatabase.CreateFolder("Assets/A0", customFolderName);
            }

            //해당폴더가 있는가?
            string path = $"Assets/A0/{customFolderName}/";
            if (!string.IsNullOrEmpty(folderName))
            {
                string[] splitFolderNameArray = folderName.Split('/');
                folderName = "";

                for (int i = 0; i < splitFolderNameArray.Length; i++)
                {
                    if (!AssetDatabase.IsValidFolder($"Assets/A0/{customFolderName + folderName}/" + splitFolderNameArray[i]))
                    {
                        //폴더가 없으면
                        Debug.Log($"Assets/A0/{customFolderName + folderName}/{splitFolderNameArray[i]} 경로에 해당되는 폴더가 없어서 폴더를 추가시켯했습니다.");
                        AssetDatabase.CreateFolder($"Assets/A0/{customFolderName + folderName}", splitFolderNameArray[i]);
                    }
                    folderName += '/' + splitFolderNameArray[i];
                }

                path += folderName.Substring(1, folderName.Length - 1) + "/";
            }

            if (string.IsNullOrEmpty(tag))
            {
                path += fileName + ".asset";
            }
            else
            {
                path += fileName + "_" + tag + ".asset";
            }

            //이미 존재하면 덮어쓰기로
            T overWriteObject = AssetDatabase.LoadMainAssetAtPath(path) as T;
            if (overWriteObject == null)
            {
                //존재하지않으면 
                //생성
                newDataObject.name = fileName;                
                AssetDatabase.CreateAsset(newDataObject, path);
                overWriteObject = newDataObject;
            }
            else
            {
                //존재하면
                //복사 및 교체되지만 링크는 유지//덮어쓰기//잘됨
                string name = overWriteObject.name;//전에 쓰던이름을 가져와서 새로 쓸오브젝트에 대입시켜줘야됨
                EditorUtility.CopySerializedIfDifferent(newDataObject, overWriteObject);
                newDataObject = overWriteObject;
                newDataObject.name = name;

                //값만 복사//잘됨
                //GetCopyOf(newDataObject, overWriteObject);
                //newDataObject = overWriteObject;
            }

            //생성
            EditorUtility.SetDirty(overWriteObject);
            //AssetDatabase.CreateAsset(dataObject, content);
            AssetDatabase.SaveAssets();//저장되지 않은 모든 자산 변경 사항을 디스크에 씁니다.
            AssetDatabase.Refresh();//새로고침
            Selection.activeObject = overWriteObject;//선택
            //Debug.Log(tag + " " + dataObject.name + "가 추가 되었습니다." + "\n" + "경로는 Assets/A0/GameData/" + path + "/ 입니다");
            if (isUseDisplay)
            {
                EditorUtility.DisplayDialog("저장 안내창", overWriteObject.name + "가 추가 되었습니다." + "\n" + "경로는 " + path + " 입니다", "OK");
            }
        }

        /// <summary>
        /// 프리팹생성 함수
        /// </summary>
        /// <param name="newDataObject">해당되는 신규데이터</param>
        /// <param name="fileName">데이터파일 이름</param>
        /// <param name="folderName">지정할 폴더이름</param>
        /// <param name="tag">꼬리표</param>
        /// <param name="isUseDisplay">안내창을 표시할건지 여부</param>
        public static void CreatePrefabAsset<T>(ref T newDataObject, string fileName, string folderName, string tag, bool isUseDisplay = true) where T : Component
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.Log("데이터오브젝트 이름을 정해주세요");
                return;
            }

            //데이터에셋 새롭게 만들기
            //해당폴더가 있는가?
            if (!AssetDatabase.IsValidFolder("Assets/A0"))
            {
                //폴더가 없으면
                Debug.Log("\n" + "경로에 해당되는 폴더가 없어서 폴더를 추가시켯했습니다.");
                AssetDatabase.CreateFolder("Assets", "A0");
            }

            //해당폴더가 있는가?
            if (!AssetDatabase.IsValidFolder($"Assets/A0/{customFolderName}"))
            {
                //폴더가 없으면
                Debug.Log("\n" + "경로에 해당되는 폴더가 없어서 폴더를 추가시켯했습니다.");
                AssetDatabase.CreateFolder("Assets/A0", customFolderName);
            }

            //해당폴더가 있는가?
            string path = $"Assets/A0/{customFolderName}/";
            if (!string.IsNullOrEmpty(folderName))
            {
                string[] splitFolderNameArray = folderName.Split('/');
                folderName = "";

                for (int i = 0; i < splitFolderNameArray.Length; i++)
                {
                    if (!AssetDatabase.IsValidFolder($"Assets/A0/{customFolderName + folderName}/" + splitFolderNameArray[i]))
                    {
                        //폴더가 없으면
                        Debug.Log($"Assets/A0/{customFolderName + folderName}/{splitFolderNameArray[i]} 경로에 해당되는 폴더가 없어서 폴더를 추가시켯했습니다.");
                        AssetDatabase.CreateFolder($"Assets/A0/{customFolderName + folderName}", splitFolderNameArray[i]);
                    }
                    folderName += '/' + splitFolderNameArray[i];
                }

                path += folderName.Substring(1, folderName.Length - 1) + "/";
            }

            if (string.IsNullOrEmpty(tag))
            {
                path += fileName + ".prefab";
            }
            else
            {
                path += fileName + "_" + tag + ".prefab";
            }

            //이미 존재하면 덮어쓰기로
            T overWriteObject = AssetDatabase.LoadMainAssetAtPath(path) as T;
            if (overWriteObject == null)
            {
                Debug.Log("존재X");
                //존재하지않으면 
                //생성
                newDataObject.name = fileName;
                var gameObject = PrefabUtility.SaveAsPrefabAsset(newDataObject.gameObject, path, out bool check);

                //var gameObject = PrefabUtility.SaveAsPrefabAssetAndConnect(newDataObject.gameObject,path,InteractionMode.AutomatedAction);
                //AssetDatabase.CreateAsset(newDataObject, path);


                overWriteObject = newDataObject = gameObject.GetComponent<T>();
            }
            else
            {
                Debug.Log("존재O");
                //존재하면
                //복사 및 교체되지만 링크는 유지//덮어쓰기//잘됨
                string name = overWriteObject.name;//전에 쓰던이름을 가져와서 새로 쓸오브젝트에 대입시켜줘야됨
                EditorUtility.CopySerializedIfDifferent(newDataObject, overWriteObject);
                newDataObject = overWriteObject;
                newDataObject.name = name;

                //값만 복사//잘됨
                //GetCopyOf(newDataObject, overWriteObject);
                //newDataObject = overWriteObject;
            }

            //생성
            //AssetDatabase.CreateAsset(dataObject, content);
            EditorUtility.SetDirty(overWriteObject);
            AssetDatabase.SaveAssets();//저장되지 않은 모든 자산 변경 사항을 디스크에 씁니다.
            AssetDatabase.Refresh();//새로고침
            Selection.activeObject = overWriteObject;//선택
            //EditorUtility.SetDirty(prefabReference);
            //Debug.Log(tag + " " + dataObject.name + "가 추가 되었습니다." + "\n" + "경로는 Assets/A0/GameData/" + path + "/ 입니다");
            if (isUseDisplay)
            {
                EditorUtility.DisplayDialog("저장 안내창", overWriteObject.name + "가 추가 되었습니다." + "\n" + "경로는 " + path + " 입니다", "OK");
            }
        }

        /// <summary>
        /// Create Asset File
        /// </summary>
        /// <param name="newDataObject">NewDataObject</param>
        /// <param name="fileName">DataFile Name</param>
        /// <param name="isUseDisplay">Is Show Display</param>
        public static void CreateDataObject<T>(ref T newDataObject, string fileName, DefaultAsset folder, bool isUseDisplay = true) where T : Object
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.Log("Write FileName.");
                return;
            }

            var path = $"{AssetDatabase.GetAssetPath(folder)}/{fileName}.asset";


            //이미 존재하면 덮어쓰기로
            T overWriteObject = AssetDatabase.LoadMainAssetAtPath(path) as T;
            if (overWriteObject == null)
            {
                //존재하지않으면 
                //생성
                newDataObject.name = fileName;
                AssetDatabase.CreateAsset(newDataObject, path);
                overWriteObject = newDataObject;
            }
            else
            {
                //존재하면
                //복사 및 교체되지만 링크는 유지//덮어쓰기//잘됨
                string name = overWriteObject.name;//전에 쓰던이름을 가져와서 새로 쓸오브젝트에 대입시켜줘야됨
                EditorUtility.CopySerializedIfDifferent(newDataObject, overWriteObject);
                newDataObject = overWriteObject;
                newDataObject.name = name;
            }

            //생성
            EditorUtility.SetDirty(overWriteObject);
            AssetDatabase.SaveAssets();//저장되지 않은 모든 자산 변경 사항을 디스크에 씁니다.
            AssetDatabase.Refresh();//새로고침
            Selection.activeObject = overWriteObject;//선택
            //Debug.Log(tag + " " + dataObject.name + "가 추가 되었습니다." + "\n" + "경로는 Assets/A0/GameData/" + path + "/ 입니다");
            if (isUseDisplay)
            {
                EditorUtility.DisplayDialog("Save Info", overWriteObject.name + "Added." + "\n" + "Path : " + path, "OK");
            }
        }

        /// <summary>
        /// Create Prefab Asset
        /// </summary>
        /// <param name="newDataObject">prefab target</param>
        /// <param name="fileName">DataFile Name</param>
        /// <param name="folder">Folder</param>
        /// <param name="isUseDisplay">Is Show Display</param>
        public static void CreatePrefabAsset<T>(ref T newDataObject, string fileName, DefaultAsset folder, bool isUseDisplay = true) where T : Component
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.Log("Write FileName");
                return;
            }
            if (newDataObject == null)
            {
                return;
            }

            var path = $"{AssetDatabase.GetAssetPath(folder)}/{fileName}.prefab";

            //이미 존재하면 덮어쓰기로
            T overWriteObject = AssetDatabase.LoadMainAssetAtPath(path) as T;
            newDataObject.name = fileName;
            var gameObject = PrefabUtility.SaveAsPrefabAsset(newDataObject.gameObject, path, out bool check);
            overWriteObject = newDataObject = gameObject.GetComponent<T>();

            //생성
            EditorUtility.SetDirty(overWriteObject);
            AssetDatabase.SaveAssets();//저장되지 않은 모든 자산 변경 사항을 디스크에 씁니다.
            AssetDatabase.Refresh();//새로고침
            Selection.activeObject = overWriteObject;//선택
            //EditorUtility.SetDirty(prefabReference);
            //Debug.Log(tag + " " + dataObject.name + "가 추가 되었습니다." + "\n" + "경로는 Assets/A0/GameData/" + path + "/ 입니다");
            if (isUseDisplay)
            {
                EditorUtility.DisplayDialog("Save Info", overWriteObject.name + "Added." + "\n" + "Path : " + path, "OK");
            }
        }


        /// <summary>
        /// 사용중지//리소스데이터생성 함수//이건아직 갱신안한거임 갱신하기//
        /// </summary>
        /// <param name="newDataObject">해당되는 신규데이터</param>
        /// <param name="fileName">데이터파일 이름</param>
        /// <param name="folderName">지정할 폴더이름</param>
        public static void CreateResourceDataObject<T>(ref T newDataObject, string fileName, string folderName, bool isUseDisplay = true) where T : Object
        {
            if (fileName == "")
            {
                Debug.Log("데이터오브젝트 이름을 정해주세요");
                return;
            }

            //데이터에셋 새롭게 만들기
            //해당폴더가 있는가?
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                //폴더가 없으면
                Debug.Log("\n" + "경로에 해당되는 폴더가 없어서 폴더를 추가시켯했습니다.");
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            //해당폴더가 있는가?
            string path = "Assets/Resources/";
            if (!string.IsNullOrEmpty(folderName))
            {
                string[] splitFolderNameArray = folderName.Split('/');
                folderName = "";

                for (int i = 0; i < splitFolderNameArray.Length; i++)
                {
                    if (!AssetDatabase.IsValidFolder($"Assets/A0/{customFolderName + folderName}/" + splitFolderNameArray[i]))
                    {
                        //폴더가 없으면
                        Debug.Log($"Assets/A0/{customFolderName + folderName}/{splitFolderNameArray[i]} 경로에 해당되는 폴더가 없어서 폴더를 추가시켯했습니다.");
                        AssetDatabase.CreateFolder($"Assets/A0/{customFolderName + folderName}", splitFolderNameArray[i]);
                    }
                    folderName += '/' + splitFolderNameArray[i];
                }

                path += folderName.Substring(1, folderName.Length - 1) + "/";
            }

            path += fileName + ".asset";

            //이미 존재하면 덮어쓰기
            T overWriteObject = AssetDatabase.LoadMainAssetAtPath(path) as T;
            if (overWriteObject == null)
            {
                //존재하지않으면 
                //생성
                AssetDatabase.CreateAsset(newDataObject, path);
                overWriteObject = newDataObject;
            }
            else
            {
                //존재하면
                //복사 및 교체되지만 링크는 유지//덮어쓰기//잘됨
                string name = overWriteObject.name;//전에 쓰던이름을 가져와서 새로 쓸오브젝트에 대입시켜줘야됨
                EditorUtility.CopySerializedIfDifferent(newDataObject, overWriteObject);
                newDataObject = overWriteObject;
                newDataObject.name = name;

                //값만 복사//잘됨
                //GetCopyOf(newDataObject, overWriteObject);
                //newDataObject = overWriteObject;
            }

            //AssetDatabase.CreateAsset(dataObject, content);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();//새로고침
            Selection.activeObject = overWriteObject;//선택
            //Debug.Log(tag + " " + dataObject.name + "가 추가 되었습니다." + "\n" + "경로는 Assets/A0/GameData/" + folderName + "/ 입니다");
            if (isUseDisplay)
            {
                EditorUtility.DisplayDialog("저장 안내창", overWriteObject.name + "가 추가 되었습니다." + "\n" + "경로는 " + path + " 입니다", "OK");
            }
        }

    


        /// <summary>
        /// 모든리소시스에서 특정타입에 대한 파일들을 찾아오는 함수
        /// </summary>
        /// <typeparam name="T">UnityEngine.Object를 상속받은 타입</typeparam>
        /// <returns></returns>
        public static ResourcesBible GetResourcesForAllSearch<T>() where T : Object
        {
            //AssetDataBase 폴더에 있는 것들중에 원하는거 가져오기

            //먼저 리소시스폴더들을 찾아서 경로들을 가져오고 해당경로들에서 내부경로를 다 찾아오기//재귀처리하기
            List<string> resourcesPathList = new List<string>();
            FindPathForResourcesDirectory("Assets", ref resourcesPathList);            

            //이제 각각 경로에서 파일 가져오기
            //여기도 재귀해서 폴더내부찾기
            //파일찾는거 체크//딕셔너리로 체크하는것도?//그냥 다가져와서 미리 체크해버리자구.
            ResourcesBible targetObjectList = new();
            foreach (var path in resourcesPathList)
            {
                //해당 리소시스경로에서 모든 폴더를 찾은후 원하는 파일들을 찾아옴
                //내부에서 찾는것도?
                FindResourcesInAllDirectoryFiles<T>(path, ref targetObjectList);

                //lLcroweUtil.LogIList(tempData);
                //재귀적으로 찾는거 보기
            }

            //이름찾기
            //Debug.Log($"찾은 모든 수량{targetObjectList.Count}");
            //lLcroweUtil.LogIList(resourcesPathList);

            return targetObjectList;
        }

        //재귀적으로 Asset파일의 모든 디렉토리에서 Resources폴더를 찾아서 경로를 받아오는 함수        
        private static void FindPathForResourcesDirectory(string path, ref List<string> resourcesPathList)
        {
            var directories = GetFolders(path);
            foreach (var item in directories)
            {
                //리소시스면 등록하고 다음걸로 
                if (item.Name == "Resources")
                {
                    resourcesPathList.Add($"{path}/{item.Name}");
                    continue;
                }

                //아닐시 재귀작동
                FindPathForResourcesDirectory($"{path}/{item.Name}", ref resourcesPathList);
            }
        }

        /// <summary>
        /// Resources폴더 내부의 모든 폴더에서 특정 오브젝트를 찾아서 등록하는 함수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="targetBible"></param>
        private static void FindResourcesInAllDirectoryFiles<T>(string path, ref ResourcesBible targetBible) where T : Object
        {
            //폴더를 먼저찾아서 최하단까지 들어간후
            var directories = GetFolders(path);
            foreach (var item in directories)
            {
                FindResourcesInAllDirectoryFiles<T>($"{path}/{item.Name}", ref targetBible);
            }

            //거기서부터 파일들 찾아옴//폴더가 더없으면 //파일들 찾아와서 가져오기
            var tempData = GetUnityObjectFiles<T>(path, null);
            foreach (var item in tempData)
            {
                if (item == null)
                {
                    continue;
                }

                //텍스쳐쪽이면처리
                Sprite sprite = item as Sprite;
                if (sprite != null)
                {
                    T temp = sprite as T;
                    targetBible.TryAdd(item.name, temp);
                }
                targetBible.TryAdd(item.name, item);
            }
        }

        /// <summary>
        /// Resources의 모든폴더경로를 가져오는 함수
        /// </summary>
        /// <returns>폴더경로들</returns>
        public static List<string> GetResourcesInAllPath()
        {
            List<string> pathList = new List<string>();            
            FindPathForResourcesDirectory("Assets", ref pathList);

            List<string> resourcesInPathList = new List<string>();
            resourcesInPathList.Add("");//초기장소
            foreach (var item in pathList)
            {
                FindPathForResourceDirectoryInAllFolders(item, ref resourcesInPathList);
            }
            
            return resourcesInPathList;
        }

        private static void FindPathForResourceDirectoryInAllFolders(string path, ref List<string> resourcesPathList)
        {
            var directories = GetFolders(path);
            string resourcesPath = "Resources/";
            foreach (var item in directories)
            {
                string tempPath = $"{path}/{item.Name}";

                //문자열에서 Resources전까지는 다 잘라버림//등록
                int indexOfTarget = tempPath.IndexOf(resourcesPath);
                if (indexOfTarget >= 0)
                {
                    string resultString = tempPath.Substring(indexOfTarget + resourcesPath.Length);
                    resourcesPathList.Add(resultString);
                }
                FindPathForResourceDirectoryInAllFolders(tempPath, ref resourcesPathList);
            }
        }

        public static void UndoStack(Object targetUndoObject, string content)
        {
            //테스트중
            //언도 리도 둘다 스택처리
            //Undo의 대상이 되는 것은 UnityEngine.Object를 상속//Serialize 가능한 오브젝트.

            EditorGUI.BeginChangeCheck();
            //취소할시 보여줄 내용
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCreatedObjectUndo(targetUndoObject, content);

                //변경전의 오브젝트상태
                Undo.RecordObject(targetUndoObject, content);
            }

        }

        /// <summary>
        /// 배열에 Add해주는 함수
        /// </summary>
        /// <typeparam name="T">타입</typeparam>
        /// <param name="array">참고할 배열</param>
        /// <param name="target">더할 개체</param>        
        public static void AddArray<T>(ref T[] array, T target)
        {
            //리스트사용해서 처리
            List<T> tempList = new List<T>();
            tempList.AddRange(array);
            tempList.Add(target);
        }

        /// <summary>
        /// 배열에 Remove해주는 함수
        /// </summary>
        /// <typeparam name="T">타입</typeparam>
        /// <param name="array">참고할 배열</param>
        /// <param name="target">뺼 개체</param>
        public static void RemoveArray<T>(ref T[] array, T target)
        {
            //리스트사용해서 처리
            List<T> tempList = new List<T>();
            tempList.AddRange(array);
            tempList.Remove(target);
        }

        /// <summary>
        /// 인스팩터 리스트에 있는 데이터를 보여주면서 삭제버튼까지보여주는 함수(바로다음줄에 추가기능만들것)
        /// </summary>        
        /// <param name="list">리스트</param>
        /// <param name="content">내용</param>
        private static void MenuItemDataShow<T>(ref List<T> list, string content)
        {
            //리스트형
            //단일형//다수형은 문제가 있으니 로직만 참고하고 별개로 구현
            //삭제만 구현됨
            //추가 같은경우 다형성이 많아 모든걸 하나로 할 방법이 딱히 없음
            //현 함수시작후 구현이 제일 좋아보임
            if (list != null)
            {
                EditorGUILayout.LabelField("-----------------------------------");//필요없으면 지워도 될듯
                string temp;
                if (list.Count == 0)
                {
                    temp = "-=저장된 " + content + "없습니다=-";
                }
                else
                {
                    temp = "-=저장된 " + content + "표시=-";
                }
                EditorGUILayout.LabelField(temp);

                for (int i = 0; i < list.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(list[i].ToString() + "-" + content + list[i].ToString());
                    if (GUILayout.Button("조건 삭제"))
                    {
                        int value = i;
                        list.RemoveAt(value);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.LabelField("-----------------------------------");
            }
        }

        /// <summary>
        /// 인스팩터 배열에 있는 데이터를 보여주면서 삭제버튼까지보여주는 함수(바로다음줄에 추가기능만들것)
        /// </summary>
        /// <param name="array">배열</param>
        /// <param name="content">내용</param>
        private static void MenuItemDataShow<T>(ref T[] array, string content)
        {
            //배열형
            //단일형//다수형은 문제가 있으니 로직만 참고하고 별개로 구현
            //삭제만 구현됨
            //추가 같은경우 다형성이 많아 모든걸 하나로 할 방법이 딱히 없음
            //현 함수시작후 구현이 제일 좋아보임
            if (array != null)
            {
                EditorGUILayout.LabelField("-----------------------------------");
                string temp;
                if (array.Length == 0)
                {
                    temp = "-=저장된 " + content + "없습니다=-";
                }
                else
                {
                    temp = "-=저장된 " + content + "표시=-";
                }
                EditorGUILayout.LabelField(temp);

                for (int i = 0; i < array.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(array[i].ToString() + "-" + content + array[i].ToString());
                    if (GUILayout.Button("조건 삭제"))
                    {
                        List<T> tempList = new List<T>();
                        tempList.AddRange(array);

                        int value = i;
                        tempList.RemoveAt(value);

                        array = tempList.ToArray();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.LabelField("-----------------------------------");
            }
        }

        private static string targetTag;
        //private static LayerMask targetLayerMask;
        public static void ConstraintsConditionDataShow(ConstraintsCondition constraintsCondition)
        {
            //여기 아이템메뉴써서 고치기
            EditorGUILayout.LabelField("--=제약조건=--");
            if (constraintsCondition.useTagCondition)
            {
                MenuItemDataShow(ref constraintsCondition.interectTags, "태그");

                targetTag = EditorGUILayout.TagField("추가할 태그", targetTag);
                if (GUILayout.Button("태그추가"))
                {
                    AddArray(ref constraintsCondition.interectTags, targetTag);
                }
            }

            if (constraintsCondition.useLayerCondition)
            {
                //MenuItemDataShow(ref constraintsCondition.interectTags, "레이어");

                constraintsCondition.interectLayer = EditorGUILayout.LayerField("타겟팅할 레이어", constraintsCondition.interectLayer);
                //if (GUILayout.Button("레이어추가"))
                //{
                //    AddArray(ref constraintsCondition.interectLayers, targetLayerMask);
                //}
            }
        }

        


        //20230417//신규 포트폴리오 데이터보여주기관련
        //새롭게 제작된 라벨종류로 적용

        /// <summary>
        /// 라벨베이스 데이터를 에디터에 보여주는 함수
        /// </summary>
        /// <param name="content">컨텐츠</param>
        /// <param name="labelBase">라벨베이스 데이터</param>
        public static void LabelBaseDataShow(string content, LabelBase labelBase)
        {
            labelBase.labelID = EditorGUILayout.TextField(content + " ID", labelBase.labelID);
        }

        /// <summary>
        /// 아이콘라벨베이스 데이터를 에디터에 보여주는 함수
        /// </summary>
        /// <param name="content">컨텐츠</param>
        /// <param name="iconLabelBase">아이콘라벨베이스 데이터</param>
        public static void IconLabelBaseDataShow(string content, IconLabelBase iconLabelBase)
        {
            LabelBaseDataShow(content, iconLabelBase);
            iconLabelBase.labelNameOrTitle = EditorGUILayout.TextField(content + " 이름", iconLabelBase.labelNameOrTitle);
            iconLabelBase.name = iconLabelBase.labelNameOrTitle;//<==에디터에서 감지할수 있게 해주는것
            EditorGUILayout.LabelField(content + " 아이콘");
            //씬상의 객체를 허용하지 않음
            iconLabelBase.icon = (Sprite)EditorGUILayout.ObjectField(iconLabelBase.icon, typeof(Sprite), true);            
            EditorGUILayout.LabelField(content + " 상세설명");
            iconLabelBase.description = EditorGUILayout.TextArea(iconLabelBase.description, GUILayout.Height(100));
        }

        //public static void UnitStatusDataDataShow(string content, UnitStatusData unitStatusData)
        //{
        //    unitStatusData.teamType = (UnitTeamType)EditorGUILayout.EnumPopup("공격타입", unitStatusData.teamType);

        //    unitStatusData.maxHealth = EditorGUILayout.IntField(content + "최대체력", unitStatusData.maxHealth);
        //    unitStatusData.curHealth = EditorGUILayout.IntField(content + "현재체력", unitStatusData.curHealth);
        //    unitStatusData.damage = EditorGUILayout.IntField(content + "대미지", unitStatusData.damage);
        //    unitStatusData.skillDamage = EditorGUILayout.IntField(content + "스킬대미지", unitStatusData.skillDamage);
        //    unitStatusData.attackCoolTime = EditorGUILayout.FloatField(content + "공격속도", unitStatusData.attackCoolTime);            
        //    unitStatusData.attackDistance = EditorGUILayout.FloatField(content + "공격거리", unitStatusData.attackDistance);
        //    unitStatusData.projectilePrefab = (DamageModule)EditorGUILayout.ObjectField(content + "투사체", unitStatusData.projectilePrefab, typeof(DamageModule), false);
        //    unitStatusData.damageType = (DamageType)EditorGUILayout.EnumPopup("공격타입", unitStatusData.damageType);
        //    unitStatusData.projectileSpeed = EditorGUILayout.FloatField(content + "투사체속도", unitStatusData.projectileSpeed);
        //    unitStatusData.damageRange =EditorGUILayout.FloatField(content + "공격범위", unitStatusData.damageRange);

        //    unitStatusData.moveSpeed = EditorGUILayout.FloatField(content + "이동속도", unitStatusData.moveSpeed);
        //    unitStatusData.damageArmor = EditorGUILayout.IntField(content + "공격방어력", unitStatusData.damageArmor);
        //    unitStatusData.skillArmor = EditorGUILayout.IntField(content + "스킬공격방어력", unitStatusData.skillArmor);
        //    unitStatusData.criticalChance = EditorGUILayout.IntField(content + "크리티컬찬스", unitStatusData.criticalChance);
        //    unitStatusData.criticalScaling = EditorGUILayout.FloatField(content + "크리티컬배수", unitStatusData.criticalScaling);
        //    unitStatusData.damageArmorPenetration = EditorGUILayout.IntField(content + "방어 관통력", unitStatusData.damageArmorPenetration);
        //    unitStatusData.skillArmorPenetration = EditorGUILayout.IntField(content + "스킬방어 관통력", unitStatusData.skillArmorPenetration);
        //    unitStatusData.dodgeChanceRatio = EditorGUILayout.IntField(content + "회피찬스", unitStatusData.dodgeChanceRatio);
        //    unitStatusData.combatEndHill = EditorGUILayout.IntField(content + "전투끝 후 체력회복", unitStatusData.combatEndHill);
        //    unitStatusData.skillChargeSpeed = EditorGUILayout.FloatField(content + "스킬차지 시간", unitStatusData.skillChargeSpeed);
        //    unitStatusData.effectResistance = EditorGUILayout.IntField(content + "효과저항", unitStatusData.effectResistance);
        //    unitStatusData.increaseTakenDamageRatio = EditorGUILayout.IntField(content + "받는 피해량 상승", unitStatusData.increaseTakenDamageRatio);
        //    unitStatusData.decreaseTakenDamageRatio = EditorGUILayout.IntField(content + "피해차감", unitStatusData.decreaseTakenDamageRatio);
        //    unitStatusData.increaseHillPowerRatio = EditorGUILayout.IntField(content + "힐 파워 상승", unitStatusData.increaseHillPowerRatio);
        //}

        /// <summary>
        /// 트랜스폼데이터를 보여주는 함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="content">내용</param>
        public static void TransformDataShow(Transform tr, string content)
        {
            //쿼터니온을 오일러로 바꾸고 쿼터니온으로 바꾸는 과정에서
            //몇도이상에서 안넘어가는 문제가 발생
            //그래서 오일러 값으로만 진행//뭔가 미세한 차이가 있는데

            tr.position = EditorGUILayout.Vector3Field(content + " 위치", tr.position);
            tr.eulerAngles = EditorGUILayout.Vector3Field(content + " 회전", tr.rotation.eulerAngles);
            tr.localScale = EditorGUILayout.Vector3Field(content + " 스케일", tr.localScale);            
        }

        public static void DictionaryShow<T1, T2>(CustomDictionary<T1,T2> CustomDictionary)
        {
            foreach (var item in CustomDictionary)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(item.Key.ToString() + ":" + item.Value.ToString());
                if (GUILayout.Button("삭제"))
                {
                    CustomDictionary.Remove(item.Key);
                    EditorGUILayout.EndHorizontal();
                    CustomDictionary.SyncDictionaryToList();
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        //차이에 맞게 수정해야됨
        //700 515

        private static GUILayoutOption[] animationCurveOptions = new[] 
        {
            //GUILayout.Width (600),
            GUILayout.Height (100),
            GUILayout.ExpandHeight (true)
        };
        
        private static GUILayoutOption[] showDataOptions = new[]
        {
            GUILayout.Width (200),
            //GUILayout.ex(100),
            GUILayout.ExpandWidth(true)
        };

        private static GUILayoutOption[] showLabelDataOptions = new[]
        {
            GUILayout.Width(170),
            
            
        };

        /// <summary>
        /// 커브데이터 보여주는 함수
        /// </summary>
        /// <param name="content">내용</param>
        /// <param name="curveData">커브데이터</param>
        /// <param name="limitMaxLevel">제한시킬최대레벨 (0이하일시 무시)</param>
        /// <param name="isModifyIntValue">Int로 수정여부</param>
        /// <param name="isShowData">데이터보여줄지 여부</param>
        /// <param name="scrollPos">스크롤좌표</param>
        public static void LevelCurveDataShow(string content, LevelCurveData curveData, int limitMaxLevel, bool isModifyIntValue, bool isShowData, ref Vector2 scrollPos)
        {
            int minLevel = curveData.MinLevel.GetLimitAmount(1, 1);
            float minValue = curveData.MinValue.GetLimitMin(0);
            int maxLevel = limitMaxLevel > 0 ? curveData.MaxLevel.GetLimitAmount(limitMaxLevel, limitMaxLevel) : curveData.MaxLevel;
            float maxValue = curveData.MaxValue;

            //최대치에 대한 최소치체크
            maxLevel = maxLevel < minLevel ? minLevel : maxLevel;
            maxValue = maxValue < minValue ? minValue : maxValue;

            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
            //EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal ();
            minLevel = EditorGUILayout.IntField("최소레벨", minLevel);
            maxLevel = EditorGUILayout.IntField("최대레벨", maxLevel);            
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            if (isModifyIntValue)
            {
                minValue = EditorGUILayout.IntField($"최소{content}", minValue.RoundToInt());
                maxValue = EditorGUILayout.IntField($"최대{content}", maxValue.RoundToInt());
            }
            else
            {
                minValue = EditorGUILayout.FloatField($"최소{content}", minValue);
                maxValue = EditorGUILayout.FloatField($"최대{content}", maxValue);
            }

            //데이터집어넣기
            curveData.MaxLevel = maxLevel;
            curveData.MaxValue = maxValue;
            curveData.MinLevel = minLevel;
            curveData.MinValue = minValue;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            curveData.LevelCurve = EditorGUILayout.CurveField(curveData.LevelCurve, animationCurveOptions);

            //옆에 데이터쇼도 보면 좋을거같음
            if (isShowData)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(200));
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandWidth(true));
                for (int i = curveData.MinLevel; i <= curveData.MaxLevel; i++)
                {
                    EditorGUILayout.BeginHorizontal("Box");
                    var data = curveData.LevelCurve.Evaluate(i);
                    if (isModifyIntValue)
                    {
                        var convertData = data.RoundToInt();
                        EditorGUILayout.LabelField($"Lv{i}: {convertData}", showLabelDataOptions);
                    }
                    else
                    {
                        var convertData = data.RoundToFloat(2);
                        //var convertData = System.Math.Round(data, 2);
                        EditorGUILayout.LabelField($"Lv{i}: {convertData}", showLabelDataOptions);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            //리니어 커브 정하기 집어넣자
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Linear"))
            {
                curveData.LevelCurve = AnimationCurve.Linear(curveData.MinLevel, curveData.MinValue, curveData.MaxLevel, curveData.MaxValue);
            }
            if (GUILayout.Button("EaseInOut"))
            {
                curveData.LevelCurve = AnimationCurve.EaseInOut(curveData.MinLevel, curveData.MinValue, curveData.MaxLevel, curveData.MaxValue);
            }

            //if (GUILayout.Button("EaseIn"))
            //{
            //    Keyframe[] array = new Keyframe[2]
            //    {   
            //        new Keyframe(minLevel, minValue, 0f, 0f),
            //        new Keyframe(maxLevel, maxValue, 5f, 0f)
            //    };
            //    curveData.LevelCurve.keys = array;
            //}

            //if (GUILayout.Button("EaseOut"))
            //{   
            //    Keyframe[] array = new Keyframe[2]
            //    {   
            //        new Keyframe(minLevel, minValue, 0f, 0),
            //        new Keyframe(maxLevel, maxValue, 0f, 0f)                
            //    };
            //    curveData.LevelCurve.keys = array;
            //}
            EditorGUILayout.EndHorizontal();


            ////키값가져와서 최대레벨이랑 경험치가 다르면 새로생성하게
            //Keyframe[] keyframeArray = curveData.LevelCurve.keys;
            //Keyframe firstKeyFrame = keyframeArray[0];
            //Keyframe lastKeyFrame = keyframeArray[keyframeArray.Length - 1];

            //int checkMinLv = (int)firstKeyFrame.time;
            //float checkMinExp = firstKeyFrame.value;

            //int checkMaxLv = (int)lastKeyFrame.time;
            //float checkMaxExp = lastKeyFrame.value;
            //작동방식변경되버림
            //최소치//최대치//수치체크후 낮거나 높으면 해당값에 맞추어서 변경
            //if (checkMinLv != curveData.MinLevel || checkMinExp != curveData.MinValue || checkMaxLv != curveData.MaxLevel || checkMaxExp != curveData.MaxValue)
            //{
            //    curveData.LevelCurve = new AnimationCurve(new Keyframe(curveData.MinLevel, curveData.MinValue, firstKeyFrame.inTangent, firstKeyFrame.outTangent), new Keyframe(curveData.MaxLevel, curveData.MaxValue, lastKeyFrame.inTangent, lastKeyFrame.outTangent));
            //}

            EditorGUILayout.EndVertical();
        }

        // 최대값에 따라 inTangent 계산하는 함수
        private static float CalInTangent(float currentValue, float maxValue)
        {
            // 최대값에 따라 inTangent를 동적으로 계산하는 로직을 작성
            // 이 예시에서는 간단하게 최대값을 기준으로 inTangent를 조절하는 로직을 사용
            float normalizedValue = Mathf.Clamp01(currentValue / maxValue);
            float inTangent = Mathf.Lerp(1.0f, 5.0f, normalizedValue); // 최대값에 따라 조절 가능한 예시

            return inTangent;
        }

        /// <summary>
        /// 애니메이션커브를 에디터상에서 보여주는 클래스
        /// </summary>
        public class AnimationCurveShowEditor
        {
            private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(10, value: 1000));
            private Vector2 animationScroll;
            private bool isWrite = false;

            /// <summary>
            /// 커브를 사용해서 Int[] 배열을 자동으로 맞추어주는 함수
            /// </summary>
            /// <param name="intArray">int배열</param>
            public static void ArrayCurveShow(AnimationCurveShowEditor animationCurveShowEditor, ref int[] intArray)
            {
                if (intArray.Length <= 1)
                {
                    //작동불가
                    //2개의 인덱스가 필요함
                    EditorGUILayout.HelpBox("최소 2개의 인덱스가 필요함", MessageType.Warning);
                    return;
                }

                //최소레벨 = 0
                //최대레벨 = 맥스치
                //최소값 = 0번째인덱스
                //최대값 = 맥스치인덱스
                int firstLevel = 0;
                int lastLevel = intArray.Length - 1;
                int firstValue = intArray[firstLevel];
                int lastValue = intArray[lastLevel];

                Keyframe[] keyframeArray = animationCurveShowEditor.animationCurve.keys;
                Keyframe firstKeyFrame = keyframeArray[0];
                Keyframe lastKeyFrame = keyframeArray[keyframeArray.Length - 1];

                int checkMinLv = (int)firstKeyFrame.time;
                int checkMinValue = (int)firstKeyFrame.value;

                int checkMaxLv = (int)lastKeyFrame.time;
                int checkMaxValue = (int)lastKeyFrame.value;

                //적용할건지 체크
                animationCurveShowEditor.isWrite = EditorGUILayout.Toggle("수치를 적용할건지 여부", animationCurveShowEditor.isWrite);
                //수치체크
                EditorGUILayout.LabelField("증가치에 사용될 커브");
                animationCurveShowEditor.animationCurve = EditorGUILayout.CurveField(animationCurveShowEditor.animationCurve);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Linear"))
                {
                    animationCurveShowEditor.animationCurve = AnimationCurve.Linear(firstLevel, firstValue, lastLevel, lastValue);

                }
                if (GUILayout.Button("EaseInOut"))
                {
                    animationCurveShowEditor.animationCurve = AnimationCurve.EaseInOut(firstLevel, firstValue, lastLevel, lastValue);
                }

                //수치체크
                if (checkMinLv != firstLevel || checkMinValue != firstValue || checkMaxLv != lastLevel || checkMaxValue != lastValue)
                {
                    animationCurveShowEditor.animationCurve = new AnimationCurve(new Keyframe(firstLevel, firstValue, firstKeyFrame.inTangent, firstKeyFrame.outTangent), new Keyframe(lastLevel, lastValue, lastKeyFrame.inTangent, lastKeyFrame.outTangent));
                }
                EditorGUILayout.EndHorizontal();

                animationCurveShowEditor.animationScroll = EditorGUILayout.BeginScrollView(animationCurveShowEditor.animationScroll, GUILayout.Height(200));
                //시작과 마지막은 변동되야하므로 패스
                for (int i = 0; i < intArray.Length; i++)
                {
                    if (i != 0 || i != intArray.Length - 1)
                    {
                        intArray[i] = (int)animationCurveShowEditor.animationCurve.Evaluate(i);
                    }
                    EditorGUILayout.BeginHorizontal("Box");
                    EditorGUILayout.LabelField("Lv " + i);
                    EditorGUILayout.LabelField(intArray[i].ToString());
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }

            /// <summary>
            /// 커브를 사용해서 float[] 배열을 자동으로 맞추어주는 함수
            /// </summary>
            /// <param name="floatArray">float배열</param>
            public static void ArrayCurveShow(AnimationCurveShowEditor animationCurveShowEditor, ref float[] floatArray)
            {
                if (floatArray.Length <= 1)
                {
                    //작동불가
                    //2개의 인덱스가 필요함
                    EditorGUILayout.HelpBox("최소 2개의 인덱스가 필요함", MessageType.Warning);
                    return;
                }

                //최소레벨 = 0
                //최대레벨 = 맥스치
                //최소값 = 0번째인덱스
                //최대값 = 맥스치인덱스
                int firstLevel = 0;
                int lastLevel = floatArray.Length - 1;
                float firstValue = floatArray[firstLevel];
                float lastValue = floatArray[lastLevel];

                Keyframe[] keyframeArray = animationCurveShowEditor.animationCurve.keys;
                Keyframe firstKeyFrame = keyframeArray[0];
                Keyframe lastKeyFrame = keyframeArray[keyframeArray.Length - 1];

                int checkMinLv = (int)firstKeyFrame.time;
                float checkMinValue = firstKeyFrame.value;

                int checkMaxLv = (int)lastKeyFrame.time;
                float checkMaxValue = lastKeyFrame.value;

                //수치체크

                EditorGUILayout.LabelField("증가치에 사용될 커브");
                animationCurveShowEditor.animationCurve = EditorGUILayout.CurveField(animationCurveShowEditor.animationCurve);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Linear"))
                {
                    animationCurveShowEditor.animationCurve = AnimationCurve.Linear(firstLevel, firstValue, lastLevel, lastValue);

                }
                if (GUILayout.Button("EaseInOut"))
                {
                    animationCurveShowEditor.animationCurve = AnimationCurve.EaseInOut(firstLevel, firstValue, lastLevel, lastValue);
                }

                //수치체크
                if (checkMinLv != firstLevel || checkMinValue != firstValue || checkMaxLv != lastLevel || checkMaxValue != lastValue)
                {
                    animationCurveShowEditor.animationCurve = new AnimationCurve(new Keyframe(firstLevel, firstValue, firstKeyFrame.inTangent, firstKeyFrame.outTangent), new Keyframe(lastLevel, lastValue, lastKeyFrame.inTangent, lastKeyFrame.outTangent));
                }
                EditorGUILayout.EndHorizontal();

                animationCurveShowEditor.animationScroll = EditorGUILayout.BeginScrollView(animationCurveShowEditor.animationScroll, GUILayout.Height(200));
                //시작과 마지막은 변동되야하므로 패스
                for (int i = 0; i < floatArray.Length; i++)
                {
                    if (i != 0 || i != floatArray.Length - 1)
                    {
                        floatArray[i] = animationCurveShowEditor.animationCurve.Evaluate(i);
                    }
                    EditorGUILayout.BeginHorizontal("Box");
                    EditorGUILayout.LabelField("Lv " + i);
                    EditorGUILayout.LabelField(floatArray[i].ToString());
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// 데이터를 보여주고 특정데이터를 추가하거나 삭제해주기 위한 프리셋을 제공하는 함수
        /// </summary>
        /// <typeparam name="T1">데이터타입1</typeparam>
        /// <typeparam name="T2">데이터타입2</typeparam>
        /// <param name="targetData">데이터</param>
        /// <param name="targetValue">값</param>
        /// <param name="keyList">데이터배열</param>
        /// <param name="valueList">값배열</param>
        /// <param name="content">내용</param>
        public static void ArrayDataShow<T1, T2>(T1 targetData, T2 targetValue, ref List<T1> keyList, ref List<T2> valueList, string content)
        {
            //두타입용

            if (GUILayout.Button("조건추가"))
            {
                if (keyList.Contains(targetData))
                {
                    Debug.Log("이미존재하는 데이터입니다.");
                    return;
                }

                if (targetData == null)
                {
                    Debug.Log("비어있는 데이터입니다.");
                    return;
                }

                keyList.Add(targetData);
                valueList.Add(targetValue);
            }

            EditorGUILayout.LabelField("-----------Start");
            for (int i = 0; i < keyList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(keyList[i].ToString() + ":" + valueList[i] + "" + content);
                if (GUILayout.Button("삭제"))
                {
                    int value = i;
                    keyList.RemoveAt(value);
                    valueList.RemoveAt(value);
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("-----------End");
        }

        /// <summary>
        /// 데이터를 보여주고 특정데이터를 추가하거나 삭제해주기 위한 프리셋을 제공하는 함수
        /// </summary>
        /// <typeparam name="T1">데이터타입1</typeparam>
        /// <typeparam name="T2">데이터타입2</typeparam>
        /// <param name="targetData">데이터</param>
        /// <param name="targetValue">값</param>
        /// <param name="dataArray">데이터배열</param>
        /// <param name="valueArray">값배열</param>
        /// <param name="content">내용</param>
        public static void ArrayDataShow<T1,T2>(T1 targetData, T2 targetValue, ref T1[] dataArray, ref T2[] valueArray, string content)
        {
            //두타입용

            if (GUILayout.Button("조건추가"))
            {
                if (dataArray.Contains(targetData))
                {
                    Debug.Log("이미존재하는 데이터입니다.");
                    return;
                }

                if (targetData == null)
                {
                    Debug.Log("비어있는 데이터입니다.");
                    return;
                }

                List<T1> tempT1List = new List<T1>();
                List<T2> tempT2List = new List<T2>();

                tempT1List.AddRange(dataArray);
                tempT2List.AddRange(valueArray);

                tempT1List.Add(targetData);
                tempT2List.Add(targetValue);

                dataArray = tempT1List.ToArray();
                valueArray = tempT2List.ToArray();
            }

            EditorGUILayout.LabelField("-----------Start");
            for (int i = 0; i < dataArray.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(dataArray[i].ToString() + ":" + valueArray[i] + "" + content);
                if (GUILayout.Button("삭제"))
                {
                    List<T1> tempT1List = new List<T1>();
                    List<T2> tempT2List = new List<T2>();

                    tempT1List.AddRange(dataArray);
                    tempT2List.AddRange(valueArray);

                    int value = i;
                    tempT1List.RemoveAt(value);
                    tempT2List.RemoveAt(value);

                    dataArray = tempT1List.ToArray();
                    valueArray = tempT2List.ToArray();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("-----------End");
        }




        /// <summary>
        /// 데이터를 보여주고 특정데이터를 추가하거나 삭제해주기 위한 프리셋을 제공하는 함수
        /// </summary>
        /// <typeparam name="T">데이터타입</typeparam>
        /// <param name="targetObject">타겟 오브젝트</param>
        /// <param name="array">데이터 배열</param>
        /// <param name="content">내용</param>
        public static void ArrayDataShow<T>(T targetObject, ref T[] array, string content)
        {
            //사용방법
            //targetObject = EditorGUILayout.TextField("사용할 " + content, targetObject); 를
            //현 함수를 쓰기전에 한번쓰고 현함수에 진입하기
            //잘작동되는지 체크하기
            EditorGUILayout.LabelField("-----------Start");
            for (int i = 0; i < array.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(array[i].ToString() + "-" + content);
                if (GUILayout.Button("삭제"))
                {
                    List<T> tempList = new List<T>();

                    tempList.AddRange(array);

                    int value = i;
                    tempList.RemoveAt(value);

                    array = tempList.ToArray();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("-----------End");

            if (GUILayout.Button($"{content}추가"))
            {
                if (array.Contains(targetObject))
                {
                    Debug.Log("이미존재하는 " + content + "입니다.");
                    return;
                }

                if (targetObject == null)
                {
                    Debug.Log("비어있는 데이터입니다.");
                    return;
                }

                List<T> tempList = new List<T>();

                tempList.AddRange(array);

                tempList.Add(targetObject);

                array = tempList.ToArray();
            }
        }

        /// <summary>
        /// 데이터를 보여주고 특정데이터를 추가하거나 삭제해주기 위한 프리셋을 제공하는 함수
        /// </summary>
        /// <typeparam name="T">데이터타입</typeparam>
        /// <param name="targetObject">타겟 오브젝트</param>
        /// <param name="list">데이터 배열</param>
        /// <param name="content">내용</param>
        public static void ArrayDataShow<T>(T targetObject, ref List<T> list, string content)
        {
            //사용방법
            //targetObject = EditorGUILayout.TextField("사용할 " + content, targetObject); 를
            //현 함수를 쓰기전에 한번쓰고 현함수에 진입하기
            //잘작동되는지 체크하기
            EditorGUILayout.LabelField("-----------Start");
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(list[i].ToString() + "-" + content);
                if (GUILayout.Button("삭제"))
                {
                    int value = i;
                    list.RemoveAt(value);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.LabelField("-----------End");

            if (GUILayout.Button("추가"))
            {
                if (list.Contains(targetObject))
                {
                    Debug.Log("이미존재하는 " + content + "입니다.");
                    return;
                }

                if (targetObject == null)
                {
                    Debug.Log("비어있는 데이터입니다.");
                    return;
                }
                list.Add(targetObject);
            }
        }


        /// <summary>
        /// 오브젝트필드와 비우기버튼
        /// </summary>
        /// <typeparam name="T">타입</typeparam>
        /// <param name="contentText">내용</param>
        /// <param name="targetObject">타겟 오브젝트</param>
        /// <param name="allowSceneObjects">씬상의 오브젝트를 할당할 여부</param>
        public static void ObjectFieldAndNullButton<T>(string contentText, ref T targetObject, bool allowSceneObjects) where T : Object
        { 
            //이름
            if (targetObject != null)
            {
                EditorGUILayout.LabelField($"Select=>[ {targetObject.name} ]");
            }

            EditorGUILayout.BeginHorizontal();
            targetObject = EditorGUILayout.ObjectField(contentText, targetObject, typeof(T), allowSceneObjects) as T;
            
            if (GUILayout.Button("Emtpy", GUILayout.Width(100)))
            {
                targetObject = null;
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 폴더경로를 지정해주는 필드
        /// </summary>
        /// <param name="content">내용</param>
        /// <param name="defaultAsset">지정해줄 대상</param>
        public static void FolderPathField(string content, ref DefaultAsset defaultAsset)
        {
            ObjectFieldAndNullButton(content, ref defaultAsset, false);
        }

        /// <summary>
        /// 에디터버튼(래핑함)+씬뷰 갱신
        /// </summary>
        /// <param name="content">버튼컨텐츠</param>
        /// <param name="action">액션</param>
        public static void EditorButton(string content, System.Action action)
        {            
            if (GUILayout.Button(content))
            {
                action?.Invoke();
                SceneView.RepaintAll();
                if (EditorSceneManager.GetActiveScene().isDirty == false)
                {
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }


        /// <summary>
        /// 에디터버튼(래핑함)
        /// </summary>
        /// <param name="content">버튼컨텐츠</param>
        /// <param name="action">액션</param>
        //public static void Button(string content, System.Action action)
        //{
            //이곳은 위에 버튼과 합치는게 맞아보임
            //if (GUILayout.Button(content))
            //{
             //   action?.Invoke();
              //  SceneView.RepaintAll();                
            //}
        //}

        /// <summary>
        /// Enum팝업
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentText"></param>
        /// <param name="targetObject"></param>
        public static void EnumPopup<T>(string contentText, ref T targetObject) where T : System.Enum
        {
            targetObject = (T)EditorGUILayout.EnumPopup(contentText, targetObject);
        }

        /// <summary>
        /// 에디터레이아웃 수평
        /// </summary>
        /// <param name="action">작동될 함수</param>
        public static void EditorGUILayoutHorizontal(System.Action action)
        {
            EditorGUILayout.BeginHorizontal();
            action?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 에디터레이아웃 수직
        /// </summary>
        /// <param name="action">작동될 함수</param>
        public static void EditorGUILayoutVertical(System.Action action)
        {
            EditorGUILayout.BeginVertical();
            action?.Invoke();
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// OnSelectionChange함수에 써주면 게임오브젝트를 선택했을시 특정컴포넌트를 받아오는 함수(EditorWindow)
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="targetComponent">타겟컴포넌트</param>
        /// <param name="editorWindow">에디터윈도우</param>
        public static void OnSelectionChangeForEditorWindow<T>(ref T targetComponent , EditorWindow editorWindow) where T : Component 
        {
            GameObject gameObject = Selection.activeObject as GameObject;
            if (gameObject == null)
            {
                targetComponent = null;
                return;
            }

            gameObject.TryGetComponent(out targetComponent);

            //GUI랑 함께
            editorWindow.Repaint();
        }

        /// <summary>
        /// OnSelectionChange함수에 써주면 게임오브젝트를 선택했을시 특정컴포넌트를 받아오는 함수(Editor)
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="targetComponent">타겟컴포넌트</param>
        /// <param name="editor">인스팩터에디터</param>
        public static void OnSelectionChangeForEditor<T>(ref T targetComponent, Editor editor) where T : Component
        {
            GameObject gameObject = Selection.activeObject as GameObject;
            if (gameObject == null)
            {
                targetComponent = null;
                return;
            }

            gameObject.TryGetComponent(out targetComponent);

            //GUI랑 함께
            editor.Repaint();
        }

        /// <summary>
        /// 씬뷰에서 게임오브젝트들이 선택안되게 해주는 함수
        /// </summary>
        public static void DontSelectObjectOnSceneView()
        {
            //SceneView업데이트할때 사용해야함
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        /// <summary>
        /// 이벤트에 들어온 마우스위치에 따른 씬뷰를 통한 레이를 가져오는 함수
        /// </summary>
        /// <param name="mousePos">마우스위치</param>
        /// <returns>레이</returns>
        public static Ray GetUnityEditorGUIToWorldRay(Vector2 mousePos)
        {
            //SceneView업데이트할때 사용해야함
            //Ray ray = sceneViewCamera.ScreenPointToRay(mousePos);//그냥하면 Y축이 뒤집힘
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            return ray;
        }

        /// <summary>
        /// 물리3D 자동시뮬레이션 on
        /// </summary>
        public static void OnAutoPhysicsSimulation()
        {
            //자동제어
            Physics.autoSimulation = false;
        }

        /// <summary>
        /// 물리3D 자동시뮬레이션 off
        /// </summary>
        public static void OffAutoPhysicsSimulation()
        {
            //자동제어
            Physics.autoSimulation = true;
        }

        /// <summary>
        /// 물리3D 수동시뮬레이션
        /// </summary>
        public static void PhysicsSimulation()
        {
            //수동제어
            Physics.Simulate(Time.fixedDeltaTime);
        }
     
        /// <summary>
        /// 물리2D 수동시뮬레이션
        /// </summary>
        public static void Physics2DSimulation()
        {
            //여긴따로 오토가 없음
            //수동제어
            Physics2D.Simulate(Time.fixedDeltaTime);
        }

        /// <summary>
        /// OnGUI의 GUI개체의 백그라운드 컬러를 변경하는 함수
        /// </summary>
        /// <param name="color">컬러</param>
        public static void OnGUIBackGroundColorLayout(Color color, System.Action action)
        {
            GUI.backgroundColor = color;
            action?.Invoke();
            //기본색깔
            GUI.backgroundColor = GUIBackGroundColor;
        }
        
        private static Color GUIBackGroundColor = new Color32(194, 194, 194, 255);//new Color32(56, 56, 56, 255);

        public static void UndoGuideLineFunc(Transform transform)
        {
            //가이드라인//undo,redo둘다 스택//undo할수 있는대상은 시리얼라이징된객체들

            //1.Undo.RegisterCreatedObjectUndo//생성후 등록//게임오브젝트Only
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Undo.RegisterCreatedObjectUndo(cube, "Create Cube");

            //2.Undo.DestroyObjectImmediate//파괴와 동시에 등록
            Undo.DestroyObjectImmediate(cube);

            //3.Undo.RecordObjects//현재 상태를 등록후 변경//변경되기전 상태를 등록//트랜스폼으로 할시 로컬포지션으로 작동
            //Undo.RecordObjects();
            Undo.RecordObject(transform,"");
            transform.position = Vector3.zero;

            //4.Undo.AddComponent//등록과 동시에 추가
            Rigidbody rb = Undo.AddComponent<Rigidbody>(transform.gameObject);

            //5.Undo.IncrementCurrentGroup//여러개를 한프레임안에서 같이 undo를 진행시 함께묶이는걸 분리시키기위한 용도
            Undo.IncrementCurrentGroup();

            //6.Undo.SetTransformParent//등록과 동시에 부모관계를 설정해준다.
            Undo.SetTransformParent(cube.transform, transform,"");

            //7.Undo.RevertAllInCurrentGroup//취소하는기능//적재하지않고 원복만 시켜줌//특정버튼을 눌렷을시 사용하게 제작할것
            Undo.RevertAllInCurrentGroup();
        }





        //기즈모처리

        /// <summary>
        /// 원형을 그려주는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        public static void DrawSphere(Vector3 pos)
        {  
            Handles.DrawWireDisc(pos, Vector3.right, 0.2f);
            Handles.DrawWireDisc(pos, Vector3.up, 0.2f);
            Handles.DrawWireDisc(pos, Vector3.forward, 0.2f);
        }

        /// <summary>
        /// 특정 방향을 알려주는콘을 그려주는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="angle">앵글</param>
        public static void DrawConn(Vector3 pos, Quaternion angle)
        {
            float gizmoSize = 0.5f;
            //Forward (z)방향으로 원
            //그려줄 거리를 설정 후 => 현재위치 + 그려줄거리 계산 후 => 현재 회전값 곱해주면 원하는 위치배치//Forward 방향 지정후=> 회전 방향 곱해주면 원하는 방향으로         
            //바깥        
            Handles.color = Color.green;
            Handles.DrawWireDisc(GetRotateNorVector(angle, Vector3.forward) + pos, GetRotateNorVector(angle, Vector3.forward), gizmoSize);
            Handles.color = Color.blue;
            Handles.DrawWireDisc(GetRotateNorVector(angle, Vector3.forward) * 0.95f + pos, GetRotateNorVector(angle, Vector3.forward), gizmoSize * 0.8f);
            Handles.color = Color.green;
            Handles.DrawWireDisc(GetRotateNorVector(angle, Vector3.forward) * 0.9f + pos, GetRotateNorVector(angle, Vector3.forward), gizmoSize * 0.4f);
            Handles.DrawWireDisc(GetRotateNorVector(angle, Vector3.forward) * 0.75f + pos, GetRotateNorVector(angle, Vector3.forward), gizmoSize * 0.3f);
            Handles.DrawWireDisc(GetRotateNorVector(angle, Vector3.forward) * 0.85f + pos, GetRotateNorVector(angle, Vector3.forward), gizmoSize * 0.2f);
            //안쪽
        }

        /// <summary>
        /// 특정방향을 알려주는 라인울 그려주는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="angle">앵글</param>
        /// <param name="distance">거리</param>
        public static void DrawHandleLine(Vector3 pos, Quaternion angle, float distance, Vector3 norVector)
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(pos, GetRotateNorVector(angle, norVector) * distance + pos);
        }

        /// <summary>
        /// 방향에 따른 노말벡터를 가져오는 함수
        /// </summary>
        /// <param name="angle">벡터방향</param>
        /// <param name="normal">0~1사이의 노말벡터</param>
        /// <returns>방향 노말벡터</returns>
        private static Vector3 GetRotateNorVector(Quaternion angle, Vector3 normal)
        {
            //transform Up Forward와 동일한 함수임//쿼터니언은 축이 두개!
            return angle * normal;
        }

        /// <summary>
        /// 트랜스폼 기즈모
        /// </summary>
        /// <param name="targerTr">타겟이 될 트랜스폼</param>
        public static void TransformHandle(Transform targerTr)
        {
            if (targerTr == null)
            {
                Debug.Log("배치할 트랜스폼이 없습니다.");
                return;
            }

            //로컬월드 축방향설정을 위한 처리
            targerTr.position = Handles.DoPositionHandle(targerTr.position, Tools.pivotRotation == PivotRotation.Local ? targerTr.rotation : Quaternion.identity);
            targerTr.rotation = Handles.DoRotationHandle(targerTr.rotation, targerTr.position);

            //이거는 테스트해보기//다를거없음
            //targerTr.position = Handles.PositionHandle(targerTr.position, targerTr.rotation);
            //targerTr.rotation = Handles.RotationHandle(targerTr.rotation, targerTr.position);
        }






        public static Vector2 GetScreenMiddlePos()
        {
            Vector2 screenPos = new Vector2(Screen.width, Screen.height) * 0.5f;
            return screenPos;
        }

        /// <summary>
        /// 자식에 있는 게임오브젝트들을 다 지우는 함수
        /// </summary>
        /// <param name="targetTr"></param>
        public static void ClearTransformForChild(Transform targetTr)
        {
            var tempTrArray = targetTr.GetComponentsInChildren<Transform>();
            //0에서하면 부모까지 없앰
            for (int i = 1; i < tempTrArray.Length; i++)
            {
                var tempTr = tempTrArray[i];
                if (tempTr == null)
                {
                    continue;
                }
                Object.DestroyImmediate(tempTr.gameObject);
            }
        }

        /// <summary>
        /// 자식에 있는 특정컴포넌트를 찾아서 가져오는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트</typeparam>
        /// <param name="targetTr">타겟팅할 Tr</param>
        /// <param name="includeInactive">비활성화 포함여부</param>
        /// <returns>찾은 컴포넌트들</returns>
        public static T[] GetChildComponents<T>(Component targetTr, bool includeInactive = false) where T : Component
        {
            var tempArray = targetTr.GetComponentsInChildren<T>(includeInactive);
            return tempArray;
        }

        /// <summary>
        /// 자식컴포넌트들중에 특정부모컴포넌트들만 가져오는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트</typeparam>
        /// <param name="targetTr">타겟 컴포넌트</param>
        /// <param name="parent">부모tr</param>
        /// <param name="includeInactive">비활성화 포함여부</param>
        /// <returns>찾은 컴포넌트들</returns>
        public static T[] GetChildComponentsToParent<T>(Component targetTr, Transform parent, bool includeInactive = false) where T : Component
        {
            var tempArray = targetTr.GetComponentsInChildren<T>(includeInactive);
            List<T> tempList = new();            
            for (int i = 0; i < tempArray.Length; i++)
            {
                var temp = tempArray[i];
                if (temp.transform.parent == parent)
                {
                    tempList.Add(temp);
                }
            }
            return tempList.ToArray();
        }

        /// <summary>
        /// 자식컴포넌트들중에 특정부모컴포넌트만 가져오는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트</typeparam>
        /// <param name="targetTr">타겟 컴포넌트</param>
        /// <param name="parent">부모tr</param>
        /// <param name="includeInactive">비활성화 포함여부</param>
        /// <returns>찾은 컴포넌트</returns>
        public static T GetChildComponentToParent<T>(Component targetTr, Transform parent, bool includeInactive = false) where T : Component
        {
            var tempArray = targetTr.GetComponentsInChildren<T>(includeInactive);            
            for (int i = 0; i < tempArray.Length; i++)
            {
                var temp = tempArray[i];
                if (temp.transform.parent == parent)
                {
                    return temp;
                }
            }
            return null;
        }

        /// <summary>
        /// 이름으로 자식게임오브젝트를 찾은후 없으면 신규게임오브젝트와 컴포넌트들을 배치시켜주는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="tr">타겟팅할 Tr</param>
        /// <param name="findGameObjectName">찾을 게임오브젝트 이름</param>
        /// <param name="targetComponent">타겟이 될 컴포넌트</param>
        /// <param name="parentTr">부모가 될Tr</param>
        public static void CheckEmptyComponentForNewGameObject<T>(Transform tr, string findGameObjectName, ref T targetComponent, Transform parentTr = null) where T : Component
        {
            if (targetComponent != null)
            {
                return;
            }

            var tempTr = tr.Find(findGameObjectName);
            if (tempTr == null)
            {
                tempTr = new GameObject().transform;
                tempTr.name = findGameObjectName;
                tempTr.parent = parentTr == null ? tr : parentTr;
                tempTr.localPosition = Vector2.zero;
            }
            var tempComponent = tempTr.GetComponent<T>();
            if (tempComponent == null)
            {
                tempComponent = tempTr.GetAddComponent<T>();
            }
            targetComponent = tempComponent;
        }

        /// <summary>
        /// 컴포넌트가 비었으면 추가해주는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트</typeparam>
        /// <param name="targetTr">타겟팅할 Tr</param>
        /// <param name="target">타겟팅할 컴포넌트</param>
        /// <param name="isNewGameObject">신규게임오브젝트생성여부</param>
        /// <param name="gameObjectName">게임오브젝트이름</param>
        public static void NullAddComonent<T>(Transform targetTr, ref T target, bool isNewGameObject = false, string gameObjectName = "") where T : Component
        {
            if (target != null)
            {
                return;
            }

            if (isNewGameObject)
            {
                CheckEmptyComponentForNewGameObject(targetTr, gameObjectName, ref target, targetTr);
            }
            else
            {
                target = targetTr.GetAddComponent<T>();
            }
        }

        /// <summary>
        /// 컴포넌트가 비어있지않으면 제거하는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트</typeparam>
        /// <param name="target">타겟팅할 컴포넌트</param>
        /// <param name="isDestroyGameObject">게임오브젝트파괴여부</param>
        public static void NotNullDestroy<T>(ref T target, bool isDestroyGameObject) where T : Component
        {
            if (target == null)
            {
                return;
            }

            if (isDestroyGameObject)
            {
                Object.DestroyImmediate(target.gameObject);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
        }

       

        /// <summary>
        /// UI작업에 필요한 요소. Transform에 Recttransform이
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static RectTransform AddGetRectTransform(Transform transform)
        {
            if (!transform.gameObject.TryGetComponent(out RectTransform rect))
            {
                rect = transform.gameObject.AddComponent<RectTransform>();
            }
            return rect;
        }





        public static void DrawRect(Rect rect, Vector2 pos, bool isEditor)
        {
            //new Vector3(rectTransfrom.rect.center.x, rectTransfrom.rect.center.y, 0.01f)
            if (isEditor)
            {
                Handles.DrawWireCube(pos, new Vector3(rect.size.x, rect.size.y, 0.01f));
            }
            else
            {
                Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
            }
        }

        public static Vector3[] ConvertBoxVectorForAA(RectTransform rectTransform)
        {
            Vector3[] connerArray = new Vector3[5];
            rectTransform.GetWorldCorners(connerArray);
            connerArray[4] = connerArray[0];
            return connerArray;
        }

        //IMGUI 인스팩터에디터용
        public static void NextLine(ref Rect position)
        {
            position.y += EditorGUIUtility.singleLineHeight;
        }
        public static void NextLine(this SerializedProperty property, ref Rect position)
        {
            position.y += EditorGUI.GetPropertyHeight(property, true);
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

        public static void EditorFoldLayout(ref bool isFold, string content, System.Action action)
        {
            isFold = EditorGUILayout.Foldout(isFold, $"{content}설정_폴드", true);
            if (isFold)
            {
                action?.Invoke();
            }
        }

        public static float GetPropertyHeight(this SerializedProperty property, string propertyName)
        {
            float height = 0;
            var list = property.FindPropertyRelative(propertyName);
            if (list != null)
            {
                height += EditorGUI.GetPropertyHeight(list, true);
            }
            return height;
        }

        public static bool GetPropery(this SerializedProperty property, string propertyName, out SerializedProperty outProperty)
        {
            outProperty = property.FindPropertyRelative(propertyName);
            if (outProperty == null)
            {
                Debug.Log($"감지된 프로퍼티가 없슴");
                return false;
            }
            return true;
        }

        public static void GetDrawCurProperty(this SerializedProperty property, string propertyName)
        {
            property = property.FindPropertyRelative(propertyName);
            if (property == null)
            {
                Debug.Log($"감지된 프로퍼티가 없슴");
                return;
            }
            EditorGUILayout.PropertyField(property);
        }

        public static FileInfo GetFile(string path, string fileName ,string extend)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfoArray = directoryInfo.GetFiles(extend);
            FileInfo fileInfo = null;
            for (int i = 0; i < fileInfoArray.Length; i++)
            {
                if (fileInfoArray[i].Name == fileName)
                {
                    fileInfo = fileInfoArray[i];
                    break;
                }
            }
            return fileInfo;
        }

        public static FileInfo[] GetFiles(string path, string extend)
        {   
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfoArray = null;
            if (string.IsNullOrEmpty(extend))
            {
                fileInfoArray = directoryInfo.GetFiles();
            }
            else
            {
                fileInfoArray = directoryInfo.GetFiles(extend);
            }
            
            return fileInfoArray;
        }

        public static DirectoryInfo[] GetFolders(string path = "Assets")
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            var directoryInfoArray = directoryInfo.GetDirectories();
            return directoryInfoArray;
        }

        /// <summary>
        /// T에 해당되는 스크립터블오브젝트들을 특정경로와 확장자로 찾아서 파일들을 가져오는 함수
        /// </summary>
        /// <param name="path">경로</param>
        /// <param name="extend">확장자</param>
        /// <returns>스크립터블파일들</returns>
        public static T[] GetScriptableObjectFile<T>(string path, string extend) where T : ScriptableObject
        {
            return GetUnityObjectFiles<T>(path, extend);
            ////경로 : "Assets/A0/GameData/ItemDataFolder";
            ////확장자 : "*.asset"
            //FileInfo[] fileInfoArray = GetFiles(path, extend);
            //List<T> tempList = new List<T>();
            ////Debug.Log(path);
            ////Debug.Log(fileInfoArray[0].Name);////파일이름만
            ////Debug.Log(fileInfoArray[0].FullName);//디렉토리까지전체나옴

            ////Debug.Log(fileInfoArray.Length);

            //for (int i = 0; i < fileInfoArray.Length; i++)
            //{
            //    FileInfo fileInfo = fileInfoArray[i];

            //    T asset = (T)AssetDatabase.LoadAssetAtPath(path + "/" + fileInfo.Name, typeof(T));
            //    if (asset != null)
            //    {
            //        tempList.Add(asset);
            //    }
            //}
            //return tempList.ToArray();
        }

        /// <summary>
        /// T에 해당되는 UnityEngine.Object들을 특정경로와 확장자로 찾아서 파일들을 가져오는 함수
        /// </summary>
        /// <param name="path">경로</param>
        /// <param name="extend">확장자</param>
        /// <returns>오브젝트파일들</returns>
        public static T[] GetUnityObjectFiles<T>(string path, string extend) where T : Object
        {
            //경로 : "Assets/A0/GameData/ItemDataFolder";
            //확장자 : "*.asset"//"*.Prefab"
            FileInfo[] fileInfoArray = GetFiles(path, extend);
            List<T> tempList = new List<T>();
            //Debug.Log(path);
            //Debug.Log(fileInfoArray[0].Name);////파일이름만
            //Debug.Log(fileInfoArray[0].FullName);//디렉토리까지전체나옴

            //Debug.Log(fileInfoArray.Length);

            for (int i = 0; i < fileInfoArray.Length; i++)
            {
                FileInfo fileInfo = fileInfoArray[i];

                T asset = AssetDatabase.LoadAssetAtPath(path + "/" + fileInfo.Name, typeof(T)) as T;
                if (asset != null)
                {
                    tempList.Add(asset);
                }
            }
            return tempList.ToArray();
        }

        /// <summary>
        /// 유니티프로젝트의 경로를 가져오는 함수
        /// </summary>
        /// <param name="target">유니티오브젝트</param>
        /// <returns>경로</returns>
        public static string GetUnityProjectPath(Object target)
        {
            //C:/Private_lLcrowe/unityProject/Dependency/Assets/Assets/CustomAbility/AbilityStatusStateEnum.cs
            string path = Application.dataPath;
            if (target != null)
            {
                path = path.Replace("/Assets", $"/{AssetDatabase.GetAssetPath(target)}");
            }
            return path;
        }

        //폴더에 따른 경로주기
        //public static string GetUnityObjectPath(Object target)
        //{
        //    //C:/Private_lLcrowe/unityProject/Dependency/Assets/Assets/CustomAbility/AbilityStatusStateEnum.cs
        //    string path = Application.dataPath.Replace("/Assets", $"/{AssetDatabase.GetAssetPath(target)}");
        //    return path;
        //}

        //첫번쨰 캐릭터 대문자 처리    
        public static string GetFirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// 유니티오브젝트의 핑해주는 함수
        /// </summary>
        /// <param name="target">유니티오브젝트</param>
        public static void PingUnityObject(Object target)
        {
            EditorGUIUtility.PingObject(target);
        }

        /// <summary>
        /// CSV 데이터 읽어오는 클래스
        /// </summary>
        public class CSVReader
        {
            //데이터나오는게 List<Dic<string, object>>//줄(Line)<데이터<헤더(Header), 값(Value)>>
            //1번줄 <ItemID, 돌멩이1> , <ItemID, 돌멩이2>        
            //키 => 라인(줄), 값 => 해당라인의 데이터들 Header와 해당라인의 값 
            //사용할려면 2번줄부터 사용하기

            // "\""
            //^".*",".*",".*",".*"\\n$
            static string HEADER_SPLIT_REPLACE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
            static string LINE_SPLIT_REPLACE = @"\r\n|\n\r|\n|\r";
            static char[] TRIM_CHARS = { '\"' };
            
            /// <summary>
            /// 리소스폴더에서 CSV읽는 함수
            /// </summary>
            /// <param name="path">경로</param>
            /// <param name="fileName">파일이름</param>
            /// <returns></returns>
            public static List<Dictionary<string, object>> ReadResources(string path, string fileName)
            {   
                TextAsset data = Resources.Load(path + "/" + fileName) as TextAsset;
                return ReadTextAsset(data);
            }

            /// <summary>
            /// AssetDatabase에서 경로와 확장자에 따라 읽어오는 함수
            /// </summary>
            /// <param name="path">경로</param>
            /// <param name="fileName">파일이름</param>
            /// <returns></returns>
            public static List<Dictionary<string, object>> ReadAssetDatabase(string path, string fileName)
            {   
                TextAsset data = (TextAsset)AssetDatabase.LoadAssetAtPath(path + "/" + fileName, typeof(TextAsset));
                return ReadTextAsset(data);
            }

            /// <summary>
            /// TextAsset의 CSV를 읽어오는 함수
            /// </summary>
            /// <param name="textFile">텍스트에셋</param>
            /// <returns></returns>
            public static List<Dictionary<string, object>> ReadTextAsset(TextAsset textFile)
            {
                var list = new List<Dictionary<string, object>>();
                TextAsset data = textFile;
                if (data == null)
                {
                    Debug.Log("데이터가 비었습니다");
                    return list;
                }

                return SplitLineAndHeader(ref list, data.text);
            }

            public static List<Dictionary<string, object>> ReadString(string content)
            {
                var list = new List<Dictionary<string, object>>();
                if (string.IsNullOrEmpty(content))
                {
                    Debug.Log("데이터가 비었습니다");
                    return list;
                }

                return SplitLineAndHeader(ref list, content);
            }


            private static List<Dictionary<string, object>> SplitLineAndHeader(ref List<Dictionary<string, object>> list, string text)
            {
                //라인제거
                var lines = Regex.Split(text, LINE_SPLIT_REPLACE);

                //라인존재 체크
                if (lines.Length <= 1) return list;

                var header = Regex.Split(lines[0], HEADER_SPLIT_REPLACE);

                //헤더 잔털제거                
                for (int i = 0; i < header.Length; i++)
                {
                    string value = header[i];
                    header[i] = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                }

                //라인추가
                for (var i = 1; i < lines.Length; i++)
                {

                    var values = Regex.Split(lines[i], HEADER_SPLIT_REPLACE);
                    if (values.Length == 0 || values[0] == "") continue;

                    var entry = new Dictionary<string, object>();
                    for (var j = 0; j < header.Length && j < values.Length; j++)
                    {
                        string value = values[j];
                        value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                        object finalvalue = value;
                        int n;
                        float f;
                        if (int.TryParse(value, out n))
                        {
                            finalvalue = n;
                        }
                        else if (float.TryParse(value, out f))
                        {
                            finalvalue = f;
                        }
                        entry[header[j]] = finalvalue;
                    }
                    list.Add(entry);
                }
                return list;
            }
        }

        /// <summary>
        /// CSV에 맞게 데이터 쓰기
        /// </summary>
        public class CSVWritter
        {
            /// <summary>
            /// CSV 쓰기
            /// </summary>
            /// <param name="lineData">라인데이터</param>
            /// <param name="filePath">파일경로</param>
            /// <param name="fileName">파일이름</param>
            public static void WriteCsv(List<string[]> lineData, string filePath, string fileName)
            {
                string[][] output = new string[lineData.Count][];

                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = lineData[i];
                }

                int length = output.GetLength(0);
                string delimiter = ",";

                StringBuilder stringBuilder = new StringBuilder();

                for (int index = 0; index < length; index++)
                    stringBuilder.AppendLine(string.Join(delimiter, output[index]));

                //Stream fileStream = new FileStream(filePath + "/" + fileName + ".csv", FileMode.CreateNew, FileAccess.Write);            
                //자동으로 UTF-8로 나옴//맞는지 의문//알아서 UTF-8로 나옴
                StreamWriter writer = File.CreateText(filePath + "/" + fileName + ".csv");
                writer.WriteLine(stringBuilder);
                writer.Dispose();
                writer.Close();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif
