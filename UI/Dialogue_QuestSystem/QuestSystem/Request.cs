#if Doozy
using lLCroweTool.Dictionary;
using lLCroweTool.GameWorldTimeSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.QuestSystem
{
    [Serializable]
    public class Request
    {
        //발행된 퀘스트//인게임에서 퀘스트를 발행할때 작동되는 부분

        //설정부분데이터
        //퀘스트노드의 데이터 세팅에 맞게 제작

        //퀘스트북
        public string targetQuestBookIDKey;//퀘스트북아이디
        public QuestNodeData targetQuestNodeData;//타겟팅된 퀘스트노드데이터  

        public bool isActive = false;//퀘스트 작동여부

        public int questActionCount;//퀘스트 작동횟수
        public int questSuccessedCount;//퀘스트 성공 횟수
        public int questFailedCount;//퀘스트 실패 횟수
        public int questGiveUpCount;//퀘스트 포기 횟수

        private float time;
        public int timeValue;
        public WorldTime worldTime;

        //미션방식 숫자를 지정해줌
        [System.Serializable] public class MissionCheckBible : CustomDictionary<string, bool> { }
        [System.Serializable] public class MissionCheckerBible : CustomDictionary<string, QuestMissionChecker> { }


        [SerializeField] private MissionCheckBible missionSuccessBible = new MissionCheckBible();//성공해야 퀘스트를 성공하는 미션
        [SerializeField] private MissionCheckBible missionFailBible = new MissionCheckBible();//실패해야 퀘스트를 성공하는 미션
        [SerializeField] private MissionCheckerBible missionBible = new MissionCheckerBible();//모든미션

        [SerializeField] private RequestUI targetRequestUI;
        [SerializeField] private QuestMissionChecker[] missionCheckerArray = new QuestMissionChecker[0];
        [SerializeField] private string[] missionHUDTextArray = new string[0];

        /// <summary>
        /// 리퀘스트 초기화함수
        /// </summary>
        /// <param name="questNodeData"></param>
        public void InitQuestData(string questBookID, QuestNodeData questNodeData)
        {
            missionSuccessBible.Clear();
            missionFailBible.Clear();
            missionBible.Clear();
            targetQuestBookIDKey = questBookID;
            targetQuestNodeData = questNodeData;
            timeValue = questNodeData.time;
            worldTime.SetWorldTime(timeValue);

            List<string> missionHUDTextList = new List<string>();//미션HUD에 보여줄 텍스트
            List<QuestMissionChecker> missionCheckerList = new List<QuestMissionChecker>();//미션체커

            int i;

            for (i = 0; i < questNodeData.dialogueMissionDataArray.Length; i++)
            {
                QuestMissionChecker missionChecker = new QuestMissionChecker();
                string missionID = questNodeData.dialogueMissionDataArray[i].actorInfoData.actorName + "_TalkActor";
                missionChecker.InitMissionCountChecker(missionID, CountCheckerType.Success, 1);
                missionSuccessBible.Add(missionID, missionChecker.CheckValue());
                missionBible.Add(missionID, missionChecker);
                missionHUDTextList.Add(missionID);
                missionCheckerList.Add(missionChecker);
            }

            for (i = 0; i < questNodeData.battleMissionDataArray.Length; i++)
            {
                QuestMissionChecker missionChecker = new QuestMissionChecker();
                string missionID = questNodeData.battleMissionDataArray[i].missionTargetID + "_DestroyTarget";
                missionChecker.InitMissionCountChecker(missionID, CountCheckerType.Success, questNodeData.battleMissionDataArray[i].value);
                missionSuccessBible.Add(missionID, missionChecker.CheckValue());
                missionBible.Add(missionID, missionChecker);
                missionHUDTextList.Add(missionID);
                missionCheckerList.Add(missionChecker);
            }

            for (i = 0; i < questNodeData.surviveMissionDataArray.Length; i++)
            {
                QuestMissionChecker missionChecker = new QuestMissionChecker();
                string missionID = questNodeData.surviveMissionDataArray[i].missionTargetID + "_SurviveTarget";
                missionChecker.InitMissionCountChecker(missionID, CountCheckerType.Failed, questNodeData.surviveMissionDataArray[i].value);
                missionFailBible.Add(missionID, missionChecker.CheckValue());
                missionBible.Add(missionID, missionChecker);
                missionHUDTextList.Add(missionID);
                missionCheckerList.Add(missionChecker);
            }

            for (i = 0; i < questNodeData.attractMoveMissionDataArray.Length; i++)
            {
                QuestMissionChecker missionChecker = new QuestMissionChecker();
                AttractMoveMissionData tempData = questNodeData.attractMoveMissionDataArray[i];
                //로직에 따른 보여줄 UI와 아이디처리
                //
                string missionID;
                if (tempData.isUseTargetUnitStat)
                {
                    if (tempData.unitStatusData == null)
                    {
                        missionID = tempData.unitTeamType.ToString() + "_unitStatDataIsNull";
                    }
                    else
                    {
                        missionID = tempData.unitTeamType.ToString() + "_" + tempData.unitStatusData.objectName;
                    }
                }
                else
                {
                    missionID = tempData.unitTeamType.ToString();
                }

                missionChecker.InitMissionCountChecker(missionID, CountCheckerType.Success, 1);
                missionSuccessBible.Add(missionID, missionChecker.CheckValue());
                missionBible.Add(missionID, missionChecker);

                string localize;
                switch (tempData.unitTeamType)
                {
                    case UnitTeamType.Player:
                        localize = "_MoveTarget";
                        break;
                    case UnitTeamType.Ally:
                    case UnitTeamType.Enemy:
                    case UnitTeamType.NPC:
                    case UnitTeamType.Unknown:
                    case UnitTeamType.Neutrality:
                        localize = "_AttractTarget";
                        break;
                    default:
                        localize = "_MoveAttractTarget";
                        break;
                }

                //_AttractTarget;
                //_MoveTarget;
                missionHUDTextList.Add(missionID + localize);
                missionCheckerList.Add(missionChecker);
            }

            for (i = 0; i < questNodeData.itemGatherMissionDataArray.Length; i++)
            {
                QuestMissionChecker missionChecker = new QuestMissionChecker();
                string missionID = questNodeData.itemGatherMissionDataArray[i].itemData.objectName + "_ItemGather";
                missionChecker.InitMissionCountChecker(missionID, CountCheckerType.Success, 1);
                missionSuccessBible.Add(missionID, missionChecker.CheckValue());
                missionBible.Add(missionID, missionChecker);
                missionHUDTextList.Add(missionID);
                missionCheckerList.Add(missionChecker);
            }

            for (i = 0; i < questNodeData.arriveMapMissionDataArray.Length; i++)
            {
                QuestMissionChecker missionChecker = new QuestMissionChecker();
                string missionID = questNodeData.arriveMapMissionDataArray[i].mapMarkerData.objectName + "_Arrive";
                missionChecker.InitMissionCountChecker(missionID, CountCheckerType.Success, 1);
                missionSuccessBible.Add(missionID, missionChecker.CheckValue());
                missionBible.Add(missionID, missionChecker);
                missionHUDTextList.Add(missionID);
                missionCheckerList.Add(missionChecker);
            }

            missionCheckerArray = missionCheckerList.ToArray();
            missionHUDTextArray = missionHUDTextList.ToArray();
        }

        /// <summary>
        /// 리퀘스트를 작동시키는 함수
        /// </summary>
        public void ActiveRequest()
        {
            isActive = true;
            RequestUI requestUI = QuestSystemManager.Instance.RequestRequestUI();
            targetRequestUI = requestUI;
            targetRequestUI.InitRequestUI(this, missionHUDTextArray);
            questActionCount++;
        }

        /// <summary>
        /// 리퀘스트내역을 갱신해주는 함수
        /// </summary>
        /// <param name="targetGUID">타겟GUID</param>
        /// <param name="isClear">클리어여부</param>
        public void UpdateQuestData(string mission)
        {
            if (missionBible.ContainsKey(mission))
            {
                missionBible[mission].AddCount();
            }
        }

        /// <summary>
        /// 타겟이 된 퀘스트북아이디 가져오기 함수
        /// </summary>
        /// <returns>퀘스트북아이디</returns>
        public string GetTargetQuestBookIDKey()
        {
            return targetQuestBookIDKey;
        }

        /// <summary>
        /// 리퀘스트 완료여부
        /// </summary>
        /// <returns>완료여부</returns>
        public bool CheckSuccessReQuest()
        {
            return !missionSuccessBible.ContainsValue(false);
        }

        /// <summary>
        /// 리퀘스트 실패여부
        /// </summary>
        /// <returns>실패여부</returns>
        public bool CheckFailedReQuest()
        {
            return missionSuccessBible.ContainsValue(true);
        }

        /// <summary>
        /// 특정이벤트로 퀘스트가 성공했는지 실패했는지 체크해주는 함수
        /// </summary>
        /// <param name="isSuccess"></param>
        public void CheckFinal(bool isSuccess, bool isGiveUp = false)
        {
            if (isSuccess)
            {
                questSuccessedCount++;
            }
            else
            {
                questFailedCount++;
                if (isGiveUp)
                {
                    questGiveUpCount++;
                }
            }

            //리퀘스트 초기화
            isActive = false;
            targetRequestUI.ResetRequestUI();
            targetRequestUI = null;
        }

        /// <summary>
        /// 퀘스트노드에 있는 미션들이 담긴 체커를 가져오는 함수
        /// </summary>
        /// <returns>미션체커들</returns>
        public QuestMissionChecker[] GetMissionCheckerArray()
        {
            return missionCheckerArray;
        }

        /// <summary>
        /// 설정된 미션HUDText 배열을 가져오는 함수
        /// </summary>
        /// <returns>미션HUDText 배열</returns>
        public string[] GetMissionHUDTextArray()
        {
            return missionHUDTextArray;
        }

        /// <summary>
        /// 리퀘스트의 작동여부를 가져오는 함수
        /// </summary>
        /// <returns>작동여부</returns>
        public bool GetIsActive()
        {
            return isActive;
        }


        public void SetTime(float value)
        {
            time = value;
        }

        public float GetTime()
        {
            return time;
        }
    }
}

#endif