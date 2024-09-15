#if Doozy
using Micosmo.SensorToolkit;
using UnityEngine;

namespace lLCroweTool.QuestSystem
{
    /// <summary>
    /// 게임월드에서 퀘스트를 요청해주는 오브젝트_감지버전
    /// </summary>
    public class QuestGiver_Detect : MonoBehaviour
    {
        //대화시스템을 안통하고 곧장 퀘스트를 발행해주는 역할을 함
        //트리거형태//액터를 안통함//이벤트용도
        
        
        [Header("한번만 작동되는지 여부")]
        public bool isOnce = false;
        [Header("줄 퀘스트북아이디")]
        [QuestBookGroup] public string[] questBookID;//줄 퀘스트북 아이디
        private Sensor sensor;


        private void Awake()
        {
            sensor = GetComponent<Sensor>();
            sensor.OnDetected.AddListener(DetectPlayerUnit);            
        }

        /// <summary>
        /// 플레이어유닛이 감지되었을시 작동되는 함수
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="sensor"></param>
        private void DetectPlayerUnit(GameObject gameObject, Sensor sensor)
        {
            //유닛오브젝트가 있으며 해당유닛이 플레이어팀이면 퀘스트를 발동            
            if (QuestSystemManager.Instance.CheckDetectWorldUnitObject(gameObject, sensor, UnitTeamType.Player, isOnce))
            {
                for (int i = 0; i < questBookID.Length; i++)
                {
                    QuestSystemManager.Instance.RequestQuestBook(questBookID[i]);
                }
            }
        }
    }
}
#endif