using UnityEngine;


namespace lLCroweTool
{
    public class DeStructionPartObject : MonoBehaviour
    {
        //파괴가능한 부위 오브젝트
        //PartDestructionSystem에서 이용함
        //작동시키는 방식 선택하기
        //1. 오브젝트가 꺼지면 작동
        //2. 이벤트로 작동
        //3. 해당 컴포넌트가 꺼지면 작동// <== 이방식 채택
        //같은 게임오브젝트에 집어넣어도 작동되게 설계했음

        //다른오브젝트가 현재오브젝트 이벤트를 작동시킬려면
        //해당 이벤트함수에다 

        //오브젝트 파트의 카테고리 정의
        public PartCAT objectPartCAT;

        //캐싱용
        private PartDestructionSystem targetSystem;


        //비활성화시 작동
        //게임오브젝트 비활성화시 작동인가
        //아니면 컴포넌트 비활성화시 작동인가
        //테스트해볼것=>>(테스트결과쓰기) : 게임오브젝트를 비활성화 시켜도 ondisable이
        //작동되며 컴포넌트를 비활성화 시켜도 ondisable이 작동됨

        private void OnDisable()
        {
            //부위파괴오브젝트를 업데이트시켜줌
            targetSystem.UpdatePartDestruction();
        }

        //세팅용
        public void SetTargetSystem(PartDestructionSystem _targetSystem)
        {
            targetSystem = _targetSystem;
        }
    }

}

