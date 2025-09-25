using lLCroweTool.Dictionary;
using System.Collections.Generic;

namespace lLCroweTool.DataBase
{
    public static class DataBaseUtil
    {


        //============================================================
        //DataBaseManager등 CSV대용량데이터에서 사용하는 함수들
        //============================================================

        /// <summary>
        /// 바이블에 Info리스트에 있는것들을 등록하는 함수
        /// </summary>
        /// <typeparam name="T1">커스텀딕셔너리string,LabelBase</typeparam>
        /// <typeparam name="T2">LabelBase</typeparam>
        /// <param name="bible">커스텀딕셔너리string,LabelBase</param>
        /// <param name="infoList">LabelBase리스트</param>
        public static void AddBibleForInfoList<T1, T2>(this T1 bible, List<T2> infoList) where T1 : CustomDictionary<string, T2> where T2 : LabelBase
        {
            foreach (var item in infoList)
            {
                if (!bible.TryAdd(item.labelID, item))
                {
#if lLcroweLogSystem
                    lLCroweTool.LogSystem.LogManager.Log("DataImport", $"{typeof(T2)}타입의 {item.labelID}이름을 가진 데이터가 중복되어 등록이 안됫습니다.");
#endif
                }
            }
        }

        /// <summary>
        /// Info리스트에 있는것을 특정데이터모델에 등록후 바이블에 등록하는 함수
        /// </summary>
        /// <typeparam name="T1">커스텀딕셔너리string,LabelBase</typeparam>
        /// <typeparam name="T2">LabelBase</typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="bible">커스텀딕셔너리string,LabelBase</param>
        /// <param name="infoList">LabelBase리스트</param>
        /// <param name="func">데이터할당 기능이 있는 함수(New클래스, Info)</param>
        public static void AddBibleForInfoData<T1, T2, T3>(this T1 bible, List<T2> infoList, System.Func<T3, T2, T3> func) where T1 : CustomDictionary<string, T3> where T2 : LabelBase where T3 : class, new()
        {
            if (func == null)
            {
#if lLcroweLogSystem
                lLCroweTool.LogSystem.LogManager.Log("DataImport", $"{typeof(T3)}타입이 들어갈 이벤트함수가 비어있어서 작동을 안합니다.");
#endif
                return;
            }
            foreach (var item in infoList)
            {
                T3 customData = new T3();
                customData = func.Invoke(customData, item);
                if (bible.TryAdd(item.labelID, customData))
                {
#if lLcroweLogSystem
                    lLCroweTool.LogSystem.lLCroweTool.LogSystem.LogManager.Log("DataImport", $"{typeof(T2)}타입의 {item.labelID}이름을 가진 데이터가 중복되어 등록이 안됫습니다.");
#endif
                }
            }
        }

        //Func에 들어갈 함수예시
        //private CustomClass func(CustomClass customClass, LabelBase labelBase)
        //{
        //    customClass.xx1 = labelBase;
        //    customClass.xx2 = false;
        //    return customClass;
        //}


    }
}
