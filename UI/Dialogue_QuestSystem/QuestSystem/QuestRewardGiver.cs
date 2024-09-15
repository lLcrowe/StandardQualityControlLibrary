#if Doozy
using UnityEngine;

namespace lLCroweTool.QuestSystem
{
    public class QuestRewardGiver : MonoBehaviour
    {
        //퀘스트가 끝났을시 보상을 줄떄 
        //타겟이 되는 오브젝트
        //액터로 제작해야함
        public ActorInfoObjectScript actorInfoData;

        private void OnEnable()
        {
            QuestSystemManager.Instance.AddSceneQuestRewardGiver(actorInfoData, this);
        }

        private void OnDisable()
        {
            QuestSystemManager.Instance.RemoveSceneQuestRewardGiver(actorInfoData);
        }
    }
}
#endif