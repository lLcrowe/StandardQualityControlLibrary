using System.Collections.Generic;
using UnityEngine;


namespace lLCroweTool.RaderSystem
{
    public class Rader2D : MonoBehaviour
    {
        //20240606
        //로직변경//새롭게 처리
        //

        //레이더핑
        public RaderPingInfo pingInfo;


        [Header("레이더가 최대로 탐지할수 있는 수")]
        public int maxRaderDetectCount = 20;
        private int hitLength;

          

        [Space]
        [Header("레이더 세팅관련")]
        public RaderType raderCATType;//레이더 작동 타입
        public LayerMask targetRaderLayer;//어떤레이어를 감지할것인지
        public Transform raderObject;
        //원형레이더//이미지는 동그란원형에 외곽라인이 보이는 이미지 사용
        //라인레이더//이미지는 선만 있는 이미지를 사용
        //회전시킬 오브젝트에 스프라이트컴포넌트를 집어넣고 해당 스프라이트를 Left로 설정하고 크기를 조절함.
        public float raderSpeed;//레이더 스피드//레이더스피드는 레이더최대 사거리를 절반이하로 설정하여함
        private float raderCurRange;//현재 레이더 사거리
        public float raderMaxRange;//레이더 최대 사거리
        public List<Collider2D> raderDetectList = new List<Collider2D>(20);//레이더에 감지된 콜라이더리스트

        //투명도 및 색깔관련
        
        private Color pulseColor;

        //캐싱
        private RaycastHit2D[] pulseHits;
        private Transform tr;
        private SpriteRenderer sr;

        private static RaderPingMark2D targetPingPrefab;

        //별개부분
        //원형레이더에서 사용함
        [Space]
        [Header("원형레이더 거리에 따른 Fade 효과")]
        public float rangeFade = 30;//일단은 그냥두고 고정 값찾아보기 고정값 찾은후는 없애고 고정시키기
       

        private void Awake()
        {
            tr = transform;
            sr = raderObject.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            UpdateRader2D();
        }

        public void UpdateRader2D()
        {
            //switch (raderCATType)
            //{                
            //    case RaderType.Circle:
            //        //크기조절레이더
            //        raderCurRange += raderSpeed;
            //        //raderCurRange += raderSpeed * Time.deltaTime;
            //        if (raderCurRange > raderMaxRange)
            //        {
            //            raderCurRange = 0;
            //            raderDetectList.Clear();
            //            //무한반복이 아닐시 현재 컴포넌트를 비활성화시킴
            //        }
            //        raderObject.localScale = Vector2.one * raderCurRange;

            //        //RaycastHit2D[] pulseHits = Physics2D.CircleCastAll(transform.position, raderCurRange / 2f, Vector2.zero, raderCurRange, targetRaderLayer);


            //        pulseHits = Physics2D.CircleCastAll(tr.position, raderCurRange * 0.5f, Vector2.zero, raderCurRange * 0.5f, targetRaderLayer);
            //        hitLength = pulseHits.Length;
            //        if (hitLength > maxRaderDetectCount)
            //        {
            //            hitLength = maxRaderDetectCount;
            //        }

            //        for (int i = 0; i < hitLength; i++)
            //        {
            //            if (pulseHits[i].collider != null)
            //            {
            //                if (pulseHits[i].collider.TryGetComponent(out RaderPingTarget targetWorldObject))
            //                {
            //                    if (!raderDetectList.Contains(pulseHits[i].collider))
            //                    {
            //                        raderDetectList.Add(pulseHits[i].collider);
            //                        //오브젝트폴로 변경
                                   
            //                    }
            //                }
            //            }
            //        }

            //        if (raderCurRange > raderMaxRange - rangeFade)
            //        {
            //            pulseColor.a = Mathf.Lerp(0f, 1f, (raderMaxRange - raderCurRange) / rangeFade);
            //        }
            //        else
            //        {
            //            pulseColor.a = 1f;
            //        }
            //        sr.color = pulseColor;
            //        break;
            //    case RaderType.Sweep:
            //        //회전레이더
            //        float preRotation = (raderObject.eulerAngles.z % 360) - 180;
            //        raderObject.eulerAngles -= Vector3.forward * raderSpeed * 40;
            //        //raderObject.eulerAngles -= Vector3.forward * raderSpeed * 40 * Time.deltaTime;
            //        float curRotation = (raderObject.eulerAngles.z % 360) - 180;

            //        if (preRotation < 0 && curRotation >= 0)
            //        {
            //            raderDetectList.Clear();
            //            //무한반복이 아닐시 현재 컴포넌트를 비활성화시킴
            //        }

            //        float angleRad = raderObject.eulerAngles.z * (Mathf.PI / 180f);
            //        Vector2 Direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            //        pulseHits = Physics2D.RaycastAll(tr.position, Direction, raderMaxRange, targetRaderLayer);
            //        hitLength = pulseHits.Length;
            //        if (hitLength > maxRaderDetectCount)
            //        {
            //            hitLength = maxRaderDetectCount;
            //        }

            //        //디버그
            //        //Debug.DrawRay(transform.position, Direction * raderMaxRange, Color.red);
            //        for (int i = 0; i < hitLength; i++)
            //        {
            //            if (pulseHits[i].collider != null)
            //            {
            //                if (pulseHits[i].collider.TryGetComponent(out TestWorldObject targetWorldObject))
            //                {
            //                    if (!raderDetectList.Contains(pulseHits[i].collider))
            //                    {
            //                        raderDetectList.Add(pulseHits[i].collider);
            //                        //오브젝트폴로 변경
                                
            //                        SetRaderPingColor(targetWorldObject, targetPingPrefab);
            //                    }
            //                }
            //            }
            //        }
            //        break;
            //}





            //var prefab = GetPingObject();
            //SetRaderPingMark2D(prefab,, pingInfo.raderPingData);
        }

        //레이더핑의 색깔과 스프라이트를 결정해주는곳
        private void SetRaderPingMark2D(RaderPingMark2D raderPingMark2D, RaderPingTarget raderPingTarget, RaderPingData raderPingData)
        {
            int targetTag = raderPingTarget.GetPingTag();
            PingPreset pingPreset = null;

            if (targetTag < 0)
            {
                //음수일시 알수 없는것
                pingPreset = raderPingData.unknownPreset;
            }
            else
            {
                //통과태그, 블럭
                pingPreset = targetTag == raderPingData.pingTag ? raderPingData.passPreset : raderPingData.blockPreset;
            }

            raderPingMark2D.Init(pingPreset.pingColor, pingPreset.pingSprite, pingPreset.time,
                pingPreset.pingStartScale, pingPreset.pingEndScale, raderPingTarget.transform.position, pingPreset.audioClip);
        }

        public RaderPingMark2D GetPingObject()
        {
            var prefab = ObjectPoolManager.Instance.RequestDynamicComponentObject(GetStaticPingObject());
            return prefab;
        }

        private static RaderPingMark2D GetStaticPingObject()
        {
            if (targetPingPrefab == null)
            {
                var go = new GameObject();
                go.name = "-RaderPingMark2D-";
                targetPingPrefab = go.AddComponent<RaderPingMark2D>();
            }
            return targetPingPrefab;
        }    
             
        public enum RaderType
        {
            //네이밍다시체크
            Circle,//일정범위를 원형으로 퍼뜨려서 체크하는 방식
            Sweep,//일정범위를 회전시켜 체크하는 방식
            //Scan,//일정범위를 위에서 아래로 체크하는 방식
        }

        private void OnDrawGizmosSelected()
        {
            switch (raderCATType)
            {
                //=======================================================================================================
                case RaderType.Circle:
                    Gizmos.DrawWireSphere(transform.position, raderMaxRange * 0.5f);
                    Gizmos.DrawWireSphere(transform.position, raderCurRange * 0.5f);
                    break;
                case RaderType.Sweep:
                    Gizmos.DrawWireSphere(transform.position, raderMaxRange);
                    float angleRad = raderObject.eulerAngles.z * (Mathf.PI / 180f);
                    Vector2 Direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

                    Gizmos.DrawRay(transform.position, Direction * raderMaxRange);
                    break;
            }
                   
        }
    }
}
