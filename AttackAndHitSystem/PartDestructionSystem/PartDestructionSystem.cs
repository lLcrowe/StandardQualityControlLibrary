using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool
{
    public class PartDestructionSystem : MonoBehaviour
    {
        //부위 파괴를 가지는 오브젝트에 붙착시켜서 작동시킴
        //메인오브젝트가 다파괴될시 작동되는 이벤트와 
        //서브오브젝트가 다파괴될시 작동되는 이벤트로 나누어져있고
        //해당 컴포넌트들을 비활성화시켜줘야됨//오브젝트를 비활성화시키는게 아니라


        //작동시킬 이벤트
        public UnityEvent mainPartDestructionEvent;
        public UnityEvent subPartDestructionEvent;

        //중요부위
        public List<DeStructionPartObject> mainPartDesctuctionObjectList = new List<DeStructionPartObject>();

        //서브부위
        public List<DeStructionPartObject> subPartDesctuctionObjectList = new List<DeStructionPartObject>();

        
        //파트 디스트럭션 업데이트
        public void UpdatePartDestruction()
        {
            //main 업데이트
            int tmp = 0;
            for (int i = 0; i < mainPartDesctuctionObjectList.Count; i++)
            {
                if (mainPartDesctuctionObjectList[i].enabled)
                {
                    break;
                }
                else
                {
                    tmp++;
                }
            }
            if (tmp >= mainPartDesctuctionObjectList.Count)
            {
                mainPartDestructionEvent.Invoke();
            }

            //서브 업데이트
            tmp = 0;
            for (int i = 0; i < subPartDesctuctionObjectList.Count; i++)
            {
                if (subPartDesctuctionObjectList[i].enabled)
                {
                    break;
                }
                else
                {
                    tmp++;
                }
            }
            if (tmp >= subPartDesctuctionObjectList.Count)
            {
                subPartDestructionEvent.Invoke();
            }
        }

        //자식들이 가지는 부위파괴컴포넌트를 가져와 세팅해준다.
        //버튼으로 해서 미리 설정하는게 좋아보임
        //box사용할것
        public void GetDestructionPart()
        {
            DeStructionPartObject[] targets = transform.GetComponentsInChildren<DeStructionPartObject>();
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].objectPartCAT == PartCAT.Main)
                {
                    mainPartDesctuctionObjectList.Add(targets[i]);
                }
                else if (targets[i].objectPartCAT == PartCAT.Sub)
                {
                    subPartDesctuctionObjectList.Add(targets[i]);
                }
                targets[i].SetTargetSystem(this);
            }
        }
    }
}

//파트 카테고리
public enum PartCAT
{
    Main,//메인
    Sub,//서브
}

//중요도 등급 //임의로 추가가능
public enum Partimportance
{
    A,//높은 중요도
    B,
    C,//낮은 중요도
}