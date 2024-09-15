#if Doozy
using UnityEngine;
using Micosmo.SensorToolkit;
using lLCroweTool.WorldObjectSystem;
using lLCroweTool.InventorySystem;
using lLCroweTool.StatusModuleSystem;
using lLCroweTool.DialogueSystem;

namespace lLCroweTool.QuestSystem
{
    public class QuestMissionTargetObject : MonoBehaviour
    {
        //퀘스트북의 퀘스트를 타겟팅하는 오브젝트이다
        //현오브젝트에서 이벤트를 주면 해당 퀘스트가 갱신됨
        //특정퀘스트북의 퀘스트의 미션을 조건들을 올려줌
        //미션에 맞게 특정 오브젝트들에게 붙착시키기

        public string[] questBookID = new string[0];//퀘스트북아이디        
        public string[] questName = new string[0];//퀘스트이름        
        public QuestMissionType[] questMissionType = new QuestMissionType[0];//해당미션의타입으로 퀘스트들에서 체크후 밑의 미션아이디를 출력
        public string[] missionID = new string[0];//미션아이디

        private void Awake()
        {
            InitQuestMissionTargetObject(this);
        }

        private static void InitQuestMissionTargetObject(QuestMissionTargetObject questMissionTargetObject)
        {
            //오브젝트체크
            //최적화를 띵킹
            //이벤트를 등록//특정오브젝트들에 이벤트를 등록. 특정이벤트가 해당오브젝트에서 작동되면 자동으로 작동되게
            //작동확인후 현 컴포넌트 없애버리기
            DialogueObjectTarget dialogueTargetWorldObject = questMissionTargetObject.GetComponent<DialogueObjectTarget>();
            TestWorldObject worldObject = questMissionTargetObject.GetComponent<TestWorldObject>();
            WorldItem worldItem = questMissionTargetObject.GetComponent<WorldItem>();
            Sensor sensor = questMissionTargetObject.GetComponent<Sensor>();

            for (int i = 0; i < questMissionTargetObject.questMissionType.Length; i++)
            {
                switch (questMissionTargetObject.questMissionType[i])
                {
                    case QuestMissionType.Dialogue:
                        //대화
                        if (dialogueTargetWorldObject != null)
                        {   
                            dialogueTargetWorldObject.InterectEvent.AddListener(delegate {
                                QuestSystemManager.Instance.UpdateReQuestMission(questMissionTargetObject.questBookID[i], questMissionTargetObject.questName[i], questMissionTargetObject.missionID[i]);
                            });
                        }
                        break;
                    case QuestMissionType.Battle:
                    case QuestMissionType.Survive:
                        //전투&파괴//생존
                        //유닛오브젝트의 스탯
                        if (worldObject != null)
                        {
                            if (worldObject.isUseStat)
                            {
                                UnitStatusModule unitStatusModule = worldObject.unitStatus;
                                unitStatusModule.depletionEvent.AddListener(delegate { QuestSystemManager.Instance.UpdateReQuestMission(questMissionTargetObject.questBookID[i], questMissionTargetObject.questName[i], questMissionTargetObject.missionID[i]); });
                            }
                        }
                        break;
                    case QuestMissionType.AttractMove:
                        //현 오브젝트 위치로 유인 & 움직임//데이터보기
                        //유닛을 감지해야됨//퀘스트노드데이터와 데이터재처리
                        if (sensor != null)
                        {
                            //미션데이터에서 컨디션을 추출해서 작업하기
                            //미리빼오기
                            AttractMoveMissionData temp = QuestSystemManager.Instance.GetAttractMoveMissionData(questMissionTargetObject.questBookID[i], questMissionTargetObject.questName[i], questMissionTargetObject.missionID[i]);
                            sensor.OnDetected.AddListener(delegate
                            {
                                //GameObject gameObject = sensor.GetDetected()[sensor.GetDetected().Count - 1];//마지막에 감지된 게임오브젝트
                                GameObject gameObject = sensor.GetDetections()[sensor.GetDetections().Count - 1];//마지막에 감지된 게임오브젝트
                                if (gameObject.TryGetComponent(out TestWorldUnitObject unitObject))
                                {
                                    //팀체크
                                    if (unitObject.unitTeamType == temp.unitTeamType)
                                    {
                                        //타겟스탯 사용여부
                                        if (temp.isUseTargetUnitStat)
                                        {
                                            //유닛스탯 사용여부
                                            if (unitObject.isUseStat)
                                            {
                                                //데이터비교
                                                if (temp.unitStatusData == unitObject.unitStatus.unitStatusData)
                                                {
                                                    QuestSystemManager.Instance.UpdateReQuestMission(questMissionTargetObject.questBookID[i], questMissionTargetObject.questName[i], questMissionTargetObject.missionID[i]);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            QuestSystemManager.Instance.UpdateReQuestMission(questMissionTargetObject.questBookID[i], questMissionTargetObject.questName[i], questMissionTargetObject.missionID[i]);
                                        }
                                    }
                                }
                            });
                        }
                        break;
                    case QuestMissionType.ItemGather:
                        //아이템모으기
                        //월드아이템
                        if (worldItem != null)
                        {   
                            worldItem.interectEvent.AddListener(delegate
                            {
                                QuestSystemManager.Instance.UpdateReQuestMission(questMissionTargetObject.questBookID[i], questMissionTargetObject.questName[i], questMissionTargetObject.missionID[i]);
                            });
                        }
                        break;
                    case QuestMissionType.ArriveMap:
                        //맵도착
                        //어덯게 도착할건지 생각해보기
                        //위의 위치에 도달하면 하는거나
                        //맵이 로딩될떄?
                        //플레이어유닛이 현 타겟이 된 곳을 다았을시 현재 맵데이터를 체크
                        if (sensor != null)
                        {
                            sensor.OnDetected.AddListener(delegate
                            {
                                //GameObject gameObject = sensor.GetDetected()[sensor.GetDetected().Count - 1];//마지막에 감지된 게임오브젝트
                                GameObject gameObject = sensor.GetDetections()[sensor.GetDetections().Count - 1];//마지막에 감지된 게임오브젝트
                                if (gameObject.TryGetComponent(out TestWorldUnitObject unitObject))
                                {
                                    //팀체크
                                    if (unitObject.unitTeamType == UnitTeamType.Player)
                                    {
                                        QuestSystemManager.Instance.UpdateReQuestMission(questMissionTargetObject.questBookID[i], questMissionTargetObject.questName[i], questMissionTargetObject.missionID[i]);
                                    }
                                }
                            });
                        }
                        break;
                }
            }
        }
    }
}
#endif