using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace lLCroweTool.QC.EditorOnly
{
    public static class DataBaseEditorUtil
    {
        //이거 이제 필요없긴함
        /// <summary>
        /// 카테고리 경로타입(XXFolder => XX)
        /// </summary>
        public enum CATPathType
        {
            /// <summary>
            /// 특정폴더를 참조안함
            /// </summary>
            Default,
            Sprite,
            UIBar,
            UI,
            UnitObject,
            UnitCard,
            DamageObject,
            Effect,
            Font,
            Material,
            UIMaterial,
        }

        /// <summary>
        /// 비어있는 데이터일 경우 대신나오는 스프라이트
        /// </summary>
        private static Sprite NullSprite => Resources.Load<Sprite>("NullSprite");

        /// <summary>
        /// 카테고리에 따른 경로를 반환하는 함수
        /// </summary>
        /// <param name="cATPathType">카테고리타입</param>
        /// <returns>카테고리별 경로</returns>
        public static string GetResourcesPath(CATPathType cATPathType)
        {
            string path;
            switch (cATPathType)
            {
                case CATPathType.Default:
                    //여기는 특정폴더를 참조안함
                    path = "";
                    break;
                case CATPathType.Sprite:
                    //자체폴더
                    path = $"{cATPathType}Folder";
                    break;
                default:
                    //그외거는 프리팹쪽을 참조
                    path = $"Prefab/{cATPathType}Folder";
                    break;
            }
            return path;
        }

        /// <summary>
        /// 리소시스 폴더에서 타입에 따른 오브젝트를 가져오는 함수
        /// </summary>
        /// <typeparam name="T">Object를 상속한 타입</typeparam>
        /// <param name="fileName">파일이름</param>
        /// <param name="pathType">경로타입</param>
        /// <returns>리소시스폴더에서 찾은 오브젝트</returns>
        public static T GetResourcesForObject<T>(DataBaseEditorInfo dataBaseEditorInfo, string fileName, CATPathType pathType) where T : Object
        {
            //비어있는지 체크
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            //데이터베이스가 있는지
            if (ReferenceEquals(null, dataBaseEditorInfo))
            {
                return null;
            }
            
            //바이블사용
            T targetObject = null;            
            ResourcesBible bible = null;
            switch (pathType)
            {
                case CATPathType.Sprite:
                    bible = dataBaseEditorInfo.resourcesSpriteBible;
                    bible.TryGetValue(fileName, out var sprite);
                    targetObject = sprite as T;
                    break;
                case CATPathType.Default:
                case CATPathType.UIBar:
                case CATPathType.UI:
                case CATPathType.UnitObject:
                case CATPathType.UnitCard:
                case CATPathType.DamageObject:
                case CATPathType.Effect:
                case CATPathType.Font:
                case CATPathType.Material:
                case CATPathType.UIMaterial:
                    bible = dataBaseEditorInfo.resourcesObjectBible;
                    bible.TryGetValue(fileName, out var target);
                    //타입처리//게임오브젝트면 체크
                    GameObject go = target as GameObject;
                    if (go == null)
                    {
                        targetObject = target as T;
                    }
                    else
                    {
                        go.TryGetComponent<T>(out targetObject);
                    }
                    break;
            }
            return targetObject;
        }


        /// <summary>
        /// 리소시스폴더에서 컴포넌트로 프리팹을 찾는 함수.
        /// </summary>
        /// <typeparam name="T">컴포넌트타입</typeparam>
        /// <param name="fileName">파일이름</param>
        /// <param name="pathType">경로타입</param>
        /// <returns>컴포넌트 프리팹</returns>
        public static T GetResourcesForComponent<T>(DataBaseEditorInfo dataBaseEditorInfo, string fileName, CATPathType pathType) where T : Component
        {
            var target = GetResourcesForObject<T>(dataBaseEditorInfo, fileName, pathType);
            return target;
        }

        /// <summary>
        /// 리소시스폴더에서 컴포넌트로 프리팹을 찾는 함수.
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="item">CSV아이템</param>
        /// <param name="cellID">Cell아이디</param>
        /// <param name="pathType">경로타입</param>
        /// <returns>컴포넌트 프리팹</returns>
        public static T GetResourcesForComponent<T>(this Dictionary<string, object> item, DataBaseEditorInfo dataBaseEditorInfo, string cellID, CATPathType pathType) where T : Component
        {
            cellID = item.GetConvertString(cellID);//파일이름으로 변경
            var target = GetResourcesForObject<T>(dataBaseEditorInfo, cellID, pathType);
            return target;
        }

        /// <summary>
        /// 리소시스 폴더에서 타입에 따른 오브젝트를 가져오는 함수
        /// </summary>
        /// <typeparam name="T">오브젝트</typeparam>
        /// <param name="item">CSV아이템</param>
        /// <param name="cellID">Cell아이디</param>
        /// <param name="pathType">경로타입</param>
        /// <returns>Object를 상속받은 Object</returns>
        public static T GetResourcesForObject<T>(this Dictionary<string, object> item, DataBaseEditorInfo dataBaseEditorInfo, string cellID, CATPathType pathType) where T : Object
        {
            cellID = item.GetConvertString(cellID);//파일이름으로 변경
            return GetResourcesForObject<T>(dataBaseEditorInfo, cellID, pathType);
        }

        /// <summary>
        /// 리소시스폴더에서 스프라이트를 가져오는 함수. 비어있으면 NullSprite로 반환한다.
        /// </summary>
        /// <param name="fileName">파일이름</param>
        /// <returns>스프라이트파일</returns>
        public static Sprite GetResourcesForSprite(DataBaseEditorInfo dataBaseEditorInfo, string fileName)
        {
            var target = GetResourcesForObject<Sprite>(dataBaseEditorInfo, fileName, CATPathType.Sprite);

            if (ReferenceEquals(target, null))
            {
                return NullSprite;
            }
            return target;
        }

        /// <summary>
        /// 리소시스폴더에서 스프라이트를 가져오는 함수.
        /// </summary>
        /// <param name="item">CSV아이템</param>
        /// <param name="cellID">Cell아이디</param>
        /// <returns>스프라이트파일</returns>
        public static Sprite GetResourcesForSprite(this Dictionary<string, object> item, DataBaseEditorInfo dataBaseEditorInfo, string cellID)
        {
            cellID = item.GetConvertString(cellID);//파일이름으로 변경
            var target = GetResourcesForSprite(dataBaseEditorInfo, cellID);
            return target;
        }


        //Try 함수는 다 키값 확인하게 처리하자

        public static bool GetConvertBool(this Dictionary<string, object> item, string cellID)
        {
            bool.TryParse(item[cellID].ToString(), out bool result);
            return result;
        }

        public static bool GetTryConvertBool(this Dictionary<string, object> item, string cellID, out bool result)
        {
            result = default(bool);
            bool check = item.CheckExistKey(cellID);
            if (check)
            {
                check = bool.TryParse(item[cellID].ToString(), out result);
            }
            return check;
        }

        public static int GetConvertInt(this Dictionary<string, object> item, string cellID)
        {
            int result = -1;
            int.TryParse(item[cellID].ToString(), out result);
            return result;
        }

        public static bool GetTryConvertInt(this Dictionary<string, object> item, string cellID, out int result)
        {
            result = default(int);
            bool check = item.CheckExistKey(cellID);
            if (check)
            {
                check = int.TryParse(item[cellID].ToString(), out result);
            }
            return check;
        }

        public static float GetConvertFloat(this Dictionary<string, object> item, string cellID)
        {
            float result = -1;
            float.TryParse(item[cellID].ToString(), out result);
            return result;
        }

        public static bool GetTryConvertFloat(this Dictionary<string, object> item, string cellID, out float result)
        {
            result = default(float);
            bool check = item.CheckExistKey(cellID);
            if (check)
            {
                check = float.TryParse(item[cellID].ToString(), out result);
            }
            return check;
        }

        public static string GetConvertString(this Dictionary<string, object> item, string cellID)
        {
            //return (string)item[cellID];//0같은 숫자일시 문제발생
            return item[cellID].ToString();
        }
        
        public static bool GetTryConvertString(this Dictionary<string, object> item, string cellID, out string result)
        {
            bool check = item.CheckExistKey(cellID);
            result = "NullKey";
            if (check)
            {
                result = item[cellID].ToString();
            }
            return check;
        }

        public static T GetConvertEnum<T>(this Dictionary<string, object> item, string cellID) where T : struct
        {
            //(EAchievementActionTagType)Enum.Parse(typeof(EAchievementActionTagType), (string)item["achievementActionType"]);
            T temp = (T)System.Enum.Parse(typeof(T), (string)item[cellID]);
            return temp;
        }

        public static bool GetTryConvertEnum<T>(this Dictionary<string, object> item, string cellID, out T result) where T : struct
        {
            result = default(T);
            bool check = item.CheckExistKey(cellID);
            if (check)
            {
                check = System.Enum.TryParse(typeof(T), cellID, out object value);
                if (check)
                {
                    result = (T)value;
                }
            }
            return check;
        }

        public static bool CheckExistKey(this Dictionary<string, object> item, string cellID)
        {   
            return item.ContainsKey(cellID);
        }

        //public static T GetConvertEnum1<T>(this Dictionary<string, object> item, string cellID) where T : System.Enum
        //{
        //    T temp = default(T);
        //    if (System.Enum.TryParse(typeof(T), (string)item[cellID], out object result))
        //    {
        //        temp = (T)result;
        //    }

        //    return temp;
        //}

        //구글스프레드 키
        //DOCID와 동일//시트선택할때마다 안바뀌는부분 https://docs.google.com/spreadsheets/d/{DOCID}/
        //static string key = "YOUR_GOOGLE_SHEETS_DOC_ID_HERE";

        //적혀있던 숫자를 쓰는게 아님//시트이름 적어야됨
        //static string sheetName = "YOUR_SHEET_NAME_HERE";

        /// <summary>
        /// CSV파일을 가져오는 반복기함수 (해당링크가 Private이 아니어야됨)
        /// </summary>
        /// <param name="key">구글스프레드 키</param>
        /// <param name="sheetData">시트데이터</param>
        /// <param name="action">TextAsset을 가져오기 위한 액션</param>
        /// <returns></returns>
        public static IEnumerator GetCSVForURL(string key, SheetData sheetData, System.Action<TextAsset> action)
        {
            //CSV에서 정리 해야됨//CSV칸이 남아있는거 그대로 가져오는 느낌이
            EditorCoroutine editorCoroutine = new EditorCoroutine(GetCSVForURL(key, sheetData, (string text) =>
            {
                //완료
                TextAsset textAsset = new TextAsset(text);
                lLcroweUtilEditor.CreateDataObject(ref textAsset, sheetData.fileName, null, null, false);
                action.Invoke(textAsset);
            }));
            
            do
            {   
                yield return null;
            } while (!editorCoroutine.IsRun);

            yield break;
        }

        /// <summary>
        /// CSV파일을 가져오는 반복기함수 (해당링크가 Private이 아니어야됨)
        /// </summary>
        /// <param name="key">구글스프레드 키</param>
        /// <param name="sheetData">시트데이터</param>
        /// <param name="action">String을 가져오기 위한 액션</param>
        /// <returns></returns>
        public static IEnumerator GetCSVForURL(string key, SheetData sheetData, System.Action<string> action)
        {
            string URL = $"https://docs.google.com/spreadsheets/d/{key}/gviz/tq?tqx=out:csv&sheet={sheetData.sheetName}";
            UnityWebRequest www = UnityWebRequest.Get(URL);
            var op = www.SendWebRequest();
            do
            {
                yield return op;
            } while (!op.isDone);

            //결과받기
            string text = www.downloadHandler.text;
            action?.Invoke(text);
            yield break;
        }


        /// <summary>
        /// 여러 코루틴을 돌리고 전체완료시 원하는 액션을 작동시키는 함수
        /// </summary>
        /// <param name="enumeratorArray">이너머레이터</param>
        /// <param name="action">액션</param>
        /// <returns></returns>
        public static IEnumerator ActionEditorCoroutineArray(IEnumerator[] enumeratorArray, System.Action action)
        {   
            EditorCoroutine[] editorCoroutineArray = new EditorCoroutine[enumeratorArray.Length];
            for (int i = 0; i < enumeratorArray.Length; i++)
            {
                var enumerator = enumeratorArray[i];
                editorCoroutineArray[i] = new EditorCoroutine(enumerator);
            }
            
            //다돌렸는지 체크
            int count = 0;            
            do
            {
                count = 0;
                for (int i = 0; i < editorCoroutineArray.Length; i++)
                {
                    if (editorCoroutineArray[i].IsRun)
                    {
                        continue;
                    }
                    count++;
                }

                if (count == editorCoroutineArray.Length)
                {
                    break;
                }
               
                yield return null;
            } while (true);
            action?.Invoke();
        }
    }

    public class SheetData
    {
        public string sheetName;
        public string fileName;

        /// <summary>
        /// 시트데이터,
        /// </summary>
        /// <param name="sheetName">시트이름</param>
        /// <param name="fileName">생성한 파일이름(string으로 가져올시 필요없음)</param>        
        public SheetData(string sheetName, string fileName)
        {
            this.sheetName = sheetName;
            this.fileName = fileName;
        }
    }
}
