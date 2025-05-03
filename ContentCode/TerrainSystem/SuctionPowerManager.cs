#if Explore2D
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace lLCroweTool.TerrainSystem.SuctionPower
{
    //흡입력 매니저
    public class SuctionPowerManager : MonoBehaviour
    {
        //터레인 시스템중에서 산소시스템의 공기가 빠져나가는 효과를 만들어줘서 제공해주는 매니저
        //사용처 산소의 공기가 사라지는 이벤트일때 발동
        //월드레이어에서만 작동하기//태그는 상관없음
        //펄싱폭발에서 Enable 작동으로 작업해야함

        private static SuctionPowerManager instance;
        public static SuctionPowerManager Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<SuctionPowerManager>();
                    if (ReferenceEquals(instance, null))
                    {
                        GameObject tmp = new GameObject();
                        instance = tmp.AddComponent<SuctionPowerManager>();
                        tmp.name = "-=SuctionPowerManager=-";
                    }
                }
                return instance;
            }
        }
        
        private bool isFind = false;
        private PulsingExplosion targetObject;
        private List<PulsingExplosion> pulsingExplosionPrefabList = new List<PulsingExplosion>();

        public LayerMask targetInterectLayerMask;

        private void Awake()
        {
            instance = this;            
        }

        /// <summary>
        /// 펄싱폭발을 가져와서 세팅해주는 함수이다
        /// </summary>
        /// <param name="_pos"></param>
        public PulsingExplosion CallSuctionPower(Transform _pos, bool _useTriggerColliders = true, bool _useSolidColliders = true, float _frequency = 0.05f, int _duration = 0, bool _loopForever = false, float _explosionRadius = 3f, float _explosionForce = -25f, bool _modifyForceByDistance = true, ForceMode2D _forceMode = ForceMode2D.Force)
        {
            targetObject = RequestPulsingExplosionPrefab();
            targetObject.transform.position = _pos.position;
            targetObject.transform.parent = _pos;
            targetObject.gameObject.SetActive(true);
            //targetObject.onExplosion.AddListener(unityAction);

            //세팅관련
            targetObject.useTriggerColliders = _useTriggerColliders;
            targetObject.useSolidColliders = _useSolidColliders;

            targetObject.frequency = _frequency;
            targetObject.duration = _duration;
            targetObject.loopForever = _loopForever;

            targetObject.explosionRadius = _explosionRadius;

            targetObject.explosionForce = _explosionForce;
            targetObject.modifyForceByDistance = _modifyForceByDistance;
            targetObject.forceMode = _forceMode;
            return targetObject;
        }

        public PulsingExplosion CallSuctionPower(Transform _parent, Vector2 _pos, bool _useTriggerColliders = true, bool _useSolidColliders = true, float _frequency = 0.05f, int _duration = 0, bool _loopForever = false, float _explosionRadius = 3f, float _explosionForce = -25f, bool _modifyForceByDistance = true, ForceMode2D _forceMode = ForceMode2D.Force)
        {
            targetObject = RequestPulsingExplosionPrefab();
            targetObject.transform.position = _pos;
            targetObject.transform.parent = _parent;
            targetObject.gameObject.SetActive(true);
            //targetObject.onExplosion.AddListener(unityAction);

            //세팅관련
            targetObject.useTriggerColliders = _useTriggerColliders;
            targetObject.useSolidColliders = _useSolidColliders;

            targetObject.frequency = _frequency;
            targetObject.duration = _duration;
            targetObject.loopForever = _loopForever;

            targetObject.explosionRadius = _explosionRadius;

            targetObject.explosionForce = _explosionForce;
            targetObject.modifyForceByDistance = _modifyForceByDistance;
            targetObject.forceMode = _forceMode;
            return targetObject;
        }

        /// <summary>
        /// 펄싱폭발을 반납하는 함수이다.
        /// </summary>
        /// <param name="_pulsingExplosion">반납할 펄싱폭발</param>
        public void ReturnSuctionPower(PulsingExplosion _pulsingExplosion)
        {
            targetObject = _pulsingExplosion;
            if (!ReferenceEquals(targetObject, null))
            {
                targetObject.transform.position = transform.position;
                targetObject.transform.parent = transform;
                targetObject.gameObject.SetActive(false);
            }
        }
        
        private PulsingExplosion RequestPulsingExplosionPrefab()
        {
            //초기화
            isFind = false;
            targetObject = null;

            //로직작동
            for (int i = 0; i < pulsingExplosionPrefabList.Count; i++)
            {
                if (!pulsingExplosionPrefabList[i].gameObject.activeSelf)
                {
                    isFind = true;
                    targetObject = pulsingExplosionPrefabList[i];
                    //targetObject.onExplosion.RemoveAllListeners();
                    //리셋구간
                    break;
                }
            }
            //찾은게 없다면 오브젝트 하나를 만들어준다.
            if (!isFind)    
            {
                GameObject go = new GameObject();
                targetObject = go.AddComponent<PulsingExplosion>();
                pulsingExplosionPrefabList.Add(targetObject);
                targetObject.gameObject.SetActive(false);
                targetObject.name = "흡입오브젝트";
                targetObject.onEnable = true;
                targetObject.layerFilter = targetInterectLayerMask;
                targetObject.explosionOffset = Vector2.zero;

            }
            return targetObject;
        }
    }
}
#endif