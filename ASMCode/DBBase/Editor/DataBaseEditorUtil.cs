using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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


        //20230417//신규 포트폴리오 데이터보여주기관련
        //새롭게 제작된 라벨종류로 적용
        //20250505//DataBase쪽으로 위치이동


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
        /// 모든리소시스에서 특정타입에 대한 파일들을 찾아오는 함수
        /// </summary>
        /// <typeparam name="T">UnityEngine.Object를 상속받은 타입</typeparam>
        /// <returns></returns>
        public static ResourcesBible GetResourcesForAllSearch<T>() where T : Object
        {
            //AssetDataBase 폴더에 있는 것들중에 원하는거 가져오기

            //먼저 리소시스폴더들을 찾아서 경로들을 가져오고 해당경로들에서 내부경로를 다 찾아오기//재귀처리하기
            List<string> resourcesPathList = new List<string>();
            lLcroweUtilEditor.FindPathForResourcesDirectory("Assets", ref resourcesPathList);

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

        /// <summary>
        /// Resources폴더 내부의 모든 폴더에서 특정 오브젝트를 찾아서 등록하는 함수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="targetBible"></param>
        private static void FindResourcesInAllDirectoryFiles<T>(string path, ref ResourcesBible targetBible) where T : Object
        {
            //폴더를 먼저찾아서 최하단까지 들어간후
            var directories = lLcroweUtilEditor.GetFolders(path);
            foreach (var item in directories)
            {
                FindResourcesInAllDirectoryFiles<T>($"{path}/{item.Name}", ref targetBible);
            }

            //거기서부터 파일들 찾아옴//폴더가 더없으면 //파일들 찾아와서 가져오기
            var tempData = lLcroweUtilEditor.GetUnityObjectFiles<T>(path, null);
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
