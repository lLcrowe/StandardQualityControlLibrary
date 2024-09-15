#if Light2D
using UnityEngine;
using System.Collections;
using FunkyCode;

namespace lLCroweTool.LampSystem
{
    [RequireComponent(typeof(Light2D))]
    public class Lamp : MonoBehaviour
    {

        protected Light2D lightingSource;

        private void Awake()
        {
            lightingSource = GetComponent<Light2D>();
        }
        public void ActiveLamp()
        {
            lightingSource.enabled = !lightingSource.enabled;
        }
        public void ResetLamp()
        {
            lightingSource.enabled = true;
            lightingSource.color.a = 1;
        }

    }
}
#endif