#if Doozy
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using Doozy.Engine.UI;
using lLCroweTool.ToolTipSystem;

namespace lLCroweTool.QuestSystem
{
    public class RewardSelectButton : MonoBehaviour
    {   
        //아이템 보상
        public ToolTipUITargetImage itemImageObject;
        public TextMeshProUGUI itemCountTextObject;

        //유닛 보상
        public ToolTipUITargetImage unitImageObject;
        public TextMeshProUGUI unitCountTextObject;

        //경험치 보상
        public ToolTipUITargetImage unitExprienceImageObject;
        public TextMeshProUGUI unitExprienceValueTextObject;

        //스킬포인트보상
        public ToolTipUITargetImage skillPointImageObject;
        public TextMeshProUGUI skillPointValueTextObject;

        //스킬보상        
        public ToolTipUITargetImage skillImageObject;

        //오브젝트 보상
        public ToolTipUITargetImage targetObjectImageObject;        

        //정렬해주는 타겟
        public VerticalLayoutGroup verticalLayout;

        //버튼을 지정해주기//해당버튼을 크게해서 보상들을 클릭할수 있게 or 맨아래에 배치
        public UIButton selectButton;        

        public void SetQuestRewardButton(RewardData rewardData, bool useButton, string buttonText, UnityAction selectAction)
        {
            ResetRewardSelectButton();

            if (rewardData.itemData != null)
            {
                itemImageObject.gameObject.SetActive(true);
                itemImageObject.transform.SetParent(verticalLayout.transform);
                itemImageObject.SetToolTipUITarget(rewardData.itemData);                
                itemCountTextObject.gameObject.SetActive(true);
                itemCountTextObject.transform.SetParent(verticalLayout.transform);
                itemCountTextObject.text = rewardData.itemCount.ToString();
            }

            if (rewardData.targetUnitObject != null)
            {
                unitImageObject.gameObject.SetActive(true);
                unitImageObject.transform.SetParent(verticalLayout.transform);
                unitImageObject.SetToolTipUITarget(rewardData.targetUnitObject.unitStatus.unitStatusData);
                unitCountTextObject.gameObject.SetActive(true);
                unitCountTextObject.transform.SetParent(verticalLayout.transform);
                unitCountTextObject.text = rewardData.unitCount.ToString();
            }          

            if (rewardData.unitExprience != 0)
            {
                unitExprienceImageObject.gameObject.SetActive(true);
                unitExprienceImageObject.transform.SetParent(verticalLayout.transform);
                unitExprienceImageObject.SetToolTipUITarget(QuestSystemManager.Instance.unitExprienceDeScriptionData);
                unitExprienceValueTextObject.gameObject.SetActive(true);
                unitExprienceValueTextObject.transform.SetParent(verticalLayout.transform);
                unitExprienceValueTextObject.text = rewardData.unitExprience.ToString();
            }           

            if (rewardData.skillPoint != 0)
            {
                skillPointImageObject.gameObject.SetActive(true);
                skillPointImageObject.transform.SetParent(verticalLayout.transform);
                skillPointImageObject.SetToolTipUITarget(QuestSystemManager.Instance.skillPointDeScriptionData);
                skillPointValueTextObject.gameObject.SetActive(true);
                skillPointValueTextObject.transform.SetParent(verticalLayout.transform);
                skillPointValueTextObject.text = rewardData.unitExprience.ToString();
            }           

            if (rewardData.skillData != null)
            {
                skillImageObject.gameObject.SetActive(true);
                skillImageObject.transform.SetParent(verticalLayout.transform);
                skillImageObject.SetToolTipUITarget(QuestSystemManager.Instance.skillPointDeScriptionData);               
            }           

            if (rewardData.objectDeScriptionData != null)
            {
                targetObjectImageObject.gameObject.SetActive(true);
                targetObjectImageObject.transform.SetParent(verticalLayout.transform);
                targetObjectImageObject.SetToolTipUITarget(rewardData.objectDeScriptionData);
            }

            if (useButton)
            {
                selectButton.SetLabelText(buttonText);
                selectButton.gameObject.SetActive(true);
                //버튼액션관련
                selectButton.Button.onClick.AddListener(selectAction);
            }
            else
            {
                selectButton.gameObject.SetActive(false);
            }
            
        }

        private void ResetRewardSelectButton()
        {
            itemImageObject.gameObject.SetActive(false);
            itemImageObject.transform.SetParent(transform);
            itemCountTextObject.gameObject.SetActive(false);
            itemCountTextObject.transform.SetParent(transform);

            unitImageObject.gameObject.SetActive(false);
            unitImageObject.transform.SetParent(transform);
            unitCountTextObject.gameObject.SetActive(false);
            unitCountTextObject.transform.SetParent(transform);

            unitExprienceImageObject.gameObject.SetActive(false);
            unitExprienceImageObject.transform.SetParent(transform);
            unitExprienceValueTextObject.gameObject.SetActive(false);
            unitExprienceValueTextObject.transform.SetParent(transform);

            skillPointImageObject.gameObject.SetActive(false);
            skillPointImageObject.transform.SetParent(transform);
            skillPointValueTextObject.gameObject.SetActive(false);
            skillPointValueTextObject.transform.SetParent(transform);

            skillImageObject.gameObject.SetActive(false);
            skillImageObject.transform.SetParent(transform);

            targetObjectImageObject.gameObject.SetActive(false);
            targetObjectImageObject.transform.SetParent(transform);

            selectButton.Button.onClick.RemoveAllListeners();
        }
    }
}
#endif