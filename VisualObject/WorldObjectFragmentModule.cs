#if MEC
using UnityEngine;
using MEC;
using System.Collections.Generic;
using lLCroweTool.Sound;

namespace lLCroweTool.Visual.Fragment
{
    public class WorldObjectFragmentModule : MonoBehaviour
    {
        //월드오브젝트가 파괴될시
        //파괴된 오브젝트를 보여주거나 생성
        //폭파에셋사용, 트레일 에셋사용(이팩트용)
        //랜덤포지션//폭발로 파괴되는가//산산조각
        //고정포지션//시체가 남는것인가 // 시체 애니메이션 후 정비   
        //파편 or 부분파괴
        //어덯게해야될지 고민이됨. 메모리를 사용률을 높힐지, 계산을 최적화해줄지
        //일단은 Destory로 해결

        //20221022
        //비쥬얼 파편들을 체크후 변경할예정
        //비쥬얼매니저로 관할 변경
        //이름변경
        //목적은 월드오브젝트가 파괴될떄 파편 생성 모듈
        //파편과 파괴됫을시 이팩트만 보여줌

        public DestroyFragmentShowType destroyFragmentShowType;//파괴파편보여줄타입
        public DestroyFragmentPosRandomType fragmentPosRandomType;//랜덤일시 작동되게

        [Header("파편종류 설정")]        
        public VisualFragmentObject[] fragmentPrefabArray;//제작될 파괴파편오브젝트들

        [Header("파편들이 나올 위치")]
        //랜덤포지션을 미리 정해두고 작업하는게 나아보임
        public Vector2[] fragmentPosArray = new Vector2[0];//로컬포지션//파편들이 나올 위치들
        public float randomDistance;//파편랜덤 거리

        [Header("파편오브젝트에 사용할 설정들")]
        public int spawnAmount = 20;//파편수량
        [Min(0)]
        public float spawnDelayTime = 0;//다음게 나올시간

        public bool isFadeObject;//천천히 사라지는 오브젝트인가 여부//사라진다면 파편오브젝트 비활성화시킴//안사라진다면 그대로 둠
        [Min(2)]        
        public float fadeTime;//사라지는 시간//최소2초 // 랜덤으로 1초에서2초간격으로 사라지게
        public bool isDisableObject;//비활성화여부
        [Min(0)]
        public float disableTime;//비활성화시간

        [Header("파편들이 출현할때 나오는 소리")]
       /* [SoundGroup] */public AudioClip actionSoundName;//작동할 소리
        public EffectObject effectObjectPrefab;//이팩트매니저에서 사용할 효과//이팩트매니저에서 효과작동

        //폭발에셋2D 사용
        [Header("터질때 나오는 소리")]
        /*[SoundGroup] */public AudioClip explosionSoundName;//작동할 소리
        public UniversalExplosion explosion;//컴포넌트로 같이두면 될듯함

        private Transform tr;

        private void Awake()
        {
            tr = transform;
            explosion = GetComponent<UniversalExplosion>();
        }

        /// <summary>
        /// 월드오브젝트 파괴&파편 활성화
        /// </summary>
        public void ActiveDestoryEffect()
        {
            if (fragmentPosArray.Length == 0)
            {
                Debug.Log("파괴파편오브젝트들의 위치를 추가하지않았습니다..");
                return;
            }

            //자기위치에 이팩트를 띄우고
            //사운드
            //파편이팩트작동
            Vector2 originPos = tr.position;
            var prefab = ObjectPoolManager.Instance.RequestDynamicComponentObject(effectObjectPrefab);
            prefab.InitTrObjPrefab(tr, tr);
            SoundManager.PlaySound3DAtTransform(actionSoundName, tr);
            Timing.RunCoroutine(ActiveDestoryEffect(this));
        }

        private Vector2 GetTargetFragmentPos(Vector2 _fragmentPos)
        {
            Vector2 temp = (tr.right * _fragmentPos.x + tr.up * _fragmentPos.y) + tr.position;
            return temp;
        }

        //[ButtonMethod]
        public void TestActionButton()
        {
            ActiveDestoryEffect();
        }

        private static IEnumerator<float> ActiveDestoryEffect(WorldObjectFragmentModule worldObjectDestroy)
        {
            //파편을 생성후
            //터뜨림
            for (int i = 0; i < worldObjectDestroy.spawnAmount; i++)
            {
                int randomIndex = Random.Range(0, worldObjectDestroy.fragmentPrefabArray.Length);
                VisualFragmentObject targetObject = ObjectPoolManager.Instance.RequestDynamicComponentObject(worldObjectDestroy.fragmentPrefabArray[randomIndex]) as VisualFragmentObject;

                //포지션 설정
                Transform tempTr = targetObject.GetTransform();
                tempTr.parent = null;
                switch (worldObjectDestroy.destroyFragmentShowType)
                {
                    case DestroyFragmentShowType.FixedPos:
                        tempTr.localPosition = worldObjectDestroy.GetTargetFragmentPos(worldObjectDestroy.fragmentPosArray[randomIndex]);
                        break;

                    case DestroyFragmentShowType.RandomPos:
                        tempTr.localPosition = worldObjectDestroy.GetTargetFragmentPos(GetRandomPos(worldObjectDestroy.fragmentPosArray[randomIndex], worldObjectDestroy.randomDistance, worldObjectDestroy.fragmentPosRandomType));
                        break;
                }

                //충돌체 설정
                randomIndex = Random.Range(0, 1);
                if (randomIndex % 2 == 0)
                {
                    targetObject.SetColliderTrigger(true);
                }
                else
                {
                    targetObject.SetColliderTrigger(false);
                }

                //설정에 따른 파편오브젝트 작동
                float tempFadeTime = Random.Range(2, worldObjectDestroy.fadeTime + 1);

                //계산공식//
                //최종 다 스폰했을시 시간 = fragmentDelayTime * fragmentSpawnAmount 0.1 * 10
                //사라지는 시간 = disapperTime 5
                //현재 작동된 스폰상태 = curSpawnAmount 
                //float tempDisapperTime = disapperTime + (fragmentDelayTime * fragmentSpawnAmount);
                //Debug.Log(tempDisapperTime);

                //파편활성화
                VisualFragmentObject.ActionPhysic(targetObject, worldObjectDestroy.isFadeObject, tempFadeTime, worldObjectDestroy.isDisableObject, worldObjectDestroy.disableTime);
                yield return Timing.WaitForSeconds(worldObjectDestroy.spawnDelayTime);
            }


            //사운드
            SoundManager.PlaySound3DAtTransform(worldObjectDestroy.explosionSoundName, worldObjectDestroy.tr);
            //다 소환 됫으면 터뜨림
            worldObjectDestroy.explosion.Activate();


        }
      
        /// <summary>
        /// 랜덤포지션주는 함수
        /// </summary>
        /// <param name="_pos">주어진 좌표</param>
        /// <param name="_size">랜덤으로 줄 사이즈</param>
        /// <returns></returns>
        public static Vector2 GetRandomPos(Vector2 _pos, float _size, DestroyFragmentPosRandomType _posRandomType)
        {
            Vector2 tempDirection = Vector2.zero;

            switch (_posRandomType)
            {
                case DestroyFragmentPosRandomType.Box:
                    //박스형
                    tempDirection.x = Random.Range(_pos.x - _size, _pos.x + _size);
                    tempDirection.y = Random.Range(_pos.y - _size, _pos.y + _size);
                    break;
                case DestroyFragmentPosRandomType.Circle:
                    //원형
                    tempDirection = Random.insideUnitSphere * _size;
                    tempDirection += _pos;
                    break;
            }

            return tempDirection;
        }

        private void OnDestroy()
        {  
            fragmentPosArray = null;
            explosion = null;
            tr = null;
        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < fragmentPosArray.Length; i++)
            {
                //현재 위치 + 트랜스폼 up right * 파편위치 
                Gizmos.DrawWireSphere(transform.position + (transform.right * fragmentPosArray[i].x + transform.up * fragmentPosArray[i].y), 0.5f);
            }
        }
    }

    /// <summary>
    /// 파괴파편을 보여줄때 타입   
    /// </summary>
    public enum DestroyFragmentShowType
    {
        RandomPos,//좌표와 좌표사이안의 사각형내에 랜덤으로 배치//좌표는 두개 사각형, 오브젝트순서 랜덤으로 나오게함
        FixedPos,//고정좌표와 고정오브젝트를 소환//좌표와 오브젝트순서가 일치되어야함
    }

    /// <summary>
    /// 파괴파편의 위치선정랜덤타입
    /// </summary>
    public enum DestroyFragmentPosRandomType
    {
        Circle,
        Box,
    }
}

#endif