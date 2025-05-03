using UnityEngine;
using DG.Tweening;

namespace lLCroweTool.Visual.RotateMultiple
{
    public class RotateMultipleTransform : MonoBehaviour
    {
        //여러트랜스폼들을 회전시키는 역할을 가짐
        //말 그대로 회전시키는 함수
        //아직임시 테스트도 안함
        public TargetRotateObject[] targetRotateObjectArray;

        private void Update()
        {
            
            for (int i = 0; i < targetRotateObjectArray.Length; i++)
            {
                //첫번쨰
                targetRotateObjectArray[i].target.DOLocalRotate(Vector2.zero, targetRotateObjectArray[i].rotateSpeed, RotateMode.FastBeyond360);

                //두번째
                targetRotateObjectArray[i].target.Rotate(Vector3.forward, targetRotateObjectArray[i].target.rotation.z + targetRotateObjectArray[i].rotateSpeed);
            }
        }
    }

    [System.Serializable]
    public class TargetRotateObject
    {
        public Transform target;
        public float rotateSpeed = 1f;
    }

}