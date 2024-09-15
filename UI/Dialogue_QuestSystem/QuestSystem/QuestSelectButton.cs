#if Doozy
using Doozy.Engine.UI;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using EnhancedUI.EnhancedScroller;

namespace lLCroweTool.QuestSystem
{
    public class QuestSelectButton : EnhancedScrollerCellView
    {
        //퀘스트선택버튼
        //게임오브젝트에 Horizontal Layout Group, (기본설정)
        //Content Size Fillter PreferredSize PreferredSize 로 설정할것        

        public TextMeshProUGUI questTitleText;
        public Image questTypeimage;
        public UIButton selectButton;        

        //private void Awake()
        //{
        //    selectButton = GetComponent<UIButton>();
        //}

        public void SetQuestSelectButton(QuestBookData questBookData, UnityAction action)
        {
            questTypeimage.sprite = QuestSystemManager.Instance.GetQuestMissionTypeImage(questBookData.questType);
            questTitleText.text = questBookData.questBookTitle;
            selectButton.Button.onClick.AddListener(action);
        }

        public void ResetQuestSelectButton()
        {
            questTypeimage.sprite = null;
            questTitleText.text = "None";
            selectButton.Button.onClick.RemoveAllListeners();            
        }
    }
}
#endif