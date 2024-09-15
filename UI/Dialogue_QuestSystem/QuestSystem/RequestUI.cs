#if Doozy
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using lLCroweTool.TimerSystem;

namespace lLCroweTool.QuestSystem
{
    public class RequestUI : UpdateTimerModule_Base
    {
        //리퀘스트의 진행상황을 보여주는 기능을 가짐
        //리퀘스트안의 미션체커의 진행상황을 보여줌
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI timerTextObject;
        public Request targetRequest;        
        public QuestMissionCheckerUI[] missionCheckerUIArray;

        protected override void Awake()
        {            
        }
        /// <summary>
        /// 리퀘스트UI 초기화함수
        /// </summary>
        /// <param name="request"> 타겟팅한 리퀘스트</param>
        /// <param name="missionHUDInfoNameArray">미션HUD이름</param>
        /// <param name="missionCheckerList">미션체커 리스트</param>
        public void InitRequestUI(Request request, string[] missionHUDInfoNameArray)
        {   
            targetRequest = request;
            titleText.text = request.targetQuestNodeData.questName;             
            if (targetRequest.targetQuestNodeData.isUseTimer)
            {
                timerTextObject.text = targetRequest.worldTime.GetWorldTime();
                timerTextObject.gameObject.SetActive(true);
            }
            else
            {
                timerTextObject.gameObject.SetActive(false);
            }
            
            List<QuestMissionCheckerUI> tempList = new List<QuestMissionCheckerUI>();
            for (int i = 0; i < missionHUDInfoNameArray.Length; i++)
            {
                QuestMissionCheckerUI missionCheckerUI = QuestSystemManager.Instance.RequestMissionCheckerUI();
                missionCheckerUI.gameObject.SetActive(true);
                tempList.Add(missionCheckerUI);
                //로컬라이징해서 가져옴
                string resultText = "";
                string[] tempArray = missionHUDInfoNameArray[i].Split('_');
                for (int j = 0; j < tempArray.Length; j++)
                {
                    string temp = LocalizingManager.Instance.GetLocalLizeText(tempArray[j]);
                    resultText += temp;
                }

                missionCheckerUI.SetMissionNameText(resultText);
                missionCheckerUI.SetMissionCheckText(request.GetMissionCheckerArray()[i].GetCheckValue());
                missionCheckerUIArray[i].SetMissionCountValue(request.GetMissionCheckerArray()[i].GetCount());                
            }
            missionCheckerUIArray = tempList.ToArray();
            gameObject.SetActive(true);
        }

        public override void UpdateTimerModuleFunc()
        {
            if (!QuestSystemManager.Instance.ExistActiveRequest(targetRequest))
            {
                //gameObject.SetActive(false);
                return;
            }

            if (targetRequest.targetQuestNodeData.isUseTimer)
            {
                timerTextObject.text = targetRequest.worldTime.GetWorldTime();
            }

            for (int i = 0; i < targetRequest.GetMissionCheckerArray().Length; i++)
            {
                if (targetRequest.GetMissionCheckerArray()[i].GetCount() != missionCheckerUIArray[i].GetCountValue())
                {
                    missionCheckerUIArray[i].SetMissionCountValue(targetRequest.GetMissionCheckerArray()[i].GetCount());
                }
            }
        }

        public void ResetRequestUI()
        {
            targetRequest = null;
            gameObject.SetActive(false);
            for (int i = 0; i < missionCheckerUIArray.Length; i++)
            {
                missionCheckerUIArray[i].gameObject.SetActive(false);
            }
            missionCheckerUIArray = null;
        }
    }
}
#endif