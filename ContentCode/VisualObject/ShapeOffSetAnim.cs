#if Shapes

using UnityEngine;
using Shapes;
using lLCroweTool.TimerSystem;

namespace lLCroweTool.Visual.ShapeAsset
{
    [RequireComponent(typeof(CoroutineTimerModule))]
    public class ShapeOffSetAnim : MonoBehaviour
    {
        public OffSetSetting[] offSetSettingArray;
        public IDashable[] dashableArray;
        private CoroutineTimerModule timerModule;

        private void Awake()
        {   
            timerModule = GetComponent<CoroutineTimerModule>();
            timerModule.SetTimer(0.05f);
            timerModule.AddUnityEvent(UpdateShapeLine);
            dashableArray = GetComponentsInChildren<IDashable>();
        }

        private void UpdateShapeLine()
        {
            for (int i = 0; i < dashableArray.Length; i++)
            {
                if (dashableArray[i].Dashed)
                {
                    //1에서 변경//1이 원래상태임
                    //dashableArray[i].DashOffset = dashableArray[i].DashOffset < 1 ? dashableArray[i].DashOffset += offSetSettingArray[i].GetSpeed() * Time.deltaTime : 0;
                    dashableArray[i].DashOffset += offSetSettingArray[i].GetSpeed() * Time.deltaTime;
                }
            }
        }

        [System.Serializable]
        public struct OffSetSetting
        {
            [Range(0f, 100f)]
            public float offSetSpeed;
            public bool isReverse;

            public float GetSpeed()
            {
                return isReverse ? offSetSpeed * -1 : offSetSpeed;
            }
        }
    }
}
#endif