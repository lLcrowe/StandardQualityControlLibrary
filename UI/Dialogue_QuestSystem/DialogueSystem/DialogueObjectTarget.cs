#if Doozy

using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.DialogueSystem
{
    public class DialogueObjectTarget : MonoBehaviour, IActionInterectObject
    {
        //대화시스템을 시용할 월드오브젝트//플레이어의 대화타겟
        //특정대화아이디키값을 가지고 잇으며
        //대화시스템매니저를 통해 검색후 해당 대화를 작동시킴

        [DialogueGroup] public string dialogueKey;
        public UnityEvent InterectEvent;//상호작용했을떄 나오는 이벤트

        private void Awake()
        {
            lLcroweUtil.GetAddUnitEvent(ref InterectEvent);
        }

        public bool CheckInterectObject(GameObject _targetObject)
        {
            if (dialogueKey == "-None-")
            {
                return false;
            }
            return !DialogueSystemManager.Instance.GetIsUseDialogueSystem();
        }

        public void InterectObjectAction(GameObject _targetObject)
        {
            InterectEvent?.Invoke();
            DialogueSystemManager.Instance.RequestDialogueData(dialogueKey);
        }

        public string GetInterectText()
        {
            return LocalizingManager.Instance.GetLocalLizeText("DialogueTarget");
        }
    }
}
#endif