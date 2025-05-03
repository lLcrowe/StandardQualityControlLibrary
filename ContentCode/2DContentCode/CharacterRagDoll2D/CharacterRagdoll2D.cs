using UnityEngine;

namespace lLCroweTool.Ragdoll2DSystem
{
    public class CharacterRagdoll2D : MonoBehaviour
    {
        //2D 캐릭터 래그돌시스템
        //물리를 따로 처리하기
        //레그돌 시스템제작하기

        
        //한개의 스크립트로만 가능하게
        //최소 200개까지의 래그돌이 작동되게(100개정도면 충분해보이긴함.)

        //20210523 110개 까지 괜찮음

        //현재 자식으로 존재하는 친구들
        [HideInInspector] public AnchoredJoint2D[] anchoredJoint2Ds;
        [HideInInspector] public Collider2D[] collider2Ds;
        [HideInInspector] public Rigidbody2D[] rb2ds;

        //인간형태
        //Transform headtr;
        //Transform necktr;
        //Transform bodytr;
        //Transform paristr;
        //Transform legtr;
        //Transform armtr;
        //Transform handtr;
        //Transform foottr;

        //이리 저리 체크해보면
        //특정한객체에 여러개가 붙착되있는 경우가 있다


        private void Awake()
        {
            //게임시작시 래그돌될 오브젝트 부모들을 변경시킴
            //애니포트레이트에서 이용해도 될지 확인해봐야됨
            for (int i = 0; i < rb2ds.Length; i++)
            {
                rb2ds[i].transform.parent = transform;
            }            
        }

        /// <summary>
        /// 래그돌을 키는 함수
        /// </summary>
        public void RagDollOn()
        {
            for (int i = 0; i < rb2ds.Length; i++)
            {
                rb2ds[i].isKinematic = false;                
                collider2Ds[i].enabled = true;
                if (anchoredJoint2Ds.Length < i)
                {
                    anchoredJoint2Ds[i].enabled = true;
                }
            }
        }

        /// <summary>
        /// 래그돌을 끄는 함수
        /// </summary>
        public void RagDollOff()
        {
            for (int i = 0; i < rb2ds.Length; i++)
            {
                rb2ds[i].isKinematic = true;                
                collider2Ds[i].enabled = false;
                if (anchoredJoint2Ds.Length < i)
                {
                    anchoredJoint2Ds[i].enabled = false;
                }
            }
        }
    }
}