using UnityEngine;

namespace lLCroweTool.ObjectPool
{
    /// <summary>
    /// 반환하기 위한 폴 타겟
    /// </summary>
    public class CustomPoolTarget : MonoBehaviour
    {
        [SerializeField] private Component targetComponent;
        [SerializeField] private bool isReturn = true;
        private int originInstanceID = -1;

        //public void SetPoolTargetComponent(Component component)
        public void SetPoolTargetComponent(Component component, int instanceID)
        {
            targetComponent = component;
            originInstanceID = instanceID;
        }

        /// <summary>
        /// 폴에 리턴가능한지 세팅하는 함수
        /// </summary>
        /// <param name="isReturn"></param>
        public void SetIsReturn(bool isReturn)
        {
            this.isReturn = isReturn;
        }

        //이름으로 ID값이 됨
        private void OnDisable()
        {
            if (!isReturn)
            {
                return;
            }
            ObjectPoolManager.Instance.ReturnDynamicComponentObject(targetComponent, originInstanceID);
        }
    }
}