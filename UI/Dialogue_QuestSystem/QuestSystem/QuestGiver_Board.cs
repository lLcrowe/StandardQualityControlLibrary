#if MEC
using lLCroweTool.TimerSystem;
using UnityEngine;

namespace lLCroweTool.QuestSystem
{
    /// <summary>
    /// 게임월드에서 퀘스트를 요청해주는 오브젝트_게시판버전
    /// </summary>
    [RequireComponent(typeof(PlayerUIDistinceCheckModule))]
    [RequireComponent(typeof(CoroutineTimerModule))]
    public class QuestGiver_Board : MonoBehaviour, IActionInterectObject
    {
        //상호작용하면 게시판을 열어서
        //현게시판에 등록된 퀘스트를 표시해줌
        //퀘스트를 등록하면 퀘스트시스템매니저에 등록
        private PlayerUIDistinceCheckModule distinceCheckModule;
        private CoroutineTimerModule timerModule;

        private void Awake()
        {
            distinceCheckModule = GetComponent<PlayerUIDistinceCheckModule>();
            distinceCheckModule.checkDistince = 5;
            timerModule = GetComponent<CoroutineTimerModule>();
            timerModule.SetTimer(0.02f);
            timerModule.AddUnityEvent(QuestGiverUpdate);
        }

        [Header("줄 퀘스트북아이디")]
        [QuestBookGroup] public string[] questBookID;//줄 퀘스트북 아이디
        public bool CheckInterectObject(GameObject _targetObject)
        {
            return true;
        }

        public string GetInterectText()
        {
            return LocalizingManager.Instance.GetLocalLizeText("QuestBoardOpen");
        }

        public void InterectObjectAction(GameObject _targetObject)
        {
            //거리세팅
            distinceCheckModule.SetDistinceUIChecker(_targetObject.transform, transform);
            QuestSystemManager.Instance.questDiaryViewUI.OpenQuestDiaryViewUI(true, this);
            timerModule.enabled = true;
        }

        private void QuestGiverUpdate()
        {
            //UI 꺼짐을 위한 기능
            if (distinceCheckModule.GetIsExistUnitObject())
            {
                //해당거리보다 멀어지면
                //상호작용이 가능한 거리보다 넓어야함
                if (distinceCheckModule.UpdateCheckDistince() || Input.GetKeyDown(KeyCode.Escape))
                {
                    QuestSystemManager.Instance.questDiaryViewUI.CloseQuestDiaryViewUI();
                    PlayerUIManager.Instance.RecordIOnlyOnePlayerUI(null);
                    timerModule.enabled = false;
                }
            }
        }

    }
}
#endif