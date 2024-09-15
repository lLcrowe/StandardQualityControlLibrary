using UnityEngine;
using UnityEngine.EventSystems;


namespace lLCroweTool.UI.JoyStick
{
    public class Joystick_Base : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Range(0, 10)] [SerializeField] private float handleRange = 1;

        [Range(0, 10)] [SerializeField] private float deadZone = 0;
        [SerializeField] public AxisOptions axisOptions = AxisOptions.Both;
        [SerializeField] public bool snapX = false;
        [SerializeField] public bool snapY = false;

        [SerializeField] protected RectTransform background = null;//뒷배경
        [SerializeField] private RectTransform handle = null;//방향손잡이

        private RectTransform baseRect = null;

        private Canvas canvas;
        private Camera cam;

        public Vector2 direction = Vector2.zero;

        protected virtual void Start()
        {
            baseRect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("조이스틱이 캔버스안에 배치되있지않습니다.");
            }

            Vector2 center = new Vector2(0.5f, 0.5f);
            background.pivot = center;
            handle.anchorMin = center;
            handle.anchorMax = center;
            handle.pivot = center;
            handle.anchoredPosition = Vector2.zero;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;

            Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
            Vector2 radius = background.sizeDelta / 2;
            direction = (eventData.position - position) / (radius * canvas.scaleFactor);
            FormatInput();
            HandleInput(direction.magnitude, direction.normalized, radius, cam);
            handle.anchoredPosition = direction * radius * handleRange;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            //방향 초기화
            direction = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }

        protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
        {
            if (magnitude > deadZone)
            {
                if (magnitude > 1)
                {
                    direction = normalised;
                }
            }
            else
            {
                direction = Vector2.zero;
            }

        }

        private void FormatInput()
        {
            switch (axisOptions)
            {
                case AxisOptions.Both:
                    break;
                case AxisOptions.Horizontal:
                    direction = new Vector2(direction.x, 0f);
                    break;
                case AxisOptions.Vertical:
                    direction = new Vector2(0f, direction.y);
                    break;
            }
        }

        public float GetHorizontal()
        {
            return (snapX) ? SnapFloat(direction.x, AxisOptions.Horizontal) : direction.x;
        }

        public float GetVertical()
        {
            return (snapY) ? SnapFloat(direction.y, AxisOptions.Vertical) : direction.y;
        }

        private float SnapFloat(float value, AxisOptions snapAxis)
        {
            if (value == 0)
                return value;

            if (axisOptions == AxisOptions.Both)
            {
                float angle = Vector2.Angle(direction, Vector2.up);
                switch (snapAxis)
                {
                    case AxisOptions.Both:
                        return value;
                        break;
                    case AxisOptions.Horizontal:
                        if (angle < 22.5f || angle > 157.5f)
                            return 0;
                        else
                            return (value > 0) ? 1 : -1;
                        break;
                    case AxisOptions.Vertical:
                        if (angle > 67.5f && angle < 112.5f)
                            return 0;
                        else
                            return (value > 0) ? 1 : -1;
                        break;
                }
            }
            else
            {
                if (value > 0)
                    return 1;
                if (value < 0)
                    return -1;
            }
            return 0;
        }



        protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out Vector2 localPoint))
            {
                Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
                return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
            }
            return Vector2.zero;
        }
    }

    public enum AxisOptions { Both, Horizontal, Vertical }
}
