//using UnityEngine;
//using System.Collections.Generic;


//namespace lLCroweTool
//{

//    public class MissionInterectObject_UI : MonoBehaviour
//    {
//        private static MissionInterectObject_UI instance;
//        public static MissionInterectObject_UI Instance
//        {
//            get
//            {
//                if (ReferenceEquals(instance, null))
//                {
//                    instance = FindObjectOfType<MissionInterectObject_UI>();
//                    if (ReferenceEquals(instance, null))
//                    {
//                        GameObject gameObject = new GameObject();
//                        gameObject.name = "-MissionInterectObject_UI-";
//                        instance = gameObject.AddComponent<MissionInterectObject_UI>();
//                    }
//                }
//                return instance;
//            }
//        }
//        public Canvas targetCanvas;
//        public EnergyBar timerBarPrefab;
//        private List<EnergyBar> timerBarList = new List<EnergyBar>();
//        [SerializeField] private int timerBarCount = 0;
//        private EnergyBar targetTimerBar;
//        private bool isFind = false;

//        public EnergyBar RespawntimerBar()
//        {
//            isFind = false;
//            targetTimerBar = null;
//            for (int i = 0; i < timerBarList.Count; i++)
//            {
//                if (!timerBarList[i].gameObject.activeSelf)
//                {
//                    isFind = true;
//                    targetTimerBar = timerBarList[i];
//                    break;
//                }
//            }
//            if (!isFind)
//            {
//                targetTimerBar = Instantiate(timerBarPrefab, transform.position, Quaternion.identity, targetCanvas.transform);
//                timerBarList.Add(targetTimerBar);
//                targetTimerBar.gameObject.SetActive(false);
//                targetTimerBar.gameObject.name = "-timerBarPrefab-";
//                timerBarCount++;
//            }
//            //소리작동
//            //if (isUseSound)
//            //{
//            //    int index = Random.Range(0, audioClips.Length);
//            //    SoundModuleManager.Instance.SoundPlay(true, transform, audioClips[index]);
//            //}
//            return targetTimerBar;
//        }

//        //나중에 바 타입마다 오브젝트폴로 관리
//        //public enum BarType
//        //{

//        //}


//    }

//}
