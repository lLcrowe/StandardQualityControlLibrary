//using UnityEngine;
//using System.Collections.Generic;
//#if UNITY_EDITOR

//using UnityEngine.UI;
//#endif

//namespace lLCroweTool
//{
//    public class MissionCompassManager : MonoBehaviour
//    {
//        //미션나침반매니저
//        //미션나침반을 오브젝트폴로 관리하며 미션오브젝트
//        //현매니저는 캔버스오브젝트안에 있어야함
//        //몇몇개체는 옮겨야함
//        private static MissionCompassManager instance;
//        public static MissionCompassManager Instance
//        {
//            get
//            {
//                if (ReferenceEquals(instance, null))
//                {
//                    instance = FindObjectOfType<MissionCompassManager>();
//                    if (ReferenceEquals(instance, null))
//                    {
//                        GameObject gameObject = new GameObject();
//                        gameObject.name = "-MissionCompassManager-";
//                        instance = gameObject.AddComponent<MissionCompassManager>();
//                    }
//                }
//                return instance;
//            }
//        }

//        [Header("미션컴퍼스 오브젝트폴 관련")]
//        public MissionCompass missionCompassPrefab;
//        private List<MissionCompass> missionCompassList = new List<MissionCompass>();
//        [SerializeField]private int missionCompassCount = 0;
//        private MissionCompass targetMissionCompass;
//        private bool isFind = false;
//        [Space]
//        [Header("캔버스관련")]
//        public Canvas targetCanvas;
//        public Camera targetCamera;
//        public float borderSize = 100f;

//        [Space]
//        [Header("미션컴퍼스 이미지 관련")]
//        public Sprite normalMissionArrowSprite;
//        public Sprite normalMissionCrossSprite;

//        //미션타입 메인 서브
//        //미션종류 타겟제거 보호 이동 확보


//        private void Awake()
//        {
//            instance = this;
//            //랜더모드 체크
//            if (targetCanvas.renderMode != RenderMode.ScreenSpaceCamera)
//            {
//                targetCanvas.renderMode = RenderMode.ScreenSpaceCamera;
//                targetCanvas.worldCamera = targetCamera;
//            }
//        }

//        //미션초기화
//        //미션을 재집행해주는 함수
//        public MissionCompass InitializeMission(Transform _targetMissionObject)
//        {
//            targetMissionCompass = RespawnMissionCompass();
//            targetMissionCompass.SetMissionCompass(_targetMissionObject, normalMissionArrowSprite, normalMissionCrossSprite);
//            return targetMissionCompass;
//        }


//        private MissionCompass RespawnMissionCompass()
//        {
//            isFind = false;
//            targetMissionCompass = null;
//            for (int i = 0; i < missionCompassList.Count; i++)
//            {
//                if (!missionCompassList[i].gameObject.activeSelf)
//                {
//                    isFind = true;
//                    targetMissionCompass = missionCompassList[i];
//                    break;
//                }
//            }
//            if (!isFind)
//            {
//                targetMissionCompass = Instantiate(missionCompassPrefab, transform.position, Quaternion.identity, targetCanvas.transform);
//                missionCompassList.Add(targetMissionCompass);
//                targetMissionCompass.gameObject.SetActive(false);
//                targetMissionCompass.gameObject.name = "-MissionCompass-";
//                missionCompassCount++;
//            }
//            //소리작동
//            //if (isUseSound)
//            //{
//            //    int index = Random.Range(0, audioClips.Length);
//            //    SoundModuleManager.Instance.SoundPlay(true, transform, audioClips[index]);
//            //}
//            return targetMissionCompass;
//        }

//        #region 전처리구역
//#if UNITY_EDITOR
//        //[ButtonMethod]
//        public void MakeMissionCompass()
//        {
//            GameObject gameObject = new GameObject();
//            gameObject.AddComponent<MissionCompass>();
//            gameObject.AddComponent<RectTransform>();
//            gameObject.AddComponent<Image>();
//            gameObject.name = "-MissionCompass-";
//            gameObject.transform.parent = transform;
//            gameObject.transform.localScale = Vector3.one;
//        }
//#endif
//        #endregion

//    }
//}
