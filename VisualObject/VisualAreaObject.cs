#if Shape
using UnityEngine;
using Shapes;

namespace lLCroweTool
{
   
    /// <summary>
    /// 비쥬얼적으로 보여주는 클래스이며 쉐이프 에셋을 사용함
    /// 사용처는 웨폰시스템과 AI모듈(아직은 사용안함), UI등 
    /// 범위관련해서 유저가 볼수 있는 모든곳에 사용됨.    
    /// </summary>
    public class VisualAreaObject : MonoBehaviour
    {
        //비쥬얼관련
        private bool isFill = false;
        private bool isSide = false;        
        [SerializeField] private DiscType discType;
        [SerializeField] private Disc visualFillObject;
        [SerializeField] private Disc visualSideObject;


        private void Awake()
        {
            if (!ReferenceEquals(visualFillObject, null))
            {
                isFill = true;
            }
            if (!ReferenceEquals(visualSideObject, null))
            {
                isSide = true;
            }
            ActiveVisualObject(discType);
        }

        //private void Update()
        //{
        //    //실패
        //    if (TryGetComponent(out SensorToolkit.FOVCollider2D collider))
        //    {   
        //        if (!ReferenceEquals(visualFillObject, null))
        //        {
        //            visualFillObject.Radius = collider.Length;
        //            visualFillObject.AngRadiansStart = Mathf.Deg2Rad * (90 + (collider.FOVAngle - 45));
        //            visualFillObject.AngRadiansEnd = Mathf.Deg2Rad * (90 - (collider.FOVAngle - 45));
        //        }
        //        if (!ReferenceEquals(visualSideObject, null))
        //        {
        //            visualSideObject.Radius = collider.Length;
        //            visualSideObject.AngRadiansStart = Mathf.Deg2Rad * (90 + (collider.FOVAngle - 45));
        //            visualSideObject.AngRadiansEnd = Mathf.Deg2Rad * (90 - (collider.FOVAngle - 45));
        //        }
        //    }
        //}
        /// <summary>
        /// 비쥬얼오브젝트 활성화 후 모드세팅
        /// </summary>
        /// <param name="_discTypes">모양 설정</param>
        public void ActiveVisualObject(DiscType _discType)
        {
            discType = _discType;
            switch (_discType)
            {
                case DiscType.Circle:
                    if (isFill)
                    {
                        visualFillObject.gameObject.SetActive(true);
                        visualFillObject.Type = Shapes.DiscType.Disc;
                    }
                    if (isSide)
                    {
                        visualSideObject.gameObject.SetActive(true);
                        visualSideObject.Type = Shapes.DiscType.Ring;
                    }
                    break;
                case DiscType.Arc:
                    if (isFill)
                    {
                        visualFillObject.gameObject.SetActive(true);
                        visualFillObject.Type = Shapes.DiscType.Pie;
                    }
                    if (isSide)
                    {
                        visualSideObject.gameObject.SetActive(true);
                        visualSideObject.Type = Shapes.DiscType.Arc;
                    }
                    break;
            }
        }

        /// <summary>
        /// 모든 비쥬얼오브젝트 비활성화
        /// </summary>
        public void DeActiveAllVisualObject()
        {
            if (isFill)
            {
                visualFillObject.gameObject.SetActive(false);
            }
            if (isSide)
            {
                visualSideObject.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 비쥬얼오브젝트의 길이를 설정해주는 함수(1번 순번)
        /// </summary>
        /// <param name="_range">설정할 길이</param>
        public void SetRange(float _range)
        {
            if (isFill)
            {
                visualFillObject.Radius = _range;
            }
            if (isSide)
            {
                visualSideObject.Radius = _range;
            }
        }

        /// <summary>
        /// 비쥬얼오브젝트의 각도를 설정해주는 함수(2번 순번)
        /// 사용할려면 Degree로 맞추어서 작동시킬것 90도가 위쪽방향 0 도 오른쪽
        /// </summary>
        /// <param name="startAngle">시작하는 각도</param>
        /// <param name="endAngle">끝나는 각도</param>
        public void SetArc(float startAngle, float endAngle)
        {
            if (discType == DiscType.Circle)
            {
                return;
            }
            if (isFill)
            {
                visualFillObject.AngRadiansStart = Mathf.Deg2Rad * (90 + startAngle);
                visualFillObject.AngRadiansEnd = Mathf.Deg2Rad * (90 - endAngle);

            }
            if (isSide)
            {
                visualSideObject.AngRadiansStart = Mathf.Deg2Rad * (90 + startAngle);
                visualSideObject.AngRadiansEnd = Mathf.Deg2Rad * (90 - endAngle);
            }
        }

        public Disc GetVisualFillObject()
        {
            return visualFillObject;
        }
        public Disc GetVisualSideObject()
        {
            return visualSideObject;
        }
    }
    public enum DiscType
    {
        Circle,
        Arc,
    }
}
#endif