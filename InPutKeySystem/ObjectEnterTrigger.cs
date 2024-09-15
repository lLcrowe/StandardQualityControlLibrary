using System.Collections;
using UnityEngine;

namespace Assets.StandardQualityControlLibary.InPutKeySystem
{
    public class ObjectEnterTrigger : MonoBehaviour
    {
        public System.Action enterAction;
        public System.Action exitAction;

        private void OnMouseEnter()
        {
            enterAction?.Invoke();
        }
        private void OnMouseExit()
        {
            exitAction?.Invoke();
        }
    }
}