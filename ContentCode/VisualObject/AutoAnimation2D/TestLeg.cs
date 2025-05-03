using System.Collections;
using UnityEngine;

namespace lLCroweTool.AutoAnimation2D
{
    public class TestLeg : MonoBehaviour
    {
        public Transform body;
        public Vector3 currentPosition;
        public Vector3 newPosition;
        public Vector3 oldPosition;
        
        public LayerMask hitLayer;
        public float footSpacing;
        public float stepDistance;
        public float lerp;
        public float stepHeight;
        public float moveSpeed;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.position = currentPosition;
            Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit info , 10 , hitLayer))
            {
                if (Vector3.Distance(newPosition, info.point)> stepDistance)
                {
                    lerp = 0;
                    newPosition = info.point;
                }
            }

            if (lerp < 1)
            {
                Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
                footPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

                currentPosition = footPosition;
                lerp += Time.deltaTime * moveSpeed;
            }
            else
            {
                oldPosition = newPosition;
            }
        }
    }
}