//using UnityEngine;
//using UnityEngine.UI;
//namespace lLCroweTool
//{

//    public class MissionCompass : MonoBehaviour
//    {
//        //현재 타겟이 될 미션방향을 알려주는 나침반
//        //캔버스내에 배치시켜야함
//        //캔버스를 세팅해줘야함
//        //Canvas screen overray camera -> rendercamera 타겟이 될 카메라
//        [Header("타겟이 될오브젝트")]
//        public Transform targetObject;//타겟 오브젝트
//                                      //public Transform hostObject;
//        [Header("미션캠퍼스 세팅")]
//        [SerializeField] private RectTransform screenRotatePointer;//스크린포인터가 될 오브젝트를 집어넣을것
//        [SerializeField] private RectTransform screenPositionPointer;//스크린포인터가 될 오브젝트를 집어넣을것
//        [SerializeField] private Image pointerImage;//UI 이미지를 집어넣어서 세팅해줄것
//        [SerializeField] private Text pointerText;

//        [Header("미션캠퍼스 스프라이트")]
//        public Sprite arrowSprite;
//        public Sprite crossSprite;
//        //캐싱부분
//        private float angle;
//        private bool temp;
//        private bool isOffScreen;
//        private float distince;

//        // Update is called once per frame
//        void Update()
//        {
//            //포인터 회전
//            Vector2 dir = (targetObject.position - MissionCompassManager.Instance.targetCamera.transform.position).normalized;
//            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
//            screenRotatePointer.localEulerAngles = new Vector3(0, 0, angle - 90);

//            //거리체크
//            distince = lLcroweUtil.GetDistance(MissionCompassManager.Instance.targetCamera.transform.position, targetObject.position);

//            Vector2 targetPositionScreenPoint = MissionCompassManager.Instance.targetCamera.WorldToScreenPoint(targetObject.position);
//            isOffScreen = targetPositionScreenPoint.x <= MissionCompassManager.Instance.borderSize || targetPositionScreenPoint.x >= Screen.width - MissionCompassManager.Instance.borderSize
//                || targetPositionScreenPoint.y <= MissionCompassManager.Instance.borderSize || targetPositionScreenPoint.y >= Screen.height - MissionCompassManager.Instance.borderSize;
//            if (isOffScreen)
//            {
//                //pointerImage.sprite = arrowSprite;
//                Vector2 cappedTargetScreenPosition = targetPositionScreenPoint;
//                if (cappedTargetScreenPosition.x <= MissionCompassManager.Instance.borderSize) cappedTargetScreenPosition.x = MissionCompassManager.Instance.borderSize;
//                if (cappedTargetScreenPosition.x >= Screen.width - MissionCompassManager.Instance.borderSize) cappedTargetScreenPosition.x = Screen.width - MissionCompassManager.Instance.borderSize;
//                if (cappedTargetScreenPosition.y <= MissionCompassManager.Instance.borderSize) cappedTargetScreenPosition.y = MissionCompassManager.Instance.borderSize;
//                if (cappedTargetScreenPosition.y >= Screen.height - MissionCompassManager.Instance.borderSize) cappedTargetScreenPosition.y = Screen.height - MissionCompassManager.Instance.borderSize;

//                ////포인터위치변경
//                Vector2 pointerWorldPosition = MissionCompassManager.Instance.targetCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
//                screenPositionPointer.position = pointerWorldPosition;
//                screenPositionPointer.localPosition = new Vector2(screenPositionPointer.localPosition.x, screenPositionPointer.localPosition.y);

//                //텍스트 최신화
//                pointerText.text = distince.ToString("N4");



//            }
//            else
//            {
//                ////포인터위치변경
//                //pointerImage.sprite = crossSprite;
//                Vector2 pointerWorldPosition = MissionCompassManager.Instance.targetCamera.ScreenToWorldPoint(targetPositionScreenPoint);
//                screenPositionPointer.position = pointerWorldPosition;
//                screenPositionPointer.localPosition = new Vector2(screenPositionPointer.localPosition.x, screenPositionPointer.localPosition.y);
//                screenRotatePointer.localEulerAngles = Vector2.zero;

//                //텍스트 최신화
//                pointerText.text = distince.ToString("N4");
//            }

//            //이미지관련
//            if (temp != isOffScreen)
//            {
//                if (isOffScreen)
//                {
//                    pointerImage.sprite = arrowSprite;
//                    //pointerText.gameObject.SetActive(true);
//                }
//                else
//                {
//                    pointerImage.sprite = crossSprite;
//                    //pointerText.gameObject.SetActive(false);
//                }
//                temp = isOffScreen;
//            }

           
//        }

//        public void Hide()
//        {
//            gameObject.SetActive(false);
//        }
//        public void SetMissionCompass(Transform _targetObject, Sprite _arrowSprite, Sprite _crossSprite)
//        {
//            gameObject.SetActive(true);
//            targetObject = _targetObject;
//            arrowSprite = _arrowSprite;
//            crossSprite = _crossSprite;
//            Vector2 targetPositionScreenPoint = MissionCompassManager.Instance.targetCamera.WorldToScreenPoint(targetObject.position);
//            isOffScreen = targetPositionScreenPoint.x <= MissionCompassManager.Instance.borderSize || targetPositionScreenPoint.x >= Screen.width - MissionCompassManager.Instance.borderSize
//                || targetPositionScreenPoint.y <= MissionCompassManager.Instance.borderSize || targetPositionScreenPoint.y >= Screen.height - MissionCompassManager.Instance.borderSize;
//            if (isOffScreen)
//            {
//                pointerImage.sprite = arrowSprite;
//                //pointerText.gameObject.SetActive(true);
//            }
//            else
//            {
//                pointerImage.sprite = crossSprite;
//                //pointerText.gameObject.SetActive(false);
//            }
//            pointerText.text = lLcroweUtil.GetDistance(MissionCompassManager.Instance.targetCamera.transform.position, targetObject.position).ToString();
//        }
//    }

//}
