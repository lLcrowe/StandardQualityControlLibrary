#if Doozy
using UnityEngine;
using DG.Tweening;
using lLCroweTool.BuildingSystem.SignalSystem;

namespace lLCroweTool.BuildingSystem.FuncStructure
{
    public class PipeValveStructureObject : PipeBridgeStructureObject, IActionInterectObject
    {
        //밸브가 있는 파이프건축물 오브젝트
        public bool isOpenValve = true;//밸브가 열려있는지//상호작용이 되는지
        public SpriteRenderer pipeValveSr;//밸브 랜더러
        public SignalReceiver signalReceiver;//수신기

        protected override void Awake()
        {
            base.Awake();
            signalReceiver.SetSignalType(ESignalType.Switch);
            signalReceiver.SetSignalActionEvent(OnOffPipeValve);
        }

        public void OnOffPipeValve(int index = 0)
        {
            isOpenValve = !isOpenValve;

            //밸브랜더러를 회전시켜줌
            if (isOpenValve)
            {
                //시계방향이 잠금인가?
                //확인후 수정작업
                timerModule.enabled = true;
                pipeValveSr.transform.DOLocalRotate(new Vector3(0, 0, 180), 1);
            }
            else
            {
                timerModule.enabled = false;
                pipeValveSr.transform.DOLocalRotate(new Vector3(0, 0, 0), 1);
            }
        }

        public bool CheckInterectObject(GameObject _targetObject)
        {
            return true;
        }

        public void InterectObjectAction(GameObject _targetObject)
        {
            OnOffPipeValve();
        }

        public string GetInterectText()
        {
            if (isOpenValve)
            {
                return "닫기";
            }
            return "열기";
        }
    }
}
#endif