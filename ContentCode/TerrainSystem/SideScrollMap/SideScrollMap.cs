using UnityEngine;

namespace lLCroweTool.TerrainSystem.SideScroll
{
    public class SideScrollMap : MonoBehaviour
    {
        //카메라
        //  백그라운드넣을 오브젝트
        //      백그라운드1(스크립트를 여기다)
        //      백그라운드2(스크립트를 여기다)
        //      백그라운드3(스크립트를 여기다)

        public float moveSpeed;
        private float length, startPos;
        public Transform cameraTransform;//카메라 트랜스폼//지정이 안되있으면 메인카메라를 찾음

        private void Start()
        {
            if (ReferenceEquals(cameraTransform, null))
            {
                cameraTransform = Camera.main.transform;
            }
            startPos = cameraTransform.position.x;
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }
        
        void FixedUpdate()
        {
            float temp = cameraTransform.position.x * (1 - moveSpeed);
            float distance = cameraTransform.position.x * moveSpeed;

            transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

            if (temp > startPos + length)
            {
                startPos += length;
            }
            else if(temp < startPos - length)
            {
                startPos -= length;
            }
        }
    }
}