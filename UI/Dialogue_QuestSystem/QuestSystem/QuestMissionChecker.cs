#if Doozy
using System;

namespace lLCroweTool.QuestSystem
{
    /// <summary>
    /// 리퀘스트 미션체크용
    /// </summary>
    [Serializable]
    public class QuestMissionChecker
    {
        private string missionID;
        private bool clear;
        private CountCheckerType countCheckerType;
        private int checkValue;//체크할 값
        private int count;//카운트된 값

        /// <summary>
        /// 미션체커 초기화
        /// </summary>
        /// <param name="_missionID">미션 아이디</param>
        /// <param name="_checkValue">얼만큼 체크할건지</param>        
        public void InitMissionCountChecker(string _missionID, CountCheckerType _countCheckerType, int _checkValue)
        {
            missionID = _missionID;
            clear = false;
            countCheckerType = _countCheckerType;
            checkValue = _checkValue;
            count = 0;
        }

        /// <summary>
        /// 카운트올림
        /// </summary>
        public void AddCount()
        {
            count++;
            clear = checkValue >= count ? true : false;
        }

        /// <summary>
        /// 밸류 체크
        /// </summary>
        /// <returns></returns>
        public bool CheckValue()
        {
            return clear;
        }

        public string GetMissionID()
        {
            return missionID;
        }

        public int GetCheckValue()
        {
            return checkValue;
        }

        public int GetCount()
        {
            return checkValue;
        }

        public CountCheckerType GetCountCheckerType()
        {
            return countCheckerType;
        }
    }
}
#endif